using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace Komin
{
    public class ContactData
    {
        public uint contact_id;
        public string contact_name;
        public uint status;

        public ContactData()
        {
            contact_id = 0;
            contact_name = "";
            status = 0;
        }
    }

    public class GroupData
    {
        public uint group_id;
        public string group_name;
        public uint communication_type;
        public uint creators_id;
        public List<ContactData> members;

        public GroupData()
        {
            group_id = 0;
            group_name = "";
            communication_type = 0;
            creators_id = 0;
            members = new List<ContactData>();
        }
    }

    public class UserData : ContactData
    {
        public string password;
        public List<ContactData> contacts;
        public List<GroupData> groups;

        public UserData()
            : base()
        {
            password = "";
            contacts = new List<ContactData>();
            groups = new List<GroupData>();
        }
    }

    public class FileData
    {
        public uint file_id;
        public string filename;
        public uint filesize;
        public DateTime upload;
        public DateTime timeout;
        public uint part_seq;
        public byte[] filedata;

        public FileData()
        {
            file_id = 0;
            filename = "";
            filesize = 0;
            upload = new DateTime();
            timeout = new DateTime();
            part_seq = 0;
            filedata = new byte[0];
        }
    }

    public class TextMessage
    {
        public string message;
        public DateTime send_date;

        public TextMessage()
        {
            message = "";
            send_date = new DateTime();
        }
    }

    public class PendingMessage : TextMessage //this class is used only between server and database
    {
        public uint sender_id;
        public uint receiver_id;
        public bool receiver_is_group;
        public uint message_id;

        public PendingMessage()
            : base()
        {
            sender_id = 0;
            receiver_id = 0;
            receiver_is_group = false;
            message_id = 0;
        }
    }

    public class GroupFileData : FileData //this class is used only between server and database
    {
        public uint sender_id;
        public uint group_id;

        public GroupFileData()
            : base()
        {
            sender_id = 0;
            group_id = 0;
        }
    }

    public class RemoveSummary //this class is used only between server and database
    {
        public UserData ud;
        public GroupData gd;
        public List<GroupFileData> gfdl;
        public List<uint> changed_contact_lists;
        public List<uint> changed_member_lists;

        public RemoveSummary()
        {
            ud = new UserData();
            gd = new GroupData();
            gfdl = new List<GroupFileData>();
            changed_contact_lists = new List<uint>();
            changed_member_lists = new List<uint>();
        }
    }

    public class DatabaseData //this class is used only between server and database
    {
        public List<UserData> users;
        public List<GroupData> groups;
        public List<GroupFileData> gfs;
        public List<PendingMessage> pms;

        public DatabaseData()
        {
            users = new List<UserData>();
            groups = new List<GroupData>();
            gfs = new List<GroupFileData>();
            pms = new List<PendingMessage>();
        }
    }

    public class DataTesters
    {
        public static bool TestLoginOrGroupName(string contact_name) //check is login name correct
        {
            Regex regexp = new Regex("^[A-Z][A-Za-z0-9_]*$");

            if (regexp.Matches(contact_name).Count != 1)
            {
                return false;
            }

            if (contact_name.Length > 200)
            {
                return false;
            }
            return true;
        }

        public static bool TestIPAddress(string ip) //check is IP address in correct format
        {
            Regex regexp = new Regex("^(0|(1[0-9]?[0-9]?)|(2([0-9]|(5[0-5])|([0-4][0-9]))?)|([3-9][0-9]?))\\.(0|(1[0-9]?[0-9]?)|(2([0-9]|(5[0-5])|([0-4][0-9]))?)|([3-9][0-9]?))\\.(0|(1[0-9]?[0-9]?)|(2([0-9]|(5[0-5])|([0-4][0-9]))?)|([3-9][0-9]?))\\.(0|(1[0-9]?[0-9]?)|(2([0-9]|(5[0-5])|([0-4][0-9]))?)|([3-9][0-9]?))$");

            if (regexp.Matches(ip).Count != 1)
            {
                return false;
            }
            return true;
        }
    }
}
