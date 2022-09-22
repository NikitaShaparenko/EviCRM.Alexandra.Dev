using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EviCRM.Backend4
{
    public class httpClient
    {

        HttpClient client;

        public void Init_HttpClient(HttpClient _client)
        {
            client = _client;
        }

        public async Task<string> SendPOST(List<string> args, List<string> vals, string url)
        {
            string url_postfix = "?";

            if (args != null && vals != null)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    if (i>0)
                    {
                        url_postfix += "&" + args[i] + "=" + vals[i];
                    }
                    else
                    {
                        url_postfix += args[i] + "=" + vals[i];
                    }
                }
            }

            var response = await client.PostAsync(url + url_postfix,null);

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> SendPOST(List<string> args, List<string> vals, string url,string header)
        {
            string url_postfix = "?";

            if (args != null && vals != null)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    if (i > 0)
                    {
                        url_postfix += "&" + args[i] + "=" + vals[i];
                    }
                    else
                    {
                        url_postfix += args[i] + "=" + vals[i];
                    }
                }
            }

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url + url_postfix),
                Method = HttpMethod.Post,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header));

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> SendPOST(List<string> args, List<string> vals, string url, List<string> headers)
        {
            string url_postfix = "?";

            if (args != null && vals != null)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    if (i > 0)
                    {
                        url_postfix += "&" + args[i] + "=" + vals[i];
                    }
                    else
                    {
                        url_postfix += args[i] + "=" + vals[i];
                    }
                }
            }

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url + url_postfix),
                Method = HttpMethod.Post,
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header));
                }
            }

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> SendPOST(List<string> args, List<string> vals, string url, List<string> headers,string ContentType)
        {
            string url_postfix = "?";

            if (args != null && vals != null)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    if (i > 0)
                    {
                        url_postfix += "&" + args[i] + "=" + vals[i];
                    }
                    else
                    {
                        url_postfix += args[i] + "=" + vals[i];
                    }
                }
            }

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url + url_postfix),
                Method = HttpMethod.Post,
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header));
                }
            }
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SendPOST(List<string> args, List<string> vals, string url, List<string> headers, string ContentType, byte[] file)
        {
            string url_postfix = "?";

            if (args != null && vals != null)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    if (i > 0)
                    {
                        url_postfix += "&" + args[i] + "=" + vals[i];
                    }
                    else
                    {
                        url_postfix += args[i] + "=" + vals[i];
                    }
                }
            }

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url + url_postfix);
                webRequest.Method = "POST";
                webRequest.ContentType = "multipart/form-data";

                if (headers != null)
                {
                    foreach(var elem in headers)
                    {
                        webRequest.Headers.Add(elem);
                    }
                }

                webRequest.ContentLength = file.Length;
                using (Stream postStream = webRequest.GetRequestStream())
                {
                    // Send the data.
                    postStream.Write(file, 0, file.Length);
                    postStream.Close();
                }

                
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Program.alexandra_log.Log("Yandex API", "POST Sending failed", Core.AlexandraLog.LogStatus.Fatal);
            }

            return "";
        }

        public async Task<string> SendPOST(List<string> args, List<string> vals, string url,bool form_urlencoded)
        {
            var values = new Dictionary<string, string>();

            for (int j = 0; j < args.Count; j++)
            {
                values.Add(args[j], vals[j]);
            }

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(url, content);

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> SendPOST(List<string> args, List<string> vals, string url, bool form_urlencoded, string header)
        {
            var values = new Dictionary<string, string>();

            for (int j = 0; j < args.Count; j++)
            {
                values.Add(args[j], vals[j]);
            }

            var content = new FormUrlEncodedContent(values);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header));

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> SendPOST(List<string> args, List<string> vals, string url, bool form_urlencoded, List<string> headers)
        {
            var values = new Dictionary<string, string>();

            for (int j = 0; j < args.Count; j++)
            {
                values.Add(args[j], vals[j]);
            }

            var content = new FormUrlEncodedContent(values);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
            };

            if (headers != null)
            {
               foreach(string header in headers)
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header));
                }
            }

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> SendPOST(List<string> args, List<string> vals, string url, bool form_urlencoded, List<string> headers,string ContentType)
        {
            var values = new Dictionary<string, string>();

            for (int j = 0; j < args.Count; j++)
            {
                values.Add(args[j], vals[j]);
            }

            var content = new FormUrlEncodedContent(values);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
            };

            if (headers != null)
            {
                foreach (string header in headers)
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header));
                }
            }
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

     
        public async Task<string> SendGET(string url)
        {
            return await client.GetStringAsync(url);
        }
    }
}
