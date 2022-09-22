using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EviCRM.Backend4.Core
{
    public class AlexandraCore
    {
        static MySQL mysql_C = new MySQL();
        public enum AlexandraLoginStatus {
            AuthorizedUser,
            AuthorizedAdmin,
            Unauthorized,
            Banned,
        }

        public string getMultipleStringEncodingString(List<string> input_str)
        {
            if (input_str != null)
            {

                string encoded = "";
                foreach (string str in input_str)
                {
                    encoded += str + "$$$";
                }
                return encoded;
            }
            return "";

        }
        public List<string> getMultipleStringDecodingString(string input_str)
        {
            List<string> encoded = new List<string>();
            if (input_str != null)
            {
                string[] subs = input_str.Split("$$$", StringSplitOptions.RemoveEmptyEntries);

                foreach (string str in subs)
                {
                    encoded.Add(str);
                }
                return encoded;
            }
            else
            {
                encoded.Add("");
                return encoded;
            }
        }

        public AlexandraLoginStatus LoginMiddleware(string chatID)
        {
            int p = int.MinValue;

            if (Program.telegram_chatID_lst != null)
            {
                for (int i = 0; i<Program.telegram_chatID_lst.Count;i++)
                {
                    if (Program.telegram_chatID_lst[i] == chatID)
                    {
                        p = i;
                        break;
                    }
                }
            }
            
            if (p != int.MinValue)
            {
                //Пользовать знаком
                return Program.telegram_login_status[p];
            }
            else
            {
                //Пользовать не знаком
                return AlexandraLoginStatus.Unauthorized;
            }

        }

        public async Task LoadUserRecords()
        {
            Program.telegram_chatID_lst.Clear();
            Program.telegram_login_lst.Clear();
            Program.telegram_login_status.Clear();
            Program.telegram_page.Clear();

            Program.telegram_chatID_lst = await mysql_C.getListChatIDAsync();
            Program.telegram_login_lst = await mysql_C.getListLoginAsync();

            List<string> status = await mysql_C.getListStatusAsync();

            if (status != null)
            {
                foreach (var elem in status)
                {
                    switch (elem)
                    {
                        case "ban":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.Banned);
                            break;

                        case "admin":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.AuthorizedAdmin);
                            break;

                        case "secretary":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.AuthorizedAdmin);
                            break;

                        case "owner":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.AuthorizedAdmin);
                            break;

                        case "user":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.AuthorizedUser);
                            break;

                        default:
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.Unauthorized);
                            break;
                    }

                    break;
                }
            }

            if (Program.telegram_chatID_lst != null)
            {
                for (int i = 0; i < Program.telegram_chatID_lst.Count; i++)
                {
                    Program.telegram_page.Add(Telegram.LogicDrive.AlexandraPages.Main);
                }
            }
        }
    }
}
