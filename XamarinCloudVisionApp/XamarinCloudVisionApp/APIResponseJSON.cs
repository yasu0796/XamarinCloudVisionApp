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
    /// <summary>
    /// https://cloud.google.com/vision/reference/rest/v1/images/annotate
    /// </summary>
    [DataContract]
    public class APIResponseJSON
    {
        public static APIResponseJSON Serialize(string jsonStr)
        {
            if (jsonStr == null || jsonStr.Length == 0) return null;
            var stream = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
            stream = new MemoryStream(bytes);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(APIResponseJSON));
            var b = serializer.ReadObject(stream) as APIResponseJSON;

            // データの代入
            return b;
        }

        public static string DeSerialize(APIResponseJSON json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(APIResponseJSON));

            var stream = new MemoryStream();
            serializer.WriteObject(stream, json);
            byte[] strarray = stream.ToArray();
            string result = Encoding.UTF8.GetString(strarray, 0, strarray.Length);
            return result;
        }

        [DataMember]
        public AnnotateImageResponse[] responses { get; set; }

        /// <summary>日本語での説明
        /// http://googlecloudplatform-japan.blogspot.jp/2015/12/google-cloud-vision-api.html
        /// </summary>
        [DataContract]
        public class AnnotateImageResponse
        {
            /// <summary>顔検知
            /// 画像に映る人物の顔を検知し、目や鼻、口の座標位置を検出するほか、喜びや悲しみなどの感情を含む 8 種類の属性について尤度を推定します。なお、個人の認識には対応していません。また、検知された情報が Google のサーバーに保存されることもありません</summary>
            [DataMember]
            public FaceAnnotation[] faceAnnotations { get; set; }
            /// <summary>ランドマーク検知
            /// 自然構造物や人工構造物を検知し、その緯度と経度を返します
            /// </summary>
            [DataMember]
            public EntityAnnotation[] landmarkAnnotations { get; set; }
            /// <summary>ロゴ検知
            /// 画像に含まれる製品ロゴを検知します。ブランドロゴや、その座標位置を返します
            /// </summary>
            [DataMember]
            public EntityAnnotation[] logoAnnotations { get; set; }
            /// <summary>ラベル／物体検知
            /// 画像に映るおもな物体（例えばクルマやネコなど）を、多数のカテゴリーの中からいずれかに分類します。多数の画像へのタグ付けや、画像を用いた検索やリコメンデーションなどの新しいサービスを簡単に実装できます
            /// </summary>
            [DataMember]
            public EntityAnnotation[] labelAnnotations { get; set; }
            /// <summary>OCR
            /// 画像からテキストを抽出します。多数の言語に対応し、言語の自動判定も行います
            /// </summary>
            [DataMember]
            public EntityAnnotation[] textAnnotations { get; set; }
            /// <summary>セーフサーチ検知
            /// Google SafeSearch の機能を活用し、画像が不適切な内容かどうかを検知します。ユーザーがアップロードした画像などのチェックが容易になります
            /// </summary>
            [DataMember]
            public SafeSearchAnnotation safeSearchAnnotation { get; set; }
            [DataMember]
            public Status error { get; set; }
        }

        /// <summary>
        /// https://cloud.google.com/vision/reference/rest/v1alpha1/images/annotate#FaceAnnotation
        /// </summary>
        [DataContract]
        public class FaceAnnotation
        {
            [DataMember]
            public BoundingPoly boundingPoly { get; set; }
            [DataMember]
            public BoundingPoly fdBoundingPoly { get; set; }
            [DataMember]
            public Landmark[] landmarks { get; set; }
            [DataMember]
            public double rollAngle { get; set; }
            [DataMember]
            public double panAngle { get; set; }
            [DataMember]
            public double tiltAngle { get; set; }
            [DataMember]
            public double detectionConfidence { get; set; }
            [DataMember]
            public double landmarkingConfidence { get; set; }
            [DataMember]
            public string joyLikelihood { get; set; }
            [DataMember]
            public string sorrowLikelihood { get; set; }
            [DataMember]
            public string angerLikelihood { get; set; }
            [DataMember]
            public string surpriseLikelihood { get; set; }
            [DataMember]
            public string underExposedLikelihood { get; set; }
            [DataMember]
            public string blurredLikelihood { get; set; }
            [DataMember]
            public string headwearLikelihood { get; set; }

            /// <summary>顔の特徴点．種類は
            /// https://cloud.google.com/vision/reference/rest/v1alpha1/images/annotate#Landmark
            /// </summary>
            [DataContract]
            public class Landmark
            {
                // enum はひとまず string で代替
                // enum の書き方
                // http://qiita.com/hugo-sb/items/be3a8148d1397d7a6545
                [DataMember]
                public string type { get; set; }
                [DataMember]
                public Position position { get; set; }
                
                /// <summary>
                /// https://cloud.google.com/vision/reference/rest/v1alpha1/images/annotate#Position
                /// </summary>
                [DataContract]
                public class Position
                {
                    [DataMember]
                    public double x { get; set; }
                    [DataMember]
                    public double y { get; set; }
                    [DataMember]
                    public double z { get; set; }
                }
            }
        }
        [DataContract]
        public class EntityAnnotation
        {
            [DataMember]
            public BoundingPoly boundingPoly { get; set; }
            // locations[]は未実装
            /// <summary>Knowledge Graph entity ID</summary>
            [DataMember]
            public string mid { get; set; }
            /// <summary>descriptionの言語</summary>
            [DataMember]
            public string locale { get; set; }
            [DataMember]
            public string description { get; set; }
            [DataMember]
            public double score { get; set; }
            [DataMember]
            public double confidence { get; set; }
            [DataMember]
            public double topicality { get; set; }
        }
        /// <summary>
        /// ドキュメント上：～Likelihood number[0.1] なのに実際は文字列
        /// </summary>
        [DataContract]
        public class SafeSearchAnnotation {
            [DataMember]
            public string adult { get; set; }
            [DataMember]
            public string spoof { get; set; }
            [DataMember]
            public string medical { get; set; }
            [DataMember]
            public string violence { get; set; }
        }
        /// <summary>エラーなど
        /// details[]は未実装
        /// </summary>
        [DataContract]
        public class Status
        {
            /// <summary>説明がないが，doubleではないだろうと仮定</summary>
            [DataMember]
            public int code { get; set; }
            [DataMember]
            public string message { get; set; }
        }
        /// <summary>検出された画像の境界（4点のxy座標）
        /// https://cloud.google.com/vision/reference/rest/v1alpha1/images/annotate#BoundingPoly
        /// </summary>
        [DataContract]
        public class BoundingPoly {
            [DataMember]
            public Vertex[] vertices { get; set; }
        }
        /// <summary>画像内の点を表すxy座標．BoundingPolyで使用
        /// https://cloud.google.com/vision/reference/rest/v1alpha1/images/annotate#Vertex
        /// </summary>
        [DataContract]
        public class Vertex {
            [DataMember]
            public double x { get; set; }
            [DataMember]
            public double y { get; set; }
        }
    }
}
