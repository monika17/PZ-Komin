using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

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
        byte enc_seed, dec_seed;
        //System.Timers.Timer PingTimer;
        //bool had_pinger_nop;

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
        public UserKickedFromGroup onGroupKick;
        public GroupHolderChangeNotification onGroupHolderChange;
        public GroupClosedNotification onGroupClosed;
        public SomeError onError;
        public ContactListChange onContactListChange;

        public KominClientSideConnection()
        {
            userdata = new UserData();
            jobs = new KominNetworkJobHolder();
            server = null;
            packets_to_send = new List<KominNetworkPacket>();
            commune = new BackgroundWorker();
            commune.WorkerSupportsCancellation = true;
            commune.DoWork += serverCommune;
            enc_seed = dec_seed = KominCipherSuite.InitialSeed;
            /*PingTimer = new System.Timers.Timer(3000);
            PingTimer.AutoReset = true;
            PingTimer.Elapsed += PingTimer_Elapsed;
            had_pinger_nop = false;*/
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
            onGroupHolderChange = null;
            onGroupClosed = null;
            onError = null;
            onContactListChange = null;
        }

        /*void PingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PingTimer.Enabled = false;
            NoOperation(0, false);
            if (!had_pinger_nop)
            {
                //###################################### to do: server lost connection error
                SelfStop();
                return;
            }
            had_pinger_nop = false;
            PingTimer.Enabled = true;
        }*/

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
                server = null;
                if (onError != null)
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

                SelfStop();
            }
            catch (SocketException ex)
            {
                if (onError != null)
                    onError("Nie można rozłączyć się z serwerem: błąd socketa", null);
                else
                    throw new KominClientErrorException("Nie można rozłączyć się z serwerem: błąd socketa", ex);
            }
        }

        private void SelfStop()
        {
            while (packets_to_send.Count > 0) ;
            commune.CancelAsync();
            jobs.Restart();
            server.Close();
            server = null;
            enc_seed = dec_seed = KominCipherSuite.InitialSeed;
        }

        private void serverCommune(object sender, DoWorkEventArgs e)
        {
            if (server == null)
                return;
            int packet_size = 0;
            byte[] buffer = new byte[0];
            stream = server.GetStream();

            //PingTimer.Enabled = true;
            //NoOperation(0, false);

            while (!commune.CancellationPending)
            {
                while ((server.Available <= 0) && (!commune.CancellationPending))
                {
                    if (packets_to_send.Count > 0)
                        SendPacketToServer();
                    else
                        Thread.Sleep(50);
                }
                if (commune.CancellationPending)
                    break;
                byte[] subbuffer = new byte[server.Available];
                stream.Read(subbuffer, 0, server.Available);
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
                        //InterpretePacket(packet);
                        new PacketInterpreterThread(this, packet);
                    }
                } while (packet_size != 0);
            }

            stream.FlushAsync();
            stream.Close();

            //PingTimer.Enabled = false;
        }

        class PacketInterpreterThread
        {
            Thread th;
            KominClientSideConnection conn;
            KominNetworkPacket packet;

            public PacketInterpreterThread(KominClientSideConnection conn, KominNetworkPacket packet)
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

        private void InterpretePacket(KominNetworkPacket packet)
        {
            //filter out packets not targeted to this contact or its groups
            if ((userdata.contact_id != 0) && (packet.command != (uint)KominProtocolCommands.Disconnect))
            {
                bool passed = false;
                if ((packet.target == userdata.contact_id) && (packet.target_is_group == false))
                    passed = true;
                else if (packet.target_is_group == true)
                {
                    //passed if this is packet for group this user belong to
                    foreach (GroupData group in userdata.groups)
                        if (packet.target == group.group_id)
                            passed = true;
                    //if this user don't belong to group then check is it a JoinGroup message - it may add this user to group (resolved during JoinGroup interpretation)
                    if (!passed && packet.command == (uint)KominProtocolCommands.JoinGroup)
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
                    /*if(packet.sender==0)
                        had_pinger_nop = true;*/
                    break;
                /*case KominProtocolCommands.Login: //client can't log other users into itself
                    break;*/
                case KominProtocolCommands.Logout: //client was forced to log out by server
                    if (packet.content == 0)
                    {
                        if (onServerLogout != null)
                            onServerLogout();
                    }
                    break;
                case KominProtocolCommands.SetStatus: //server sent status change notification
                    if (packet.content == (uint)KominProtocolContentTypes.ContactData)
                    {
                        bool present = false;
                        ContactData c = (ContactData)packet.GetContent(KominProtocolContentTypes.ContactData)[0];
                        //check is this contact listed on contact list
                        foreach (ContactData cd in userdata.contacts)
                            if (cd.contact_id == c.contact_id)
                            {
                                cd.status = c.status;
                                present = true;
                                break;
                            }
                        //check is this contact listed on any group members list
                        foreach (GroupData gd in userdata.groups)
                            foreach (ContactData cd in userdata.contacts)
                                if (cd.contact_id == c.contact_id)
                                {
                                    cd.status = c.status;
                                    present = true;
                                    break;
                                }

                        if (present && onStatusNotification != null)
                            onStatusNotification(c);
                    }
                    break;
                /*case KominProtocolCommands.SetPassword: //client doesn't store any passwords
                    break;*/
                /*case KominProtocolCommands.CreateContact: //client is not responsible for storing account data
                    break;*/
                case KominProtocolCommands.Accept: //server/group or other user accepted something
                    jobs.MarkNewArrival(packet.job_id, packet);
                    break;
                case KominProtocolCommands.Deny: //server/group or other user denied something
                    jobs.MarkNewArrival(packet.job_id, packet);
                    break;
                case KominProtocolCommands.Error: //server notifies an error
                    jobs.MarkNewArrival(packet.job_id, packet);
                    break;
                /*case KominProtocolCommands.AddContactToList: //server is not allowed to insert into contact lists
                    break;*/
                case KominProtocolCommands.RemoveContactFromList: //server notifies that contacts account has been removed and therefore contact has been removed from list
                    break;
                case KominProtocolCommands.PingContactRequest: //server or other user requests ping
                    break;
                case KominProtocolCommands.PingContactAnswer: //other user answers ping
                    jobs.MarkNewArrival(packet.job_id, packet);
                    break;
                case KominProtocolCommands.PingGroupAnswer: //server answers for group name validity check
                    jobs.MarkNewArrival(packet.job_id, packet);
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
                    jobs.MarkNewArrival(packet.job_id, packet);
                    break;
                case KominProtocolCommands.FinishFileTransfer: //server or other user has finished file transfer
                    jobs.MarkNewArrival(packet.job_id, packet);
                    break;
                /*case KominProtocolCommands.CreateGroup: //client is not responsible of group creation
                    break;*/
                case KominProtocolCommands.JoinGroup: //server notifies that user joined group or group invitation (determined by packet.sender)
                    if (packet.content == (uint)KominProtocolContentTypes.GroupData)
                    {
                        GroupData changed_gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];

                        if (packet.sender == 0) //message from server - join group notification
                        {
                            //change group data
                            bool found = false;
                            for (int i = 0; i < userdata.groups.Count; i++)
                                if (userdata.groups[i].group_id == changed_gd.group_id)
                                {
                                    userdata.groups[i] = changed_gd;
                                    found = true;
                                    if (onGroupJoin != null)
                                        onGroupJoin(changed_gd);
                                    break;
                                }
                            if (!found) //new group has been created
                            {
                                //check is this user a group member - if so, add group
                                foreach (ContactData cd in changed_gd.members)
                                    if (cd.contact_id == userdata.contact_id)
                                    {
                                        userdata.groups.Add(changed_gd);
                                        if (onGroupJoin != null)
                                            onGroupJoin(changed_gd);
                                    }
                            }
                        }
                        else //group invitation
                        {
                            //check is user already in group - discard if success
                            bool found = false;
                            foreach (GroupData gd in userdata.groups)
                                if (gd.group_name == changed_gd.group_name)
                                {
                                    found = true;
                                    break;
                                }
                            if (found)
                            {
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserAlreadyInGroup, packet);
                                return;
                            }
                            //user is not in group - ask for join permission
                            foreach (ContactData cd in changed_gd.members)
                                if (cd.contact_id == packet.sender)
                                {
                                    //tell sender that user approved invitation (but without any assurance user will join group)
                                    Accept(packet);
                                    if ((onGroupInvite != null) && (onGroupInvite(changed_gd, cd) == true))
                                    {
                                        //user allowed to join group
                                        JoinGroup(changed_gd.group_id);
                                    }
                                    break;
                                }
                        }
                    }
                    break;
                case KominProtocolCommands.LeaveGroup: //server notifies that user left group
                    if (packet.content == (uint)KominProtocolContentTypes.GroupData)
                    {
                        GroupData changed_gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];

                        if (packet.sender == 0) //message from server - leave group notification
                        {
                            //change group data
                            for (int i = 0; i < userdata.groups.Count; i++)
                                if (userdata.groups[i].group_id == changed_gd.group_id) //if user has information about this group...
                                {
                                    bool found = false;
                                    foreach (ContactData cd in changed_gd.members) //...then check is he still present on member list...
                                        if (cd.contact_id == userdata.contact_id)
                                        {
                                            found = true;
                                            break;
                                        }
                                    if (found)
                                        userdata.groups[i] = changed_gd; //...update group information if he is still present...
                                    else
                                        userdata.groups.RemoveAt(i); //...remove group information if not
                                    if (onGroupLeave != null)
                                        onGroupLeave(changed_gd);
                                    break;
                                }
                        }
                        else //kick from group
                        {
                            //check is user in group - discard if failed
                            bool found = false;
                            foreach (GroupData gd in userdata.groups)
                                if (gd.group_name == changed_gd.group_name)
                                {
                                    found = true;
                                    break;
                                }
                            if (!found)
                            {
                                packet.DeleteContent();
                                Error(KominNetworkErrors.UserNotInGroup, packet);
                                return;
                            }
                            //user is in group - commit leave group because of kick
                            if (changed_gd.creators_id == packet.sender)
                            {
                                //tell sender that user approved kick
                                Accept(packet);
                                LeaveGroup(changed_gd.group_id);
                                if (onGroupKick != null)
                                    onGroupKick(changed_gd);
                            }
                        }
                    }
                    break;
                case KominProtocolCommands.CloseGroup: //server notifies that group has been closed
                    if (packet.content == (uint)KominProtocolContentTypes.GroupData)
                    {
                        GroupData new_gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
                        //update group data
                        for (int i = 0; i < userdata.groups.Count; i++)
                        {
                            GroupData gd = userdata.groups[i];
                            if (gd.group_id == new_gd.group_id)
                            {
                                userdata.groups.RemoveAt(i);
                                if (onGroupClosed != null)
                                    onGroupClosed(gd);
                                break;
                            }
                        }
                    }
                    break;
                case KominProtocolCommands.GroupHolderChange: //server notifies that group holder has been changed
                    if (packet.content == (uint)KominProtocolContentTypes.GroupData)
                    {
                        GroupData new_gd = (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
                        //update group data
                        foreach (GroupData gd in userdata.groups)
                            if (gd.group_id == new_gd.group_id)
                            {
                                gd.creators_id = new_gd.creators_id;
                                if (onGroupHolderChange != null)
                                    onGroupHolderChange(new_gd);
                                break;
                            }
                    }
                    break;
                /*case KominProtocolCommands.RemoveContact: //server can't remove accounts by its own
                    break;*/
                case KominProtocolCommands.Disconnect: //server requests disconnecting
                    SelfStop();
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
            enc_seed = KominCipherSuite.Encrypt(ref packet_bytes, enc_seed);
            stream.WriteTimeout = 100;
            int attempts = 0;
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
            packets_to_send.RemoveAt(0);
        }

        //these methods are used when client wants to send something to client(s) or server
        public void NoOperation(uint target, bool target_is_group)
        {
            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = target;
            packet.target_is_group = target_is_group;
            packet.job_id = 0;
            packet.command = (uint)KominProtocolCommands.NoOperation;
            packet.DeleteContent();
            InsertPacketForSending(packet);
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
                            return;
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
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void SetStatus(uint new_status) //send status data to server
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
            packet.command = (uint)KominProtocolCommands.SetStatus;
            packet.DeleteContent();
            packet.InsertContent(KominProtocolContentTypes.StatusData, new_status);
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
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void SetPassword(string new_password) //send new password to server
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id; //client doesn't know its contact_id yet
            packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.SetPassword;
            packet.DeleteContent();
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
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
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
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void Accept(KominNetworkPacket source) //client accepts some user actions with this message
        {
            source.DeleteContent((uint)KominProtocolContentTypes.PasswordData);
            source.DeleteContent((uint)KominProtocolContentTypes.UserData);

            source.target = source.sender;
            source.sender = userdata.contact_id;
            source.target_is_group = false;
            source.command = (uint)KominProtocolCommands.Accept;
            InsertPacketForSending(source);
        }

        public void Deny(KominNetworkPacket source) //client denies some user actions with this message
        {
            source.DeleteContent((uint)KominProtocolContentTypes.PasswordData);
            source.DeleteContent((uint)KominProtocolContentTypes.UserData);

            source.target = source.sender;
            source.sender = userdata.contact_id;
            source.target_is_group = false;
            source.command = (uint)KominProtocolCommands.Deny;
            InsertPacketForSending(source);
        }

        public void Error(string err, KominNetworkPacket source) //client report other client about some error
        {
            source.DeleteContent((uint)KominProtocolContentTypes.PasswordData);
            source.DeleteContent((uint)KominProtocolContentTypes.UserData);

            source.target = source.sender;
            source.sender = userdata.contact_id;
            source.target_is_group = false;
            source.command = (uint)KominProtocolCommands.Error;
            source.InsertContent(KominProtocolContentTypes.ErrorTextData, err);
            InsertPacketForSending(source);
        }

        public void AddContactToList(string contact_name) //request new contact on private contact list
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id; //client doesn't know its contact_id yet
            packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.AddContactToList;
            packet.DeleteContent();
            ContactData cd = new ContactData();
            cd.contact_name = contact_name;
            packet.InsertContent(KominProtocolContentTypes.ContactData, cd);
            InsertPacketForSending(packet);

            do
            {
                job.WaitForNewArrival();
                packet = job.Packet;
                switch ((KominProtocolCommands)packet.command)
                {
                    case KominProtocolCommands.Accept:
                        if (onContactListChange != null)
                            onContactListChange((UserData)packet.GetContent(KominProtocolContentTypes.UserData)[0]);
                        finished = true;
                        break;
                    case KominProtocolCommands.Error:
                        finished = true;
                        jobs.FinishJob(job);
                        if (onError != null)
                        {
                            onError((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0], packet);
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void RemoveContactFromList(string contact_name) //request to remove contact from private contact list
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id; //client doesn't know its contact_id yet
            packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.RemoveContactFromList;
            packet.DeleteContent();
            ContactData cd = new ContactData();
            cd.contact_name = contact_name;
            packet.InsertContent(KominProtocolContentTypes.ContactData, cd);
            InsertPacketForSending(packet);

            do
            {
                job.WaitForNewArrival();
                packet = job.Packet;
                switch ((KominProtocolCommands)packet.command)
                {
                    case KominProtocolCommands.Accept:
                        if (onContactListChange != null)
                            onContactListChange((UserData)packet.GetContent(KominProtocolContentTypes.UserData)[0]);
                        finished = true;
                        break;
                    case KominProtocolCommands.Error:
                        finished = true;
                        jobs.FinishJob(job);
                        if (onError != null)
                        {
                            onError((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0], packet);
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public ContactData PingContactRequest(uint contact_id, string contact_name = "") //ask client about its status, capabilities etc.
        {
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
                            return null;
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

        public GroupData PingGroupRequest(string group_name) //ask server about group name validity
        {
            if (userdata.contact_id == 0)
                return null;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.PingGroupRequest;
            packet.DeleteContent();
            GroupData gd = new GroupData();
            gd.group_name = group_name;
            packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
            InsertPacketForSending(packet);

            do
            {
                job.WaitForNewArrival();
                packet = job.Packet;
                switch ((KominProtocolCommands)packet.command)
                {
                    case KominProtocolCommands.PingGroupAnswer:
                        finished = true;
                        break;
                    case KominProtocolCommands.Error:
                        finished = true;
                        jobs.FinishJob(job);
                        if (onError != null)
                        {
                            onError((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0], packet);
                            return null;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
            return (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
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

        public GroupData CreateGroup(string group_name, uint capabilities) //request group creation
        {
            if (userdata.contact_id == 0)
                return null;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.CreateGroup;
            packet.DeleteContent();
            GroupData gd = new GroupData();
            gd.creators_id = userdata.contact_id;
            gd.group_name = group_name;
            gd.communication_type = capabilities & 0x130;
            packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
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
                            return null;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
            return (GroupData)packet.GetContent(KominProtocolContentTypes.GroupData)[0];
        }

        public void JoinGroup(uint group_id, bool invite = false, uint invited_id = 0) //ask group (own join) or contact (invite contact) to join group
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            if (invite)
                packet.target = invited_id;
            else
                packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.JoinGroup;
            packet.DeleteContent();
            GroupData g = new GroupData();
            g.group_id = group_id;
            foreach (GroupData gd in userdata.groups) //find is there any group information. it will be for group invitations...
                if (group_id == gd.group_id)
                {
                    g = gd;
                    break;
                }
            packet.InsertContent(KominProtocolContentTypes.GroupData, g);
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
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void LeaveGroup(uint group_id, bool kick = false, uint kicked_id = 0) //ask group (own leave) or contact (kick) to leave group
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            if (kick)
                packet.target = kicked_id;
            else
                packet.target = 0; //server
            packet.target_is_group = false;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.LeaveGroup;
            packet.DeleteContent();
            GroupData g = new GroupData();
            g.group_id = group_id;
            foreach (GroupData gd in userdata.groups)
                if (group_id == gd.group_id)
                {
                    g = gd;
                    break;
                }
            packet.InsertContent(KominProtocolContentTypes.GroupData, g);
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
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void CloseGroup(uint group_id) //request closing group
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = group_id;
            packet.target_is_group = true;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.CloseGroup;
            packet.DeleteContent();
            GroupData gd = new GroupData();
            gd.group_id = group_id;
            packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
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
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }

        public void ChangeGroupHolder(uint group_id, uint new_holder_id) //request changing group holder to specified member. further GroupData change notification will confirm
        {
            if (userdata.contact_id == 0)
                return;

            KominNetworkJob job = jobs.AddJob();
            bool finished = false;

            KominNetworkPacket packet = new KominNetworkPacket();
            packet.sender = userdata.contact_id;
            packet.target = group_id;
            packet.target_is_group = true;
            packet.job_id = job.JobID;
            packet.command = (uint)KominProtocolCommands.GroupHolderChange;
            packet.DeleteContent();
            GroupData gd = new GroupData();
            gd.group_id = group_id;
            gd.creators_id = new_holder_id;
            packet.InsertContent(KominProtocolContentTypes.GroupData, gd);
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
                            return;
                        }
                        else
                            throw new KominClientErrorException((string)packet.GetContent(KominProtocolContentTypes.ErrorTextData)[0]);
                }
            } while (!finished);

            jobs.FinishJob(job);
        }
    }

    public delegate void NewTextMessage(uint sender, uint receiver, bool receiver_is_group, TextMessage msg);
    public delegate void NewAudioMessage(uint sender, uint receiver, bool receiver_is_group, byte[] msg);
    public delegate void NewVideoMessage(uint sender, uint receiver, bool receiver_is_group, byte[] msg);
    public delegate void ServerForcedLogout();
    public delegate void StatusNotificationArrived(ContactData changed_contact);
    public delegate bool AudioCallRequest(KominNetworkPacket packet);
    public delegate bool VideoCallRequest(KominNetworkPacket packet);
    public delegate void CloseCallNotification(KominNetworkPacket packet);
    public delegate bool SwitchToAudioRequest(KominNetworkPacket packet);
    public delegate bool SwitchToVideoRequest(KominNetworkPacket packet);
    public delegate bool FileTransferRequest(KominNetworkPacket packet); //file part message missing
    public delegate void FileTimeoutNotification(KominNetworkPacket packet);
    public delegate void FileTransferFinishedNotification(KominNetworkPacket packet);
    public delegate bool GroupInvitation(GroupData gd, ContactData invitor);
    public delegate void ContactJoinedGroup(GroupData new_gd);
    public delegate void ContactLeftGroup(GroupData new_gd);
    public delegate void UserKickedFromGroup(GroupData gd); //group in params is the group from which user has been kicked
    public delegate void GroupHolderChangeNotification(GroupData new_gd);
    public delegate void GroupClosedNotification(GroupData gd);
    public delegate void SomeError(string err_text, KominNetworkPacket packet);
    public delegate void ContactListChange(UserData new_ud);

    public class KominClientErrorException : Exception
    {
        public KominClientErrorException() : base() { }
        public KominClientErrorException(string message) : base(message) { }
        public KominClientErrorException(string message, Exception inner) : base(message, inner) { }
        protected KominClientErrorException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class watek
    {
        Thread th;

        public watek()
        {
            th = new Thread(starter);
            watek ob = null;
            th.Start(ob);
        }

        private void starter(object ob)
        {
        }
    }
}
