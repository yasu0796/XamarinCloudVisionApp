using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace CloudVisionSharedProject
{
    public class KnowledgeGraphAPI
    {
        static string APIkey = "INPUTYOURKEY";
        static string apiUrl = "https://kgsearch.googleapis.com/v1/entities:search";
        //https://kgsearch.googleapis.com/v1/entities:search?query=font&key=AIzaSyDeFjLSxxjEQrL-q851br3EpLDvcU27u00&limit=1&indent=True&languages=ja

        public async Task<KnowledgeGraphResponseJSON> GetResult(string id) {
            string str;
            KnowledgeGraphResponseJSON result;
            str = await getApiIdResult(id);
            result = KnowledgeGraphResponseJSON.Serialize(str);
            return result;
        }
        /// <summary>クエリから結果を取得</summary>
        /// <param name="reqJSON"></param>
        /// <returns></returns>
        async Task<string> getApiQueryResult(string query)
        {
            string result;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(apiUrl + "?languages=ja&key=" + APIkey + "&query=" + query);
            req.Method = "GET";

            using (WebResponse res = await req.GetResponseAsync())
            using (Stream resStream = res.GetResponseStream())
            using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
            {
                result = sr.ReadToEnd();
            }
            return result;
        }

        /// <summary>IDから結果を取得</summary>
        /// <param name="reqJSON"></param>
        /// <returns></returns>
        async Task<string> getApiIdResult(string id)
        {
            string result;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(apiUrl + "?languages=ja&key=" + APIkey + "&ids=" + id);
            req.Method = "GET";

            using (WebResponse res = await req.GetResponseAsync())
            using (Stream resStream = res.GetResponseStream())
            using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
            {
                result = sr.ReadToEnd();
            }
            return result;
        }
    }
}
