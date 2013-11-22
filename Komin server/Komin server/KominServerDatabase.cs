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

        //request server can make to database

        public int CreateUser(string contact_name, string password)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is there any contact with specified name
            cmd.CommandText = "select * from konta where nazwa=\'"+contact_name+"\'";
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows) //user already exists
            {
                rdr.Close();
                return -1;
            }
            rdr.Close();

            //find free contact_id
            cmd.CommandText = "select id_konta from konta order by id_konta asc";
            uint contact_id = 0;
            rdr = cmd.ExecuteReader();
            if(rdr.HasRows)
            while (rdr.Read())
            {
                contact_id++;
                if ((uint)((int)rdr["id_konta"]) != contact_id)
                    break;
            }
            rdr.Close();

            //create contact list table
            CreateContactList("contacts" + contact_id);

            //create account
            cmd.CommandText = "insert into konta values (" + contact_id + ", \'" + contact_name + "\', \'" + password + "\', 0, \'contacts" + contact_id + "\')";
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int CreateContactList(string list_name)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check does this list already exist
            cmd.CommandText = "select * from "+list_name;
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
            cmd.CommandText = "create table "+list_name+" (id_kontaktu int not null foreign key references konta(id_konta))";
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int CreateGroup(string group_name, uint creators_id, uint communication_type)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is creator a valid user
            cmd.CommandText = "select * from konta where id_konta="+creators_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //creator doesn't exist
                return -1;
            rdr.Close();

            //check is there any group with specified name
            cmd.CommandText = "select * from grupy where nazwa_grupy=\'" + group_name + "\'";
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows) //group already exists
                return -2;
            rdr.Close();

            //find free group_id
            cmd.CommandText = "select id_grupy from grupy order by id_grupy asc";
            uint group_id = 0;
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
            CreateContactList("members" + group_id);
            InsertContactIntoList("members" + group_id, creators_id);

            //create group
            cmd.CommandText = "insert into grupy values (" + group_id + ", \'" + group_name + "\', " + creators_id + ", " + communication_type + ", \'members" + group_id + "\')";
            cmd.ExecuteNonQuery();

            return 0;
        }

        public int InsertContactIntoList(string list_name, uint contact_id)
        {
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //check is contact a valid user
            cmd.CommandText = "select * from konta where id_konta=" + contact_id;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //contact doesn't exist
                return -1;
            rdr.Close();

            //check is list name valid
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

        public List<ContactData> GetContactListData(string list_name)
        {
            List<ContactData> ret = new List<ContactData>();
            SqlDataReader rdr;
            SqlCommand cmd = new SqlCommand("", connection);

            //get list data
            cmd.CommandText = "select * from " + list_name;
            rdr = cmd.ExecuteReader();
            if (!rdr.HasRows) //list not exists
            {
                rdr.Close();
                return null;
            }
            while (rdr.Read())
            {
                ContactData contact = GetContactData((uint)rdr["id_kontaktu"]);
                if (contact != null)
                    ret.Add(contact);
            }
            rdr.Close();

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
            cmd.CommandText = "select * from grupy";
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            while (rdr.Read())
            {
                GroupData group = GetGroupData((uint)((int)rdr["id_kontaktu"]));
                if (group.members != null)
                    foreach (ContactData member in group.members)
                        if (member.contact_id == ret.contact_id)
                        {
                            ret.groups.Add(group);
                            break;
                        }
            }
            rdr.Close();

            return ret;
        }
    }
}
