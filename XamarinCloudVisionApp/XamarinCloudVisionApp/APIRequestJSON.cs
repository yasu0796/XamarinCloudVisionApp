using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CloudVisionSharedProject
{
    [DataContract]
    public class APIRequestJSON
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageContent">Base64エンコードした画像</param>
        public APIRequestJSON(string imageContent, uint faceDetection = 10, uint landmarkDetection = 10, uint logoDetection = 10, uint labelDetection = 10, uint textDetection = 10, uint safeSearchDetection = 10, uint imageProperties = 0)
        {

            requests = new AnnotateImageRequest[1];
            requests[0] = new AnnotateImageRequest();
            requests[0].image = new Image(imageContent);
            // 本来なら features で0のものは出力しないようにすると良い
            requests[0].features = new Feature[7];
            requests[0].features[0] = new Feature("FACE_DETECTION",faceDetection);
            requests[0].features[1] = new Feature("LANDMARK_DETECTION",landmarkDetection);
            requests[0].features[2] = new Feature("LOGO_DETECTION",logoDetection);
            requests[0].features[3] = new Feature("LABEL_DETECTION",labelDetection);
            requests[0].features[4] = new Feature("TEXT_DETECTION",textDetection);
            requests[0].features[5] = new Feature("SAFE_SEARCH_DETECTION",safeSearchDetection);
            requests[0].features[6] = new Feature("IMAGE_PROPERTIES",imageProperties);// 現時点でResponceが未対応

            List<string> lng = new List<string>();
            lng.Add("ja");
            requests[0].imageContext = new ImageContext();
            requests[0].imageContext.languageHints = lng.ToArray();
        }

        /// <summary>Serialize はここでは使用しない．テストで作成しただけ</summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public APIRequestJSON Serialize (string jsonStr)
        {
            if (jsonStr == null || jsonStr.Length == 0) return null;
            var stream = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
            stream = new MemoryStream(bytes);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(APIRequestJSON));
            var b = serializer.ReadObject(stream) as APIRequestJSON;

            // データの代入
            return b;
        }

        /// <summary>このクラスに保存されている内容をJSON形式で取得します
        /// 
        /// </summary>
        public string RequestJSON {
            get {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(APIRequestJSON));
                var stream = new MemoryStream();
                serializer.WriteObject(stream, this);
                byte[] strarray = stream.ToArray();
                return Encoding.UTF8.GetString(strarray, 0, strarray.Length);
            }
        }


        [DataMember]
        AnnotateImageRequest[] requests { get; set; }

        [DataContract]
        public class AnnotateImageRequest
        {
            [DataMember]
            public Image image { get; set; }
            [DataMember]
            public Feature[] features { get; set; }
            [DataMember]
            public ImageContext imageContext { get; set; }
        }

        [DataContract]
        public class Image
        {
            [DataMember]
            public string content { get; set; }
            public Image(string contentStr)
            {
                content = contentStr;
            }
        }

        [DataContract]
        public class Feature
        {
            [DataMember]
            string type { get; set; }
            [DataMember]
            uint maxResults { get; set; }

            public Feature(string typeStr = "LOGO_DETECTION", uint results = 10)
            {
                type = typeStr;
                maxResults = results;
            }
        }

        /// <summary>
        /// https://cloud.google.com/vision/reference/rest/v1/images/annotate#imagecontext
        /// </summary>
        [DataContract]
        public class ImageContext
        {
            [DataMember]
            public string[] languageHints { get; set; }
        }
    }
}
