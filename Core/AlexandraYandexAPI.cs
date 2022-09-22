using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace EviCRM.Backend4.Core
{
    public class AuthModel
    {
        public string IAM { get; set; }
        public DateTime ExpireAt { get; set; }
    }

    public class post_request_auth
    {
        public string iamToken { get; set; }

        public string expiresAt { get; set; }
    }

    public class STT_viewmodel
    {
        public string result { get; set; }
    }

    public class AlexandraYandexAPI
    {
        const string API_OAUTH_TOKEN = "AQAAAABXA7V5AATuwW-MEe6GWUe7mIVGbZUZmps";
        const string API_YANDEX_FOLDER_ID = "b1gjsovt10enmlhllb95";

        AuthModel Alexa_Yandex_Auth = new AuthModel();

        public async Task<AuthModel> YandexAuth_IAM()
        {
            AuthModel auth_m = new AuthModel();

            try
            {
                string response = await Program.alexandra_httpClient.SendPOST(new List<string> { "yandexPassportOauthToken" }, new List<string> { API_OAUTH_TOKEN }, "https://iam.api.cloud.yandex.net/iam/v1/tokens");

                if (response != null)
                {
                    post_request_auth post_data = JsonConvert.DeserializeObject<post_request_auth>(response);
                    auth_m.IAM = post_data.iamToken;
                    auth_m.ExpireAt = DateTime.Parse(post_data.expiresAt);
                }

            }
            catch (Exception ex)
            {
                Program.alexandra_log.Log("Yandex API", "Yandex Auth : Exception = " + ex.Message, AlexandraLog.LogStatus.Fatal);
            }

            return auth_m;
        }


        public async Task<string> Send_STT(string ogg_file_path,AuthModel auth_token)
        {
            string response = "";

            byte[] file_body;

            List<string> req_args = new List<string>();
            List<string> req_vals = new List<string>();

            req_args.Add("lang");
            req_vals.Add("ru-RU");

            req_args.Add("folderId");
            req_vals.Add(API_YANDEX_FOLDER_ID);

            try
            {
                if (System.IO.File.Exists(ogg_file_path))
                {
                    file_body = System.IO.File.ReadAllBytes(ogg_file_path);
                }
                else
                {
                    Program.alexandra_log.Log("Yandex API", "Yandex Sync STT : File, path: " + ogg_file_path + " not found!", AlexandraLog.LogStatus.Fatal);
                    return "#$_file_not_found";
                }

                List<string> header = new List<string>();
                header.Add("Authorization: Bearer " + auth_token.IAM);

                response = await Program.alexandra_httpClient.SendPOST(req_args,req_vals,"https://stt.api.cloud.yandex.net/speech/v1/stt:recognize",header,"multipart/form-data",file_body);

            }
            catch(Exception ex)
            {
                Program.alexandra_log.Log("Yandex API", "Yandex Sync STT : Exception = " + ex.Message, AlexandraLog.LogStatus.Fatal);
            }


            if (response != null)
            {
                STT_viewmodel post_data = JsonConvert.DeserializeObject<STT_viewmodel>(response);
                return post_data.result;
            }

            return response;
            // await SendPOST("")
        }

        public async Task<string> Send_STT_API_KEY(string ogg_file_path,string api_key)
        {
            string response = "";

            byte[] file_body;

            List<string> req_args = new List<string>();
            List<string> req_vals = new List<string>();

            req_args.Add("lang");
            req_vals.Add("ru-RU");

            req_args.Add("folderId");
            req_vals.Add(API_YANDEX_FOLDER_ID);

            try
            {
                if (System.IO.File.Exists(ogg_file_path))
                {
                    file_body = System.IO.File.ReadAllBytes(ogg_file_path);
                }
                else
                {
                    Program.alexandra_log.Log("Yandex API", "Yandex Sync STT : File, path: " + ogg_file_path + " not found!", AlexandraLog.LogStatus.Fatal);
                    return "#$_file_not_found";
                }

                List<string> header = new List<string>();
                header.Add("Authorization: Api-Key " + api_key);

                response = await Program.alexandra_httpClient.SendPOST(req_args, req_vals, "https://stt.api.cloud.yandex.net/speech/v1/stt:recognize", header, "multipart/form-data", file_body);

            }
            catch (Exception ex)
            {
                Program.alexandra_log.Log("Yandex API", "Yandex Sync STT : Exception = " + ex.Message, AlexandraLog.LogStatus.Fatal);
            }

            if (response != null)
            {
                STT_viewmodel post_data = JsonConvert.DeserializeObject<STT_viewmodel>(response);
                return post_data.result;
            }


            return response;
            // await SendPOST("")
        }


    }
}
