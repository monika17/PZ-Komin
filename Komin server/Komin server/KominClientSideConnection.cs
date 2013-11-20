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
    class KominClientSideConnection
    {
        public TcpClient server;
        NetworkStream stream;
        public BackgroundWorker commune;
        List<KominNetworkPacket> packets_to_send;
        //user data
        public uint contact_id;
        public List<uint> group_ids;
        KominNetworkJobHolder jobs;

        public KominClientSideConnection()
        {
            contact_id = 0;
            group_ids = new List<uint>();
            jobs = new KominNetworkJobHolder();
            server = null;
            packets_to_send = new List<KominNetworkPacket>();
            commune = new BackgroundWorker();
            commune.WorkerSupportsCancellation = true;
            commune.DoWork += serverCommune;
        }
        
        ~KominClientSideConnection()
        {
            Disconnect();
        }

        public void Connect(string IP, int port)
        {
            try
            {
                server = new TcpClient();
                server.Connect(IPAddress.Parse(IP), port);
                jobs.Restart();
                commune.RunWorkerAsync();
            }
            catch (SocketException ex)
            {
                throw CantConnectToServer("Couldn't connect to server: socket error", ex);
            }
        }

        public void Disconnect()
        {
            if (server == null)
                return;

            Logout();

            jobs.Restart();
            commune.CancelAsync();
            server.Close();
            server = null;
        }

        public Exception CantConnectToServer(string msg, SocketException socket_ex)
        {
            return new Exception(msg, socket_ex);
        }

        private void serverCommune(object sender, DoWorkEventArgs e)
        {
            if (server == null)
                return;
            int packet_size = 0;
            byte[] buffer = new byte[0];
            stream = server.GetStream();

            while (!commune.CancellationPending)
            {
                while (server.Available <= 0)
                    SendPacketToServer();
                byte[] subbuffer = new byte[server.Available];
                stream.Read(subbuffer, 0, server.Available);
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
            //filter out packets not targeted to this contact or its groups
            bool passed = false;
            if (packet.target == contact_id)
                passed = true;
            else if (packet.target_is_group == true)
            {
                foreach (uint group_id in group_ids)
                    if (packet.target == group_id)
                        passed = true;
            }
            if (passed == false)
                return;

            //only contact or group targeted packets can reach here
            //client interpretes here received packets
            switch ((KominProtocolCommands)packet.command)
            {
                case KominProtocolCommands.NoOperation:
                    break;
                /*case KominProtocolCommands.Login: //client can't log other users into itself
                    break;*/
                case KominProtocolCommands.Logout: //client was forced to log out by server
                    break;
                case KominProtocolCommands.SetStatus: //server sent status change notification
                    break;
                /*case KominProtocolCommands.SetPassword: //client doesn't store any passwords
                    break;*/
                /*case KominProtocolCommands.CreateContact: //client is not responsible for storing account data
                    break;*/
                case KominProtocolCommands.Accept: //server/group or other user accepted something
                    break;
                case KominProtocolCommands.Deny: //server/group or other user denied something
                    break;
                case KominProtocolCommands.Error: //server notifies an error
                    break;
                /*case KominProtocolCommands.AddContactToList: //client is not allowed to change contact lists
                    break;
                case KominProtocolCommands.RemoveContactFromList:
                    break;*/
                case KominProtocolCommands.PingContactRequest: //server or other user requests ping
                    break;
                case KominProtocolCommands.PingContactAnswer: //other user answers ping
                    break;
                case KominProtocolCommands.SendMessage: //message from group or other client arrived
                    break;
                /*case KominProtocolCommands.PingMessages: //client doesn't store any messages
                    break;*/
                case KominProtocolCommands.RequestAudioCall: //other user requests an audio call
                    break;
                case KominProtocolCommands.RequestVideoCall: //other user requests a video call
                    break;
                case KominProtocolCommands.CloseCall: //other user requests to close audio or video call
                    break;
                case KominProtocolCommands.SwitchToAudioCall: //other user requests to change call type
                    break;
                case KominProtocolCommands.SwitchToVideoCall: //other user requests to change call type
                    break;
                case KominProtocolCommands.RequestFileTransfer: //other user requests file transfer or server notifies about group file presence
                    break;
                case KominProtocolCommands.TimeoutFileTransfer: //server or other user notifies about file timeout
                    break;
                case KominProtocolCommands.FinishFileTransfer: //server or other user has finished file transfer
                    break;
                /*case KominProtocolCommands.CreateGroup: //client is not responsible of group creation
                    break;*/
                case KominProtocolCommands.JoinGroup: //server notifies that user joined group
                    break;
                case KominProtocolCommands.LeaveGroup: //server notifies that user left group
                    break;
                case KominProtocolCommands.CloseGroup: //server notifies that group has been closed
                    break;
            }
        }

        private void InsertPacketForSending(ref KominNetworkPacket packet)
        {
            packets_to_send.Add(packet);
        }

        private void SendPacketToServer()
        {
            if (packets_to_send.Count == 0)
                return;

            byte[] packet_bytes = packets_to_send[0].PackForSending();
            stream.Write(packet_bytes, 0, packet_bytes.Length);
            packets_to_send.RemoveAt(0);
        }

        //these methods are used when client wants to send something to client(s) or server
        public void NoOperation(uint target, bool target_is_group)
        {
            if (contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = contact_id;
            packet.target = target;
            packet.target_is_group = target_is_group;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.NoOperation;
            packet.DeleteContent();
            InsertPacketForSending(ref packet);

            jobs.FinishJob(job.JobID);
        }

        public void Login(string contact_name, string password, ref uint new_status) //send login data to server
        {
            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //client doesn't know its contact_id yet
            packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.Login;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.ContactNameData, contact_name);
            packet.InsertContent(KominProtocolContentTypes.PasswordData, password);
            packet.InsertContent(KominProtocolContentTypes.StatusData, new_status);
            InsertPacketForSending(ref packet);

            do
            {
                job.WaitForNewArrival();
                packet = job.Packet;
                switch ((KominProtocolCommands)packet.command)
                {
                    case KominProtocolCommands.Accept:
                        contact_id = (uint)packet.GetContent(KominProtocolContentTypes.ContactIDData)[0];
                        new_status = (uint)packet.GetContent(KominProtocolContentTypes.StatusData)[0];
                        finished = true;
                        break;
                    case KominProtocolCommands.Error:
                        //######## no error messaging path yet created
                        finished = true;
                        break;
                }
            } while (!finished);

            jobs.FinishJob(job.JobID);
        }

        public void Logout() //request logout
        {
        }

        public void SetStatus(uint new_status) //send status data to server
        {
        }

        public void SetPassword(string new_password) //send new password to server
        {
        }

        public void CreateContact(string new_contact_name, string new_password) //request creation of new account
        {
        }

        public void Accept(/*...*/) //accept something
        {
        }

        public void Deny(/*...*/) //deny something
        {
        }

        //public void Error() { } //client doesn't report any errors

        public void AddContactToList(string contact_name) //request new contact on private contact list
        {
        }

        public void RemoveContactFromList(string contact_name) //request to remove contact from private contact list
        {
        }

        public void PingContactRequest(string contact_name) //ask client about its status, capabilities etc.
        {
        }

        public void PingContactAnswer(/*...*/) //answer to ping
        {
        }

        public void SendMessage(/*message_type, message*/) //send a message to contact or group
        {
        }

        public void PingMessages() //ask for sending stored messages
        {
        }

        public void RequestAudioCall(string contact_name) //request an audio call from contact
        {
        }

        public void RequestVideoCall(string contact_name) //request a video call from contact
        {
        }

        public void CloseCall(uint contact_id) //request closing of a call
        {
        }

        public void SwitchToAudioCall(uint contact_id) //request change to audio call
        {
        }

        public void SwitchToVideoCall(uint contact_id) //request change to video call
        {
        }

        public void RequestFileTransfer(string contact_name, bool is_group/*, fileinfo*/) //request file transfer (to contact, to group or from group)
        {
        }

        public void TimeoutFileTransfer(uint file_id) //notify contact about file timeout
        {
        }

        public void FinishFileTransfer(uint file_id) //notify contact or group about end of file transfer
        {
        }

        public void CreateGroup(string group_name/*, capabilities*/) //request group creation
        {
        }

        public void JoinGroup(string name, bool join_invite) //ask group (own join) or contact (invite contact) to join group
        {
        }

        public void LeaveGroup(string name, bool leave_kick) //ask group (own leave) or contact (kick) to leave group
        {
        }

        public void CloseGroup() //request closing group
        {
        }
    }
}
