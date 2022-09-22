using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EviCRM.Backend4
{
    public class MySQL
    {
        public static MySqlConnection mysql_connection = new MySqlConnection($mysql);

        public async Task<string> MySql_ContextAsync(string cmd)
        {
            string value = "";
            await mysql_connection.OpenAsync();

            var command = new MySqlCommand(cmd, mysql_connection);
            MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                value = reader.GetValue(0).ToString();
                Debug.WriteLine(value);
                // do something with 'value'
            }

            mysql_connection.Close();
            return value;
        }

        public string getChatIDByLogin(string login)
        {
            return "SELECT chatid FROM aux_telegrambot WHERE login = '" + login + "';";
        }
        public string getLoginByChatID(string chatid)
        {
            return "SELECT login FROM aux_telegrambot WHERE chatid = '" + chatid + "';";
        }
        public string getLoginByIDAUX(string idaux)
        {
            return "SELECT login FROM aux_telegrambot WHERE idaux = '" + idaux + "';";
        }
        public string getChatIDByIDAUX(string idaux)
        {
            return "SELECT chatid FROM aux_telegrambot WHERE idaux = '" + idaux + "';";
        }
        public string deleteByIDAUX(string idaux)
        {
            return "DELETE FROM aux_telegrambot WHERE idaux = '" + idaux + "';";
        }
        public string updateChatID(string idaux, string chatid, string login)
        {
            return "UPDATE aux_telegrambot SET chatid = '" + chatid + "' , login = '" + login + "' WHERE idaux = " + idaux + ";";
        }


        public string createContact(Contact contact, string user_)
        {
            string res_str = "INSERT INTO alexandra_contacts (firstname, lastname, phonenumber, userId, vcard, login) VALUES ('";
            
            res_str+= contact.FirstName.ToString() + "', '";
            
            if (contact.LastName != null)
            {
                res_str += contact.LastName.ToString() + "', '";
            }
            else
            {
                res_str += "', '";
            }

            res_str += contact.PhoneNumber.ToString() + "', '";

            if (contact.UserId != null)
            {
                res_str += contact.UserId.ToString() + "', '";
            }
            else
            {
                res_str += "', '";
            }

            if (contact.Vcard != null)
            {
                res_str += contact.Vcard.ToString() + "', '";
            }
            else
            {
                res_str += "', '";
            }

            res_str += user_ + "');";

            return res_str;
        }

        public string createMapPoint(Location location, string point_name,string user_)
        {
            string res_str = "INSERT INTO alexandra_locations (lat, lng, name, user) VALUES ('";
            res_str += location.Latitude.ToString() + "', '";
            res_str += location.Longitude.ToString() + "', '";
            res_str += point_name.ToString() + "', '";
            res_str += user_ + "');";

            return res_str;
        }

        public string createPoll(Poll poll)
        {
            string res_str =  "INSERT INTO alexandra_polls (multiple_answers, correctOptionId, explanation, id, is_anon, is_closed, question, options, type) VALUES ('";
            res_str += poll.AllowsMultipleAnswers.ToString() + "', '";
           
                if (poll.CorrectOptionId.HasValue)
                {
                    res_str += poll.CorrectOptionId.ToString() + "', '";
                }
                else
                {
                    res_str += "', '";
                }

            if (poll.Explanation != null)
            {
                res_str += poll.Explanation.ToString() + "', '";
            }
            else
            {
                res_str += "', '";
            }

            res_str += poll.Id.ToString() + "', '";
            res_str+= poll.IsAnonymous.ToString() + "', '";
            res_str += poll.IsClosed.ToString() + "', '";
            res_str += poll.Question.ToString() + "', '";

            List<string> temp_str = new List<string>();

            if (poll.Options != null)
            {
                for (int i = 0; i< poll.Options.Count(); i++)
                {
                    temp_str.Add(poll.Options[i].Text.ToString());
                }
            }

            res_str += Program.alexandra_core.getMultipleStringEncodingString(temp_str) + "', '";

            res_str += poll.Type.ToString() + "')";
            return res_str;
        }

        public async Task<List<string>> getListStatusAsync()
        {
            List<string> dt = new List<string>();

            string cmd = "SELECT status FROM aux_telegrambot";

            string value = "";

            await mysql_connection.OpenAsync();

            using var command = new MySqlCommand(cmd, mysql_connection); using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                value = reader.GetValue(0).ToString();
                dt.Add(value);
            }
            mysql_connection.Close();
            return dt;
        }

        public async Task<List<string>> getListChatIDAsync()
        {
            List<string> dt = new List<string>();

            string cmd = "SELECT chatid FROM aux_telegrambot";

            string value = "";

            await mysql_connection.OpenAsync();

            using var command = new MySqlCommand(cmd, mysql_connection); using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                value = reader.GetValue(0).ToString();
                dt.Add(value);
            }
            mysql_connection.Close();
            return dt;
        }

        public async Task<List<string>> getListLoginAsync()
        {
            List<string> dt = new List<string>();

            string cmd = "SELECT login FROM aux_telegrambot";

            string value = "";

            await mysql_connection.OpenAsync();

            using var command = new MySqlCommand(cmd, mysql_connection); using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                value = reader.GetValue(0).ToString();
                dt.Add(value);
            }
            mysql_connection.Close();
            return dt;
        }

    }
}
