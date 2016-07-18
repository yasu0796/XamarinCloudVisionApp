using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CloudVisionSharedProject
{
    // https://developers.google.com/knowledge-graph/
    /*
     https://kgsearch.googleapis.com/v1/entities:search?query=font&key=AIzaSyDeFjLSxxjEQrL-q851br3EpLDvcU27u00&limit=1&indent=True&languages=ja

https://kgsearch.googleapis.com/v1/entities:search?ids=kg:/m/03scnj&key=AIzaSyDeFjLSxxjEQrL-q851br3EpLDvcU27u00&limit=1&indent=True

https://kgsearch.googleapis.com/v1/entities:search?ids=kg:/m/0dl567&key=AIzaSyDeFjLSxxjEQrL-q851br3EpLDvcU27u00&limit=1&indent=True


        ものによって対応している
https://kgsearch.googleapis.com/v1/entities:search?ids=/m/07thkr&key=AIzaSyDeFjLSxxjEQrL-q851br3EpLDvcU27u00&limit=1&indent=True
         
         */
    [DataContract]
    public class KnowledgeGraphResponseJSON
    {
        public static KnowledgeGraphResponseJSON Serialize(string jsonStr)
        {
            if (jsonStr == null || jsonStr.Length == 0) return null;
            var stream = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
            stream = new MemoryStream(bytes);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(KnowledgeGraphResponseJSON));
            var b = serializer.ReadObject(stream) as KnowledgeGraphResponseJSON;

            // データの代入
            return b;
        }

        public static string DeSerialize(KnowledgeGraphResponseJSON json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(KnowledgeGraphResponseJSON));

            var stream = new MemoryStream();
            serializer.WriteObject(stream, json);
            byte[] strarray = stream.ToArray();
            string result = Encoding.UTF8.GetString(strarray, 0, strarray.Length);
            return result;
        }

        [DataMember(Name = "@context")]
        public Context context { get; set; }
        [DataMember(Name = "@type")]
        public string type { get; set; }
        [DataMember]
        public ItemListElement[] itemListElement { get; set; }

        [DataContract]
        public class Context
        {
            [DataMember(Name = "@vocab")]
            public string vocab { get; set; }
            [DataMember]
            public string goog { get; set; }
            [DataMember]
            public string EntitySearchResult { get; set; }
            [DataMember]
            public string detailedDescription { get; set; }
            [DataMember]
            public string resultScore { get; set; }
            [DataMember]
            public string kg { get; set; }
        }

        [DataContract]
        public class ItemListElement
        {
            [DataMember(Name = "@type")]
            public string type { get; set; }
            [DataMember]
            public Result result { get; set; }
            [DataMember]
            double resultScore { get; set; }
        }

        [DataContract]
        public class Result
        {
            [DataMember(Name = "@id")]
            public string id { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember(Name = "@type")]
            public string[] type { get; set; }
            [DataMember]
            public Image image { get; set; }
            [DataMember]
            public DetailedDescription detailedDescription { get; set; }
            [DataMember]
            public string url { get; set; }
            public override string ToString() { return name; }
        }
        
        [DataContract]
        public class Image
        {
            [DataMember]
            public string contentUrl { get; set; }
            [DataMember]
            public string url { get; set; }
            [DataMember]
            public string license { get; set; }
        }
        [DataContract]
        public class DetailedDescription
        {
            [DataMember]
            public string articleBody { get; set; }
            [DataMember]
            public string url { get; set; }
            [DataMember]
            public string license { get; set; }
            public override string ToString() { return articleBody; }
        }
    }
}
