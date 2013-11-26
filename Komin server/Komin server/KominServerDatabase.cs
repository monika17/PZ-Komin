using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;

namespace Komin
{
    public class KominServerDatabase
    {
        private SqlConnection connection;

        public KominServerDatabase()
        {
            connection = new SqlConnection("Data Source=.;Initial Catalog=KominServerDatabase;Integrated Security=SSPI");
            connection.Open();
        }

        public void Disconnect()
        {
            connection.Close();
        }

        //request server can make to database

        public int CreateUser(string contact_name, string password)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is there any contact with specified name
            cmd.CommandText = "select * from konta where nazwa=\'" + contact_name + "\'";
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows) //user already exists
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //find free contact_id
            cmd.CommandText = "select id_konta from konta order by id_konta asc";
            uint contact_id = 1;
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    contact_id++;
                    if ((uint)((int)rdr["id_konta"]) != contact_id)
                        break;
                }
            rdr.Close();

            //create contact list table
            if (CreateContactList("contacts" + contact_id) != 0)
                return -2; //error during creaation of contact list

            //create account
            cmd.CommandText = "insert into konta values (" + contact_id + ", \'" + contact_name + "\', \'" + password + "\', 0, \'contacts" + contact_id + "\')";
            cmd.ExecuteNonQuery();

            return 0;
        }

        private int CreateContactList(string list_name)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check does this list already exist
            cmd.CommandText = "select * from " + list_name;
            try
            {
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows) //list already exists
                {
                    rdr.Close();
                    return -1;
                }
            }
            catch (SqlException) { } //list doesn't exist

            //create list
            cmd.CommandText = "create table " + list_name + " (id_kontaktu int not null foreign key references konta(id_konta))";
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int CreateGroup(string group_name, uint creators_id, uint communication_type)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is creator a valid user
            cmd.CommandText = "select * from konta where id_konta=" + creators_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //creator doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //check is there any group with specified name
            cmd.CommandText = "select * from grupy where nazwa_grupy=\'" + group_name + "\'";
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows) //group already exists
            {
                rdr.Close();
                return -2;
            }
            rdr.Close();

            //find free group_id
            cmd.CommandText = "select id_grupy from grupy order by id_grupy asc";
            uint group_id = 1;
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    group_id++;
                    if ((uint)((int)rdr["id_grupy"]) != group_id)
                        break;
                }
            rdr.Close();

            //create member list table
            if (CreateContactList("members" + group_id) != 0)
                return -3; //error during creaation of members list
            InsertContactIntoList(group_id, true, creators_id);

            //create group
            cmd.CommandText = "insert into grupy values (" + group_id + ", \'" + group_name + "\', " + creators_id + ", " + communication_type + ", \'members" + group_id + "\')";
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int SetUserStatus(uint contact_id, uint new_status)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is contact_id a valid user
            cmd.CommandText = "select id_konta from konta where id_konta=" + contact_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //contact doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //set new status
            cmd.CommandText = "update konta set status_konta=" + new_status + " where id_konta=" + contact_id;
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int SetUserPassword(uint contact_id, string new_password)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is contact_id a valid user
            cmd.CommandText = "select id_konta from konta where id_konta=" + contact_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //contact doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //set new password
            cmd.CommandText = "update konta set haslo=" + new_password + " where id_konta=" + contact_id;
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int SetGroupCreator(uint group_id, uint new_creator_id)
        //server should earlier check is new creator active
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is group_id valid
            cmd.CommandText = "select id_grupy from grupy where id_grupy=" + group_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //group doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //check is new_creator_id a valid group member
            cmd.CommandText = "select id_kontaktu from members" + group_id + " where id_kontaktu=" + new_creator_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //new_creator_id is not a group member
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //set new group creator
            cmd.CommandText = "update grupy set id_zalozyciela=" + new_creator_id + " where id_grupy=" + group_id;
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int InsertContactIntoList(uint list_id, bool is_group, uint contact_id)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is contact a valid user
            cmd.CommandText = "select * from konta where id_konta=" + contact_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //contact doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //check is list name valid
            string list_name = (is_group == true ? "members" : "contacts") + list_id;
            cmd.CommandText = "select * from " + list_name;
            try
            {
                rdr = cmd.ExecuteReader();
            }
            catch (SqlException) //list doesn't exist
            {
                return -2;
            }
            rdr.Close();

            //insert contact
            cmd.CommandText = "insert into " + list_name + " values (" + contact_id + ")";
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int RemoveContactFromList(uint list_id, bool is_group, uint contact_id)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is list name valid
            string list_name = (is_group == true ? "members" : "contacts") + list_id;
            cmd.CommandText = "select * from " + list_name + " where id_kontaktu=" + contact_id;
            try
            {
                rdr = cmd.ExecuteReader();
                if (!rdr.HasRows) //list doesn't contain this contact_id
                {
                    rdr.Close();
                    return -2;
                }
            }
            catch (SqlException) //list doesn't exist
            {
                return -1;
            }
            rdr.Close();

            //remove contact
            cmd.CommandText = "delete from " + list_name + " where id_kontaktu=" + contact_id;
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int GetContactListCount(uint list_id, bool is_group)
        {
            int ret = 0;
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is list name valid
            string list_name = (is_group == true ? "members" : "contacts") + list_id;
            cmd.CommandText = "select * from " + list_name;
            try
            {
                rdr = cmd.ExecuteReader();
            }
            catch (SqlException) //list doesn't exist
            {
                return -1;
            }
            rdr.Close();

            //get count
            cmd.CommandText = "select count(*) as ilość from " + list_name;
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    ret = (int)rdr["ilość"];
                }
            rdr.Close();

            return ret;
        }

        public int InsertGroupFile(uint group_id, uint sender_id, ref FileData fd, uint timeout)
        //fd.filename is considered to contain server storage file path
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is sender a valid user
            cmd.CommandText = "select * from konta where id_konta=" + sender_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //contact doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //check is group valid
            cmd.CommandText = "select * from grupy where id_grupy=" + group_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //group doesn't exist
            {
                rdr.Close();
                return -2;
            }
            rdr.Close();

            //get group members list name
            cmd.CommandText = "select kontakty_nazwa from grupy where id_grupy=" + group_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //group doesn't exist
            {
                rdr.Close();
                return -2;
            }
            string list_name = "";
            while (rdr.Read())
            {
                list_name = (string)rdr["kontakty_nazwa"];
            }
            rdr.Close();

            //check is list name valid
            cmd.CommandText = "select * from " + list_name;
            try
            {
                rdr = cmd.ExecuteReader();
            }
            catch (SqlException) //list doesn't exist
            {
                return -3;
            }
            rdr.Close();

            //check is sender a member of a group
            cmd.CommandText = "select * from " + list_name + " where id_kontaktu=" + sender_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //sender isn't a group member
            {
                rdr.Close();
                return -4;
            }
            rdr.Close();

            //find free file_id
            cmd.CommandText = "select id_pliku from pliki_grup order by id_pliku asc";
            fd.file_id = 1;
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    fd.file_id++;
                    if ((uint)((int)rdr["id_pliku"]) != fd.file_id)
                        break;
                }
            rdr.Close();

            //insert group file
            DateTime dt = DateTime.Now;
            string str = dt.ToString();
            DateTime dt2 = new DateTime();
            switch (timeout)
            {
                case 0: //1 minute
                    dt2 = dt.AddMinutes(1);
                    break;
                case 1: //5 minutes
                    dt2 = dt.AddMinutes(5);
                    break;
                case 2: //2 hours
                    dt2 = dt.AddHours(2);
                    break;
                case 3: //1 day
                    dt2 = dt.AddDays(1);
                    break;
            }
            cmd.CommandText = "insert into pliki_grup values (" + fd.file_id + ", \'" + fd.filename + "\', " + fd.filesize + ", "
                + sender_id + ", " + group_id + ", \'" + dt + "\', \'" + dt2 + "\')";
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int InsertPendingMessage(uint sender_id, uint receiver_id, bool receiver_is_group, string msg)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is sender_id a valid user
            cmd.CommandText = "select id_konta from konta where id_konta=" + sender_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //user doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //check is receiver_id a valid user or group
            if (receiver_is_group == false)
                cmd.CommandText = "select id_konta from konta where id_konta=" + receiver_id;
            else
                cmd.CommandText = "select id_grupy from grupy where id_grupy=" + receiver_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //user or group doesn't exist
            {
                rdr.Close();
                return -2;
            }
            rdr.Close();

            //if receiver is group then check is sender_id a valid group member
            if (receiver_is_group == true)
            {
                cmd.CommandText = "select id_kontaktu from members" + receiver_id + " where id_kontaktu=" + sender_id;
                rdr = cmd.ExecuteReader();
                if (!rdr.HasRows) //user isn't a group member
                {
                    rdr.Close();
                    return -3;
                }
                rdr.Close();
            }

            //find fre message_id
            cmd.CommandText = "select id_wiadomosci from oczekujące_wiadomości order by id_wiadomosci asc";
            uint message_id = 0;
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    message_id++;
                    if ((uint)((int)rdr["id_wiadomosci"]) != message_id)
                        break;
                }
            rdr.Close();

            //insert pending message
            cmd.CommandText = "insert into oczekujące_wiadomości values (" + message_id + ", " + sender_id + ", " + receiver_id + ", " + (receiver_is_group == true ? 1 : 0) + ", \'" + msg + "\', \'" + DateTime.Now + "\')";
            cmd.ExecuteNonQuery();

            return 0;
        }

        public ContactData GetContactData(uint contact_id)
        {
            ContactData ret = new ContactData();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //get contact data
            cmd.CommandText = "select * from konta where id_konta=" + contact_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //user not exists
            {
                rdr.Close();
                return null;
            }
            while (rdr.Read())
            {
                ret.contact_id = (uint)((int)rdr["id_konta"]);
                ret.contact_name = (string)rdr["nazwa"];
                ret.status = (uint)((int)rdr["status_konta"]);
            }
            rdr.Close();

            return ret;
        }

        public List<ContactData> GetContactsData()
        {
            List<ContactData> ret = new List<ContactData>();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //get contact data
            cmd.CommandText = "select * from konta";
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //no user not exists
            {
                rdr.Close();
                return null;
            }
            while (rdr.Read())
            {
                ContactData cd = new ContactData();
                cd.contact_id = (uint)((int)rdr["id_konta"]);
                cd.contact_name = (string)rdr["nazwa"];
                cd.status = (uint)((int)rdr["status_konta"]);
                ret.Add(cd);
            }
            rdr.Close();

            return ret;
        }

        public GroupData GetGroupData(uint group_id)
        {
            GroupData ret = new GroupData();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);
            string members_list_name = "";

            //get group data
            cmd.CommandText = "select * from grupy where id_grupy=" + group_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //group not exists
            {
                rdr.Close();
                return null;
            }
            while (rdr.Read())
            {
                ret.group_id = (uint)((int)rdr["id_grupy"]);
                ret.group_name = (string)rdr["nazwa_grupy"];
                ret.communication_type = (uint)((int)rdr["rodzaj_komunikacji"]);
                ret.creators_id = (uint)((int)rdr["id_zalozyciela"]);
                members_list_name = (string)rdr["kontakty_nazwa"];
            }
            rdr.Close();

            //get members list
            ret.members = GetContactListData(members_list_name);

            return ret;
        }

        private List<ContactData> GetContactListData(string list_name)
        {
            List<ContactData> ret = new List<ContactData>();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //get list data
            cmd.CommandText = "select * from " + list_name;
            try
            {
                rdr = cmd.ExecuteReader();
            }
            catch (SqlException) //list not exists
            {
                return null;
            }
            List<uint> contact_ids = new List<uint>();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    contact_ids.Add((uint)((int)rdr["id_kontaktu"]));
                }
            rdr.Close();
            foreach (uint contact_id in contact_ids)
            {
                ContactData contact = GetContactData(contact_id);
                if (contact != null)
                    ret.Add(contact);
            }

            return ret;
        }

        public UserData GetUserData(string contact_name)
        {
            UserData ret = new UserData();
            string contact_list_name = "";
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //get user data
            cmd.CommandText = "select * from konta where nazwa=\'" + contact_name + "\'";
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //user not exists
            {
                rdr.Close();
                return null;
            }
            while (rdr.Read())
            {
                ret.contact_id = (uint)((int)rdr["id_konta"]);
                ret.contact_name = (string)rdr["nazwa"];
                ret.password = (string)rdr["haslo"];
                ret.status = (uint)((int)rdr["status_konta"]);
                contact_list_name = (string)rdr["lista_kontaktow"];
            }
            rdr.Close();

            //get contacts data
            ret.contacts = GetContactListData(contact_list_name);

            //get groups data
            cmd.CommandText = "select id_grupy from grupy";
            rdr = cmd.ExecuteReader();
            List<uint> group_ids = new List<uint>();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    group_ids.Add((uint)((int)rdr["id_grupy"]));
                }
            rdr.Close();
            foreach (uint group_id in group_ids)
            {
                GroupData group = GetGroupData(group_id);
                if (group.members != null)
                    foreach (ContactData member in group.members)
                        if (member.contact_id == ret.contact_id)
                        {
                            ret.groups.Add(group);
                            break;
                        }
            }

            return ret;
        }

        public GroupFileData GetGroupFile(uint group_id, uint file_id)
        //groups are notified about new files with messages including file_id so they can request files using this id
        {
            GroupFileData ret = new GroupFileData();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is group valid
            cmd.CommandText = "select id_grupy from grupy where id_grupy=" + group_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //group not exists
            {
                rdr.Close();
                return null;
            }
            rdr.Close();

            //get file data
            cmd.CommandText = "select * from pliki_grup where id_pliku=" + file_id + (group_id != 0 ? " and id_grupy=" + group_id : "");
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    ret.file_id = (uint)((int)rdr["id_pliku"]);
                    ret.filename = (string)rdr["ścieżka"];
                    ret.filesize = (uint)((int)rdr["rozmiar"]);
                    ret.sender_id = (uint)((int)rdr["id_nadawcy"]);
                    ret.group_id = (uint)((int)rdr["id_grupy"]);
                    ret.upload = (DateTime)rdr["dataczas_wyslania"];
                    ret.timeout = (DateTime)rdr["dataczas_konca"];
                }
            else //file doesn't exist
            {
                rdr.Close();
                return null;
            }
            rdr.Close();

            return ret;
        }

        public List<GroupFileData> GetGroupFiles(uint contact_id = 0, bool is_group = false)
        {
            List<GroupFileData> ret = new List<GroupFileData>();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //if contact_id is 0 then ignore this test
            if (contact_id != 0)
                //check is contact_id a valid user
                if (is_group == false)
                {
                    cmd.CommandText = "select id_konta from konta where id_konta=" + contact_id;
                    rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows) //user doesn't exist
                    {
                        rdr.Close();
                        return null;
                    }
                    rdr.Close();
                }
                else //check is contact_id a valid group
                {
                    cmd.CommandText = "select id_grupy from grupy where id_grupy=" + contact_id;
                    rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows) //group doesn't exist
                    {
                        rdr.Close();
                        return null;
                    }
                    rdr.Close();
                }

            //get file data
            cmd.CommandText = "select * from pliki_grup" + (contact_id != 0 ? " where id_" + (is_group == true ? "grupy" : "nadawcy") + "=" + contact_id : "");
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    GroupFileData gfd = new GroupFileData();
                    gfd.file_id = (uint)((int)rdr["id_pliku"]);
                    gfd.filename = (string)rdr["ścieżka"];
                    gfd.filesize = (uint)((int)rdr["rozmiar"]);
                    gfd.sender_id = (uint)((int)rdr["id_nadawcy"]);
                    gfd.group_id = (uint)((int)rdr["id_grupy"]);
                    gfd.upload = (DateTime)rdr["dataczas_wyslania"];
                    gfd.timeout = (DateTime)rdr["dataczas_konca"];
                    ret.Add(gfd);
                }
            else //file doesn't exist
            {
                rdr.Close();
                return null;
            }
            rdr.Close();

            return ret;
        }

        public List<PendingMessage> GetPendingMessages(uint contact_id, bool is_group = false)
        {
            List<PendingMessage> ret = new List<PendingMessage>();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is contact_id a valid user or group
            if (is_group == false)
                cmd.CommandText = "select id_konta from konta where id_konta=" + contact_id;
            else
                cmd.CommandText = "select id_grupy from grupy where id_grupy=" + contact_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //user or group doesn't exist
            {
                rdr.Close();
                return null;
            }
            rdr.Close();

            //get pending messages
            cmd.CommandText = "select * from oczekujące_wiadomości where id_docelowy=" + contact_id + " and czy_grupowy=" + (is_group == true ? 1 : 0);
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    PendingMessage pm = new PendingMessage();
                    pm.message_id = (uint)((int)rdr["id_wiadomosci"]);
                    pm.sender_id = (uint)((int)rdr["id_nadawcy"]);
                    pm.receiver_id = (uint)((int)rdr["id_docelowy"]);
                    pm.receiver_is_group = ((int)rdr["czy_grupowy"] != 0 ? true : false);
                    pm.message = (string)rdr["tresc_wiadomosci"];
                    pm.send_date = (DateTime)rdr["data_czas_wyslania"];
                    ret.Add(pm);
                }
            rdr.Close();

            return ret;
        }

        public int RemoveUser(string contact_name, out RemoveSummary summary)
        //server will get all removed data in summary and it has to inform all interested contacts about changes
        //server will have to check is it necessary to erase groups (using summary.changes_member_lists collection)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is contact_id a valid user
            cmd.CommandText = "select id_konta from konta where nazwa=\'" + contact_name + "\'";
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //user doesn't exist
            {
                rdr.Close();
                summary = null;
                return -1;
            }
            rdr.Close();

            summary = new RemoveSummary();

            //get user data
            summary.ud = GetUserData(contact_name);

            //get group files sent by this user
            summary.gfdl = GetGroupFiles(summary.ud.contact_id);

            //remove user data from any contacts lists
            cmd.CommandText = "select id_konta from konta where id_konta!=" + summary.ud.contact_id;
            rdr = cmd.ExecuteReader();
            List<uint> contact_ids = new List<uint>();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    contact_ids.Add((uint)((int)rdr["id_konta"]));
                }
            rdr.Close();
            foreach (uint contact_id in contact_ids)
            {
                if (RemoveContactFromList(contact_id, false, summary.ud.contact_id) == 0)
                    summary.changed_contact_lists.Add(contact_id);
            }

            //remove user data from any members lists of groups this user belongs to
            foreach (GroupData gd in summary.ud.groups)
            {
                if (RemoveContactFromList(gd.group_id, true, summary.ud.contact_id) == 0)
                    summary.changed_member_lists.Add(gd.group_id);
            }

            //remove files sent by this user to groups
            foreach (GroupFileData gfd in summary.gfdl)
            {
                RemoveGroupFile(gfd.file_id);
            }

            //remove pending messages for and from this user
            cmd.CommandText = "select id_wiadomosci from oczekujące_wiadomości where id_nadawcy=" + summary.ud.contact_id + " or (id_docelowy=" + summary.ud.contact_id + " and czy_grupowy=0)";
            rdr = cmd.ExecuteReader();
            List<uint> message_ids = new List<uint>();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    message_ids.Add((uint)((int)rdr["id_wiadomosci"]));
                }
            rdr.Close();
            foreach (uint message_id in message_ids)
            {
                RemovePendingMessage(message_id);
            }

            //remove contacts list
            RemoveContactList("contacts" + summary.ud.contact_id);

            //remove user
            cmd.CommandText = "delete from konta where id_konta=" + summary.ud.contact_id;
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int RemoveGroupFile(uint file_id)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is file_id valid
            cmd.CommandText = "select id_pliku from pliki_grup where id_pliku=" + file_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //file doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //delete file entry
            cmd.CommandText = "delete from pliki_grup where id_pliku=" + file_id;
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int RemovePendingMessage(uint message_id)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is file_id valid
            cmd.CommandText = "select id_wiadomosci from oczekujące_wiadomości where id_wiadomosci=" + message_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //pending message doesn't exist
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //delete pending message
            cmd.CommandText = "delete from oczekujące_wiadomości where id_wiadomosci=" + message_id;
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int RemoveGroup(uint group_id, out RemoveSummary summary)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is group_id valid
            cmd.CommandText = "select id_grupy from grupy where id_grupy=" + group_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //group doesn't exist
            {
                rdr.Close();
                summary = null;
                return -1;
            }
            rdr.Close();

            summary = new RemoveSummary();

            //get group data
            summary.gd = GetGroupData(group_id);

            //get group files sent to this group
            summary.gfdl = GetGroupFiles(summary.gd.group_id, true);

            //remove files sent to this group
            foreach (GroupFileData gfd in summary.gfdl)
            {
                RemoveGroupFile(gfd.file_id);
            }

            //remove pending messages for this group
            cmd.CommandText = "select id_wiadomosci from oczekujące_wiadomości where id_docelowy=" + summary.gd.group_id + " and czy_grupowy=1";
            rdr = cmd.ExecuteReader();
            List<uint> message_ids = new List<uint>();
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    message_ids.Add((uint)((int)rdr["id_wiadomosci"]));
                }
            rdr.Close();
            foreach (uint message_id in message_ids)
            {
                RemovePendingMessage(message_id);
            }

            //remove members list
            RemoveContactList("members" + summary.gd.group_id);

            //remove group
            cmd.CommandText = "delete from grupy where id_grupy=" + summary.gd.group_id;
            cmd.ExecuteNonQuery();

            return 0;
        }

        private int RemoveContactList(string list_name)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is list_name valid
            cmd.CommandText = "select * from " + list_name;
            try
            {
                rdr = cmd.ExecuteReader();
            }
            catch (SqlException) //list doesn't exist
            {
                return -1;
            }
            rdr.Close();

            //remove list content
            cmd.CommandText = "delete from " + list_name;
            cmd.ExecuteNonQuery();

            //remove list
            cmd.CommandText = "drop table " + list_name;
            cmd.ExecuteNonQuery();

            return 0;
        }
    }
}
