using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Net;


namespace CloudVisionSharedProject
{
    /// <summary>Cloud Vision APIの情報をもとに、KnowledgeGraph APIも参考にアプリで表示できるようなタグを出力する。</summary>
    public class CV2Tag
    {
        public static async Task<string[]> getTag(APIResponseJSON input)
        {
            List<string> result = new List<string>();

            if(input.responses != null)
            {
                var response = input.responses[0];
                string mid;
                KnowledgeGraphAPI kga = new KnowledgeGraphAPI();
                KnowledgeGraphResponseJSON kgrj;

                // 顔判定
                if (response.faceAnnotations != null) {
                    // 顔の人数を追加
                    int facecount = response.faceAnnotations.Length;
                    if (facecount == 1) {
                        result.Add("自撮り");
                    }
                    else
                    {
                        result.Add(facecount + "人で撮影");
                    }
                }

                // ランドマーク
                if (response.landmarkAnnotations != null)
                {
                    foreach (var l in response.landmarkAnnotations)
                    {
                        mid = l.mid;
                        kgrj = await kga.GetResult(mid);
                        if (kgrj.itemListElement.Length > 0)
                        {
                            result.Add(kgrj.itemListElement[0].result.name);
                        }
                        else
                        {
                            // 多分追加しなくていい
                            //result.Add(l.description);
                        }
                    }
                }

                // その他の判定
                if (response.labelAnnotations != null) {
                    foreach(var l in response.labelAnnotations)
                    {
                        mid = l.mid;
                        kgrj = await kga.GetResult(mid);
                        if (kgrj.itemListElement.Length > 0)
                        {
                            result.Add(kgrj.itemListElement[0].result.name);
                        }
                        else
                        {
                            // 多分追加しなくていい
                            result.Add(l.description);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        
        public static async Task<string[]> getTag(string url)
        {
            CloudVision cv = new CloudVision();
            APIResponseJSON result = new APIResponseJSON();
            string[] resulttags;

            cv.Filename = new Uri(url);
            result = await cv.GetResult();
            resulttags = await CV2Tag.getTag(result);
            return resulttags;
        }
    }
}
