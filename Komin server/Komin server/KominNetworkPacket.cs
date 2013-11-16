using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komin
{
    public class KominNetworkPacket
    {
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
        private string text_msg;
        private byte[] audio_msg;
        private byte[] video_msg;
        private string contact_name;
        private string group_name;
        private uint file_id;
        private string filename;
        private uint filesize;
        private byte[] filedata;
        private string error_text;
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
            text_msg = "";
            audio_msg = new byte[0];
            video_msg = new byte[0];
            contact_name = "";
            group_name = "";
            file_id = 0;
            filename = "";
            filesize = 0;
            filedata = new byte[0];
            error_text = "";
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

        static uint ByteArrayToUInt(byte[] ba)
        {
            return (((uint)ba[3]) << 24) + (((uint)ba[2]) << 16) + (((uint)ba[1]) << 8) + ((uint)ba[0]);
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

        static bool ByteArrayToBool(byte[] ba)
        {
            return (ba[0] == 1 ? true : false);
        }

        static byte[] StringToByteArray(string s)
        {
            byte[] bytes = new byte[s.Length * sizeof(char)];
            Buffer.BlockCopy(s.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string ByteArrayToString(byte[] ba)
        {
            char[] chars = new char[ba.Length / sizeof(char)];
            Buffer.BlockCopy(ba, 0, chars, 0, ba.Length);
            return new string(chars);
        }

        public static uint CheckSize(ref byte[] buffer)
        {
            if(buffer.Length<sizeof(uint)*6+1 /*sizeof(bool)*/)
                return 0;
            uint content_length = ByteArrayToUInt(new ArraySegment<byte>(buffer, sizeof(uint) * 5 + 1/*sizeof(bool)*/, sizeof(uint)).Array);
            if (buffer.Length < sizeof(uint) * 6 + 1 /*sizeof(bool)*/+ content_length)
                return 0;
            return sizeof(uint) * 6 + 1 /*sizeof(bool)*/+ content_length;
        }

        public byte[] PackForSending()
        {
            uint size = (uint)(sizeof(uint) * 6 + 1 /*sizeof(bool)*/ + data.Length);
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
            CreateDataBlock();
            Buffer.BlockCopy(data, 0, db, offset, data.Length);
            return db;
        }

        public void UnpackReceivedPacket(ref byte[] db)
        {
            int offset = 0;

            sender = ByteArrayToUInt(new ArraySegment<byte>(db, offset, sizeof(uint)).Array);
            offset += sizeof(uint);
            target = ByteArrayToUInt(new ArraySegment<byte>(db, offset, sizeof(uint)).Array);
            offset += sizeof(uint);
            target_is_group = ByteArrayToBool(new ArraySegment<byte>(db, offset, 1 /*sizeof(bool)*/).Array);
            offset += 1 /*sizeof(uint)*/;
            command = ByteArrayToUInt(new ArraySegment<byte>(db, offset, sizeof(uint)).Array);
            offset += sizeof(uint);
            job_id = ByteArrayToUInt(new ArraySegment<byte>(db, offset, sizeof(uint)).Array);
            offset += sizeof(uint);
            content = ByteArrayToUInt(new ArraySegment<byte>(db, offset, sizeof(uint)).Array);
            offset += sizeof(uint);
            content_length = ByteArrayToUInt(new ArraySegment<byte>(db, offset, sizeof(uint)).Array);
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
                byte[] pwd = StringToByteArray (password);
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
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.TextMessageData)) != 0)
            {
                byte[] text = StringToByteArray(text_msg);
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

            //contact name data:
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.ContactNameData)) != 0)
            {
                byte[] text = StringToByteArray(contact_name);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
            }

            //group name data:
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.GroupNameData)) != 0)
            {
                byte[] text = StringToByteArray(group_name);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
            }

            //file data:
            //file_id : uint
            //filename_length : uint
            //filename_chars : char[length]
            //filesize : uint
            //filedata_length : uint
            //filedata : byte[filedata_length]
            if ((content & ((uint)KominProtocolContentTypes.FileData)) != 0)
            {
                Buffer.BlockCopy(UIntToByteArray(file_id), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                byte[] text = StringToByteArray(filename);
                Buffer.BlockCopy(UIntToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(text, 0, data, offset, text.Length);
                offset += text.Length;
                Buffer.BlockCopy(UIntToByteArray(filesize), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                if (filedata == null)
                    filedata = new byte[0];
                Buffer.BlockCopy(UIntToByteArray((uint)filedata.Length), 0, data, offset, sizeof(uint));
                offset += sizeof(uint);
                Buffer.BlockCopy(filedata, 0, data, offset, filedata.Length);
                offset += filedata.Length;
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
                Buffer.BlockCopy(uintToByteArray((uint)text.Length), 0, data, offset, sizeof(uint));
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
                int pwd_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                password = ByteArrayToString(new ArraySegment<byte>(data, offset, pwd_length).Array);
                offset += pwd_length;
            }

            //status data:
            //status : uint
            if ((content & ((uint)KominProtocolContentTypes.StatusData)) != 0)
            {
                status = ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
            }

            //contact id data:
            //contact_id : uint
            if ((content & ((uint)KominProtocolContentTypes.ContactIDData)) != 0)
            {
                contact_id = ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
            }

            //text message data:
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.TextMessageData)) != 0)
            {
                int text_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                text_msg = ByteArrayToString(new ArraySegment<byte>(data, offset, text_length).Array);
                offset += text_length;
            }

            //audio message data:
            //size : uint (in bytes)
            //data : byte[size]
            if ((content & ((uint)KominProtocolContentTypes.AudioMessageData)) != 0)
            {
                int audio_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                audio_msg = new ArraySegment<byte>(data, offset, audio_length).Array;
                offset += audio_length;
            }

            //video message data:
            //size : uint (in bytes)
            //data : byte[size]
            if ((content & ((uint)KominProtocolContentTypes.VideoMessageData)) != 0)
            {
                int video_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                video_msg = new ArraySegment<byte>(data, offset, video_length).Array;
                offset += video_length;
            }

            //contact name data:
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.ContactNameData)) != 0)
            {
                int text_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                contact_name = ByteArrayToString(new ArraySegment<byte>(data, offset, text_length).Array);
                offset += text_length;
            }

            //group name data:
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.GroupNameData)) != 0)
            {
                int text_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                group_name = ByteArrayToString(new ArraySegment<byte>(data, offset, text_length).Array);
                offset += text_length;
            }

            //file data:
            //file_id : uint
            //filename_length : uint
            //filename_chars : char[length]
            //filesize : uint
            //filedata_length : uint
            //filedata : byte[filedata_length]
            if ((content & ((uint)KominProtocolContentTypes.FileData)) != 0)
            {
                file_id = ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                int text_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                filename = ByteArrayToString(new ArraySegment<byte>(data, offset, text_length).Array);
                offset += text_length;
                filesize = ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                int filedata_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                filedata = new ArraySegment<byte>(data, offset, filedata_length).Array;
                offset += filedata_length;
            }

            //error text data:
            //length : uint
            //chars : char[length]
            if ((content & ((uint)KominProtocolContentTypes.ErrorTextData)) != 0)
            {
                int text_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                error_text = ByteArrayToString(new ArraySegment<byte>(data, offset, text_length).Array);
                offset += text_length;
            }

            //(?) SMS data:
            //number : char[9]
            //length : uint
            //chars : char[length]
            /*if ((content & ((uint)KominProtocolContentTypes.SMSData)) != 0)
            {
                sms_number = ByteArrayToString(new ArraySegment<byte>(data, offset, 9 * sizeof(char)).Array);
                offset += 9 * sizeof(char);
                int text_length = (int)ByteArrayToUInt(new ArraySegment<byte>(data, offset, sizeof(uint)).Array);
                offset += sizeof(uint);
                sms_text = ByteArrayToString(new ArraySegment<byte>(data, offset, text_length).Array);
                offset += text_length;
            }*/
        }

        public void InsertContent(KominProtocolContentTypes type, params object[] args)
        {
            DeleteContent(type);
            content |= (uint)type;
            switch (type)
            {
                case KominProtocolContentTypes.PasswordData:
                    password = (string)args[0];
                    content_length += (uint)(sizeof(uint) + password.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.StatusData:
                    status = (uint)args[0];
                    content_length += (uint)(sizeof(uint));
                    break;
                case KominProtocolContentTypes.ContactIDData:
                    contact_id = (uint)args[0];
                    content_length += (uint)(sizeof(uint));
                    break;
                case KominProtocolContentTypes.TextMessageData:
                    text_msg = (string)args[0];
                    content_length += (uint)(sizeof(uint) + text_msg.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.AudioMessageData:
                    audio_msg = (byte[])((byte[])args[0]).Clone();
                    content_length += (uint)(sizeof(uint) + audio_msg.Length);
                    break;
                case KominProtocolContentTypes.VideoMessageData:
                    video_msg = (byte[])((byte[])args[0]).Clone();
                    content_length += (uint)(sizeof(uint) + video_msg.Length);
                    break;
                case KominProtocolContentTypes.ContactNameData:
                    contact_name = (string)args[0];
                    content_length += (uint)(sizeof(uint) + contact_name.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.GroupNameData:
                    group_name = (string)args[0];
                    content_length += (uint)(sizeof(uint) + group_name.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.FileData:
                    file_id = (uint)args[0];
                    filename = (string)args[1];
                    filesize = (uint)args[2];
                    if (args[3] == null)
                        args[3] = new byte[0];
                    filedata = (byte[])((byte[])args[3]).Clone();
                    content_length += (uint)(sizeof(uint) * 4 + filename.Length * sizeof(char) + filedata.Length);
                    break;
                case KominProtocolContentTypes.ErrorTextData:
                    error_text = (string)args[0];
                    content_length += (uint)(sizeof(uint) + error_text.Length * sizeof(char));
                    break;
                /*case KominProtocolContentTypes.SMSData:
                    char[] c = new char[9];
                    ((string)args[0]).CopyTo(0, c, 0, 9);
                    sms_number = new string(c);
                    sms_text = (string)args[1];
                    content_length += (uint)(9 * sizeof(char) + sizeof(uint) + sms_text.Length * sizeof(char));
                    break;*/
            }
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
                case KominProtocolContentTypes.ContactNameData:
                    ret = new object[1];
                    ret[0] = contact_name;
                    break;
                case KominProtocolContentTypes.GroupNameData:
                    ret = new object[1];
                    ret[0] = group_name;
                    break;
                case KominProtocolContentTypes.FileData:
                    ret = new object[4];
                    ret[0] = file_id;
                    ret[1] = filename;
                    ret[2] = filesize;
                    ret[3] = filedata;
                    break;
                case KominProtocolContentTypes.ErrorTextData:
                    ret = new object[1];
                    ret[0] = error_text;
                    break;
                /*case KominProtocolContentTypes.SMSData:
                    ret = new object[2];
                    ret[0] = sms_number;
                    ret[1] = sms_text;
                    break;*/
            }

            return ret;
        }

        public void DeleteContent()
        {
            content = 0;
            content_length = 0;
            data = new byte[0];
        }

        public void DeleteContent(KominProtocolContentTypes type)
        {
            if ((content & ((uint)type)) == 0)
                return;
            content &= ~(uint)type;
            switch (type)
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
                    content_length -= (uint)(sizeof(uint) + text_msg.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.AudioMessageData:
                    content_length -= (uint)(sizeof(uint) + audio_msg.Length);
                    break;
                case KominProtocolContentTypes.VideoMessageData:
                    content_length -= (uint)(sizeof(uint) + video_msg.Length);
                    break;
                case KominProtocolContentTypes.ContactNameData:
                    content_length -= (uint)(sizeof(uint) + contact_name.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.GroupNameData:
                    content_length -= (uint)(sizeof(uint) + group_name.Length * sizeof(char));
                    break;
                case KominProtocolContentTypes.FileData:
                    content_length -= (uint)(sizeof(uint) * 4 + filename.Length * sizeof(char) + filedata.Length);
                    break;
                case KominProtocolContentTypes.ErrorTextData:
                    content_length -= (uint)(sizeof(uint) + error_text.Length * sizeof(char));
                    break;
                /*case KominProtocolContentTypes.SMSData:
                    content_length -= (uint)(9 * sizeof(char) + sizeof(uint) + sms_text.Length * sizeof(char));
                    break;*/
            }
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
    }

    public enum KominProtocolContentTypes : uint
    {
        PasswordData = 1,
        StatusData = 2,
        ContactIDData = 4,
        TextMessageData = 8,
        AudioMessageData = 0x10,
        VideoMessageData = 0x20,
        ContactNameData = 0x40,
        GroupNameData = 0x80,
        FileData = 0x100,
        ErrorTextData = 0x200,
        //SMSData = 0x400,
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
        CallNotStartedYet = "Połączenie nie zostało otwarte",
        ServerFull = "Serwer przepełniony - nie można sie zalogować",
        ServerFileStorageFull = "Serwer nie może przyjąć pliku",
        UserIsNotGroupHolder = "Nie masz do tego uprawnień (nie jesteś założycielem grupy)";
    }
}
