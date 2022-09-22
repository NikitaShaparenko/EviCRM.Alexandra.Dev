using EviCRM.Backend4.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EviCRM.Backend4
{
    public static class GlobalVars
    {
        static List<string> telegram_chatID_lst = new List<string>();
        static List<string> telegram_login_lst = new List<string>();
    }

    public class httpServer
    {

        public const string ALEXANDRA_CONTROL_NAME = "Alexandra HTTP Web Server";
        public const string ALEXANDRA_EVICRM_URL = "localhost:7083";

        public async Task http_handler(string IP, string Port)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://" + IP + ":" + Port + "/get/");
            listener.Start();

            Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "Listening connections at " + "http://" + IP + ":" + Port + "/get/", Core.AlexandraLog.LogStatus.Debug);
            Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "Waiting for connections...", Core.AlexandraLog.LogStatus.Info);
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if (request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "[POST] : Request catched!", Core.AlexandraLog.LogStatus.Debug);
                        var content = reader.ReadToEnd();

                        Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "[POST] : Content : " + content, Core.AlexandraLog.LogStatus.Debug);
                        string responseStr = AnalyzeData(true, content);
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                }
                else
                {
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "[GET] : Request catched!", Core.AlexandraLog.LogStatus.Debug);
                        var content = reader.ReadToEnd();

                        string responseStr = AnalyzeData(false, content);
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
                       
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "[GET] : Content : " + responseStr, Core.AlexandraLog.LogStatus.Debug);
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                }
            }
            listener.Stop();
        }

        public string AnalyzeData(bool isPost, string input)
        {
            string cmd = GetRequestParseHeader(input);

            List<string> cmd_params_headers = new List<string>();
            List<string> cmd_params_values = new List<string>();

            cmd_params_headers = GetRequestParseParamsHeaders(input);
            cmd_params_values = GetRequestParseParamsValues(input);

            switch (cmd)
            {
                case "telegram_direct":
                    string chatID = getchatIDByUserName(cmd_params_values[1]);
                    string message = Base64Decode(cmd_params_values[3]);

                    Program.telegram_queue_chatID_lst.Add(chatID);
                    Program.telegram_queue_message_lst.Add(message);

                    Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "[EviCRM Server] : Telegram Direct: {Chat ID: "  + chatID + ", message: " + message, AlexandraLog.LogStatus.Info);
                    break;

                case "cron_add":

                    break;

                case "cron_delete":

                    break;

                case "set_env_path":
                    string env_path = Base64Decode(cmd_params_values[1]);
                    Program.EviCRM_WebRootEnvironment = env_path;
                    Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "[EviCRM Server] : New Environment path has been set: " + env_path,AlexandraLog.LogStatus.Info);
                    break;

            }
            return "";
        }

        public string getchatIDByUserName(string user_name)
        {
            for (int i = 0; i < Program.telegram_chatID_lst.Count; i++)
            {
                if (user_name == Program.telegram_login_lst[i])
                {
                    return Program.telegram_chatID_lst[i];
                }
            }
            return "";
        }

        public static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }
        public static string Base64Decode(string base64EncodedData)
        {
           if (IsBase64String(base64EncodedData))
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
           else
            {
                return base64EncodedData;
            }
        }
        public List<string> GetRequestParseParamsValues(string input)
        {
            //Example: http://localhost:9254/get?cmd=telegram_direct&data1=data

            List<int> values_positions = new List<int>();
            List<string> values = new List<string>();


            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '=')
                {
                    values_positions.Add(i + 1);
                }
            }

            foreach (int pos in values_positions)
            {
                int end_of_header_pos = pos;
                bool found = false;
                for (int z = pos; z < input.Length; z++)
                {
                    if (found == false)
                    {
                        if (input[z] == '&')
                        {
                            end_of_header_pos = z;
                            found = true;
                        }
                    }
                }
                if (found == false)
                {
                    end_of_header_pos = input.Length;
                }
                values.Add(input.Substring(pos, (end_of_header_pos - pos)));
            }
            return values;
        }
        public List<string> GetRequestParseParamsHeaders(string input)
        {
            //Example: http://localhost:9254/get?cmd=telegram_direct&data1=data

            List<int> values_positions = new List<int>();
            List<string> values = new List<string>();


            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '&' || input[i] == '?')
                {
                    values_positions.Add(i + 1);
                }
            }

            foreach (int pos in values_positions)
            {
                int end_of_header_pos = pos;
                bool found = false;
                for (int z = pos; z < input.Length; z++)
                {
                    if (found == false)
                    {
                        if (input[z] == '=')
                        {
                            end_of_header_pos = z;
                            found = true;
                        }
                    }
                }
                values.Add(input.Substring(pos, (end_of_header_pos - pos)));
            }
            return values;
        }
        public string GetRequestParseHeader(string inp)
        {
            bool found = false;
            //Example: http://localhost:9254/get?cmd=telegram_direct&data1=data

            if (inp.Contains("http://localhost:9254/get?"))
            {
                inp.Replace("http://localhost:9254/get?", "");
            }

            if (inp.Contains("cmd="))
            {
                int start_ch = inp.IndexOf("cmd=");
                int end_ch = start_ch;
                start_ch += 4;
                found = false;
                for (int z = start_ch; z < inp.Length; z++)
                {
                    if (!found)
                    {
                        if (inp[z] == '&')
                        {
                            end_ch = z;
                            found = true;
                        }

                    }
                }

                string cmd = "";

                cmd = inp.Substring(start_ch, (end_ch - start_ch));

                return cmd;
            }
            return "";
        }


        public class FindMeOut
        {
           public string id { get; set; }
           public string chatID { get; set; }
        }
    }
}

