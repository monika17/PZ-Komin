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
        public uint job_id;

        KominServer()
        {
            server = null;
            job_id = 0;
            listener = new BackgroundWorker();
            connections = new List<KominServerSideConnection>();
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
                server = new TcpListener(IPAddress.Parse(IP), port);
                server.Start(100);
                listener.RunWorkerAsync();
            }
            catch (SocketException ex)
            {
                throw ServerSocketException("Server couldn't be created: socket error", ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (server == null)
                    return;
                listener.CancelAsync();
                foreach (KominServerSideConnection conn in connections)
                    conn.Disconnect();
            }
            catch (SocketException ex)
            {
                throw ServerSocketException("Server couldn't be stopped: socket error", ex);
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

        public Exception ServerSocketException(string msg, SocketException socket_ex)
        {
            return new Exception(msg, socket_ex);
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
                do
                {
                    packet_size = (int)KominNetworkPacket.CheckSize(ref buffer);
                    if (packet_size != 0)
                    {
                        byte[] packet_data = new ArraySegment<byte>(buffer, 0, packet_size).Array;
                        buffer = new ArraySegment<byte>(buffer, packet_size, buffer.Length - packet_size).Array;
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
                    }
                if (redirected == false)
                {
                    //############################## <-- database requirement
                    //check over errors: UserNotExists, UserNotLoggedIn
                    //send error message or store data
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
                    break;
                case KominProtocolCommands.Logout: //client tries to log out
                    break;
                case KominProtocolCommands.SetStatus: //client tries to change its status
                    break;
                case KominProtocolCommands.SetPassword: //client tries to change its password
                    break;
                case KominProtocolCommands.CreateContact: //client tries to create an account
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
                /*case KominProtocolCommands.PingContactRequest: //server and groups don't answer to ping
                    break;*/
                case KominProtocolCommands.PingContactAnswer: //client answers to ping from server
                    break;
                case KominProtocolCommands.SendMessage: //client sends group messages
                    break;
                case KominProtocolCommands.PingMessages: //client asks for stored messages
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

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target = contact_id;
            packet.target_is_group = false;
            packet.job_id = ++server.job_id;
            packet.command = (uint)KominProtocolCommands.NoOperation;
            packet.DeleteContent();
            InsertPacketForSending(ref packet);
        }

        //public void Login() { } //server can't log any user in

        public void Logout() //server forces users logout and posts its status to groups and contacts
        {
            if (contact_id == 0)
                return;

            //####################### <-- database requirement
            //get contact name => string name;
            //get status => uint status;
            //get contact id(s) in which contact exists => List<uint> contact_ids;
            //get group id(s) contact belongs to => List<uint> group_ids;
            //get group members lists excluding this contact_id => List<List<uint>> group_contact_ids;
            //status = (status & ~(uint)(LOGONSTATE_MASK))|(uint)KominContactStatus.LogOff;
            //write new status into database

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target = contact_id;
            packet.target_is_group = false;
            packet.job_id = ++server.job_id;
            packet.command = (uint)KominProtocolCommands.Logout;
            packet.DeleteContent();
            //packet.InsertContent(KominProtocolContentTypes.StatusData, status);
            InsertPacketForSending(ref packet);

            //send status notifications for contacts
            /*packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target_is_group = false;
            packet.job_id = server.job_id;
            packet.command = (uint)KominProtocolCommands.SetStatus;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.ContactNameData, name);
            packet.InsertContent(KominProtocolContentTypes.StatusData, status);
            foreach (uint contact in contact_ids)
            {
                ServerSideConnection conn;
                foreach (ServerSideConnection con in server.connections)
                    if (con.contact_id == contact)
                    {
                        conn = con;
                        break;
                    }
                packet.target = contact;
                conn.InsertPacketForSending(ref packet);
            }
            //send status notifications for groups
            packet.target_is_group = true;
            for (int i = 0; i < group_ids.Count; i++)
                foreach (uint contact in group_contact_ids[i])
                {
                    ServerSideConnection conn;
                    foreach (ServerSideConnection con in server.connections)
                        if (con.contact_id == contact)
                        {
                            conn = con;
                            break;
                        }
                    packet.target = group_ids[i];
                    conn.InsertPacketForSending(ref packet);
                }*/
        }

        public void SetStatus(KominNetworkPacket source) //server sends user status notification to all groups and contacts
        {
            if (contact_id == 0)
                return;

            //####################### <-- database requirement
            //get contact name => string name;
            //get contact id(s) in which contact exists => List<uint> contact_ids;
            //get group id(s) contact belongs to => List<uint> group_ids;
            //get group members lists excluding this contact_id => List<List<uint>> group_contact_ids;
            //write new status into database

            //send status notifications for contacts
            /*KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = contact_id;
            packet.target_is_group = false;
            packet.job_id = ++server.job_id;
            packet.command = (uint)KominProtocolCommands.SetStatus;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.ContactNameData, name);
            packet.InsertContent(KominProtocolContentTypes.StatusData, status);
            foreach (uint contact in contact_ids)
            {
                ServerSideConnection conn;
                foreach (ServerSideConnection con in server.connections)
                    if (con.contact_id == contact)
                    {
                        conn = con;
                        break;
                    }
                packet.target = contact;
                conn.InsertPacketForSending(ref packet);
            }
            //send status notifications for groups
            packet.target_is_group = true;
            for (int i = 0; i < group_ids.Count; i++)
                foreach (uint contact in group_contact_ids[i])
                {
                    ServerSideConnection conn;
                    foreach (ServerSideConnection con in server.connections)
                        if (con.contact_id == contact)
                        {
                            conn = con;
                            break;
                        }
                    packet.target = group_ids[i];
                    conn.InsertPacketForSending(ref packet);
                }*/
        }

        //public void SetPassword() { } //server is not allowed to change password

        //public void CreateContact() { } //server is not allowed to create accounts

        public void Accept(KominNetworkPacket source) //server accepts some user actions with this message
        {
        }

        public void Deny(KominNetworkPacket source) //server denies some user actions with this message (rather use Error)
        {
        }

        public void Error(KominNetworkErrors err, KominNetworkPacket source) //server notifies user about some error
        {
            if (contact_id == 0)
                return;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //server
            packet.target = contact_id;
            packet.target_is_group = false;
            packet.job_id = source.job_id;
            packet.command = (uint)KominProtocolCommands.Error;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.ErrorTextData, err.ToString());
            InsertPacketForSending(ref packet);
        }

        //public void AddContactToList() { } //server is not allowed to interfere in contact lists

        //public void RemoveContactFromList() { } //server is not allowed to interfere in contact lists

        public void PingContactRequest() //during group creation, server asks user with this message to send its capabilities
        {
        }

        //public void PingContactAnswer() { } //groups and server don't answer to ping

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

        public void CloseGroup() //server notifies user that its group has been closed
        {
        }
    }
}
