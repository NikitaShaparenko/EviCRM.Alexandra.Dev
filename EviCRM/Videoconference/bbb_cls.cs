using Newtonsoft.Json;

using System;
using System.Dynamic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.IO;

namespace EviCRM.Alexandra.EviCRM.Videoconference
{
    internal class ClsBBB
        {
            public static string StrServerIPAddress = "localhost";
            public static string StrSalt = "";

            #region "CreateMeeting"   
            public string getSHA1(byte[] temp)
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    var hash = sha1.ComputeHash(temp);
                    return Convert.ToBase64String(hash);
                }
            }

            public string log_bbb = "";

            public byte[] StringToByteArray(string input)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                return bytes;
            }

          //Создать собрание
            public dynamic CreateMeeting(CreateMeetingParameters createmeeting)
            {
                try
                {
                    string querystring = "";
                    PropertyInfo[] properties = typeof(CreateMeetingParameters).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        Console.WriteLine("{0}:{1}", property.Name, property.GetValue(createmeeting, null));
                        log_bbb += property.Name + " | " + property.GetValue(createmeeting, null);
                        if (property.GetValue(createmeeting, null) != null)
                        {
                            //Avoid 0 value from class
                            if (property.GetValue(createmeeting, null).GetType() == typeof(int))
                            {
                                if ((int)property.GetValue(createmeeting, null) == 0)
                                {
                                    continue;
                                }
                            }

                            bool toLower = false;
                            if (property.GetValue(createmeeting, null).GetType() == typeof(bool))
                            {
                                if ((bool)property.GetValue(createmeeting, null) == false)
                                {
                                    continue;
                                }
                                else
                                {
                                    toLower = true;
                                }
                            }

                            if (querystring == "")
                            {
                                querystring += property.Name + "=" + (toLower ? property.GetValue(createmeeting, null).ToString().ToLower() : property.GetValue(createmeeting, null));
                            }
                            else
                            {
                                querystring += "&" + property.Name + "=" + (toLower ? property.GetValue(createmeeting, null).ToString().ToLower() : property.GetValue(createmeeting, null));
                            }
                        }
                    }
                    log_bbb += Environment.NewLine + querystring;
                    Console.WriteLine("querystring" + querystring);
                    string StrParameters = querystring;

                   
                    string StrSHA1_CheckSum = Crypto.getSha1("create" + StrParameters + StrSalt);

                    byte[] temp_byte = StringToByteArray("create" + StrParameters + StrSalt);
                    string temp = "create?" + StrParameters + StrSalt;


                    string txt_str = "create" + StrParameters + StrSalt;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "create?" + StrParameters + "&checksum=" + StrSHA1_CheckSum);
                    request.Method = "POST";
                    string xml = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Slide.xml");
                    var stream = request.GetRequestStream();
                    byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(xml);
                    stream.Write(messageBytes, 0, messageBytes.Length);
                    stream.Flush();
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    XDocument doc = XDocument.Parse(sr.ReadToEnd());
                    string jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
            #endregion

            //Присоединиться к собранию
            public string JoinMeeting(Parameters joinmeeting)
            {
                try
                {
                    string querystring = "";
                    PropertyInfo[] properties = typeof(Parameters).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.GetValue(joinmeeting, null) != null)
                        {
                            //Avoid 0 value from class
                            if (property.GetValue(joinmeeting, null).GetType() == typeof(int))
                            {
                                if ((int)property.GetValue(joinmeeting, null) == 0)
                                {
                                    continue;
                                }
                            }
                          
                            if (property.GetValue(joinmeeting, null).GetType() == typeof(bool))
                            {
                                if ((bool)property.GetValue(joinmeeting, null) == false)
                                {
                                    continue;
                                }
                            }

                            if (querystring == "")
                            {
                                querystring += property.Name + "=" + property.GetValue(joinmeeting, null);
                            }
                            else
                            {
                                querystring += "&" + property.Name + "=" + property.GetValue(joinmeeting, null);
                            }
                        }
                    }
                    log_bbb = querystring;
                    Console.WriteLine("querystring" + querystring);

                    string StrParameters = querystring;
                    string StrSHA1_CheckSum = Crypto.getSha1("join" + StrParameters + StrSalt);

                    log_bbb = StrServerIPAddress + "join?" + StrParameters + "&checksum=" + StrSHA1_CheckSum;
                    return StrServerIPAddress + "join?" + StrParameters + "&checksum=" + StrSHA1_CheckSum;

                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
           
//isMeetingRunning
            public dynamic IsMeetingRunning(IsMeetingRunning ismeetingrunning)
            {
                try
                {

                    string querystring = "";
                    PropertyInfo[] properties = typeof(IsMeetingRunning).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {

                       if (property.GetValue(ismeetingrunning, null) != null)
                        {
                            //Avoid 0 value from class
                            if (property.GetValue(ismeetingrunning, null).GetType() == typeof(int))
                            {
                                if ((int)property.GetValue(ismeetingrunning, null) == 0)
                                {
                                    continue;
                                }
                            }
                            if (property.GetValue(ismeetingrunning, null).GetType() == typeof(bool))
                            {
                                if ((bool)property.GetValue(ismeetingrunning, null) == false)
                                {
                                    continue;
                                }
                            }

                            if (querystring == "")
                            {
                                querystring += property.Name + "=" + property.GetValue(ismeetingrunning, null);
                            }
                            else
                            {
                                querystring += "&" + property.Name + "=" + property.GetValue(ismeetingrunning, null);
                            }
                        }
                    }
                    log_bbb = querystring;

                    string StrParameters = querystring;
                    string StrSHA1_CheckSum = Crypto.getSha1("isMeetingRunning" + StrParameters + StrSalt);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "isMeetingRunning?" + StrParameters + "&checksum=" + StrSHA1_CheckSum);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    XDocument doc = XDocument.Parse(sr.ReadToEnd());
                    string jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
           
//GetMeetingInfo
            public dynamic GetMeetingInfo(GetMeetingInfos getmeetinginfos)
            {
                try
                {
                    string querystring = "";
                    PropertyInfo[] properties = typeof(GetMeetingInfos).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {

                       if (property.GetValue(getmeetinginfos, null) != null)
                        {
                            if (property.GetValue(getmeetinginfos, null).GetType() == typeof(int))
                            {
                                if ((int)property.GetValue(getmeetinginfos, null) == 0)
                                {
                                    continue;
                                }
                            }
                            
		            if (property.GetValue(getmeetinginfos, null).GetType() == typeof(bool))
                            {
                                if ((bool)property.GetValue(getmeetinginfos, null) == false)
                                {
                                    continue;
                                }
                            }
                            if (querystring == "")
                            {
                                querystring += property.Name + "=" + property.GetValue(getmeetinginfos, null);
                            }
                            else
                            {
                                querystring += "&" + property.Name + "=" + property.GetValue(getmeetinginfos, null);
                            }
                        }
                    }
                    log_bbb = querystring;

                    string StrParameters = querystring;
                    string StrSHA1_CheckSum = Crypto.getSha1("getMeetingInfo" + StrParameters + StrSalt);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "getMeetingInfo?" + StrParameters + "&checksum=" + StrSHA1_CheckSum);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    XDocument doc = XDocument.Parse(sr.ReadToEnd());
                    string jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
            #endregion

           //EndMeeting
            public dynamic EndMeeting(EndMeeting endmeeting)
            {
                try
                {
                    string querystring = "";
                    PropertyInfo[] properties = typeof(EndMeeting).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.GetValue(endmeeting, null) != null)
                        {
                             if (property.GetValue(endmeeting, null).GetType() == typeof(int))
                            {
                                if ((int)property.GetValue(endmeeting, null) == 0)
                                {
                                    continue;
                                }
                            }
                            
if (property.GetValue(endmeeting, null).GetType() == typeof(bool))
                            {
                                if ((bool)property.GetValue(endmeeting, null) == false)
                                {
                                    continue;
                                }
                            }
                            if (querystring == "")
                            {
                                querystring += property.Name + "=" + property.GetValue(endmeeting, null);
                            }
                            else
                            {
                                querystring += "&" + property.Name + "=" + property.GetValue(endmeeting, null);
                            }
                        }
                    }
                    log_bbb = querystring;

                    string StrParameters = querystring;
                    string StrSHA1_CheckSum = Crypto.getSha1("end" + StrParameters + StrSalt);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "end?" + StrParameters + "&checksum=" + StrSHA1_CheckSum);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    XDocument doc = XDocument.Parse(sr.ReadToEnd());
                    string jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
           	
//getMeetings
            {
                try
                {
                    Random r = new Random(0);
                    string StrParameters = "";
                    string StrSHA1_CheckSum = Crypto.getSha1("getMeetings" + StrParameters + StrSalt);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "getMeetings?" + "checksum=" + StrSHA1_CheckSum);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());

                    XmlSerializer serializer = new XmlSerializer(typeof(GetMeetingResponse));
                    var obj = (GetMeetingResponse)serializer.Deserialize(sr);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    throw ex;
                }
            }

            #region "getRecordings"
            /// <summary>
            /// To get recordings
            /// </summary>
            /// <param name="MeetingId"></param>
            /// <param name="recordID"></param>
            /// <param name="state"></param>
            /// <param name="meta"></param>
            /// <returns></returns>
            public dynamic getRecordings(getRecordingsParameters getRecordings)
            {
                try
                {
                    string querystring = "";
                    PropertyInfo[] properties = typeof(getRecordingsParameters).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {

                       if (property.GetValue(getRecordings, null) != null)
                        {
                            //Avoid 0 value from class
                            if (property.GetValue(getRecordings, null).GetType() == typeof(int))
                            {
                                if ((int)property.GetValue(getRecordings, null) == 0)
                                {
                                    continue;
                                }
                            }
                            if (property.GetValue(getRecordings, null).GetType() == typeof(bool))
                            {
                                if ((bool)property.GetValue(getRecordings, null) == false)
                                {
                                    continue;
                                }
                            }
                            if (querystring == "")
                            {
                                querystring += property.Name + "=" + property.GetValue(getRecordings, null);
                            }
                            else
                            {
                                querystring += "&" + property.Name + "=" + property.GetValue(getRecordings, null);
                            }
                        }
                    }
                    log_bbb = querystring;
                    string StrParameters = querystring;
                    string StrSHA1_CheckSum = Crypto.getSha1("getRecordings" + StrParameters + StrSalt);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "getRecordings?" + "checksum=" + StrSHA1_CheckSum);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    XDocument doc = XDocument.Parse(sr.ReadToEnd());
                    string jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
            #endregion

            #region "publishRecordings"

            /// <summary>
            /// To Publish recordings
            /// </summary>
            /// <param name="recordID"></param>
            /// <param name="publish"></param>
            /// <returns></returns>
            public dynamic publishRecordings(publishRecordingsParameters publishRecordings)
            {
                try
                {
                    string querystring = "";
                    PropertyInfo[] properties = typeof(publishRecordingsParameters).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {

                        // Console.WriteLine("{0}:{1}", property.Name, property.GetValue(createmeeting, null));
                        if (property.GetValue(publishRecordings, null) != null)
                        {
                            //Avoid 0 value from class
                            if (property.GetValue(publishRecordings, null).GetType() == typeof(int))
                            {
                                if ((int)property.GetValue(publishRecordings, null) == 0)
                                {
                                    continue;
                                }
                            }
                            //Avoid false value from class
                            if (property.GetValue(publishRecordings, null).GetType() == typeof(bool))
                            {
                                if ((bool)property.GetValue(publishRecordings, null) == false)
                                {
                                    continue;
                                }
                            }
                            if (querystring == "")
                            {
                                querystring += property.Name + "=" + property.GetValue(publishRecordings, null);
                            }
                            else
                            {
                                querystring += "&" + property.Name + "=" + property.GetValue(publishRecordings, null);
                            }
                        }
                    }
                    //Console.WriteLine("querystring" + querystring);
                    log_bbb = querystring;
                    string StrParameters = querystring;
                    string StrSHA1_CheckSum = Crypto.getSha1("publishRecordings" + StrParameters + StrSalt);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "publishRecordings?" + "checksum=" + StrSHA1_CheckSum);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    XDocument doc = XDocument.Parse(sr.ReadToEnd());
                    string jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
            #endregion

            #region "deleteRecordings"
            /// <summary>
            /// To delete Recordings
            /// </summary>
            /// <param name="recordID"></param>
            /// <returns></returns>
            public dynamic deleteRecordings(deleteRecordingsParameters deleteRecordingsParameters)
            {
                try
                {
                    string StrParameters = "recordID=" + deleteRecordingsParameters.recordID;
                    string StrSHA1_CheckSum = Crypto.getSha1("deleteRecordings" + StrParameters + StrSalt);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "deleteRecordings?" + "checksum=" + StrSHA1_CheckSum);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    XDocument doc = XDocument.Parse(sr.ReadToEnd());
                    string jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
            #endregion

            #region "updateRecordings"
            /// <summary>
            /// To delete Recordings
            /// </summary>
            /// <param name="recordID"></param>
            /// <returns></returns>
            public dynamic updateRecordings(updateRecordingsParameters updateRecordings)
            {
                try
                {
                    string querystring = "";
                    PropertyInfo[] properties = typeof(updateRecordingsParameters).GetProperties();
                    foreach (PropertyInfo property in properties)
                    {

                        // Console.WriteLine("{0}:{1}", property.Name, property.GetValue(createmeeting, null));
                        if (property.GetValue(updateRecordings, null) != null)
                        {
                            //Avoid 0 value from class
                            if (property.GetValue(updateRecordings, null).GetType() == typeof(int))
                            {
                                if ((int)property.GetValue(updateRecordings, null) == 0)
                                {
                                    continue;
                                }
                            }
                            //Avoid false value from class
                            if (property.GetValue(updateRecordings, null).GetType() == typeof(bool))
                            {
                                if ((bool)property.GetValue(updateRecordings, null) == false)
                                {
                                    continue;
                                }
                            }
                            if (querystring == "")
                            {
                                querystring += property.Name + "=" + property.GetValue(updateRecordings, null);
                            }
                            else
                            {
                                querystring += "&" + property.Name + "=" + property.GetValue(updateRecordings, null);
                            }
                        }
                    }
                    log_bbb = querystring;
                    string StrParameters = querystring;
                    string StrSHA1_CheckSum = Crypto.getSha1("updateRecordings" + StrParameters + StrSalt);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(StrServerIPAddress + "updateRecordings?" + "checksum=" + StrSHA1_CheckSum);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    XDocument doc = XDocument.Parse(sr.ReadToEnd());
                    string jsonText = JsonConvert.SerializeXNode(doc);
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                    return obj;
                }
                catch (Exception ex)
                {
                    //MessageBoxShow(ex.Message);
                    return null;
                }
            }
            #endregion
        }
    }