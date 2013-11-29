using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Komin
{
    public class KominClientSideConnection
    {
        public TcpClient server;
        NetworkStream stream;
        public BackgroundWorker commune;
        List<KominNetworkPacket> packets_to_send;
        public UserData userdata;
        KominNetworkJobHolder jobs;

        //delegates
        public NewTextMessage onNewTextMessage;
        public NewAudioMessage onNewAudioMessage;
        public NewVideoMessage onNewVideoMessage;
        public ServerForcedLogout onServerLogout;
        public StatusNotificationArrived onStatusNotification;
        public AudioCallRequest onAudioCallRequest;
        public VideoCallRequest onVideoCallRequest;
        public CloseCallNotification onCloseCall;
        public SwitchToAudioRequest onSwitchToAudio;
        public SwitchToVideoRequest onSwitchToVideo;
        public FileTransferRequest onFileTransferRequest;
        public FileTimeoutNotification onFileTimeout;
        public FileTransferFinishedNotification onFileFinished;
        public GroupInvitation onGroupInvite;
        public ContactJoinedGroup onGroupJoin;
        public ContactLeftGroup onGroupLeave;
        public GroupClosedNotification onGroupClosed;
        public SomeError onError;

        public KominClientSideConnection()
        {
            userdata = new UserData();
            jobs = new KominNetworkJobHolder();
            server = null;
            packets_to_send = new List<KominNetworkPacket>();
            commune = new BackgroundWorker();
            commune.WorkerSupportsCancellation = true;
            commune.DoWork += serverCommune;
            onNewTextMessage = null;
            onNewAudioMessage = null;
            onNewVideoMessage = null;
            onServerLogout = null;
            onStatusNotification = null;
            onAudioCallRequest = null;
            onVideoCallRequest = null;
            onCloseCall = null;
            onSwitchToAudio = null;
            onSwitchToVideo = null;
            onFileTransferRequest = null;
            onFileTimeout = null;
            onFileFinished = null;
            onGroupInvite = null;
            onGroupJoin = null;
            onGroupLeave = null;
            onGroupClosed = null;
            onError = null;
        }

        public void Connect(string IP, int port)
        {
            try
            {
                server = new TcpClient();
                server.Connect(IPAddress.Parse(IP), port);
                packets_to_send.Clear();
                jobs.Restart();
                commune.RunWorkerAsync();
            }
            catch (SocketException ex)
            {
                if(onError!=null)
                    onError("Nie można połączyć się z serwerem: błąd socketa", null);
                else
                    throw new KominClientErrorException("Nie można połączyć się z serwerem: błąd socketa", ex);
            }
        }

        public void Disconnect()
        {
            if (server == null)
                return;
            try
            {
                if (userdata.contact_id != 0)
                    Logout();

                KominNetworkPacket p = new KominNetworkPacket();
                p.sender = 0;
                p.target = 0;
                p.target_is_group = false;
                p.command = (uint)KominProtocolCommands.Disconnect;
                p.job_id = 0;
                p.DeleteContent();
                InsertPacketForSending(p);
                while (packets_to_send.Count > 0) ;

                commune.CancelAsync();
                jobs.Restart();
                server.Close();
                server = null;
            }
            catch (SocketException ex)
            {
                if (onError != null)
                    onError("Nie można rozłączyć się z serwerem: błąd socketa", null);
                else
                    throw new KominClientErrorException("Nie można rozłączyć się z serwerem: błąd socketa", ex);
            }
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
                {
                    if (packets_to_send.Count > 0)
                        SendPacketToServer();
                    else
                        Thread.Sleep(50);
                }
                byte[] subbuffer = new byte[server.Available];
                stream.Read(subbuffer, 0, server.Available);
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
            //filter out packets not targeted to this contact or its groups
            if ((userdata.contact_id != 0) && (packet.command != (uint)KominProtocolCommands.Disconnect))
            {
                bool passed = false;
                if (packet.target == userdata.contact_id)
                    passed = true;
                else if (packet.target_is_group == true)
                {
                    foreach (GroupData group in userdata.groups)
                        if (packet.target == group.group_id)
                            passed = true;
                }
                if (passed == false)
                    return;
            }

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
                    jobs.MarkNewArrival(packet.job_id, ref packet);
                    break;
                case KominProtocolCommands.Deny: //server/group or other user denied something
                    jobs.MarkNewArrival(packet.job_id, ref packet);
                    break;
                case KominProtocolCommands.Error: //server notifies an error
                    jobs.MarkNewArrival(packet.job_id, ref packet);
                    break;
                /*case KominProtocolCommands.AddContactToList: //client is not allowed to change contact lists
                    break;
                case KominProtocolCommands.RemoveContactFromList:
                    break;*/
                case KominProtocolCommands.PingContactRequest: //server or other user requests ping
                    break;
                case KominProtocolCommands.PingContactAnswer: //other user answers ping
                    jobs.MarkNewArrival(packet.job_id, ref packet);
                    break;
                case KominProtocolCommands.SendMessage: //message from group or other client arrived
                    if ((packet.content & (uint)KominProtocolContentTypes.TextMessageData) != 0)
                    {
                        if (onNewTextMessage != null)
                            onNewTextMessage(packet.sender, packet.target, packet.target_is_group, (TextMessage)packet.GetContent(KominProtocolContentTypes.TextMessageData)[0]);
                    }
                    if ((packet.content & (uint)KominProtocolContentTypes.AudioMessageData) != 0)
                    {
                        if (onNewAudioMessage != null)
                            onNewAudioMessage(packet.sender, packet.target, packet.target_is_group, (byte[])packet.GetContent(KominProtocolContentTypes.AudioMessageData)[0]);
                    }
                    if ((packet.content & (uint)KominProtocolContentTypes.VideoMessageData) != 0)
                    {
                        if (onNewVideoMessage != null)
                            onNewVideoMessage(packet.sender, packet.target, packet.target_is_group, (byte[])packet.GetContent(KominProtocolContentTypes.VideoMessageData)[0]);
                    }
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
                    jobs.MarkNewArrival(packet.job_id, ref packet);
                    break;
                case KominProtocolCommands.FinishFileTransfer: //server or other user has finished file transfer
                    jobs.MarkNewArrival(packet.job_id, ref packet);
                    break;
                /*case KominProtocolCommands.CreateGroup: //client is not responsible of group creation
                    break;*/
                case KominProtocolCommands.JoinGroup: //server notifies that user joined group
                    break;
                case KominProtocolCommands.LeaveGroup: //server notifies that user left group
                    break;
                case KominProtocolCommands.CloseGroup: //server notifies that group has been closed
                    break;
                case KominProtocolCommands.Disconnect: //server requests disconnecting
                    commune.CancelAsync();
                    jobs.Restart();
                    server.Close();
                    server = null;
                    break;
            }
        }

        private void InsertPacketForSending(KominNetworkPacket packet)
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
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = target;
            packet.target_is_group = target_is_group;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.NoOperation;
            packet.DeleteContent();
            InsertPacketForSending(packet);

            jobs.FinishJob(job);
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
            ContactData cd = new ContactData();
            cd.contact_name = contact_name;
            packet.InsertContent(KominProtocolContentTypes.ContactData, cd);
            packet.InsertContent(KominProtocolContentTypes.PasswordData, password);
            packet.InsertContent(KominProtocolContentTypes.StatusData, new_status);
            InsertPacketForSending(packet);

            do
            {
                job.WaitForNewArrival();
                packet = job.Packet;
                switch ((KominProtocolCommands)packet.command)
                {
                    case KominProtocolCommands.Accept:
                        userdata.contact_id = (uint)packet.GetContent(KominProtocolContentTypes.ContactIDData)[0];
                        new_status = (uint)packet.GetContent(KominProtocolContentTypes.StatusData)[0];
                        //send self-ping request to get user data
                        packet.sender = userdata.contact_id;
                        packet.target = 0; //server
                        packet.target_is_group = false;
                        packet.job_id = job.JobID;
                        packet.command = (uint)KominProtocolCommands.PingContactRequest;
                        packet.DeleteContent();
                        packet.InsertContent(KominProtocolContentTypes.ContactIDData, packet.sender);
                        InsertPacketForSending(packet);
                        break;
                    case KominProtocolCommands.PingContactAnswer:
                        userdata = (UserData)packet.GetContent(KominProtocolContentTypes.UserData)[0];
                        PingMessages();
                        finished = true;
                        break;
                    case KominProtocolCommands.Error:
                        finished = true;
                        jobs.FinishJob(job);
                        if (onError != null)
                        {
                            onError((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0], packet);
                            break;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void Logout() //request logout
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.Logout;
            packet.DeleteContent();
            InsertPacketForSending(packet);

            do
            {
                job.WaitForNewArrival();
                packet = job.Packet;
                switch ((KominProtocolCommands)packet.command)
                {
                    case KominProtocolCommands.Accept:
                        userdata.contact_id = 0;
                        finished = true;
                        break;
                    case KominProtocolCommands.Error:
                        finished = true;
                        jobs.FinishJob(job);
                        if (onError != null)
                        {
                            onError((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0], packet);
                            break;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void SetStatus(uint new_status) //send status data to server
        {
        }

        public void SetPassword(string new_password) //send new password to server
        {
        }

        public void CreateContact(string new_contact_name, string new_password) //request creation of new account
        {
            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = 0; //client doesn't know its contact_id yet
            packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.CreateContact;
            packet.DeleteContent();
            ContactData cd = new ContactData();
            cd.contact_name = new_contact_name;
            packet.InsertContent(KominProtocolContentTypes.ContactData, cd);
            packet.InsertContent(KominProtocolContentTypes.PasswordData, new_password);
            InsertPacketForSending(packet);

            do
            {
                job.WaitForNewArrival();
                packet = job.Packet;
                switch ((KominProtocolCommands)packet.command)
                {
                    case KominProtocolCommands.Accept:
                        finished = true;
                        break;
                    case KominProtocolCommands.Error:
                        finished = true;
                        jobs.FinishJob(job);
                        if (onError != null)
                        {
                            onError((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0], packet);
                            break;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
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

        public ContactData PingContactRequest(uint contact_id, string contact_name="") //ask client about its status, capabilities etc.
        {
            if (userdata.contact_id == 0)
                return null;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = contact_id; //contact_name required if contact_id=0 (server)
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.PingContactRequest;
            packet.DeleteContent();
            if (contact_id != 0)
                packet.InsertContent(KominProtocolContentTypes.ContactIDData, contact_id);
            if (contact_name != "")
            {
                ContactData cd = new ContactData();
                cd.contact_id = contact_id;
                cd.contact_name = contact_name;
                packet.InsertContent(KominProtocolContentTypes.ContactData, cd);
            }
            InsertPacketForSending(packet);

            do
            {
                job.WaitForNewArrival();
                packet = job.Packet;
                switch ((KominProtocolCommands)packet.command)
                {
                    case KominProtocolCommands.PingContactAnswer:
                        finished = true;
                        break;
                    case KominProtocolCommands.Error:
                        finished = true;
                        jobs.FinishJob(job);
                        if (onError != null)
                        {
                            onError((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0], packet);
                            break;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
            return (ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0];
        }

        public void PingContactAnswer(/*...*/) //answer to ping
        {
        }

        public TextMessage SendMessage(uint receiver, bool receiver_is_group, string msg) //send a message to contact or group
        {
            if (userdata.contact_id == 0)
                return null; //basicaly it should be error: can't send messages when logged out
            if (receiver == 0)
                return null; //cannot send messages to server

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = receiver;
            packet.target_is_group = receiver_is_group;
            packet.job_id = 0; //messaging job id. all messages are directed by sender/target values and don't have return responses
            packet.command = (uint)KominProtocolCommands.SendMessage;
            packet.DeleteContent();
            TextMessage tmsg = new TextMessage();
            tmsg.send_date = DateTime.Now;
            tmsg.message = msg;
            packet.InsertContent(KominProtocolContentTypes.TextMessageData, tmsg);
            InsertPacketForSending(packet);

            return tmsg;
        }

        public void SendMessage(uint receiver, bool receiver_is_group, byte[] msg, bool is_video) //send audio or video message to contact or group
        {
            if (userdata.contact_id == 0)
                return; //basicaly it should be error: can't send messages when logged out
            if (receiver == 0)
                return; //cannot send messages to server

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = receiver;
            packet.target_is_group = receiver_is_group;
            packet.job_id = 0; //messaging job id. all messages are directed by sender/target values and don't have return responses
            packet.command = (uint)KominProtocolCommands.SendMessage;
            packet.DeleteContent();
            packet.InsertContent((is_video == false ? KominProtocolContentTypes.AudioMessageData : KominProtocolContentTypes.VideoMessageData), msg);
            InsertPacketForSending(packet);
        }

        public void PingMessages() //ask for sending stored messages
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = 0;
            packet.target_is_group = false;
            packet.job_id = 0;
            packet.command = (uint)KominProtocolCommands.PingMessages;
            packet.DeleteContent();
            InsertPacketForSending(packet);
        }

        public void RequestAudioCall(uint contact_id) //request an audio call from contact
        {
        }

        public void RequestVideoCall(uint contact_id) //request a video call from contact
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

        public void RequestFileTransfer(uint id, bool is_group, uint file_id, string filename, uint filesize)
        //request file transfer (to contact (is_group=false), to group (is_group=true, filename!="") or from group (is_group=true, file_id!=0))
        {
        }

        public void TimeoutFileTransfer(uint contact_id, uint file_id) //notify contact about file timeout
        {
        }

        public void FinishFileTransfer(uint id, bool is_group, uint file_id) //notify contact or group about end of file transfer
        {
        }

        public void CreateGroup(string group_name/*, capabilities*/) //request group creation
        {
        }

        public void JoinGroup(uint id, bool invite = false) //ask group (own join) or contact (invite contact) to join group
        {
        }

        public void LeaveGroup(uint id, bool kick = false) //ask group (own leave) or contact (kick) to leave group
        {
        }

        public void CloseGroup(uint group_id) //request closing group
        {
        }
    }

    public delegate void NewTextMessage(uint sender, uint receiver, bool receiver_is_group, TextMessage msg);
    public delegate void NewAudioMessage(uint sender, uint receiver, bool receiver_is_group, byte[] msg);
    public delegate void NewVideoMessage(uint sender, uint receiver, bool receiver_is_group, byte[] msg);
    public delegate void ServerForcedLogout();
    public delegate void StatusNotificationArrived(KominNetworkPacket packet);
    public delegate bool AudioCallRequest(KominNetworkPacket packet);
    public delegate bool VideoCallRequest(KominNetworkPacket packet);
    public delegate void CloseCallNotification(KominNetworkPacket packet);
    public delegate bool SwitchToAudioRequest(KominNetworkPacket packet);
    public delegate bool SwitchToVideoRequest(KominNetworkPacket packet);
    public delegate bool FileTransferRequest(KominNetworkPacket packet); //file part message missing
    public delegate void FileTimeoutNotification(KominNetworkPacket packet);
    public delegate void FileTransferFinishedNotification(KominNetworkPacket packet);
    public delegate bool GroupInvitation(KominNetworkPacket packet);
    public delegate void ContactJoinedGroup(KominNetworkPacket packet);
    public delegate void ContactLeftGroup(KominNetworkPacket packet);
    public delegate void GroupClosedNotification(KominNetworkPacket packet);
    public delegate void SomeError(string err_text, KominNetworkPacket packet);

    public class KominClientErrorException : Exception
    {
        public KominClientErrorException() : base() { }
        public KominClientErrorException(string message) : base("Operacja nie powiodła się\n" + message) { }
        public KominClientErrorException(string message, Exception inner) : base("Operacja nie powiodła się\n" + message, inner) { }
        protected KominClientErrorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
