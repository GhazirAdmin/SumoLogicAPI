using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace SumoLogicAPI
{
    class Request
    {
        private List<string> NameServer = new List<string>();
        private List<int> CountServer = new List<int>();
        private CookieContainer mycookies = new CookieContainer();
        private string JobID=null;
        private string result=null;
        public Tuple<List<string>, List<int>> PostJsonDataToApi(QueryObj QueryObj, string Baseurl,string AccessId,string AccessKey)
        {
            string credentials = Base64Encode(AccessId,AccessKey);
            HttpWebRequest httpWebRequest = null;
            httpWebRequest=Prepare(httpWebRequest, credentials, Baseurl,"POST");
            StreamWriter(httpWebRequest, QueryObj);
            JobID = GetJobID(httpWebRequest);
            httpWebRequest = Prepare(httpWebRequest, credentials, JobURL(Baseurl, JobID), "GET");
            if(GetJobStatus(httpWebRequest, JobID))
            {
                httpWebRequest = Prepare(httpWebRequest, credentials, MessageUrl(Baseurl, JobID), "GET");
                result = GetResult(httpWebRequest, JobID);
            }
            JsonToList();
            var tuple = new Tuple<List<string>,List<int>>(NameServer, CountServer);
            return tuple;
        }
        private HttpWebRequest Prepare(HttpWebRequest httpWebRequest,string credentials,string Url,string methode)
        {
            httpWebRequest = null;
            httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            if(methode=="POST")
            {
                httpWebRequest.Method = "POST";
            }
            else
            {
                httpWebRequest.Method = "GET";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.AllowAutoRedirect = true;
            }
            httpWebRequest.ReadWriteTimeout = 100000; //this can cause issues which is why we are manually setting this
            httpWebRequest.ContentType = "application/json;charset=UTF-8";
            httpWebRequest.Accept = "application/json;charset=UTF-8";
            httpWebRequest.ProtocolVersion = HttpVersion.Version11;
            httpWebRequest.Headers.Add("Authorization", "Basic"+" "+ credentials); // "Basic 4dfsdfsfs4sf5ssfsdfs=="
            httpWebRequest.CookieContainer = mycookies;
            return httpWebRequest;
        }
        private void StreamWriter(HttpWebRequest httpWebRequest, QueryObj data)
        {
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string mydata = new JavaScriptSerializer().Serialize(new
                {
                    query = data.Query,
                    from = data.From,
                    to = data.To,
                    timeZone = data.TimeZone
                });
                streamWriter.Write(mydata);
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
            }
        }
        private string GetJobID(HttpWebRequest httpWebRequest)
        {
            string jobID = null;
            
                HttpWebResponse resp = (HttpWebResponse)httpWebRequest.GetResponse();
                
                string respStr = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                var temp = respStr.Split(',');
                temp = temp[0].Split('"');
                jobID = temp[3];
                resp.Close();
           
            return jobID;
        }
        private bool GetJobStatus(HttpWebRequest httpWebRequest, string jobid)
        {
            do
            {
                Thread.Sleep(5000);
                result = GetResult(httpWebRequest, jobid);
                var temp = result.Split('"');
                if (temp.Length > 3)
                {
                    result = temp[3];
                }
            } while (result != "DONE GATHERING RESULTS");
            return true;
        }
        private string GetResult(HttpWebRequest httpWebRequest, string jobid)
        {
           var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
        private string JobURL(string BaseUrl ,string id)
        {
            result = BaseUrl + "/" + id;
            return result;
        }
        private string MessageUrl(string BaseUrl, string id)
        {
            result = BaseUrl + "/" + id+ "/messages?offset=0&limit=1000";
            return result;
        }
        private static string Base64Encode(string a,string b)
        {
            string plainText = a + ":" + b;
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        private void JsonToList() // for my case i need to extract machien names and number of machine(this is a personal use)
        {
            List<string> GlobalServers = new List<string>();
            string[] delim1 = { "MachineName: " };
            string[] delim2 = { "CreatedDateTime:" };
            var model = JsonConvert.DeserializeObject<RootObject>(result);
            for (int i = 0; i < model.messages.Count; i++)
            {
                string[] ServerContainer = model.messages[i].map._raw.Split(delim1, StringSplitOptions.None);
                ServerContainer = ServerContainer[1].Split(delim2, StringSplitOptions.None);
                var tempop = ServerContainer[0];
                GlobalServers.Add(tempop);
            }
            for (int i = 0; i < GlobalServers.Count; i++)
            {
                if (GlobalServers[i] != "0")
                {
                    NameServer.Add(GlobalServers[i]);
                    var unit = GlobalServers[i];
                    GlobalServers[i] = "0";
                    var CountIt = 1;
                    for (int j = 0; j < GlobalServers.Count; j++)
                    {
                        if (unit == GlobalServers[j])
                        {
                            CountIt++;
                            GlobalServers[j] = "0";
                        }
                    }
                    CountServer.Add(CountIt);
                }
            }
        }
    }

}
