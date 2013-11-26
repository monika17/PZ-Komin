using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace Komin
{
    public class KominServer
    {
        BackgroundWorker listener;
        TcpListener server;
        public List<KominServerSideConnection> connections;
        public KominNetworkJobHolder jobs;
        public KominServerDatabase database;

        public KominServer()
        {
            server = null;
            listener = new BackgroundWorker();
            connections = new List<KominServerSideConnection>();
            jobs = new KominNetworkJobHolder();
            database = null;
            listener.WorkerSupportsCancellation = true;
            listener.DoWork += listen;
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
                foreach (KominServerSideConnection conn in connections)
                    conn.Disconnect();
                server.Stop();
                database.Disconnect();
            }
            catch (SocketException ex)
            {
                throw new ServerSocketException("Server couldn't be stopped: socket error", ex);
            }
        }

        //accepts arriving connections
        private void listen(object sender, DoWorkEventArgs e)
        {
            if (server == null)
                return;

            while (!listener.CancellationPending)
            {
                while (!server.Pending()) ;
                KominServerSideConnection conn = new KominServerSideConnection();
                conn.client = server.AcceptTcpClient();
                conn.server = this;
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
        public TcpClient client;
        NetworkStream stream;
        public BackgroundWorker commune;
        public KominServer server;
        List<KominNetworkPacket> packets_to_send;
        //user data
        uint contact_id;

        public KominServerSideConnection()
        {
            contact_id = 0;
            client = null;
            packets_to_send = new List<KominNetworkPacket>();
            commune = new BackgroundWorker();
            commune.WorkerSupportsCancellation = true;
            commune.DoWork += clientCommune;
        }

        ~KominServerSideConnection()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            if (client == null)
                return;

            Logout();

            commune.CancelAsync();
            client.Close();
            client = null;
        }

        private void clientCommune(object sender, DoWorkEventArgs e)
        {
            if (client == null)
                return;
            int packet_size = 0;
            byte[] buffer = new byte[0];
            stream = client.GetStream();

            while (!commune.CancellationPending)
            {
                while (client.Available <= 0)
                    SendPacketToClient();
                byte[] subbuffer = new byte[client.Available];
                stream.Read(subbuffer, 0, client.Available);
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
                        InterpretePacket(ref packet);
                    }
                } while (packet_size != 0);
            }

            stream.Close();
        }

        private void InterpretePacket(ref KominNetworkPacket packet)
        {
            if ((packet.sender != contact_id) ||
                ((packet.sender == 0) && ((packet.command != (uint)KominProtocolCommands.Login) &&
                                          (packet.command != (uint)KominProtocolCommands.CreateContact))))
            {
                //packet error - illegal attempt
                return;
            }

            //filter out client targeted packets - proxy
            if ((packet.target_is_group == false) && (packet.target != 0))
            {
                bool redirected = false;
                foreach (KominServerSideConnection conn in server.connections)
                    if (conn.contact_id == packet.target)
                    {
                        conn.InsertPacketForSending(ref packet);
                        redirected = true;
                        break;
                    }
                if (redirected == false)
                {
                    ContactData cd = server.database.GetContactData(packet.target);
                    if (cd == null)
                    {
                        Error(KominNetworkErrors.UserNotExists, packet);
                        return;
                    }
                    //store data
                    server.database.InsertPendingMessage(packet.sender, packet.target, false, ((TextMessage)packet.GetContent(KominProtocolContentTypes.TextMessageData)[0]).message);
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
                    UserData ud = server.database.GetUserData(req_contact_name);
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
                        break;
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
                case KominProtocolCommands.Logout: //client tries to log out
                    if (packet.content != 0)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.WrongRequestContent, packet);
                        return;
                    }
                    ContactData contd = server.database.GetContactData(packet.sender);
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
                    UserData userd = server.database.GetUserData(contd.contact_name);
                    if ((userd.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                    {
                        Error(KominNetworkErrors.UserNotLoggedIn, packet);
                        return;
                    }
                    //set new status
                    SetStatus((uint)KominClientStatusCodes.NotAccessible);
                    //confirm logging out to user
                    packet.DeleteContent();
                    packet.InsertContent(KominProtocolContentTypes.StatusData, (uint)KominClientStatusCodes.NotAccessible);
                    Accept(packet);
                    //check is new state influ on group existence
                    foreach (GroupData gd in userd.groups)
                    {
                        uint count = 0;
                        foreach (ContactData contact in gd.members)
                            if ((contact.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                                count++;
                        if (count == gd.members.Count)
                        {
                            CloseGroup(gd.group_id);
                        }
                    }
                    break;
                case KominProtocolCommands.SetStatus: //client tries to change its status
                    if ((packet.content & 0x02) != 0x02)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.WrongRequestContent, packet);
                        return;
                    }
                    if (server.database.GetContactData(packet.sender) == null)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.UserNotExists, packet);
                        return;
                    }
                    if ((server.database.GetContactData(packet.sender).status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.UserNotLoggedIn, packet);
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
                    break;
                case KominProtocolCommands.CreateContact: //client tries to create an account
                    if (packet.content != 0x41)
                    {
                        packet.DeleteContent((uint)KominProtocolContentTypes.ContactData, true);
                        Error(KominNetworkErrors.WrongRequestContent, packet);
                        return;
                    }
                    int err = server.database.CreateUser(((ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0]).contact_name, (string)packet.GetContent(KominProtocolContentTypes.PasswordData)[0]);
                    switch (err)
                    {
                        case 0:
                            Accept(packet);
                            break;
                        case -1: //user already exists
                            Error(KominNetworkErrors.UserAlreadyExists, packet);
                            break;
                    }
                    break;
                /*case KominProtocolCommands.Accept: //server doesn't make any requests to client so no need to accept nor deny anything
                    break;
                case KominProtocolCommands.Deny:
                    break;*/
                /*case KominProtocolCommands.Error: //client doesn't send any error messages to server
                    break;*/
                case KominProtocolCommands.AddContactToList: //client wants to add someone to its own contact list
                    break;
                case KominProtocolCommands.RemoveContactFromList: //client wants to remove someone from its own contact list
                    break;
                case KominProtocolCommands.PingContactRequest: //groups don't answer to ping, server answers to pings about users
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
                            packet.InsertContent(KominProtocolContentTypes.UserData, server.database.GetUserData(server.database.GetContactData(packet.sender).contact_name));
                        }
                        else //foreign ping
                        {
                            //check for user not exists
                            ContactData contact = server.database.GetContactData((uint)packet.GetContent(KominProtocolContentTypes.ContactIDData)[0]);
                            if(contact==null)
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
                        UserData req_user = server.database.GetUserData(req_contact.contact_name);
                        if (req_user == null)
                        {
                            Error(KominNetworkErrors.UserNotExists, packet);
                            return;
                        }
                        packet.DeleteContent();
                        //self-ping
                        if (packet.sender == req_user.contact_id)
                        {
                            packet.InsertContent(KominProtocolContentTypes.UserData, server.database.GetUserData(server.database.GetContactData(packet.sender).contact_name));
                        }
                        else //foreign ping
                        {
                            packet.InsertContent(KominProtocolContentTypes.ContactData, server.database.GetContactData(req_user.contact_id));
                        }
                        PingContactAnswer(packet);
                    }
                    break;
                case KominProtocolCommands.PingContactAnswer: //client answers to ping from server
                    server.jobs.MarkNewArrival(packet.job_id, ref packet);
                    break;
                case KominProtocolCommands.SendMessage: //client sends group messages
                    break;
                case KominProtocolCommands.PingMessages: //client asks for stored messages
                    if (packet.content != 0)
                    {
                        packet.DeleteContent();
                        Error(KominNetworkErrors.WrongRequestContent, packet);
                        return;
                    }
                    List<PendingMessage> pml = server.database.GetPendingMessages(packet.sender);
                    foreach (PendingMessage pm in pml)
                    {
                        KominNetworkPacket p = new KominNetworkPacket();
                        p.sender = pm.sender_id;
                        p.target = pm.receiver_id;
                        p.target_is_group = pm.receiver_is_group;
                        p.job_id = 0;
                        p.command = (uint)KominProtocolCommands.SendMessage;
                        p.DeleteContent();
                        p.InsertContent(KominProtocolContentTypes.TextMessageData, (TextMessage)pm);
                        InsertPacketForSending(ref p);
                    }
                    break;
                /*case KominProtocolCommands.RequestAudioCall: //client can't request a call from server
                    break;
                case KominProtocolCommands.RequestVideoCall:
                    break;
                case KominProtocolCommands.CloseCall: //client can't close a call with server
                    break;
                case KominProtocolCommands.SwitchToAudioCall: //client can't change call type for calls with server
                    break;
                case KominProtocolCommands.SwitchToVideoCall:
                    break;*/
                case KominProtocolCommands.RequestFileTransfer: //client asks for group file transfer (upload or download - file_id presence decide)
                    break;
                /*case KominProtocolCommands.TimeoutFileTransfer: //client can't notify group file timeout
                    break;*/
                case KominProtocolCommands.FinishFileTransfer: //client notifies about end of file transfer (upload to server)
                    server.jobs.MarkNewArrival(packet.job_id, ref packet);
                    break;
                case KominProtocolCommands.CreateGroup: //client wants to create new group
                    break;
                case KominProtocolCommands.JoinGroup: //client wants to join an existing group
                    break;
                case KominProtocolCommands.LeaveGroup: //client wants to leave an existing group
                    break;
                case KominProtocolCommands.CloseGroup: //client wants to close group
                    break;
            }
        }

        private void InsertPacketForSending(ref KominNetworkPacket packet)
        {
            packets_to_send.Add(packet);
        }

        private void SendPacketToClient()
        {
            if (packets_to_send.Count == 0)
                return;

            byte[] packet_bytes = packets_to_send[0].PackForSending();
            stream.Write(packet_bytes, 0, packet_bytes.Length);
            packets_to_send.RemoveAt(0);
        }

        //these methods are used when server wants to send something to client(s)
        public void NoOperation()
        {
            if (contact_id == 0)
                return;

            KominNetworkJob job = server.jobs.AddJob();

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target = contact_id;
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.NoOperation;
            packet.DeleteContent();
            InsertPacketForSending(ref packet);

            server.jobs.FinishJob(job);
        }

        //public void Login() { } //server can't log any user in

        public void Logout() //server forces users logout and posts its status to groups and contacts
        {
            if (contact_id == 0)
                return;

            KominNetworkJob job = server.jobs.AddJob();

            //set new status
            SetStatus((uint)KominClientStatusCodes.NotAccessible);
            
            //notifiy logging out to user
            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target = contact_id;
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.Logout;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.StatusData, (uint)KominClientStatusCodes.NotAccessible);
            InsertPacketForSending(ref packet);

            //check is new state influ on group existence
            UserData ud = server.database.GetUserData(server.database.GetContactData(contact_id).contact_name);
            foreach (GroupData gd in ud.groups)
            {
                uint count = 0;
                foreach (ContactData contact in gd.members)
                    if ((contact.status & (uint)KominClientStatusCodes.Mask) == (uint)KominClientStatusCodes.NotAccessible)
                        count++;
                if (count == gd.members.Count)
                {
                    CloseGroup(gd.group_id);
                }
            }

            server.jobs.FinishJob(job);
        }

        public void SetStatus(uint status = uint.MaxValue) //server sends user status change notification to all users
        //if status==uint.MaxValue then no changes in database
        {
            if (contact_id == 0)
                return;

            KominNetworkJob job = server.jobs.AddJob();

            //write new status into database
            if (status != uint.MaxValue)
                server.database.SetUserStatus(contact_id, status);

            List<uint> contact_ids = new List<uint>();

            ContactData cd = server.database.GetContactData(contact_id);
            UserData ud = server.database.GetUserData(cd.contact_name);
            List<ContactData> cdl = server.database.GetContactsData();
            if (cdl != null)
            {
                foreach (ContactData contact in cdl)
                {
                    if ((contact.contact_id != contact_id) &&
                        ((contact.status & (uint)KominClientStatusCodes.Mask) != (uint)KominClientStatusCodes.NotAccessible))
                        contact_ids.Add(contact.contact_id);
                }
            }

            //send status notifications to contacts
            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.SetStatus;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.ContactData, cd);
            foreach (uint contact in contact_ids)
            {
                packet.target = contact;
                foreach (KominServerSideConnection con in server.connections)
                    if (con.contact_id == contact)
                    {
                        con.InsertPacketForSending(ref packet);
                        break;
                    }
            }

            server.jobs.FinishJob(job);
        }

        //public void SetPassword() { } //server is not allowed to change password

        //public void CreateContact() { } //server is not allowed to create accounts

        public void Accept(KominNetworkPacket source) //server accepts some user actions with this message
        {
            source.DeleteContent((uint)KominProtocolContentTypes.PasswordData);
            source.DeleteContent((uint)KominProtocolContentTypes.UserData);

            source.target = source.sender;
            source.sender = 0;
            source.target_is_group = false;
            source.command = (uint)KominProtocolCommands.Accept;
            InsertPacketForSending(ref source);
        }

        public void Deny(KominNetworkPacket source) //server denies some user actions with this message (rather use Error)
        {
            source.DeleteContent((uint)KominProtocolContentTypes.PasswordData);
            source.DeleteContent((uint)KominProtocolContentTypes.UserData);

            source.target = source.sender;
            source.sender = 0;
            source.target_is_group = false;
            source.command = (uint)KominProtocolCommands.Deny;
            InsertPacketForSending(ref source);
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
            InsertPacketForSending(ref source);
        }

        //public void AddContactToList() { } //server is not allowed to interfere in contact lists

        //public void RemoveContactFromList() { } //server is not allowed to interfere in contact lists

        public void PingContactRequest() //during group creation, server asks user with this message to get their capabilities
        {
        }

        public void PingContactAnswer(KominNetworkPacket answer) //server answers to pings about users
        {
            answer.target = answer.sender;
            answer.sender = 0; //server
            answer.target_is_group = false;
            answer.command = (uint)KominProtocolCommands.PingContactAnswer;
            InsertPacketForSending(ref answer);
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

        public void JoinGroup() //server notifies user that new user joined its group
        {
        }

        public void LeaveGroup() //server notifies user that user left its group
        {
        }

        public void CloseGroup(uint group_id) //server closes group and notifies members that this group has been closed
        {
        }
    }
}
