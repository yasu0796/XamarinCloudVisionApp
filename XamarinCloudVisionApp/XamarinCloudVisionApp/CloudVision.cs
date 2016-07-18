using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace CloudVisionSharedProject
{
    // https://cloud.google.com/vision/docs/
    public class CloudVision
    {
        static string APIkey = "INPUTYOURKEY";
        #region 定数
        static Uri apiUrl = new Uri("https://vision.googleapis.com/v1/images:annotate");
        /// <summary>対応ファイル拡張子（BMP，RAWは容量の関係で省略）</summary>
        static string[] acceptFileExts = new string[] { "jpg", "jpeg", "png", "gif", "ico" };
        #endregion

        public Uri Filename { get; set; }

        #region APIパラメータ
        uint faceDetection, landmarkDetection, logoDetection, labelDetection, textDetection, safeSearchDetection, imageProperties;
        #endregion

        public CloudVision(uint faceDetectionCount = 10, uint landmarkDetectionCount = 10, uint logoDetectionCount = 10, uint labelDetectionCount = 10, uint textDetectionCount = 10, uint safeSearchDetectionCount = 10, uint imagePropertiesCount = 0) {
            faceDetection = faceDetectionCount;
            landmarkDetection = landmarkDetectionCount;
            logoDetection = logoDetectionCount;
            labelDetection = labelDetectionCount;
            textDetection = textDetectionCount;
            safeSearchDetection = safeSearchDetectionCount;
            imageProperties = imagePropertiesCount;
        }

        public async Task<APIResponseJSON> GetResult()
        {
            string resultJSON;
            // 画像の読み込み/エンコード
            string img = await base64encode(Filename);
            // JSON の生成
            APIRequestJSON req = new APIRequestJSON(img, faceDetection, labelDetection, labelDetection, textDetection, safeSearchDetection, imageProperties);
            /*using (StreamWriter sw = new StreamWriter("request.json"))
            {
                sw.Write(req.RequestJSON);
            }*/
            // リクエスト（JSON で取得）
            resultJSON = await getApiResult(req);// デバッグ時削除


            /*using (StreamReader sr = new StreamReader("sampleresponse.json"))
            {
                resultJSON = sr.ReadToEnd();
            }*/
            APIResponseJSON res = APIResponseJSON.Serialize(resultJSON);
            //var a = res.responses[0].faceAnnotations[0].landmarks[0];


            // JSON を APIResponseJSON へ変換し返却
            //Console.WriteLine(req.RequestJSON);

            return res;
        }

        /// <summary>API呼び出しのためURLの画像をBASE64エンコード</summary>
        /// <param name="url"></param>
        /// <returns></returns>
        async Task<string> base64encode(Uri url)
        {
            // 結果
            byte[] response;
            string encodedResponse;

            // 画像の読み込み
            if (url == null)
            {
                // Filename が null なら終了
                throw new FormatException("[base64encode] Filename が null で呼び出されました．");
            }/*
#if WINDOWS_UWP
            if (url.IsFile)
            {
                // ファイルの場合、そのままだと Unversal では動作しない
                using (StreamReader sr = new StreamReader(url.AbsoluteUri))
                using (MemoryStream ms = new MemoryStream()) // 結果出力用 Stream
                {
                    byte[] buf = new byte[1024];
                    int count = 0;
                    do
                    {
                        count = sr.Read(buf, 0, buf.Length);
                        ms.Write(buf, 0, count);
                        //fs.Write(buf, 0, count);
                    } while (count != 0);
                    response = ms.ToArray();
                }
            }

            else
#endif*/
            {
                // 一時ファイル名の確定
                string tempfilename = getFilenameFromUrl(url);


                // 他のプログラムと違って GZIP 圧縮不要（もとから画像のため）
                WebRequest req = WebRequest.Create(url);
                //var r = client.GetAsync(uri).Result;
                //req.GetResponseAsync
                using (WebResponse res = await req.GetResponseAsync())
                {
                    // ヘッダーよりおかしいtypeがあれば警告予定
                    var headers = res.Headers;

                    string contentType = headers["Content-Type"];

                    using (Stream st = res.GetResponseStream()) // 読み込み Stream
                    using (MemoryStream ms = new MemoryStream()) // 結果出力用 Stream
                                                                 //using (FileStream fs = new FileStream(tempfilename, FileMode.Create)) // 結果確認用ファイル保存用 Stream
                    {
                        byte[] buf = new Byte[1024];
                        int count = 0;
                        do
                        {
                            count = st.Read(buf, 0, buf.Length);
                            ms.Write(buf, 0, count);
                            //fs.Write(buf, 0, count);
                        } while (count != 0);
                        response = ms.ToArray();
                    }
                }
            }

            // エンコード
            encodedResponse = Convert.ToBase64String(response);

            return encodedResponse;
        }

        /// <summary>URLからそのファイル名を求めます
        /// http://stackoverflow.com/questions/1105593/get-file-name-from-uri-string-in-c-sharp
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string getFilenameFromUrl(Uri url)
        {
            string[] parts = url.ToString().Split('/');
            string filename;
            if (parts.Length > 0)
            {
                filename = parts[parts.Length - 1];
            }
            else
            {
                filename = url.ToString();
            }
            return filename;
        }

        async Task<string> getApiResult(APIRequestJSON reqJSON)
        {
            string result;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(apiUrl + "?key=" + APIkey);
            //req.AutomaticDecompression = DecompressionMethods.GZip; Universal は無理
            req.Method = "POST";
            using (Stream reqstream = await req.GetRequestStreamAsync())
            {
                byte[] postByteData = Encoding.UTF8.GetBytes(reqJSON.RequestJSON);
                reqstream.Write(postByteData, 0, postByteData.Length);
            }

            using (WebResponse res = await req.GetResponseAsync())
            using (Stream resStream = res.GetResponseStream())
            using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
            {
                result = sr.ReadToEnd();
            }
            return result;
        }

        /// <summary>ファイル名の拡張子から対応画像拡張子かを判断．オンラインの物は拡張子がない場合があるのでチェック不可</summary>
        /// <param name="localfilename"></param>
        /// <returns></returns>
        static public bool CheckImageFileName(string localfilename)
        {
            bool result = false;
            // 大文字小文字対応
            localfilename = localfilename.ToLower();
            // ファイル名の拡張子取得
            string[] fileSplit = localfilename.Split('.');
            string fileExt = fileSplit[fileSplit.Length - 1];

            foreach (string s in acceptFileExts)
            {
                if (fileExt == s)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        
        /// <summary>ファイル名の拡張子からJSON拡張子かを判断．オンラインの物は拡張子がない場合があるのでチェック不可</summary>
        /// <param name="localfilename"></param>
        /// <returns></returns>
        static public bool CheckJsonFileName(string localfilename)
        {
            bool result = false;
            // ファイル名の拡張子取得
            string[] fileSplit = localfilename.Split('.');
            string fileExt = fileSplit[fileSplit.Length - 1];
            localfilename = localfilename.ToLower();

            result = fileExt == "json";
            return result;
        }

        /// <summary>Windows 開くダイアログの Filter に </summary>
        public static string ExtFilterforDialog
        {
            get
            {
                string result = "対応画像ファイル|";
                foreach (string s in acceptFileExts)
                {
                    result += "*." + s + ";";
                }

                result += "|JSON ファイル|*.json";
                result += "|すべてのファイル|*.*";
                return result;
            }
        }
    }
}
