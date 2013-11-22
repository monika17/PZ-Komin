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

    public class DataTesters
    {
        public bool TestLoginOrGroupName(string contact_name) //check is login name correct
        {
            Regex regexp = new Regex("[A-Z][A-Za-z0-9_]*");

            if (regexp.Matches(contact_name).Count != 1)
            {
                return false;
            }
            return true;
        }
    }
}
