using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Komin
{
    public delegate void server_logging_routine(string msg);

    public class KominServer
    {
        BackgroundWorker listener;
        TcpListener server;
        public List<KominServerSideConnection> connections;
        public KominNetworkJobHolder jobs;
        public static KominServerDatabase database;
        private bool running;
        public server_logging_routine log;

        public KominServer()
        {
            server = null;
            listener = new BackgroundWorker();
            connections = new List<KominServerSideConnection>();
            jobs = new KominNetworkJobHolder();
            database = null;
            running = false;
            listener.WorkerSupportsCancellation = true;
            listener.DoWork += listen;
            log = null;
            if (!Directory.Exists(".\\ServerFileStore"))
            {
                Directory.CreateDirectory(".\\ServerFileStore");
            }
        }

        public string[] DetectIPAddresses()
        {
            int count = 0;
            IPAddress[] addrs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in addrs)
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                    count++;
            string[] returns = new string[count];
            count = 0;
            foreach (IPAddress addr in addrs)
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    byte[] b = addr.GetAddressBytes();
                    returns[count++] = b[0] + "." + b[1] + "." + b[2] + "." + b[3];
                }
            return returns;
        }

        public void Start(string IP, int port)
        {
            try
            {
                database = new KominServerDatabase();
                server = new TcpListener(IPAddress.Parse(IP), port);
                server.Start(100);
                jobs.Restart();
                listener.RunWorkerAsync();
                running = true;
                if (log != null) log("Server started at " + IP + ":" + port);
            }
            catch (SocketException ex)
            {
                throw new ServerSocketException("Server couldn't be created: socket error", ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (server == null)
                    return;

                jobs.Restart();
                listener.CancelAsync();
                while (connections.Count > 0)
                    connections[0].Disconnect();
                server.Stop();
                database.Disconnect();
                running = false;
                if (log != null) log("Server stopped");
            }
            catch (SocketException ex)
            {
                throw new ServerSocketException("Server couldn't be stopped: socket error", ex);
            }
        }

        public bool IsRunning()
        {
            return running;
        }

        //accepts arriving connections
        private void listen(object sender, DoWorkEventArgs e)
        {
            if (server == null)
                return;

            uint last_client_id = 0;

            while (!listener.CancellationPending)
            {
                while (!server.Pending()) Thread.Sleep(50);
                KominServerSideConnection conn = new KominServerSideConnection();
                conn.client = server.AcceptTcpClient();
                conn.server = this;
                conn.log = log;
                conn.client_id = ++last_client_id;
                conn.commune.RunWorkerAsync();
                connections.Add(conn);
            }
        }

        public class ServerSocketException : Exception
        {
            public ServerSocketException() : base() { }
            public ServerSocketException(string message) : base(message) { }
            public ServerSocketException(string message, Exception inner) : base(message, inner) { }
            protected ServerSocketException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) { }
        }
    }

    public class KominServerSideConnection
    {
        public const int PingAnswerAwaitTime = 10000;
        public const int PingRequestSendTime = 3000;

        public TcpClient client;
        NetworkStream stream;
        public BackgroundWorker commune;
        public KominServer server;
        List<KominNetworkPacket> packets_to_send;
        public server_logging_routine log;
        public uint client_id;
        byte enc_seed, dec_seed;
        System.Timers.Timer PingTimer;
        bool had_ping_answer;
        bool waiting_for_ping_answer;
        //user data
        uint contact_id;

        public KominServerSideConnection()
        {
            client_id = 0;
            contact_id = 0;
            client = null;
            packets_to_send = new List<KominNetworkPacket>();
            commune = new BackgroundWorker();
            commune.WorkerSupportsCancellation = true;
            commune.DoWork += clientCommune;
            log = null;
            enc_seed = dec_seed = KominCipherSuite.InitialSeed;
            PingTimer = new System.Timers.Timer(PingRequestSendTime);
            PingTimer.Elapsed += PingTimer_Elapsed;
            had_ping_answer = false;
            waiting_for_ping_answer = false;
        }

        void PingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PingTimer.Enabled = false;
            if (waiting_for_ping_answer && !had_ping_answer) //ping answer didn't arrive on time
            {
                if (log != null) log("Client " + client_id + " lost connection");
                server.jobs.CancelJobs(contact_id, false);
                Disconnect();
                return;
            }
            else if(!waiting_for_ping_answer) //it's time to send another ping request
            {
                PingTimer.Interval = PingAnswerAwaitTime;
                had_ping_answer = false;
                waiting_for_ping_answer = true;
                PeriodicPingRequest();
                PingTimer.Enabled = true;
            }
        }

        ~KominServerSideConnection()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            if (client == null)
                return;

            if (contact_id != 0)
                Logout();

            KominNetworkPacket p = new KominNetworkPacket();
            p.sender = 0;
            p.target = 0;
            p.target_is_group = false;
            p.command = (uint)KominProtocolCommands.Disconnect;
            p.job_id = 0;
            p.DeleteContent();
            InsertPacketForSending(p);

            if (log != null) log("Client " + client_id + " disconnected");

            SelfStop();
        }

        private void SelfStop()
        {
            while (packets_to_send.Count > 0) ;
            PingTimer.Enabled = false;
            commune.CancelAsync();
            client.Close();
            client = null;
            enc_seed = dec_seed = KominCipherSuite.InitialSeed;
            server.connections.Remove(this);

            if (log != null) log("Client " + client_id + " removed");
        }

        private void clientCommune(object sender, DoWorkEventArgs e)
        {
            if (client == null)
                return;
            int packet_size = 0;
            byte[] buffer = new byte[0];
            stream = client.GetStream();

            if (log != null) log("Client " + client_id + " connected");

            waiting_for_ping_answer = true;
            had_ping_answer = false;
            //PeriodicPingRequest();
            //PingTimer.Enabled = true;
            
            while (!commune.CancellationPending)
            {
                while (client.Available <= 0)
                {
                    if (packets_to_send.Count > 0)
                        SendPacketToClient();
                    else
                        Thread.Sleep(50);
                }
                byte[] subbuffer = new byte[client.Available];
                stream.Read(subbuffer, 0, client.Available);
                dec_seed = KominCipherSuite.Decrypt(ref subbuffer, dec_seed);
                Array.Resize<byte>(ref buffer, buffer.Length + subbuffer.Length);
                Buffer.BlockCopy(subbuffer, 0, buffer, buffer.Length - subbuffer.Length, subbuffer.Length);
                do
                {
                    packet_size = (int)KominNetworkPacket.CheckSize(ref buffer);
                    if (packet_size != 0)
                    {
                        byte[] packet_data = new byte[packet_size];
                        Buffer.BlockCopy(buffer, 0, packet_data, 0, packet_size);
                        Buffer.BlockCopy(buffer, packet_size, buffer, 0, buffer.Length - packet_size);
                        Array.Resize<byte>(ref buffer, buffer.Length - packet_size);
                        KominNetworkPacket packet = new KominNetworkPacket();
                        packet.UnpackReceivedPacket(ref packet_data);
                        new PacketInterpreterThread(this, packet);
                    }
                } while (packet_size != 0);
            }

            stream.FlushAsync();
            stream.Close();

            PingTimer.Enabled = false;
        }

        class PacketInterpreterThread
        {
            Thread th;
            KominServerSideConnection conn;
            KominNetworkPacket packet;

            public PacketInterpreterThread(KominServerSideConnection conn, KominNetworkPacket packet)
            {
                this.conn = conn;
                this.packet = packet;
                th = new Thread(starter);
                th.Start();
            }

            void starter()
            {
                conn.InterpretePacket(packet);
                th.Abort();
            }
        }

        internal void InterpretePacket(KominNetworkPacket packet)
        {
            if (log != null) log("{E:" + enc_seed + ",D:" + dec_seed + "} Client " + client_id + "=>Server: sender=" + packet.sender + "   receiver=" + packet.target + "   is_group=" + (packet.target_is_group ? "true" : "false") + "   cmd=" + ((KominProtocolCommands)packet.command).ToString().ToUpper() + "   jobID=" + packet.job_id + "   content=" + packet.content);

            if (((packet.sender != contact_id) ||
                 ((packet.sender == 0) && ((packet.command != (uint)KominProtocolCommands.Login) &&
                                           (packet.command != (uint)KominProtocolCommands.CreateContact) &&
                                           (packet.command != (uint)KominProtocolCommands.PingContactRequest) &&
                                           (packet.command != (uint)KominProtocolCommands.PeriodicPingRequest) &&
                                           (packet.command != (uint)KominProtocolCommands.PeriodicPingAnswer) &&
                                           (packet.command != (uint)KominProtocolCommands.Disconnect) &&
                                           (packet.command != (uint)KominProtocolCommands.NoOperation)))) &&
                ((packet.sender == 0) && ((packet.command != (uint)KominProtocolCommands.PeriodicPingRequest) &&
                                          (packet.command != (uint)KominProtocolCommands.PeriodicPingAnswer))))
            {
                //packet error - illegal attempt
                return;
            }
            
            //filter out client targeted packets - proxy
            if ((packet.target_is_group == false) && (packet.target != 0))
            {
                bool redirected = false;
                if ((packet.target == packet.sender) && (packet.command == (uint)KominProtocolCommands.SendMessage)) //disable audio and video loopbacks
                {
                    packet.DeleteContent((uint)KominProtocolContentTypes.AudioMessageData | (uint)KominProtocolContentTypes.VideoMessageData);
                    if (packet.content == 0)
                        return; //discard packet if empty message
                }
                foreach (KominServerSideConnection conn in server.connections)
                    if (conn.contact_id == packet.target)
                    {
                        conn.InsertPacketForSending(packet);
                        redirected = true;
                        break;
                    }
                if (redirected == false) //opposite side couldn't receive packet
                {
                    ContactData cd = KominServer.database.GetContactData(packet.target);
                    if (cd == null)
                    {
                        Error(KominNetworkErrors.UserNotExists, packet);
                        return;
                    }
                    else
                    {
                        //store text message
                        if ((packet.command == (uint)KominProtocolCommands.SendMessage) && ((packet.content & (uint)KominProtocolContentTypes.TextMessageData) == (uint)KominProtocolContentTypes.TextMessageData))
                            KominServer.database.InsertPendingMessage(packet.sender, packet.target, false, ((TextMessage)packet.GetContent(KominProtocolContentTypes.TextMessageData)[0]).message);
                        else
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserNotLoggedIn, packet);
                        }
                    }
                }
                return;
            }

            //only server or group targeted packets can reach here
            //server interpretes here received packets
            switch ((KominProtocolCommands)packet.command)
            {
                case KominProtocolCommands.NoOperation:
                    break;
                case KominProtocolCommands.Login: //client tries to log in
                    {
                        if (packet.content != 0x43)
                        {
                            packet.DeleteContent((uint)KominProtocolContentTypes.ContactData, true);
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        //get request data
                        string req_contact_name = ((ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0]).contact_name;
                        string req_password = (string)packet.GetContent(KominProtocolContentTypes.PasswordData)[0];
                        uint req_status = (uint)packet.GetContent(KominProtocolContentTypes.StatusData)[0];
                        UserData ud = KominServer.database.GetUserData(req_contact_name);
                        if (ud == null)
                        {
                            packet.DeleteContent((uint)KominProtocolContentTypes.ContactData, true);
                            Error(KominNetworkErrors.UserNotExists, packet);
                            return;
                        }
                        if ((ud.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible)
                        {
                            packet.DeleteContent((uint)KominProtocolContentTypes.ContactData, true);
                            Error(KominNetworkErrors.UserAlreadyLoggedIn, packet);
                            return;
                        }
                        if (ud.password != req_password)
                        {
                            packet.DeleteContent((uint)KominProtocolContentTypes.ContactData, true);
                            Error(KominNetworkErrors.WrongPassword, packet);
                            return;
                        }
                        if (((req_status & ((uint)KominClientStatusCodes.Mask + (uint)KominClientCapabilities.Mask)) != req_status) ||
                            ((req_status & (uint)KominClientStatusCodes.Mask) > (uint)KominClientStatusCodes.MaxValue) ||
                            ((req_status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible))
                        {
                            packet.DeleteContent((uint)KominProtocolContentTypes.StatusData, true);
                            Error(KominNetworkErrors.WrongStatus, packet);
                            return;
                        }
                        //set new status
                        contact_id = ud.contact_id;
                        SetStatus(req_status);
                        //confirm logging in to user
                        packet.InsertContent(KominProtocolContentTypes.ContactIDData, ud.contact_id);
                        ContactData cd = new ContactData();
                        cd = (ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0];
                        cd.contact_id = ud.contact_id;
                        cd.status = req_status;
                        packet.InsertContent(KominProtocolContentTypes.ContactData, cd);
                        packet.InsertContent(KominProtocolContentTypes.StatusData, cd.status);
                        Accept(packet);
                        break;
                    }
                case KominProtocolCommands.Logout: //client tries to log out
                    {
                        if (packet.content != 0)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        ContactData contd = KominServer.database.GetContactData(packet.sender);
                        if (contd == null)
                        {
                            Error(KominNetworkErrors.UserNotExists, packet);
                            return;
                        }
                        if (packet.sender != contact_id)
                        {
                            Error(KominNetworkErrors.CannotInfluOtherUsers, packet);
                            return;
                        }
                        UserData userd = KominServer.database.GetUserData(contd.contact_name);
                        if ((userd.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                        {
                            Error(KominNetworkErrors.UserNotLoggedIn, packet);
                            return;
                        }
                        //set new status
                        SetStatus((uint)KominClientStatusCodes.NotAccessible);
                        //check is new state influ on group existence
                        foreach (GroupData gd in userd.groups)
                        {
                            bool holder_changed = false;
                            foreach (ContactData contact in gd.members)
                                if ((contact.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible) ;
                                else if (contact.contact_id != contact_id)
                                {
                                    if (!holder_changed)
                                    {
                                        KominServer.database.SetGroupCreator(gd.group_id, contact.contact_id);
                                        gd.creators_id = contact.contact_id;
                                        GroupHolderChange(gd);
                                        holder_changed = true;
                                    }
                                }
                            if (!holder_changed)
                            {
                                CloseGroup(gd.group_id);
                            }
                        }
                        //confirm logging out to user
                        packet.DeleteContent();
                        packet.InsertContent(KominProtocolContentTypes.StatusData, (uint)KominClientStatusCodes.NotAccessible);
                        Accept(packet);
                        contact_id = 0;
                        break;
                    }
                case KominProtocolCommands.SetStatus: //client tries to change its status
                    if ((packet.content & 0x02) != 0x02)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.WrongRequestContent, packet);
                        return;
                    }
                    if (KominServer.database.GetContactData(packet.sender) == null)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.UserNotExists, packet);
                        return;
                    }
                    if (KominServer.database.GetContactData(packet.sender).contact_id != contact_id)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.CannotInfluOtherUsers, packet);
                        return;
                    }
                    uint new_status = (uint)packet.GetContent(KominProtocolContentTypes.StatusData)[0];
                    if (((new_status & ((uint)KominClientStatusCodes.Mask + (uint)KominClientCapabilities.Mask)) != new_status) ||
                        ((new_status & (uint)KominClientStatusCodes.Mask) > (uint)KominClientStatusCodes.MaxValue))
                    {
                        packet.DeleteContent((uint)KominProtocolContentTypes.StatusData, true);
                        Error(KominNetworkErrors.WrongStatus, packet);
                        return;
                    }
                    if ((new_status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                    {
                        Accept(packet);
                        Logout();
                        return;
                    }
                    SetStatus(new_status);
                    Accept(packet);
                    break;
                case KominProtocolCommands.SetPassword: //client tries to change its password
                    if ((packet.content & 0x01) != 0x01)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.WrongRequestContent, packet);
                        return;
                    }
                    if (KominServer.database.GetContactData(packet.sender) == null)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.UserNotExists, packet);
                        return;
                    }
                    if ((KominServer.database.GetContactData(packet.sender).status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.UserNotLoggedIn, packet);
                        return;
                    }
                    if (KominServer.database.GetContactData(packet.sender).contact_id != contact_id)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.CannotInfluOtherUsers, packet);
                        return;
                    }
                    string new_password = (string)packet.GetContent(KominProtocolContentTypes.PasswordData)[0];
                    if (new_password.Length == 0)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.WrongPassword, packet);
                        return;
                    }
                    KominServer.database.SetUserPassword(packet.sender, new_password);
                    Accept(packet);
                    break;
                case KominProtocolCommands.CreateContact: //client tries to create an account
                    if (packet.content != 0x41)
                    {
                        packet.DeleteContent((uint)KominProtocolContentTypes.ContactData, true);
                        Error(KominNetworkErrors.WrongRequestContent, packet);
                        return;
                    }
                    int err = KominServer.database.CreateUser(((ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0]).contact_name, (string)packet.GetContent(KominProtocolContentTypes.PasswordData)[0]);
                    switch (err)
                    {
                        case 0:
                            Accept(packet);
                            break;
                        case -1: //user already exists
                            Error(KominNetworkErrors.UserAlreadyExists, packet);
                            return;
                    }
                    break;
                /*case KominProtocolCommands.Accept: //server doesn't make any requests to client so no need to accept nor deny anything
                    break;
                case KominProtocolCommands.Deny:
                    break;*/
                /*case KominProtocolCommands.Error: //client doesn't send any error messages to server
                    break;*/
                case KominProtocolCommands.AddContactToList: //client wants to add someone to its own contact list
                    {
                        if ((packet.content & 0x40) != 0x40)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        if (KominServer.database.GetContactData(packet.sender).contact_id != contact_id)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.CannotInfluOtherUsers, packet);
                            return;
                        }
                        ContactData cd = (ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0];
                        cd = KominServer.database.GetContactData(cd.contact_name);
                        if (cd == null)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserNotExists, packet);
                            return;
                        }
                        switch (KominServer.database.InsertContactIntoList(packet.sender, false, cd.contact_id))
                        {
                            case 0:
                                packet.DeleteContent();
                                UserData ud = KominServer.database.GetUserData(packet.sender);
                                ud.password = "";
                                packet.InsertContent(KominProtocolContentTypes.UserData, ud);
                                Accept(packet, false);
                                break;
                            case -1:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserNotExists, packet);
                                return;
                            case -2:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.ServerInternalError, packet);
                                return;
                            case -3:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserExistsOnContactList, packet);
                                return;
                        }
                        break;
                    }
                case KominProtocolCommands.RemoveContactFromList: //client wants to remove someone from its own contact list
                    {
                        if ((packet.content & 0x40) != 0x40)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        if (KominServer.database.GetContactData(packet.sender).contact_id != contact_id)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.CannotInfluOtherUsers, packet);
                            return;
                        }
                        ContactData cd = (ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0];
                        cd = KominServer.database.GetContactData(cd.contact_name);
                        if (cd == null)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserNotExists, packet);
                            return;
                        }
                        switch (KominServer.database.RemoveContactFromList(packet.sender, false, cd.contact_id))
                        {
                            case 0:
                                packet.DeleteContent();
                                UserData ud = KominServer.database.GetUserData(packet.sender);
                                ud.password = "";
                                packet.InsertContent(KominProtocolContentTypes.UserData, ud);
                                Accept(packet, false);
                                break;
                            case -1:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.ServerInternalError, packet);
                                return;
                            case -2:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserNotExistsOnContactList, packet);
                                return;
                        }
                        break;
                    }
                case KominProtocolCommands.PingContactRequest: //groups don't answer to ping, server answers to pings about users
                    {
                        if (((packet.content & 0xFBB) != 0) || ((packet.content & 0x44) == 0))
                        {
                            packet.DeleteContent((uint)KominProtocolContentTypes.ContactIDData, true);
                            packet.DeleteContent((uint)KominProtocolContentTypes.ContactData, true);
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        //choose validation type. contact_id is more valuable than ContactData
                        if ((packet.content & (uint)KominProtocolContentTypes.ContactIDData) != 0)
                        {
                            //self-ping
                            if (packet.sender == (uint)packet.GetContent(KominProtocolContentTypes.ContactIDData)[0])
                            {
                                packet.DeleteContent((uint)KominProtocolContentTypes.ContactIDData, true);
                                packet.InsertContent(KominProtocolContentTypes.ContactData, KominServer.database.GetContactData(packet.sender));
                                packet.InsertContent(KominProtocolContentTypes.UserData, KominServer.database.GetUserData(packet.sender));
                            }
                            else //foreign ping
                            {
                                //check for user not exists
                                ContactData contact = KominServer.database.GetContactData((uint)packet.GetContent(KominProtocolContentTypes.ContactIDData)[0]);
                                if (contact == null)
                                {
                                    Error(KominNetworkErrors.UserNotExists, packet);
                                    return;
                                }
                                packet.DeleteContent((uint)KominProtocolContentTypes.ContactIDData, true);
                                packet.InsertContent(KominProtocolContentTypes.ContactData, contact);
                            }
                            PingContactAnswer(packet);
                        }
                        else if ((packet.content & (uint)KominProtocolContentTypes.ContactData) != 0)
                        {
                            ContactData req_contact = (ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0];
                            //get req_contact.contact_id
                            UserData req_user = KominServer.database.GetUserData(req_contact.contact_name);
                            if (req_user == null)
                            {
                                Error(KominNetworkErrors.UserNotExists, packet);
                                return;
                            }
                            packet.DeleteContent();
                            //self-ping
                            if (packet.sender == req_user.contact_id)
                            {
                                packet.InsertContent(KominProtocolContentTypes.ContactData, KominServer.database.GetContactData(packet.sender));
                                packet.InsertContent(KominProtocolContentTypes.UserData, KominServer.database.GetUserData(packet.sender));
                            }
                            else //foreign ping
                            {
                                packet.InsertContent(KominProtocolContentTypes.ContactData, KominServer.database.GetContactData(req_user.contact_id));
                            }
                            PingContactAnswer(packet);
                        }
                        break;
                    }
                case KominProtocolCommands.PingContactAnswer: //user answers to ping from server
                    server.jobs.MarkNewArrival(packet.job_id, packet);
                    break;
                case KominProtocolCommands.PingGroupRequest: //user wants to check is specified group name valid
                    {
                        if ((packet.content & 0x080) != 0x080)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        GroupData gd = KominServer.database.GetGroupData(((GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0]).group_name);
                        if (gd == null)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.GroupNotExists, packet);
                            return;
                        }
                        PingGroupAnswer(gd, packet);
                        break;
                    }
                case KominProtocolCommands.SendMessage: //user sends group messages
                    {
                        if (((packet.content & 0x038) == 0) || ((packet.content | 0x038) != 0x038)) //##### files disabled - 0x138 to enable
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        GroupData gd = KominServer.database.GetGroupData(packet.target);
                        foreach (ContactData member in gd.members)
                            if ((member.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible)
                            {
                                if (member.contact_id == packet.sender)
                                {
                                    //for loopback exclude audio and video
                                    KominNetworkPacket p = new KominNetworkPacket();
                                    p.sender = packet.sender;
                                    p.target = packet.target;
                                    p.target_is_group = packet.target_is_group;
                                    p.command = packet.command;
                                    p.job_id = packet.job_id;
                                    p.DeleteContent();
                                    p.CopyContent(packet);
                                    p.DeleteContent((uint)KominProtocolContentTypes.AudioMessageData | (uint)KominProtocolContentTypes.VideoMessageData);
                                    if (p.content != 0)
                                        InsertPacketForSending(p);
                                }
                                else
                                    foreach (KominServerSideConnection conn in server.connections)
                                        if (conn.contact_id == member.contact_id)
                                            conn.InsertPacketForSending(packet);
                            }
                        break;
                    }
                case KominProtocolCommands.PingMessages: //user asks for stored messages
                    {
                        if (packet.content != 0)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        List<PendingMessage> pml = KominServer.database.GetPendingMessages(packet.sender);
                        foreach (PendingMessage pm in pml)
                        {
                            KominNetworkPacket p = new KominNetworkPacket();
                            p.sender = pm.sender_id;
                            p.target = pm.receiver_id;
                            p.target_is_group = pm.receiver_is_group;
                            p.job_id = 0;
                            p.command = (uint)KominProtocolCommands.SendMessage;
                            p.DeleteContent();
                            TextMessage tm = new TextMessage();
                            tm.message = pm.message;
                            tm.send_date = pm.send_date;
                            p.InsertContent(KominProtocolContentTypes.TextMessageData, tm);
                            InsertPacketForSending(p);
                            KominServer.database.RemovePendingMessage(pm.message_id);
                        }
                        break;
                    }
                /*case KominProtocolCommands.RequestAudioCall: //user can't request a call from server
                    break;
                case KominProtocolCommands.RequestVideoCall:
                    break;*/
                case KominProtocolCommands.CloseCall: //group holder wants to close call - broadcast this information in group
                    {
                        if (packet.content != 0)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        GroupData gd = KominServer.database.GetGroupData(packet.target);
                        if (gd == null)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.GroupNotExists, packet);
                            return;
                        }
                        if (packet.sender != gd.creators_id)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserIsNotGroupHolder, packet);
                            return;
                        }
                        if ((gd.communication_type & 0x030) == 0)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.CallNotStartedYet, packet);
                            return;
                        }
                        gd.communication_type &= 0x101; //clear audio and video communication types
                        if (KominServer.database.SetGroupCommunicationType(gd.group_id, gd.communication_type) == -1)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.GroupNotExists, packet);
                            return;
                        }
                        packet.DeleteContent();
                        packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
                        foreach (ContactData member in gd.members)
                            if ((member.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible)
                            {
                                foreach (KominServerSideConnection conn in server.connections)
                                    if (conn.contact_id == member.contact_id)
                                        conn.InsertPacketForSending(packet);
                            }
                        Accept(packet);
                        break;
                    }
                /*case KominProtocolCommands.SwitchToAudioCall: //group holder wants to change call type - broadcast this information in group
                    break;
                case KominProtocolCommands.SwitchToVideoCall:
                    break;*/
                case KominProtocolCommands.RequestFileTransfer: //user asks for group file transfer (upload or download - file_id presence decide)
                    break;
                /*case KominProtocolCommands.TimeoutFileTransfer: //user can't notify group file timeout
                    break;*/
                case KominProtocolCommands.FinishFileTransfer: //user notifies about end of file transfer (upload to server)
                    server.jobs.MarkNewArrival(packet.job_id, packet);
                    break;
                case KominProtocolCommands.CreateGroup: //user wants to create new group
                    {
                        if ((packet.content & 0x080) != 0x080)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        uint new_group_holder = packet.sender;
                        GroupData new_gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
                        ContactData cd = KominServer.database.GetContactData(new_group_holder);
                        if (cd != null)
                        {
                            if ((cd.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                            {
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserNotLoggedIn, packet);
                                return;
                            }
                        }
                        else
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserNotExists, packet);
                            return;
                        }
                        uint capabilities = new_gd.communication_type & 0x138;
                        //#################################################### to do: check server acceptance of capabilities
                        switch (KominServer.database.CreateGroup(new_gd.group_name, new_group_holder, capabilities))
                        {
                            case 0:
                                packet.DeleteContent();
                                new_gd = KominServer.database.GetGroupData(new_gd.group_name);
                                packet.InsertContent(KominProtocolContentTypes.GroupData, new_gd);
                                Accept(packet);
                                JoinGroup(new_gd);
                                break;
                            case -1:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserNotExists, packet);
                                return;
                            case -2:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.GroupAlreadyExists, packet);
                                return;
                            case -3:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.ServerInternalError, packet);
                                return;
                        }
                        break;
                    }
                case KominProtocolCommands.JoinGroup: //user wants to join an existing group
                    {
                        if ((packet.content & 0x080) != 0x080)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        GroupData gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
                        if (gd.group_name == "")
                            gd = KominServer.database.GetGroupData(gd.group_id);
                        else
                            gd = KominServer.database.GetGroupData(gd.group_name);
                        if (gd == null)
                        {
                            Error(KominNetworkErrors.GroupNotExists, packet);
                            return;
                        }
                        switch (KominServer.database.InsertContactIntoList(gd.group_id, true, packet.sender))
                        {
                            case 0:
                                gd = KominServer.database.GetGroupData(gd.group_id);
                                packet.DeleteContent();
                                packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
                                Accept(packet);
                                JoinGroup(gd);
                                break;
                            case -1:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserNotExists, packet);
                                return;
                            case -2:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.GroupNotExists, packet);
                                return;
                            case -3:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserAlreadyInGroup, packet);
                                return;
                        }
                        break;
                    }
                case KominProtocolCommands.LeaveGroup: //user wants to leave an existing group
                    {
                        if ((packet.content & 0x080) != 0x080)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        GroupData gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
                        if (gd.group_name == "")
                            gd = KominServer.database.GetGroupData(gd.group_id);
                        else
                            gd = KominServer.database.GetGroupData(gd.group_name);
                        if (gd == null)
                        {
                            Error(KominNetworkErrors.GroupNotExists, packet);
                            return;
                        }
                        switch (KominServer.database.RemoveContactFromList(gd.group_id, true, packet.sender))
                        {
                            case 0:
                                gd = KominServer.database.GetGroupData(gd.group_id);
                                packet.DeleteContent();
                                packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
                                //check is new state influ on group existence
                                bool holder_changed = false;
                                foreach (ContactData contact in gd.members)
                                    if ((contact.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible) ;
                                    else if (contact.contact_id != contact_id)
                                    {
                                        if (!holder_changed)
                                        {
                                            KominServer.database.SetGroupCreator(gd.group_id, contact.contact_id);
                                            gd.creators_id = contact.contact_id;
                                            GroupHolderChange(gd);
                                            holder_changed = true;
                                        }
                                    }
                                Accept(packet);
                                LeaveGroup(gd);
                                if (!holder_changed)
                                    CloseGroup(gd.group_id);
                                break;
                            case -1:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.ServerInternalError, packet);
                                return;
                            case -2:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserNotInGroup, packet);
                                return;
                        }
                        break;
                    }
                case KominProtocolCommands.CloseGroup: //user wants to close group
                    {
                        if ((packet.content & 0x080) != 0x080)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        GroupData req_gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
                        req_gd = KominServer.database.GetGroupData(req_gd.group_id);
                        if (req_gd == null)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.GroupNotExists, packet);
                            return;
                        }
                        if (req_gd.creators_id != packet.sender)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserIsNotGroupHolder, packet);
                            return;
                        }
                        switch (CloseGroup(req_gd.group_id))
                        {
                            case 0:
                                Accept(packet);
                                break;
                            case -1:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.GroupNotExists, packet);
                                return;
                            case -2:
                                packet.DeleteContent();
                                Error(KominNetworkErrors.ServerInternalError, packet);
                                return;
                        }
                        break;
                    }
                case KominProtocolCommands.GroupHolderChange: //user wants to promote other user on group holder
                    {
                        if ((packet.content & 0x080) != 0x080)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.WrongRequestContent, packet);
                            return;
                        }
                        GroupData req_gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
                        uint new_holder = req_gd.creators_id;
                        req_gd = KominServer.database.GetGroupData(req_gd.group_id);
                        if (req_gd == null)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.GroupNotExists, packet);
                            return;
                        }
                        if (req_gd.creators_id != packet.sender)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserIsNotGroupHolder, packet);
                            return;
                        }
                        if (req_gd.creators_id == new_holder)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserIsAlreadyGroupHolder, packet);
                            return;
                        }
                        bool found = false;
                        foreach (ContactData cd in req_gd.members)
                            if (cd.contact_id == new_holder)
                            {
                                if ((cd.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                                {
                                    packet.DeleteContent();
                                    Error(KominNetworkErrors.UserNotLoggedIn, packet);
                                    return;
                                }
                                switch (KominServer.database.SetGroupCreator(req_gd.group_id, new_holder))
                                {
                                    case 0:
                                        Accept(packet);
                                        req_gd.creators_id = new_holder;
                                        GroupHolderChange(req_gd);
                                        break;
                                    case -1:
                                        packet.DeleteContent();
                                        Error(KominNetworkErrors.GroupNotExists, packet);
                                        return;
                                    case -2:
                                        packet.DeleteContent();
                                        Error(KominNetworkErrors.UserNotInGroup, packet);
                                        return;
                                }
                                found = true;
                                break;
                            }
                        if (!found)
                        {
                            packet.DeleteContent();
                            Error(KominNetworkErrors.UserNotInGroup, packet);
                            return;
                        }
                        break;
                    }
                case KominProtocolCommands.RemoveContact: //user wants to remove its account
                    break;
                /*case KominProtocolCommands.PeriodicPingRequest: //server doesn't respond to periodic ping requests
                    break;*/
                case KominProtocolCommands.PeriodicPingAnswer: //client answers to periodic ping
                    //plan next request sending
                    PingTimer.Enabled = false;
                    waiting_for_ping_answer = false;
                    had_ping_answer = true;
                    PingTimer.Interval = PingRequestSendTime;
                    PingTimer.Enabled = true;
                    break;
                case KominProtocolCommands.Disconnect: //client notifies about disconnecting
                    SelfStop();
                    break;
            }
        }

        private void InsertPacketForSending(KominNetworkPacket packet)
        {
            packets_to_send.Add(packet);
        }

        private void SendPacketToClient()
        {
            if (packets_to_send.Count == 0)
                return;

            byte[] packet_bytes = packets_to_send[0].PackForSending();
            enc_seed = KominCipherSuite.Encrypt(ref packet_bytes, enc_seed);
            int attempts = 0;
            stream.WriteTimeout = 100;
            while (attempts < 3)
            {
                try
                {
                    stream.Write(packet_bytes, 0, packet_bytes.Length);
                    break;
                }
                catch (IOException)
                {
                    attempts++;
                }
            }
            if (attempts == 3)
            {
                if (log != null) log("{E:" + enc_seed + ",D:" + dec_seed + "} Server=>Client " + client_id + ": write timeout - packet discarded");
            }
            else
            {
                if (log != null) log("{E:" + enc_seed + ",D:" + dec_seed + "} Server=>Client " + client_id + ": sender=" + packets_to_send[0].sender + "   receiver=" + packets_to_send[0].target + "   is_group=" + (packets_to_send[0].target_is_group ? "true" : "false") + "   cmd=" + ((KominProtocolCommands)packets_to_send[0].command).ToString().ToUpper() + "   jobID=" + packets_to_send[0].job_id + "   content=" + packets_to_send[0].content);
            }
            packets_to_send.RemoveAt(0);
        }

        //these methods are used when server wants to send something to client(s)
        public void NoOperation()
        {
            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target = contact_id;
            packet.target_is_group = false;
            packet.job_id = 0;
            packet.command = (uint)KominProtocolCommands.NoOperation;
            packet.DeleteContent();
            InsertPacketForSending(packet);
        }

        //public void Login() { } //server can't log any user in

        public void Logout() //server forces users logout and posts its status to groups and contacts
        {
            if (waiting_for_ping_answer && !had_ping_answer && (contact_id != 0)) ;
            else if (contact_id == 0)
                return;

            KominNetworkJob job = server.jobs.AddJob();

            //set new status
            SetStatus((uint)KominClientStatusCodes.NotAccessible);

            //check does new state influ on group existence
            UserData ud = KominServer.database.GetUserData(KominServer.database.GetContactData(contact_id).contact_name);
            foreach (GroupData gd in ud.groups)
            {
                bool holder_changed = false;
                foreach (ContactData contact in gd.members)
                    if ((contact.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible) ;
                    else if (contact.contact_id != contact_id)
                    {
                        if (!holder_changed)
                        {
                            KominServer.database.SetGroupCreator(gd.group_id, contact.contact_id);
                            gd.creators_id = contact.contact_id;
                            GroupHolderChange(gd);
                            holder_changed = true;
                        }
                    }
                if (!holder_changed)
                {
                    CloseGroup(gd.group_id);
                }
            }

            if (!(waiting_for_ping_answer && !had_ping_answer && (contact_id != 0)))
            {
                //notifiy logging out to user
                KominNetworkPacket packet = new KominNetworkPacket();
                packet.sender = 0; //server
                packet.target = contact_id;
                packet.target_is_group = false;
                packet.job_id = job.JobID;
                packet.command = (uint)KominProtocolCommands.Logout;
                packet.DeleteContent();
                InsertPacketForSending(packet);
            }

            server.jobs.FinishJob(job);
        }

        public void SetStatus(uint status = uint.MaxValue) //server sends user status change notification to all users
        //if status==uint.MaxValue then no changes in database
        //clients have to resolve is this notification valid for them
        {
            if (contact_id == 0)
                return;

            KominNetworkJob job = server.jobs.AddJob();

            //write new status into database
            if (status != uint.MaxValue)
                KominServer.database.SetUserStatus(contact_id, status);

            List<uint> contact_ids = new List<uint>();

            ContactData cd = KominServer.database.GetContactData(contact_id);
            UserData ud = KominServer.database.GetUserData(cd.contact_name);
            List<ContactData> cdl = KominServer.database.GetContactsData();
            if (cdl != null)
            {
                foreach (ContactData contact in cdl)
                {
                    if (((contact.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible))
                        contact_ids.Add(contact.contact_id);
                }
            }

            //send status notifications to contacts
            foreach (uint contact in contact_ids)
                foreach (KominServerSideConnection con in server.connections)
                    if (con.contact_id == contact)
                    {
                        KominNetworkPacket packet = new KominNetworkPacket();
                        packet.sender = 0; //server
                        packet.target = con.contact_id;
                        packet.target_is_group = false;
                        packet.job_id = job.JobID;
                        packet.command = (uint)KominProtocolCommands.SetStatus;
                        packet.DeleteContent();
                        packet.InsertContent(KominProtocolContentTypes.ContactData, cd);
                        con.InsertPacketForSending(packet);
                        break;
                    }

            server.jobs.FinishJob(job);
        }

        //public void SetPassword() { } //server is not allowed to change password

        //public void CreateContact() { } //server is not allowed to create accounts

        public void Accept(KominNetworkPacket source, bool remove_ud = true) //server accepts some user actions with this message
        {
            source.DeleteContent((uint)KominProtocolContentTypes.PasswordData);
            if (remove_ud)
                source.DeleteContent((uint)KominProtocolContentTypes.UserData);

            source.target = source.sender;
            source.sender = 0;
            source.target_is_group = false;
            source.command = (uint)KominProtocolCommands.Accept;
            InsertPacketForSending(source);
        }

        public void Deny(KominNetworkPacket source) //server denies some user actions with this message (rather use Error)
        {
            source.DeleteContent((uint)KominProtocolContentTypes.PasswordData);
            source.DeleteContent((uint)KominProtocolContentTypes.UserData);

            source.target = source.sender;
            source.sender = 0;
            source.target_is_group = false;
            source.command = (uint)KominProtocolCommands.Deny;
            InsertPacketForSending(source);
        }

        public void Error(string err, KominNetworkPacket source) //server notifies user about some error
        {
            source.DeleteContent((uint)KominProtocolContentTypes.PasswordData);
            source.DeleteContent((uint)KominProtocolContentTypes.UserData);

            source.target = source.sender;
            source.sender = 0; //server
            source.target_is_group = false; //group can't receive errors
            source.command = (uint)KominProtocolCommands.Error;
            source.InsertContent(KominProtocolContentTypes.ErrorTextData, err);
            InsertPacketForSending(source);
        }

        //public void AddContactToList() { } //server is not allowed to insert into contact lists by its own

        public void RemoveContactFromList() //server notifies about contact remove from contact list during account removing
        {
        }

        public void PingContactRequest() //during group creation, server asks user with this message to get their capabilities
        {
        }

        public void PingContactAnswer(KominNetworkPacket answer) //server answers to pings about users
        {
            answer.target = answer.sender;
            answer.sender = 0; //server
            answer.target_is_group = false;
            answer.command = (uint)KominProtocolCommands.PingContactAnswer;
            InsertPacketForSending(answer);
        }

        public void PingGroupAnswer(GroupData gd, KominNetworkPacket packet) //server answers for group ping check
        {
            packet.target = packet.sender;
            packet.sender = 0;
            packet.target_is_group = false;
            packet.command = (uint)KominProtocolCommands.PingGroupAnswer;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
            InsertPacketForSending(packet);
        }

        //public void SendMessage() { } //server can't force any user to send anything

        //public void PingMessages() { } //server don't need to ask user for stored incoming messages

        //public void RequestAudioCall() { } //server can't request a call

        //public void RequestVideoCall() { } //server can't request a call

        //public void CloseCall() { } //server can't close a call

        //public void SwitchToAudioCall() { } //server can't change call type

        //public void SwitchToVideoCall() { } //server can't change call type

        public void RequestFileTransfer() //server notifies group about new group file
        {
        }

        public void TimeoutFileTransfer() //server notifies group about file timeouts
        {
        }

        public void FinishFileTransfer() //server notifies user that group file transmission has finished
        {
        }

        //public void CreateGroup() { } //server is not allowed to create groups

        public void JoinGroup(GroupData gd) //server notifies group that new user joined it
        {
            if (contact_id == 0)
                return;

            KominNetworkJob job = server.jobs.AddJob();

            foreach (ContactData cd in gd.members)
                foreach (KominServerSideConnection con in server.connections)
                    if ((con.contact_id == cd.contact_id) && ((cd.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible))
                    {
                        KominNetworkPacket packet = new KominNetworkPacket();
                        packet.sender = 0; //server
                        packet.target = gd.group_id;
                        packet.target_is_group = true;
                        packet.command = (uint)KominProtocolCommands.JoinGroup;
                        packet.job_id = job.JobID;
                        packet.DeleteContent();
                        packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
                        con.InsertPacketForSending(packet);
                    }

            server.jobs.FinishJob(job);
        }

        public void LeaveGroup(GroupData gd) //server notifies users that user left its group
        {
            if (contact_id == 0)
                return;

            KominNetworkJob job = server.jobs.AddJob();

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target = gd.group_id;
            packet.target_is_group = true;
            packet.command = (uint)KominProtocolCommands.LeaveGroup;
            packet.job_id = job.JobID;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
            InsertPacketForSending(packet);
            foreach (ContactData cd in gd.members)
                if (cd.contact_id != contact_id)
                    foreach (KominServerSideConnection con in server.connections)
                        if ((con.contact_id == cd.contact_id) && ((cd.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible))
                        {
                            packet = new KominNetworkPacket();
                            packet.sender = 0; //server
                            packet.target = gd.group_id;
                            packet.target_is_group = true;
                            packet.command = (uint)KominProtocolCommands.LeaveGroup;
                            packet.job_id = job.JobID;
                            packet.DeleteContent();
                            packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
                            con.InsertPacketForSending(packet);
                        }

            server.jobs.FinishJob(job);
        }

        public int CloseGroup(uint group_id) //server closes group, notifies members that this group has been closed and erases group files
        {
            if (contact_id == 0)
                return -2;

            KominNetworkJob job = server.jobs.AddJob();

            RemoveSummary sum;
            switch (KominServer.database.RemoveGroup(group_id, out sum))
            {
                case 0:
                    //notify members that group has been dissolved
                    foreach (ContactData cd in sum.gd.members)
                        if ((cd.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible)
                            foreach (KominServerSideConnection con in server.connections)
                                if (cd.contact_id == con.contact_id)
                                {
                                    KominNetworkPacket packet = new KominNetworkPacket();
                                    packet.sender = 0;
                                    packet.target = sum.gd.group_id;
                                    packet.target_is_group = true;
                                    packet.command = (uint)KominProtocolCommands.CloseGroup;
                                    packet.job_id = job.JobID;
                                    packet.DeleteContent();
                                    packet.InsertContent(KominProtocolContentTypes.GroupData, sum.gd);
                                    con.InsertPacketForSending(packet);
                                }
                    //erase group files
                    if (sum.gfdl != null)
                        foreach (GroupFileData gfd in sum.gfdl)
                        {
                            try
                            {
                                //File.Delete(".\\ServerFileStore\\file" + gfd.file_id + "\\" + gfd.filename);
                                Directory.Delete(".\\ServerFileStore\\file" + gfd.file_id, true);
                            }
                            catch (Exception) { }
                        }
                    server.jobs.FinishJob(job);
                    break;
                case -1:
                    return -1;
            }

            return 0;
        }

        public void GroupHolderChange(GroupData gd) //server notifies group members that group holder has been changed
        {
            if (contact_id == 0)
                return;

            KominNetworkJob job = server.jobs.AddJob();

            foreach (ContactData cd in gd.members)
                foreach (KominServerSideConnection con in server.connections)
                    if ((con.contact_id == cd.contact_id) && ((cd.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible))
                    {
                        KominNetworkPacket packet = new KominNetworkPacket();
                        packet.sender = 0; //server
                        packet.target = gd.group_id;
                        packet.target_is_group = true;
                        packet.command = (uint)KominProtocolCommands.GroupHolderChange;
                        packet.job_id = job.JobID;
                        packet.DeleteContent();
                        packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
                        con.InsertPacketForSending(packet);
                    }

            server.jobs.FinishJob(job);
        }

        private void PeriodicPingRequest() //server sends periodic ping signal to client
        {
            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target = 0; //don't care if client know its id
            packet.target_is_group = false;
            packet.command = (uint)KominProtocolCommands.PeriodicPingRequest;
            packet.job_id = 0;
            packet.DeleteContent();
            InsertPacketForSending(packet);
        }

        private void PeriodicPingAnswer() { } //server don't answer to periodic ping
    }
}
