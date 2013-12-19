using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Timers;
using System.Windows.Forms;

namespace Komin
{
    public class KominServerDatabase
    {
        private SqlConnection connection;
        private System.Timers.Timer RequestTimer;
        private bool RequestTimerLock;
        private List<string> reqtexts; //first letter means request type: R - reader, E - execute
        private uint done_req_count;
        public bool error_during_req;
        public SqlDataReader ReqRdr;
        private bool req_finished; //variable for threads to inform that reader can be closed and next request can be executed

        public KominServerDatabase()
        {
            reqtexts = new List<string>();
            done_req_count = 0;
            req_finished = true;
            error_during_req = false;
            ReqRdr = null;
            RequestTimerLock = false;
            RequestTimer = new System.Timers.Timer(5);
            RequestTimer.AutoReset = true;
            RequestTimer.Elapsed += RequestTimer_Elapsed;
            connection = new SqlConnection("Data Source=.;Initial Catalog=KominServerDatabase;Integrated Security=SSPI");
            connection.Open();
            //logout all users
            SqlCommand cmd = new SqlCommand("update konta set status_konta=0", connection);
            cmd.ExecuteNonQuery();
            RequestTimer.Enabled = true;
        }

        public uint MakeReaderRequest(string reqtext)
        {
            reqtexts.Add("R" + reqtext);
            return done_req_count + (uint)reqtexts.Count;
        }

        public uint MakeExecuteRequest(string reqtext)
        {
            reqtexts.Add("E" + reqtext);
            return done_req_count + (uint)reqtexts.Count;
        }

        public void WaitForRequestExecution(uint reqid)
        {
            while (done_req_count != reqid) System.Threading.Thread.Sleep(10);
        }

        public void MarkReaderRequestCompleted()
        {
            req_finished = true;
        }

        void RequestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (reqtexts.Count == 0) return;
            RequestTimer.Enabled = false;
            while (RequestTimerLock == true) ;
            RequestTimerLock = true;
            SqlCommand cmd = new SqlCommand(reqtexts[0].Substring(1, reqtexts[0].Length-1), connection);
            if (reqtexts[0][0]=='R')
            {
                try
                {
                    error_during_req = false;
                    ReqRdr = cmd.ExecuteReader();
                    req_finished = false;
                    reqtexts.RemoveAt(0);
                    done_req_count++;
                    while (!req_finished) System.Threading.Thread.Sleep(10);
                    ReqRdr.Close();
                    ReqRdr = null;
                }
                catch (SqlException)
                {
                    req_finished = true;
                    reqtexts.RemoveAt(0);
                    done_req_count++;
                    error_during_req = true;
                }
            }
            else if(reqtexts[0][0]=='E')
            {
                try
                {
                    error_during_req = false;
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException) { error_during_req = true; }
                req_finished = true;
                reqtexts.RemoveAt(0);
                done_req_count++;
            }
            RequestTimerLock = false;
            RequestTimer.Enabled = true;
        }

        public void Disconnect()
        {
            connection.Close();
        }

        //request server can make to database

        public DatabaseData GetAllData()
        {
            DatabaseData ret = new DatabaseData();

            WaitForRequestExecution(MakeReaderRequest("select id_konta from konta"));
            List<uint> uids = new List<uint>();
            while (ReqRdr.Read())
                uids.Add((uint)((int)ReqRdr["id_konta"]));
            MarkReaderRequestCompleted();
            foreach (uint uid in uids)
                ret.users.Add(GetUserData(uid));

            WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy"));
            List<uint> gids = new List<uint>();
            while (ReqRdr.Read())
                gids.Add((uint)((int)ReqRdr["id_grupy"]));
            MarkReaderRequestCompleted();
            foreach (uint gid in gids)
                ret.groups.Add(GetGroupData(gid));

            ret.gfs = GetGroupFiles();

            WaitForRequestExecution(MakeReaderRequest("select * from oczekujące_wiadomości"));
            List<uint> pmids = new List<uint>();
            while (ReqRdr.Read())
            {
                PendingMessage pm = new PendingMessage();
                pm.message_id = (uint)((int)ReqRdr["id_wiadomosci"]);
                pm.sender_id = (uint)((int)ReqRdr["id_nadawcy"]);
                pm.receiver_id = (uint)((int)ReqRdr["id_docelowy"]);
                pm.receiver_is_group = ((int)ReqRdr["czy_grupowy"] != 0 ? true : false);
                pm.message = (string)ReqRdr["tresc_wiadomosci"];
                pm.send_date = (DateTime)ReqRdr["data_czas_wyslania"];
            }
            MarkReaderRequestCompleted();

            return ret;
        }

        public int CreateUser(string contact_name, string password)
        {
            //check is there any contact with specified name
            WaitForRequestExecution(MakeReaderRequest("select * from konta where nazwa=\'" + contact_name + "\'"));
            if (ReqRdr.HasRows) //user already exists
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //find free contact_id
            WaitForRequestExecution(MakeReaderRequest("select id_konta from konta order by id_konta desc"));
            uint contact_id = 1;
            if (ReqRdr.HasRows)
            {
                ReqRdr.Read();
                contact_id = 1+(uint)((int)ReqRdr["id_konta"]);
            }
            MarkReaderRequestCompleted();

            //create contact list table
            if (CreateContactList("contacts" + contact_id) != 0)
                return -2; //error during creation of contact list

            //create account
            WaitForRequestExecution(MakeExecuteRequest("insert into konta values (" + contact_id + ", \'" + contact_name + "\', \'" + password + "\', 0, \'contacts" + contact_id + "\')"));

            return 0;
        }

        private int CreateContactList(string list_name)
        {
            //check does this list already exist
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name));
            if (error_during_req) { } //list doesn't exist
            else //list already exists
            {
                MarkReaderRequestCompleted();
                return -1;
            }

            //create list
            WaitForRequestExecution(MakeExecuteRequest("create table " + list_name + " (id_kontaktu int not null foreign key references konta(id_konta))"));

            return 0;
        }

        public int CreateGroup(string group_name, uint creators_id, uint communication_type)
        {
            //check is creator a valid user
            WaitForRequestExecution(MakeReaderRequest("select * from konta where id_konta=" + creators_id));
            if (!ReqRdr.HasRows) //creator doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //check is there any group with specified name
            WaitForRequestExecution(MakeReaderRequest("select * from grupy where nazwa_grupy=\'" + group_name + "\'"));
            if (ReqRdr.HasRows) //group already exists
            {
                MarkReaderRequestCompleted();
                return -2;
            }
            MarkReaderRequestCompleted();

            //find free group_id
            WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy order by id_grupy desc"));
            uint group_id = 1;
            if (ReqRdr.HasRows)
            {
                ReqRdr.Read();
                group_id = 1 + (uint)((int)ReqRdr["id_grupy"]);
            }
            MarkReaderRequestCompleted();

            //create member list table
            if (CreateContactList("members" + group_id) != 0)
                return -3; //error during creation of members list
            InsertContactIntoList(group_id, true, creators_id);

            //create group
            WaitForRequestExecution(MakeExecuteRequest("insert into grupy values (" + group_id + ", \'" + group_name + "\', " + creators_id + ", " + communication_type + ", \'members" + group_id + "\')"));

            return 0;
        }

        public int SetUserStatus(uint contact_id, uint new_status)
        {
            //check is contact_id a valid user
            WaitForRequestExecution(MakeReaderRequest("select id_konta from konta where id_konta=" + contact_id));
            if (!ReqRdr.HasRows) //contact doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //set new status
            WaitForRequestExecution(MakeExecuteRequest("update konta set status_konta=" + new_status + " where id_konta=" + contact_id));

            return 0;
        }

        public int SetUserPassword(uint contact_id, string new_password)
        {
            //check is contact_id a valid user
            WaitForRequestExecution(MakeReaderRequest("select id_konta from konta where id_konta=" + contact_id));
            if (!ReqRdr.HasRows) //contact doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //set new password
            WaitForRequestExecution(MakeExecuteRequest("update konta set haslo=" + new_password + " where id_konta=" + contact_id));

            return 0;
        }

        public int SetGroupCreator(uint group_id, uint new_creator_id)
        //server should earlier check is new creator active
        {
            //check is group_id valid
            WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy where id_grupy=" + group_id));
            if (!ReqRdr.HasRows) //group doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //check is new_creator_id a valid group member
            WaitForRequestExecution(MakeReaderRequest("select id_kontaktu from members" + group_id + " where id_kontaktu=" + new_creator_id));
            if (!ReqRdr.HasRows) //new_creator_id is not a group member
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //set new group creator
            WaitForRequestExecution(MakeExecuteRequest("update grupy set id_zalozyciela=" + new_creator_id + " where id_grupy=" + group_id));

            return 0;
        }

        public int InsertContactIntoList(uint list_id, bool is_group, uint contact_id)
        {
            //check is contact a valid user
            WaitForRequestExecution(MakeReaderRequest("select * from konta where id_konta=" + contact_id));
            if (!ReqRdr.HasRows) //contact doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //check is list name valid
            string list_name = (is_group == true ? "members" : "contacts") + list_id;
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name));
            if(error_during_req) //list doesn't exist
                return -2;
            MarkReaderRequestCompleted();

            //check is this contact already on the list
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name));
            bool found = false;
            while (ReqRdr.Read())
            {
                if ((uint)((int)ReqRdr["id_kontaktu"]) == contact_id)
                {
                    found = true;
                    break;
                }
            }
            MarkReaderRequestCompleted();
            if (found) //contact already on the list
                return -3;

            //insert contact
            WaitForRequestExecution(MakeExecuteRequest("insert into " + list_name + " values (" + contact_id + ")"));

            return 0;
        }

        public int RemoveContactFromList(uint list_id, bool is_group, uint contact_id)
        {
            //check is list name valid
            string list_name = (is_group == true ? "members" : "contacts") + list_id;
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name + " where id_kontaktu=" + contact_id));
            if (error_during_req) //list doesn't exist
                return -1;
            if (!ReqRdr.HasRows) //list doesn't contain this contact_id
            {
                MarkReaderRequestCompleted();
                return -2;
            }
            MarkReaderRequestCompleted();

            //remove contact
            WaitForRequestExecution(MakeExecuteRequest("delete from " + list_name + " where id_kontaktu=" + contact_id));

            return 0;
        }

        public int GetContactListCount(uint list_id, bool is_group)
        {
            int ret = 0;
            
            //check is list name valid
            string list_name = (is_group == true ? "members" : "contacts") + list_id;
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name));
            if(error_during_req) //list doesn't exist
                return -1;
            MarkReaderRequestCompleted();

            //get count
            WaitForRequestExecution(MakeReaderRequest("select count(*) as ilość from " + list_name));
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    ret = (int)ReqRdr["ilość"];
                }
            MarkReaderRequestCompleted();

            return ret;
        }

        public int InsertGroupFile(uint group_id, uint sender_id, ref FileData fd, uint timeout)
        //fd.filename is considered to contain server storage file path
        {
            //check is sender a valid user
            WaitForRequestExecution(MakeReaderRequest("select * from konta where id_konta=" + sender_id));
            if (!ReqRdr.HasRows) //contact doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //check is group valid
            WaitForRequestExecution(MakeReaderRequest("select * from grupy where id_grupy=" + group_id));
            if (!ReqRdr.HasRows) //group doesn't exist
            {
                MarkReaderRequestCompleted();
                return -2;
            }
            MarkReaderRequestCompleted();

            //get group members list name
            WaitForRequestExecution(MakeReaderRequest("select kontakty_nazwa from grupy where id_grupy=" + group_id));
            if (!ReqRdr.HasRows) //group doesn't exist
            {
                MarkReaderRequestCompleted();
                return -2;
            }
            string list_name = "";
            while (ReqRdr.Read())
            {
                list_name = (string)ReqRdr["kontakty_nazwa"];
            }
            MarkReaderRequestCompleted();

            //check is list name valid
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name));
            if(error_during_req) //list doesn't exist
                return -3;
            MarkReaderRequestCompleted();

            //check is sender a member of a group
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name + " where id_kontaktu=" + sender_id));
            if (!ReqRdr.HasRows) //sender isn't a group member
            {
                MarkReaderRequestCompleted();
                return -4;
            }
            MarkReaderRequestCompleted();

            //find free file_id
            WaitForRequestExecution(MakeReaderRequest("select id_pliku from pliki_grup order by id_pliku desc"));
            fd.file_id = 1;
            if (ReqRdr.HasRows)
            {
                ReqRdr.Read();
                fd.file_id = 1 + (uint)((int)ReqRdr["id_pliku"]);
            }
            MarkReaderRequestCompleted();

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
            WaitForRequestExecution(MakeExecuteRequest("insert into pliki_grup values (" + fd.file_id + ", \'" + fd.filename + "\', " + fd.filesize + ", "
                + sender_id + ", " + group_id + ", \'" + dt + "\', \'" + dt2 + "\')"));

            return 0;
        }

        public int InsertPendingMessage(uint sender_id, uint receiver_id, bool receiver_is_group, string msg)
        {
            //check is sender_id a valid user
            WaitForRequestExecution(MakeReaderRequest("select id_konta from konta where id_konta=" + sender_id));
            if (!ReqRdr.HasRows) //user doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //check is receiver_id a valid user or group
            if (receiver_is_group == false)
                WaitForRequestExecution(MakeReaderRequest("select id_konta from konta where id_konta=" + receiver_id));
            else
                WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy where id_grupy=" + receiver_id));
            if (!ReqRdr.HasRows) //user or group doesn't exist
            {
                MarkReaderRequestCompleted();
                return -2;
            }
            MarkReaderRequestCompleted();

            //if receiver is group then check is sender_id a valid group member
            if (receiver_is_group == true)
            {
                WaitForRequestExecution(MakeReaderRequest("select id_kontaktu from members" + receiver_id + " where id_kontaktu=" + sender_id));
                if (!ReqRdr.HasRows) //user isn't a group member
                {
                    MarkReaderRequestCompleted();
                    return -3;
                }
                MarkReaderRequestCompleted();
            }

            //find free message_id
            WaitForRequestExecution(MakeReaderRequest("select id_wiadomosci from oczekujące_wiadomości order by id_wiadomosci desc"));
            uint message_id = 0;
            if (ReqRdr.HasRows)
            {
                ReqRdr.Read();
                message_id = 1 + (uint)((int)ReqRdr["id_wiadomosci"]);
            }
            MarkReaderRequestCompleted();

            //insert pending message
            WaitForRequestExecution(MakeExecuteRequest("insert into oczekujące_wiadomości values (" + message_id + ", " + sender_id + ", " + receiver_id + ", " + (receiver_is_group == true ? 1 : 0) + ", \'" + msg + "\', \'" + DateTime.Now + "\')"));

            return 0;
        }

        public ContactData GetContactData(uint contact_id)
        {
            ContactData ret = new ContactData();
            
            //get contact data
            WaitForRequestExecution(MakeReaderRequest("select * from konta where id_konta=" + contact_id));
            if (!ReqRdr.HasRows) //user not exists
            {
                MarkReaderRequestCompleted();
                return null;
            }
            while (ReqRdr.Read())
            {
                ret.contact_id = (uint)((int)ReqRdr["id_konta"]);
                ret.contact_name = (string)ReqRdr["nazwa"];
                ret.status = (uint)((int)ReqRdr["status_konta"]);
            }
            MarkReaderRequestCompleted();

            return ret;
        }

        public ContactData GetContactData(string contact_name)
        {
            ContactData ret = new ContactData();
            
            //get contact data
            WaitForRequestExecution(MakeReaderRequest("select * from konta where nazwa=\'" + contact_name + "\'"));
            if (!ReqRdr.HasRows) //user not exists
            {
                MarkReaderRequestCompleted();
                return null;
            }
            while (ReqRdr.Read())
            {
                ret.contact_id = (uint)((int)ReqRdr["id_konta"]);
                ret.contact_name = (string)ReqRdr["nazwa"];
                ret.status = (uint)((int)ReqRdr["status_konta"]);
            }
            MarkReaderRequestCompleted();

            return ret;
        }

        public List<ContactData> GetContactsData()
        {
            List<ContactData> ret = new List<ContactData>();
            
            //get contact data
            WaitForRequestExecution(MakeReaderRequest("select * from konta"));
            if (!ReqRdr.HasRows) //no user not exists
            {
                MarkReaderRequestCompleted();
                return null;
            }
            while (ReqRdr.Read())
            {
                ContactData cd = new ContactData();
                cd.contact_id = (uint)((int)ReqRdr["id_konta"]);
                cd.contact_name = (string)ReqRdr["nazwa"];
                cd.status = (uint)((int)ReqRdr["status_konta"]);
                ret.Add(cd);
            }
            MarkReaderRequestCompleted();

            return ret;
        }

        public GroupData GetGroupData(string group_name)
        {
            GroupData ret = new GroupData();
            string members_list_name = "";

            //get group data
            WaitForRequestExecution(MakeReaderRequest("select * from grupy where nazwa_grupy=\'" + group_name + "\'"));
            if (!ReqRdr.HasRows) //group not exists
            {
                MarkReaderRequestCompleted();
                return null;
            }
            while (ReqRdr.Read())
            {
                ret.group_id = (uint)((int)ReqRdr["id_grupy"]);
                ret.group_name = (string)ReqRdr["nazwa_grupy"];
                ret.communication_type = (uint)((int)ReqRdr["rodzaj_komunikacji"]);
                ret.creators_id = (uint)((int)ReqRdr["id_zalozyciela"]);
                members_list_name = (string)ReqRdr["kontakty_nazwa"];
            }
            MarkReaderRequestCompleted();

            //get members list
            ret.members = GetContactListData(members_list_name);

            return ret;
        }

        public GroupData GetGroupData(uint group_id)
        {
            GroupData ret = new GroupData();
            string members_list_name = "";

            //get group data
            WaitForRequestExecution(MakeReaderRequest("select * from grupy where id_grupy=" + group_id));
            if (!ReqRdr.HasRows) //group not exists
            {
                MarkReaderRequestCompleted();
                return null;
            }
            while (ReqRdr.Read())
            {
                ret.group_id = (uint)((int)ReqRdr["id_grupy"]);
                ret.group_name = (string)ReqRdr["nazwa_grupy"];
                ret.communication_type = (uint)((int)ReqRdr["rodzaj_komunikacji"]);
                ret.creators_id = (uint)((int)ReqRdr["id_zalozyciela"]);
                members_list_name = (string)ReqRdr["kontakty_nazwa"];
            }
            MarkReaderRequestCompleted();

            //get members list
            ret.members = GetContactListData(members_list_name);

            return ret;
        }

        private List<ContactData> GetContactListData(string list_name)
        {
            List<ContactData> ret = new List<ContactData>();
            
            //get list data
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name));
            if(error_during_req) //list not exists
                return null;
            List<uint> contact_ids = new List<uint>();
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    contact_ids.Add((uint)((int)ReqRdr["id_kontaktu"]));
                }
            MarkReaderRequestCompleted();
            foreach (uint contact_id in contact_ids)
            {
                ContactData contact = GetContactData(contact_id);
                if (contact != null)
                    ret.Add(contact);
            }

            return ret;
        }

        public UserData GetUserData(uint contact_id)
        {
            UserData ret = new UserData();
            string contact_list_name = "";
            
            //get user data
            WaitForRequestExecution(MakeReaderRequest("select * from konta where id_konta=" + contact_id));
            if (!ReqRdr.HasRows) //user not exists
            {
                MarkReaderRequestCompleted();
                return null;
            }
            while (ReqRdr.Read())
            {
                ret.contact_id = (uint)((int)ReqRdr["id_konta"]);
                ret.contact_name = (string)ReqRdr["nazwa"];
                ret.password = (string)ReqRdr["haslo"];
                ret.status = (uint)((int)ReqRdr["status_konta"]);
                contact_list_name = (string)ReqRdr["lista_kontaktow"];
            }
            MarkReaderRequestCompleted();

            //get contacts data
            ret.contacts = GetContactListData(contact_list_name);

            //get groups data
            WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy"));
            List<uint> group_ids = new List<uint>();
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    group_ids.Add((uint)((int)ReqRdr["id_grupy"]));
                }
            MarkReaderRequestCompleted();
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

        public UserData GetUserData(string contact_name)
        {
            UserData ret = new UserData();
            string contact_list_name = "";
            
            //get user data
            WaitForRequestExecution(MakeReaderRequest("select * from konta where nazwa=\'" + contact_name + "\'"));
            if (!ReqRdr.HasRows) //user not exists
            {
                MarkReaderRequestCompleted();
                return null;
            }
            while (ReqRdr.Read())
            {
                ret.contact_id = (uint)((int)ReqRdr["id_konta"]);
                ret.contact_name = (string)ReqRdr["nazwa"];
                ret.password = (string)ReqRdr["haslo"];
                ret.status = (uint)((int)ReqRdr["status_konta"]);
                contact_list_name = (string)ReqRdr["lista_kontaktow"];
            }
            MarkReaderRequestCompleted();

            //get contacts data
            ret.contacts = GetContactListData(contact_list_name);

            //get groups data
            WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy"));
            List<uint> group_ids = new List<uint>();
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    group_ids.Add((uint)((int)ReqRdr["id_grupy"]));
                }
            MarkReaderRequestCompleted();
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
            
            //check is group valid
            WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy where id_grupy=" + group_id));
            if (!ReqRdr.HasRows) //group not exists
            {
                MarkReaderRequestCompleted();
                return null;
            }
            MarkReaderRequestCompleted();

            //get file data
            WaitForRequestExecution(MakeReaderRequest("select * from pliki_grup where id_pliku=" + file_id + (group_id != 0 ? " and id_grupy=" + group_id : "")));
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    ret.file_id = (uint)((int)ReqRdr["id_pliku"]);
                    ret.filename = (string)ReqRdr["ścieżka"];
                    ret.filesize = (uint)((int)ReqRdr["rozmiar"]);
                    ret.sender_id = (uint)((int)ReqRdr["id_nadawcy"]);
                    ret.group_id = (uint)((int)ReqRdr["id_grupy"]);
                    ret.upload = (DateTime)ReqRdr["dataczas_wyslania"];
                    ret.timeout = (DateTime)ReqRdr["dataczas_konca"];
                }
            else //file doesn't exist
            {
                MarkReaderRequestCompleted();
                return null;
            }
            MarkReaderRequestCompleted();

            return ret;
        }

        public List<GroupFileData> GetGroupFiles(uint contact_id = 0, bool is_group = false)
        {
            List<GroupFileData> ret = new List<GroupFileData>();
            
            //if contact_id is 0 then ignore this test
            if (contact_id != 0)
                //check is contact_id a valid user
                if (is_group == false)
                {
                    WaitForRequestExecution(MakeReaderRequest("select id_konta from konta where id_konta=" + contact_id));
                    if (!ReqRdr.HasRows) //user doesn't exist
                    {
                        MarkReaderRequestCompleted();
                        return null;
                    }
                    MarkReaderRequestCompleted();
                }
                else //check is contact_id a valid group
                {
                    WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy where id_grupy=" + contact_id));
                    if (!ReqRdr.HasRows) //group doesn't exist
                    {
                        MarkReaderRequestCompleted();
                        return null;
                    }
                    MarkReaderRequestCompleted();
                }

            //get file data
            WaitForRequestExecution(MakeReaderRequest("select * from pliki_grup" + (contact_id != 0 ? " where id_" + (is_group == true ? "grupy" : "nadawcy") + "=" + contact_id : "")));
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    GroupFileData gfd = new GroupFileData();
                    gfd.file_id = (uint)((int)ReqRdr["id_pliku"]);
                    gfd.filename = (string)ReqRdr["ścieżka"];
                    gfd.filesize = (uint)((int)ReqRdr["rozmiar"]);
                    gfd.sender_id = (uint)((int)ReqRdr["id_nadawcy"]);
                    gfd.group_id = (uint)((int)ReqRdr["id_grupy"]);
                    gfd.upload = (DateTime)ReqRdr["dataczas_wyslania"];
                    gfd.timeout = (DateTime)ReqRdr["dataczas_konca"];
                    ret.Add(gfd);
                }
            else //file doesn't exist
            {
                MarkReaderRequestCompleted();
                return null;
            }
            MarkReaderRequestCompleted();

            return ret;
        }

        public List<PendingMessage> GetPendingMessages(uint contact_id, bool is_group = false)
        {
            List<PendingMessage> ret = new List<PendingMessage>();
            
            //check is contact_id a valid user or group
            if (is_group == false)
                WaitForRequestExecution(MakeReaderRequest("select id_konta from konta where id_konta=" + contact_id));
            else
                WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy where id_grupy=" + contact_id));
            if (!ReqRdr.HasRows) //user or group doesn't exist
            {
                MarkReaderRequestCompleted();
                return null;
            }
            MarkReaderRequestCompleted();

            //get pending messages
            WaitForRequestExecution(MakeReaderRequest("select * from oczekujące_wiadomości where id_docelowy=" + contact_id + " and czy_grupowy=" + (is_group == true ? 1 : 0)));
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    PendingMessage pm = new PendingMessage();
                    pm.message_id = (uint)((int)ReqRdr["id_wiadomosci"]);
                    pm.sender_id = (uint)((int)ReqRdr["id_nadawcy"]);
                    pm.receiver_id = (uint)((int)ReqRdr["id_docelowy"]);
                    pm.receiver_is_group = ((int)ReqRdr["czy_grupowy"] != 0 ? true : false);
                    pm.message = (string)ReqRdr["tresc_wiadomosci"];
                    pm.send_date = (DateTime)ReqRdr["data_czas_wyslania"];
                    ret.Add(pm);
                }
            MarkReaderRequestCompleted();

            return ret;
        }

        public int RemoveUser(string contact_name, out RemoveSummary summary)
        //server will get all removed data in summary and it has to inform all interested contacts about changes
        //server will have to check is it necessary to erase groups (using summary.changes_member_lists collection)
        {
            //check is contact_id a valid user
            WaitForRequestExecution(MakeReaderRequest("select id_konta from konta where nazwa=\'" + contact_name + "\'"));
            if (!ReqRdr.HasRows) //user doesn't exist
            {
                MarkReaderRequestCompleted();
                summary = null;
                return -1;
            }
            MarkReaderRequestCompleted();

            summary = new RemoveSummary();

            //get user data
            summary.ud = GetUserData(contact_name);

            //get group files sent by this user
            summary.gfdl = GetGroupFiles(summary.ud.contact_id);

            //remove user data from any contacts lists
            WaitForRequestExecution(MakeReaderRequest("select id_konta from konta where id_konta!=" + summary.ud.contact_id));
            List<uint> contact_ids = new List<uint>();
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    contact_ids.Add((uint)((int)ReqRdr["id_konta"]));
                }
            MarkReaderRequestCompleted();
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
            WaitForRequestExecution(MakeReaderRequest("select id_wiadomosci from oczekujące_wiadomości where id_nadawcy=" + summary.ud.contact_id + " or (id_docelowy=" + summary.ud.contact_id + " and czy_grupowy=0)"));
            List<uint> message_ids = new List<uint>();
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    message_ids.Add((uint)((int)ReqRdr["id_wiadomosci"]));
                }
            MarkReaderRequestCompleted();
            foreach (uint message_id in message_ids)
            {
                RemovePendingMessage(message_id);
            }

            //remove contacts list
            RemoveContactList("contacts" + summary.ud.contact_id);

            //remove user
            WaitForRequestExecution(MakeExecuteRequest("delete from konta where id_konta=" + summary.ud.contact_id));

            return 0;
        }

        public int RemoveGroupFile(uint file_id)
        {
            //check is file_id valid
            WaitForRequestExecution(MakeReaderRequest("select id_pliku from pliki_grup where id_pliku=" + file_id));
            if (!ReqRdr.HasRows) //file doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //delete file entry
            WaitForRequestExecution(MakeExecuteRequest("delete from pliki_grup where id_pliku=" + file_id));

            return 0;
        }

        public int RemovePendingMessage(uint message_id)
        {
            //check is file_id valid
            WaitForRequestExecution(MakeReaderRequest("select id_wiadomosci from oczekujące_wiadomości where id_wiadomosci=" + message_id));
            if (!ReqRdr.HasRows) //pending message doesn't exist
            {
                MarkReaderRequestCompleted();
                return -1;
            }
            MarkReaderRequestCompleted();

            //delete pending message
            WaitForRequestExecution(MakeExecuteRequest("delete from oczekujące_wiadomości where id_wiadomosci=" + message_id));

            return 0;
        }

        public int RemoveGroup(uint group_id, out RemoveSummary summary)
        {
            //check is group_id valid
            WaitForRequestExecution(MakeReaderRequest("select id_grupy from grupy where id_grupy=" + group_id));
            if (!ReqRdr.HasRows) //group doesn't exist
            {
                MarkReaderRequestCompleted();
                summary = null;
                return -1;
            }
            MarkReaderRequestCompleted();

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
            WaitForRequestExecution(MakeReaderRequest("select id_wiadomosci from oczekujące_wiadomości where id_docelowy=" + summary.gd.group_id + " and czy_grupowy=1"));
            List<uint> message_ids = new List<uint>();
            if (ReqRdr.HasRows)
                while (ReqRdr.Read())
                {
                    message_ids.Add((uint)((int)ReqRdr["id_wiadomosci"]));
                }
            MarkReaderRequestCompleted();
            foreach (uint message_id in message_ids)
            {
                RemovePendingMessage(message_id);
            }

            //remove members list
            RemoveContactList("members" + summary.gd.group_id);

            //remove group
            WaitForRequestExecution(MakeExecuteRequest("delete from grupy where id_grupy=" + summary.gd.group_id));

            return 0;
        }

        private int RemoveContactList(string list_name)
        {
            //check is list_name valid
            WaitForRequestExecution(MakeReaderRequest("select * from " + list_name));
            if(error_during_req) //list doesn't exist
                return -1;
            MarkReaderRequestCompleted();

            //remove list content
            WaitForRequestExecution(MakeExecuteRequest("delete from " + list_name));

            //remove list
            WaitForRequestExecution(MakeExecuteRequest("drop table " + list_name));

            return 0;
        }
    }
}
