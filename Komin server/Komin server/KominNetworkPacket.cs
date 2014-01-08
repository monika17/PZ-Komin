using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Komin
{
    public class KominNetworkPacket
    {
        public static int HeaderSize = sizeof(uint) * 6 + 1 /*sizeof(bool)*/;

        public uint sender; //contact_id (0 is server)
        public uint target; //contact_id (0 is server)
        public bool target_is_group;
        public uint command;
        public uint job_id; //for differing separate requests; complete identifier is created by sender+job_id of first message
        public uint content;
        public uint content_length; //in bytes
        //public int part; //-1 if not parted
        public byte[] data;

        private string password;
        private uint status;
        private uint contact_id;
        private TextMessage text_msg;
        private byte[] audio_msg;
        private byte[] video_msg;
        private ContactData contact;
        private GroupData group;
        private FileData file;
        private string error_text;
        private UserData userdata;
        /*private string sms_number;
        private string sms_text;*/

        public KominNetworkPacket()
        {
            sender = 0;
            target = 0;
            target_is_group = false;
            command = 0;
            job_id = 0;
            content = 0;
            content_length = 0;
            data = null;

            password = "";
            status = 0;
            contact_id = 0;
            text_msg = null;
            audio_msg = new byte[0];
            video_msg = new byte[0];
            contact = null;
            group = null;
            file = null;
            error_text = "";
            userdata = null;
            /*sms_number = "";
            sms_text = "";*/
        }

        static byte[] UIntToByteArray(uint i)
        {
            byte[] output = new byte[4];
            output[0] = (byte)(i & 0xff);
            output[1] = (byte)((i >> 8) & 0xff);
            output[2] = (byte)((i >> 16) & 0xff);
            output[3] = (byte)((i >> 24) & 0xff);
            return output;
        }

        static uint ByteArrayToUInt(byte[] ba, int offset = 0)
        {
            return (((uint)ba[offset + 3]) << 24) + (((uint)ba[offset + 2]) << 16) + (((uint)ba[offset + 1]) << 8) + ((uint)ba[offset]);
        }

        static byte[] BoolToByteArray(bool b)
        {
            byte[] output = new byte[1];
            if (b == true)
                output[0] = 1;
            else
                output[0] = 0;
            return output;
        }

        static bool ByteArrayToBool(byte[] ba, int offset = 0)
        {
            return (ba[offset] == 1 ? true : false);
        }

        static byte[] StringToByteArray(string s)
        {
            byte[] bytes = new byte[s.Length * sizeof(char)];
            Buffer.BlockCopy(s.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string ByteArrayToString(byte[] ba, int offset = 0, int length_in_bytes = -1)
        {
            if (length_in_bytes == -1)
                length_in_bytes = ba.Length;
            if (length_in_bytes == 0)
                return "";
            char[] chars = new char[length_in_bytes / sizeof(char)];
            Buffer.BlockCopy(ba, offset, chars, 0, length_in_bytes);
            return new string(chars);
        }

        static byte[] DateTimeToByteArray(DateTime dt)
        {
            byte[] output = new byte[7];
            output[0] = (byte)dt.Second;
            output[1] = (byte)dt.Minute;
            output[2] = (byte)dt.Hour;
            output[3] = (byte)dt.Day;
            output[4] = (byte)dt.Month;
            output[5] = (byte)((dt.Year >> 8) & 0xff);
            output[6] = (byte)(dt.Year & 0xff);
            return output;
        }

        static DateTime ByteArrayToDateTime(byte[] ba, int offset = 0)
        {
            return new DateTime((((int)ba[5]) << 8) + ba[6], ba[4], ba[3], ba[2], ba[1], ba[0]);
        }

        public static uint CheckSize(ref byte[] buffer)
        {
            if (buffer.Length < HeaderSize)
                return 0;
            uint command = ByteArrayToUInt(buffer, sizeof(uint) * 2 + 1/*sizeof(bool)*/);
            if (command > (uint)KominProtocolCommands.MaxValue) //packet malfunction - discard buffer
            {
                Array.Resize<byte>(ref buffer, 0);
                return 0;
            }
            uint content = ByteArrayToUInt(buffer, sizeof(uint) * 4 + 1/*sizeof(bool)*/);
            uint content_length = ByteArrayToUInt(buffer, sizeof(uint) * 5 + 1/*sizeof(bool)*/);
            uint min_length = 0;
            if ((content & (uint)KominProtocolContentTypes.PasswordData) != 0)
                min_length += (uint)sizeof(uint);
            if ((content & (uint)KominProtocolContentTypes.StatusData) != 0)
                min_length += (uint)sizeof(uint);
            if ((content & (uint)KominProtocolContentTypes.ContactIDData) != 0)
                min_length += (uint)sizeof(uint);
            if ((content & (uint)KominProtocolContentTypes.TextMessageData) != 0)
                min_length += (uint)(7 + sizeof(uint));
            if ((content & (uint)KominProtocolContentTypes.AudioMessageData) != 0)
                min_length += (uint)sizeof(uint);
            if ((content & (uint)KominProtocolContentTypes.VideoMessageData) != 0)
                min_length += (uint)sizeof(uint);
            if ((content & (uint)KominProtocolContentTypes.ContactData) != 0)
                min_length += (uint)(sizeof(uint) * 3);
            if ((content & (uint)KominProtocolContentTypes.GroupData) != 0)
                min_length += (uint)(sizeof(uint) * 5);
            if ((content & (uint)KominProtocolContentTypes.FileData) != 0)
                min_length += (uint)(sizeof(uint) * 5 + 14);
            if ((content & (uint)KominProtocolContentTypes.ErrorTextData) != 0)
                min_length += (uint)sizeof(uint);
            if ((content & (uint)KominProtocolContentTypes.UserData) != 0)
                min_length += (uint)(sizeof(uint) * 5);
            /*if ((content & (uint)KominProtocolContentTypes.SMSData) != 0)
                min_length += (uint)(9 * sizeof(char) + sizeof(uint));*/
            if (content_length < min_length) //packet malfunction - discard buffer
            {
                Array.Resize<byte>(ref buffer, 0);
                return 0;
            }
            if (buffer.Length < HeaderSize + content_length)
                return 0;
            return (uint)HeaderSize + content_length;
        }

        public byte[] PackForSending()
        {
            CreateDataBlock();
            uint size = (uint)(HeaderSize + data.Length);
            byte[] db = new byte[size];
            int offset = 0;
            Buffer.BlockCopy(UIntToByteArray(sender), 0, db, offset, sizeof(uint));
            offset += sizeof(uint);
            Buffer.BlockCopy(UIntToByteArray(target), 0, db, offset, sizeof(uint));
            offset += sizeof(uint);
            Buffer.BlockCopy(BoolToByteArray(target_is_group), 0, db, offset, 1 /*sizeof(bool)*/);
            offset += 1 /*sizeof(bool)*/;
            Buffer.BlockCopy(UIntToByteArray(command), 0, db, offset, sizeof(uint));
            offset += sizeof(uint);
            Buffer.BlockCopy(UIntToByteArray(job_id), 0, db, offset, sizeof(uint));
            offset += sizeof(uint);
            Buffer.BlockCopy(UIntToByteArray(content), 0, db, offset, sizeof(uint));
            offset += sizeof(uint);
            Buffer.BlockCopy(UIntToByteArray(content_length), 0, db, offset, sizeof(uint));
            offset += sizeof(uint);
            Buffer.BlockCopy(data, 0, db, offset, data.Length);
            return db;
        }

        public void UnpackReceivedPacket(ref byte[] db)
        {
            int offset = 0;

            sender = ByteArrayToUInt(db, offset);
            offset += sizeof(uint);
            target = ByteArrayToUInt(db, offset);
            offset += sizeof(uint);
            target_is_group = ByteArrayToBool(db, offset);
            offset += 1 /*sizeof(uint)*/;
            command = ByteArrayToUInt(db, offset);
            offset += sizeof(uint);
            job_id = ByteArrayToUInt(db, offset);
            offset += sizeof(uint);
            content = ByteArrayToUInt(db, offset);
            offset += sizeof(uint);
            content_length = ByteArrayToUInt(db, offset);
            offset += sizeof(uint);
            data = new byte[content_length];
            Buffer.BlockCopy(db, offset, data, 0, (int)content_length);
            InterpretDataBlock();
        }

        private void CreateDataBlock()
        {
            data = new byte[content_length];
            int offset = 0;

            //password data
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.PasswordData)) != 0)
            {
                byte[] pwd = StringToByteArray(password);
                Buffer.BlockCopy(UIntToByteArray((uint)pwd.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(pwd, 0, data, offset, pwd.Length);
                offset += pwd.Length;
            }

            //status data:
            //status : uint
            if ((content & ((uint)KominProtocolContentTypes.StatusData)) != 0)
            {
                Buffer.BlockCopy(UIntToByteArray(status), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
            }

            //contact id data:
            //contact_id : uint
            if ((content & ((uint)KominProtocolContentTypes.ContactIDData)) != 0)
            {
                Buffer.BlockCopy(UIntToByteArray(contact_id), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
            }

            //text message data:
            //send_datetime : byte[7] (2 bytes for year, 1 byte for each of month, day, hour, minute, second)
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.TextMessageData)) != 0)
            {
                Buffer.BlockCopy(DateTimeToByteArray(text_msg.send_date), 0, data, offset, 7);
                offset += 7;
                byte[] text = StringToByteArray(text_msg.message);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
            }

            //audio message data:
            //size : uint (in bytes)
            //data : byte[size]
            if ((content & ((uint)KominProtocolContentTypes.AudioMessageData)) != 0)
            {
                if (audio_msg == null)
                    audio_msg = new byte[0];
                Buffer.BlockCopy(UIntToByteArray((uint)audio_msg.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(audio_msg, 0, data, offset, audio_msg.Length);
                offset += audio_msg.Length;
            }

            //video message data:
            //size : uint (in bytes)
            //data : byte[size]
            if ((content & ((uint)KominProtocolContentTypes.VideoMessageData)) != 0)
            {
                if (video_msg == null)
                    video_msg = new byte[0];
                Buffer.BlockCopy(UIntToByteArray((uint)video_msg.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(video_msg, 0, data, offset, video_msg.Length);
                offset += video_msg.Length;
            }

            //contact data:
            //contact_id : uint
            //contact_name_length : uint
            //contact_name : char[contact_name_length]
            //status : uint
            if ((content & ((uint)KominProtocolContentTypes.ContactData)) != 0)
            {
                Buffer.BlockCopy(UIntToByteArray((uint)contact.contact_id), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                byte[] text = StringToByteArray(contact.contact_name);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
                Buffer.BlockCopy(UIntToByteArray((uint)contact.status), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
            }

            //group data:
            //group_id : uint
            //group_name_length : uint
            //group_name : char[group_name_length]
            //communication_type : uint
            //creators_id : uint
            //members_count : uint
            //struct{
            //    contact_id : uint
            //    contact_name_length : uint
            //    contact_name : char[contact_name_length]
            //    status : uint
            //} [members_count]
            if ((content & ((uint)KominProtocolContentTypes.GroupData)) != 0)
            {
                Buffer.BlockCopy(UIntToByteArray((uint)group.group_id), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                byte[] text = StringToByteArray(group.group_name);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
                Buffer.BlockCopy(UIntToByteArray((uint)group.communication_type), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(UIntToByteArray((uint)group.creators_id), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(UIntToByteArray((uint)group.members.Count), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                foreach (ContactData cd in group.members)
                {
                    Buffer.BlockCopy(UIntToByteArray((uint)cd.contact_id), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    text = StringToByteArray(cd.contact_name);
                    Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    Buffer.BlockCopy(text, 0, data, offset, text.Length);
                    offset += text.Length;
                    Buffer.BlockCopy(UIntToByteArray((uint)cd.status), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                }
            }

            //file data:
            //file_id : uint
            //filename_length : uint
            //filename_chars : char[length]
            //filesize : uint
            //upload_datetime : byte[7] (2 bytes for year, 1 byte for each of month, day, hour, minute, second)
            //timeout_datetime : byte[7] (2 bytes for year, 1 byte for each of month, day, hour, minute, second)
            //part_seq : uint
            //filedata_length : uint
            //filedata : byte[filedata_length]
            if ((content & ((uint)KominProtocolContentTypes.FileData)) != 0)
            {
                Buffer.BlockCopy(UIntToByteArray(file.file_id), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                byte[] text = StringToByteArray(file.filename);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
                Buffer.BlockCopy(UIntToByteArray(file.filesize), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(DateTimeToByteArray(file.upload), 0, data, offset, 7);
                offset += 7;
                Buffer.BlockCopy(DateTimeToByteArray(file.timeout), 0, data, offset, 7);
                offset += 7;
                Buffer.BlockCopy(UIntToByteArray(file.part_seq), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                if (file.filedata == null)
                    file.filedata = new byte[0];
                Buffer.BlockCopy(UIntToByteArray((uint)file.filedata.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(file.filedata, 0, data, offset, file.filedata.Length);
                offset += file.filedata.Length;
            }

            //error text data:
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.ErrorTextData)) != 0)
            {
                byte[] text = StringToByteArray(error_text);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
            }

            //user data:
            //contact_id : uint
            //contact_name_length : uint
            //contact_name : char[contact_name_length]
            //status : uint
            //contacts_count : uint
            //struct{
            //    contact_id : uint
            //    contact_name_length : uint
            //    contact_name : char[contact_name_length]
            //    status : uint
            //} [contacts_count]
            //groups_count : uint
            //struct{
            //    group_id : uint
            //    group_name_length : uint
            //    group_name : char[group_name_length]
            //    communication_type : uint
            //    creators_id : uint;
            //    members_count : uint
            //    struct{
            //        contact_id : uint
            //        contact_name_length : uint
            //        contact_name : char[contact_name_length]
            //        status : uint
            //    } [members_count]
            //} [groups_count]
            if ((content & ((uint)KominProtocolContentTypes.UserData)) != 0)
            {
                Buffer.BlockCopy(UIntToByteArray((uint)userdata.contact_id), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                byte[] text = StringToByteArray(userdata.contact_name);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
                Buffer.BlockCopy(UIntToByteArray((uint)userdata.status), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(UIntToByteArray((uint)userdata.contacts.Count), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                foreach (ContactData cd in userdata.contacts)
                {
                    Buffer.BlockCopy(UIntToByteArray((uint)cd.contact_id), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    text = StringToByteArray(cd.contact_name);
                    Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    Buffer.BlockCopy(text, 0, data, offset, text.Length);
                    offset += text.Length;
                    Buffer.BlockCopy(UIntToByteArray((uint)cd.status), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                }
                Buffer.BlockCopy(UIntToByteArray((uint)userdata.groups.Count), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                foreach (GroupData gd in userdata.groups)
                {
                    Buffer.BlockCopy(UIntToByteArray((uint)gd.group_id), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    text = StringToByteArray(gd.group_name);
                    Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    Buffer.BlockCopy(text, 0, data, offset, text.Length);
                    offset += text.Length;
                    Buffer.BlockCopy(UIntToByteArray((uint)gd.communication_type), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    Buffer.BlockCopy(UIntToByteArray((uint)gd.creators_id), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    Buffer.BlockCopy(UIntToByteArray((uint)gd.members.Count), 0, data, offset, sizeof(uint));
                    offset += sizeof(uint);
                    foreach (ContactData cd in gd.members)
                    {
                        Buffer.BlockCopy(UIntToByteArray((uint)cd.contact_id), 0, data, offset, sizeof(uint));
                        offset += sizeof(uint);
                        text = StringToByteArray(cd.contact_name);
                        Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                        offset += sizeof(uint);
                        Buffer.BlockCopy(text, 0, data, offset, text.Length);
                        offset += text.Length;
                        Buffer.BlockCopy(UIntToByteArray((uint)cd.status), 0, data, offset, sizeof(uint));
                        offset += sizeof(uint);
                    }
                }
            }

            //(?) SMS data:
            //number : char[9]
            //length : uint
            //chars : char[length]
            /*if ((content & ((uint)KominProtocolContentTypes.SMSData)) != 0)
            {
                byte[] text = StringToByteArray(sms_number);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
                text = StringToByteArray(sms_text);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
            }*/
        }

        private void InterpretDataBlock()
        {
            int offset = 0;

            //password data
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.PasswordData)) != 0)
            {
                int pwd_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                password = ByteArrayToString(data, offset, pwd_length);
                offset += pwd_length;
            }

            //status data:
            //status : uint
            if ((content & ((uint)KominProtocolContentTypes.StatusData)) != 0)
            {
                status = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
            }

            //contact id data:
            //contact_id : uint
            if ((content & ((uint)KominProtocolContentTypes.ContactIDData)) != 0)
            {
                contact_id = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
            }

            //text message data:
            //send_datetime : byte[7] (2 bytes for year, 1 byte for each of month, day, hour, minute, second)
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.TextMessageData)) != 0)
            {
                text_msg = new TextMessage();
                text_msg.send_date = ByteArrayToDateTime(data, offset);
                offset += 7;
                int text_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                text_msg.message = ByteArrayToString(data, offset, text_length);
                offset += text_length;
            }

            //audio message data:
            //size : uint (in bytes)
            //data : byte[size]
            if ((content & ((uint)KominProtocolContentTypes.AudioMessageData)) != 0)
            {
                int audio_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                audio_msg = new byte[audio_length];
                Buffer.BlockCopy(data, offset, audio_msg, 0, audio_length);
                offset += audio_length;
            }

            //video message data:
            //size : uint (in bytes)
            //data : byte[size]
            if ((content & ((uint)KominProtocolContentTypes.VideoMessageData)) != 0)
            {
                int video_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                video_msg = new byte[video_length];
                Buffer.BlockCopy(data, offset, video_msg, 0, video_length);
                offset += video_length;
            }

            //contact data:
            //contact_id : uint
            //contact_name_length : uint
            //contact_name : char[contact_name_length]
            //status : uint
            if ((content & ((uint)KominProtocolContentTypes.ContactData)) != 0)
            {
                contact = new ContactData();
                contact.contact_id = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                int text_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                contact.contact_name = ByteArrayToString(data, offset, text_length);
                offset += text_length;
                contact.status = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
            }

            //group data:
            //group_id : uint
            //group_name_length : uint
            //group_name : char[group_name_length]
            //communication_type : uint
            //creators_id : uint
            //members_count : uint
            //struct{
            //    contact_id : uint
            //    contact_name_length : uint
            //    contact_name : char[contact_name_length]
            //    status : uint
            //} [members_count]
            if ((content & ((uint)KominProtocolContentTypes.GroupData)) != 0)
            {
                group = new GroupData();
                group.group_id = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                int text_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                group.group_name = ByteArrayToString(data, offset, text_length);
                offset += text_length;
                group.communication_type = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                group.creators_id = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                int contacts_count = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                group.members.Clear();
                for (; contacts_count > 0; contacts_count--)
                {
                    ContactData cd = new ContactData();
                    cd.contact_id = ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    text_length = (int)ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    cd.contact_name = ByteArrayToString(data, offset, text_length);
                    offset += text_length;
                    cd.status = ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    group.members.Add(cd);
                }
            }

            //file data:
            //file_id : uint
            //filename_length : uint
            //filename_chars : char[length]
            //filesize : uint
            //upload_datetime : byte[7] (2 bytes for year, 1 byte for each of month, day, hour, minute, second)
            //timeout_datetime : byte[7] (2 bytes for year, 1 byte for each of month, day, hour, minute, second)
            //part_seq : uint
            //filedata_length : uint
            //filedata : byte[filedata_length]
            if ((content & ((uint)KominProtocolContentTypes.FileData)) != 0)
            {
                file = new FileData();
                file.file_id = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                int text_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                file.filename = ByteArrayToString(data, offset, text_length);
                offset += text_length;
                file.filesize = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                file.upload = ByteArrayToDateTime(data, offset);
                offset += 7;
                file.timeout = ByteArrayToDateTime(data, offset);
                offset += 7;
                file.part_seq = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                int filedata_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                file.filedata = new byte[filedata_length];
                Buffer.BlockCopy(data, offset, file.filedata, 0, filedata_length);
                offset += filedata_length;
            }

            //error text data:
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.ErrorTextData)) != 0)
            {
                int text_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                error_text = ByteArrayToString(data, offset, text_length);
                offset += text_length;
            }

            //user data:
            //contact_id : uint
            //contact_name_length : uint
            //contact_name : char[contact_name_length]
            //status : uint
            //contacts_count : uint
            //struct{
            //    contact_id : uint
            //    contact_name_length : uint
            //    contact_name : char[contact_name_length]
            //    status : uint
            //} [contacts_count]
            //groups_count : uint
            //struct{
            //    group_id : uint
            //    group_name_length : uint
            //    group_name : char[group_name_length]
            //    communication_type : uint
            //    creators_id : uint;
            //    members_count : uint
            //    struct{
            //        contact_id : uint
            //        contact_name_length : uint
            //        contact_name : char[contact_name_length]
            //        status : uint
            //    } [members_count]
            //} [groups_count]
            if ((content & ((uint)KominProtocolContentTypes.UserData)) != 0)
            {
                userdata = new UserData();
                userdata.contact_id = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                int text_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                userdata.contact_name = ByteArrayToString(data, offset, text_length);
                offset += text_length;
                userdata.status = ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                int contacts_count = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                userdata.contacts.Clear();
                for (; contacts_count > 0; contacts_count--)
                {
                    ContactData cd = new ContactData();
                    cd.contact_id = ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    text_length = (int)ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    cd.contact_name = ByteArrayToString(data, offset, text_length);
                    offset += text_length;
                    cd.status = ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    userdata.contacts.Add(cd);
                }
                int groups_count = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                userdata.groups.Clear();
                for (; groups_count > 0; groups_count--)
                {
                    GroupData gd = new GroupData();
                    gd.group_id = ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    text_length = (int)ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    gd.group_name = ByteArrayToString(data, offset, text_length);
                    offset += text_length;
                    gd.communication_type = ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    gd.creators_id = ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    contacts_count = (int)ByteArrayToUInt(data, offset);
                    offset += sizeof(uint);
                    gd.members.Clear();
                    for (; contacts_count > 0; contacts_count--)
                    {
                        ContactData cd = new ContactData();
                        cd.contact_id = ByteArrayToUInt(data, offset);
                        offset += sizeof(uint);
                        text_length = (int)ByteArrayToUInt(data, offset);
                        offset += sizeof(uint);
                        cd.contact_name = ByteArrayToString(data, offset, text_length);
                        offset += text_length;
                        cd.status = ByteArrayToUInt(data, offset);
                        offset += sizeof(uint);
                        gd.members.Add(cd);
                    }
                    userdata.groups.Add(gd);
                }
            }

            //(?) SMS data:
            //number : char[9]
            //length : uint
            //chars : char[length]
            /*if ((content & ((uint)KominProtocolContentTypes.SMSData)) != 0)
            {
                sms_number = ByteArrayToString(data, offset, 9 * sizeof(char));
                offset += 9 * sizeof(char);
                int text_length = (int)ByteArrayToUInt(data, offset);
                offset += sizeof(uint);
                sms_text = ByteArrayToString(data, offset, text_length);
                offset += text_length;
            }*/
        }

        public void InsertContent(KominProtocolContentTypes type, params object[] args)
        {
            DeleteContent((uint)type);
            switch (type)
            {
                case KominProtocolContentTypes.PasswordData:
                    if (args[0].GetType().Name != "String")
                        return;
                    password = (string)args[0];
                    content_length += (uint)(sizeof(uint) + password.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.StatusData:
                    if (args[0].GetType().Name != "UInt32")
                        return;
                    status = (uint)args[0];
                    content_length += (uint)(sizeof(uint));
                    break;
                case KominProtocolContentTypes.ContactIDData:
                    if (args[0].GetType().Name != "UInt32")
                        return;
                    contact_id = (uint)args[0];
                    content_length += (uint)(sizeof(uint));
                    break;
                case KominProtocolContentTypes.TextMessageData:
                    if (args[0].GetType().Name != "TextMessage")
                        return;
                    text_msg = (TextMessage)args[0];
                    content_length += (uint)(7 + sizeof(uint) + text_msg.message.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.AudioMessageData:
                    if (args[0].GetType().Name != "Byte[]")
                        return;
                    audio_msg = (byte[])((byte[])args[0]).Clone();
                    content_length += (uint)(sizeof(uint) + audio_msg.Length);
                    break;
                case KominProtocolContentTypes.VideoMessageData:
                    if (args[0].GetType().Name != "Byte[]")
                        return;
                    video_msg = (byte[])((byte[])args[0]).Clone();
                    content_length += (uint)(sizeof(uint) + video_msg.Length);
                    break;
                case KominProtocolContentTypes.ContactData:
                    if (args[0].GetType().Name != "ContactData")
                        return;
                    contact = (ContactData)args[0];
                    content_length += (uint)(sizeof(uint) * 3 + contact.contact_name.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.GroupData:
                    if (args[0].GetType().Name != "GroupData")
                        return;
                    group = (GroupData)args[0];
                    uint group_sum = 0;
                    foreach (ContactData cd in group.members)
                        group_sum += (uint)(sizeof(uint) * 3 + cd.contact_name.Length * sizeof(char));
                    content_length += (uint)(sizeof(uint) * 5 + group.group_name.Length * sizeof(char) + group_sum);
                    break;
                case KominProtocolContentTypes.FileData:
                    if (args[0].GetType().Name != "FileData")
                        return;
                    file = (FileData)args[0];
                    content_length += (uint)(sizeof(uint) * 5 + 14 + file.filename.Length * sizeof(char) + file.filedata.Length);
                    break;
                case KominProtocolContentTypes.ErrorTextData:
                    if (args[0].GetType().Name != "String")
                        return;
                    error_text = (string)args[0];
                    content_length += (uint)(sizeof(uint) + error_text.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.UserData:
                    if (args[0].GetType().Name != "UserData")
                        return;
                    userdata = (UserData)args[0];
                    uint contacts_sum = 0;
                    foreach (ContactData cd in userdata.contacts)
                        contacts_sum += (uint)(sizeof(uint) * 3 + cd.contact_name.Length * sizeof(char));
                    uint groups_sum = 0;
                    foreach (GroupData gd in userdata.groups)
                    {
                        groups_sum += (uint)(sizeof(uint) * 5 + gd.group_name.Length * sizeof(char));
                        foreach (ContactData cd in gd.members)
                            groups_sum += (uint)(sizeof(uint) * 3 + cd.contact_name.Length * sizeof(char));
                    }
                    content_length += (uint)(sizeof(uint) * 5 + userdata.contact_name.Length * sizeof(char) + contacts_sum + groups_sum);
                    break;
                /*case KominProtocolContentTypes.SMSData:
                    if (args[0].GetType().Name != "String")
                        return;
                    if (args[1].GetType().Name != "String")
                        return;
                    char[] c = new char[9];
                    ((string)args[0]).CopyTo(0, c, 0, 9);
                    sms_number = new string(c);
                    sms_text = (string)args[1];
                    content_length += (uint)(9 * sizeof(char) + sizeof(uint) + sms_text.Length * sizeof(char));
                    break;*/
            }
            content |= (uint)type;
        }

        public object[] GetContent(KominProtocolContentTypes type)
        {
            if ((content & (uint)type) == 0)
                return null;

            object[] ret = null;

            switch (type)
            {
                case KominProtocolContentTypes.PasswordData:
                    ret = new object[1];
                    ret[0] = password;
                    break;
                case KominProtocolContentTypes.StatusData:
                    ret = new object[1];
                    ret[0] = status;
                    break;
                case KominProtocolContentTypes.ContactIDData:
                    ret = new object[1];
                    ret[0] = contact_id;
                    break;
                case KominProtocolContentTypes.TextMessageData:
                    ret = new object[1];
                    ret[0] = text_msg;
                    break;
                case KominProtocolContentTypes.AudioMessageData:
                    ret = new object[1];
                    ret[0] = audio_msg;
                    break;
                case KominProtocolContentTypes.VideoMessageData:
                    ret = new object[1];
                    ret[0] = video_msg;
                    break;
                case KominProtocolContentTypes.ContactData:
                    ret = new object[1];
                    ret[0] = contact;
                    break;
                case KominProtocolContentTypes.GroupData:
                    ret = new object[1];
                    ret[0] = group;
                    break;
                case KominProtocolContentTypes.FileData:
                    ret = new object[1];
                    ret[0] = file;
                    break;
                case KominProtocolContentTypes.ErrorTextData:
                    ret = new object[1];
                    ret[0] = error_text;
                    break;
                case KominProtocolContentTypes.UserData:
                    ret = new object[1];
                    ret[0] = userdata;
                    break;
                /*case KominProtocolContentTypes.SMSData:
                    ret = new object[2];
                    ret[0] = sms_number;
                    ret[1] = sms_text;
                    break;*/
            }

            return ret;
        }

        public bool HasContent(uint type)
        {
            return ((content & type) == type);
        }

        public void DeleteContent()
        {
            content = 0;
            content_length = 0;
            data = new byte[0];
        }

        public void DeleteContent(uint type, bool except_specified = false)
        {
            if (except_specified)
                type = ~type;

            type &= (uint)KominProtocolContentTypes.Mask;
            type &= content;

            if ((content & type) == 0)
                return;

            content &= ~type;
            List<KominProtocolContentTypes> types = new List<KominProtocolContentTypes>();
            for (int i = 1; i <= (int)KominProtocolContentTypes.MaxValue; i <<= 1)
            {
                if ((type & i) != 0)
                    types.Add((KominProtocolContentTypes)i);
            }
            foreach (KominProtocolContentTypes t in types)
                switch (t)
                {
                    case KominProtocolContentTypes.PasswordData:
                        content_length -= (uint)(sizeof(uint) + password.Length * sizeof(char));
                        break;
                    case KominProtocolContentTypes.StatusData:
                        content_length -= (uint)(sizeof(uint));
                        break;
                    case KominProtocolContentTypes.ContactIDData:
                        content_length -= (uint)(sizeof(uint));
                        break;
                    case KominProtocolContentTypes.TextMessageData:
                        content_length -= (uint)(7 + sizeof(uint) + text_msg.message.Length * sizeof(char));
                        break;
                    case KominProtocolContentTypes.AudioMessageData:
                        content_length -= (uint)(sizeof(uint) + audio_msg.Length);
                        break;
                    case KominProtocolContentTypes.VideoMessageData:
                        content_length -= (uint)(sizeof(uint) + video_msg.Length);
                        break;
                    case KominProtocolContentTypes.ContactData:
                        content_length -= (uint)(sizeof(uint) * 3 + contact.contact_name.Length * sizeof(char));
                        break;
                    case KominProtocolContentTypes.GroupData:
                        uint group_sum = 0;
                        foreach (ContactData cd in group.members)
                            group_sum += (uint)(sizeof(uint) * 3 + cd.contact_name.Length * sizeof(char));
                        content_length -= (uint)(sizeof(uint) * 5 + group.group_name.Length * sizeof(char) + group_sum);
                        break;
                    case KominProtocolContentTypes.FileData:
                        content_length -= (uint)(sizeof(uint) * 5 + 14 + file.filename.Length * sizeof(char) + file.filedata.Length);
                        break;
                    case KominProtocolContentTypes.ErrorTextData:
                        content_length -= (uint)(sizeof(uint) + error_text.Length * sizeof(char));
                        break;
                    case KominProtocolContentTypes.UserData:
                        uint contacts_sum = 0;
                        foreach (ContactData cd in userdata.contacts)
                            contacts_sum += (uint)(sizeof(uint) * 3 + cd.contact_name.Length * sizeof(char));
                        uint groups_sum = 0;
                        foreach (GroupData gd in userdata.groups)
                        {
                            groups_sum += (uint)(sizeof(uint) * 5 + gd.group_name.Length * sizeof(char));
                            foreach (ContactData cd in gd.members)
                                groups_sum += (uint)(sizeof(uint) * 3 + cd.contact_name.Length * sizeof(char));
                        }
                        content_length -= (uint)(sizeof(uint) * 5 + userdata.contact_name.Length * sizeof(char) + contacts_sum + groups_sum);
                        break;
                    /*case KominProtocolContentTypes.SMSData:
                        content_length -= (uint)(9 * sizeof(char) + sizeof(uint) + sms_text.Length * sizeof(char));
                        break;*/
                }
        }

        ///<summary>
        ///Copy content from specified packet
        ///<param name="packet">Source packet</param>
        ///</summary>
        public void CopyContent(KominNetworkPacket packet)
        {
            List<KominProtocolContentTypes> types = new List<KominProtocolContentTypes>();
            for (int i = 1; i <= (int)KominProtocolContentTypes.MaxValue; i <<= 1)
            {
                if ((packet.content & i) != 0)
                    types.Add((KominProtocolContentTypes)i);
            }
            foreach (KominProtocolContentTypes t in types)
                InsertContent(t, packet.GetContent(t));
        }
    }

    public enum KominProtocolCommands : uint
    {
        NoOperation = 0,
        Login,
        Logout,
        SetStatus,
        SetPassword,
        CreateContact,
        Accept,
        Deny,
        Error,
        AddContactToList,
        RemoveContactFromList,
        PingContactRequest,
        PingContactAnswer,
        PingGroupRequest,
        PingGroupAnswer,
        SendMessage,
        PingMessages,
        RequestAudioCall,
        RequestVideoCall,
        CloseCall,
        SwitchToAudioCall,
        SwitchToVideoCall,
        RequestFileTransfer,
        TimeoutFileTransfer,
        FinishFileTransfer,
        CreateGroup,
        JoinGroup,
        LeaveGroup,
        CloseGroup,
        GroupHolderChange,
        RemoveContact,
        PeriodicPingRequest,
        PeriodicPingAnswer,
        Disconnect,
        MaxValue = Disconnect
    }

    public enum KominProtocolContentTypes : uint
    {
        PasswordData = 1,
        StatusData = 2,
        ContactIDData = 4,
        TextMessageData = 8,
        AudioMessageData = 0x10,
        VideoMessageData = 0x20,
        ContactData = 0x40,
        GroupData = 0x80,
        FileData = 0x100,
        ErrorTextData = 0x200,
        UserData = 0x400,
        //SMSData = 0x800,
        MaxValue = UserData,
        Mask = 0x7FF
    }

    public struct KominNetworkErrors
    {
        public static string WrongRequestContent = "Nieprawidłowe dane żądania",
        UserNotExists = "Użytkownik nie istnieje",
        UserAlreadyExists = "Użytkownik już istnieje",
        UserAlreadyLoggedIn = "Użytkownik jest już zalogowany",
        UserNotLoggedIn = "Użytkownik nie jest zalogowany",
        WrongPassword = "Błędne hasło",
        WrongStatus = "Nieprawidłowy status",
        UserExistsOnContactList = "Użytkownik już znajduje się na liście kontaktów",
        UserAlreadyInGroup = "Użytkownik znajduje się już w tej grupie",
        UserNotExistsOnContactList = "Użytkownik nie znajduje się na liście kontaktów",
        UserNotInGroup = "Użytkownik nie należy do grupy",
        GroupNotExists = "Grupa nie istnieje",
        GroupAlreadyExists = "Grupa już istnieje",
        SenderNotInGroup = "Nie należysz do tej grupy",
        CallNotStartedYet = "Rozmowa nie została jeszcze rozpoczęta",
        //ServerFull = "Serwer przepełniony - nie można sie zalogować",
        ServerFileStorageFull = "Serwer nie może przyjąć pliku",
        UserIsAlreadyGroupHolder = "Wskazany użytkownik już jest założycielem grupy",
        UserIsNotGroupHolder = "Nie masz do tego uprawnień (nie jesteś założycielem grupy)",
        CannotInfluOtherUsers = "Nie wolno wpływać na innych użytkowników",
        ServerInternalError = "Wewnętrzny błąd serwera";
    }

    public enum KominClientStatusCodes : uint
    {
        NotAccessible = 0,  //niedostepny
        Invisible = 1,      //niewidoczny
        Busy = 2,           //zajęty/zaraz wracam
        Accessible = 3,     //dostępny
        MaxValue = Accessible,
        Mask = 0x3 //valid bits mask only
    }

    public enum KominClientCapabilities : uint
    {
        HasMicrophone = 0x10,
        MicrophoneMuted = 0x20,
        SoundMuted = 0x40,
        AudioCapabilitiesMask = 0x70,

        HasCamera = 0x80,
        CameraDisabled = 0x100,
        VisionDisabled = 0x200,
        VideoCapabilitiesMask = 0x380,

        Mask = AudioCapabilitiesMask + VideoCapabilitiesMask
    }
}
