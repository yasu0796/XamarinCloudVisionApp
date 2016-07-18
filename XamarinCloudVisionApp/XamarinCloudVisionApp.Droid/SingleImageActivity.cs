using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Graphics;
using Android.Widget;
using Android.Views;
using Android.Util;
using Android.Provider;

using System.Threading.Tasks;
using System.IO;
using CloudVisionSharedProject;

namespace XamarinCloudVisionApp.Droid
{
    [Activity(Label = "SingleImageActivity")]
    public class SingleImageActivity : Activity
    {
        // 必要がなくなれば消すこと
        readonly ImageRepository _db = new ImageRepository();
        ImageItemforUI card = new ImageItemforUI();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ContentResolver cr = ContentResolver;
            // Create your application here
            
            // タイトルバーを消す
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.SingleImage);   // <--追加

            // 上に戻るボタンを表示
            //ActionBar.SetDisplayHomeAsUpEnabled(true);
            Button textview = FindViewById<Button>(Resource.Id.textView1);
            using (ImageButton button = FindViewById<ImageButton>(Resource.Id.imageButton1))
            //ImageView button = FindViewById<ImageView>(Resource.Id.imageButton1);
            {

                //this.Title = "ここに画像のファイル名が入ります";
                textview.Text = "ここに解析結果が入ります";

                var intent = this.Intent;
                if (intent != null && intent.HasExtra("data"))
                {
                    //Android.Net.Uri imgUri;
                    card = intent.GetParcelableExtra("data") as ImageItemforUI;
                    var tags = _db.GetImgTagInfo(card.imageId);
                    string tagtext = "";
                    //Title = card.path;
                    if (tags.Length == 0)
                        tagtext = "この画像は未解析です。この部分をクリックすると解析します。";
                    for (int i = 0; i < tags.Length; i++)
                    {
                        tagtext += tags[i].TagStr;
                        tagtext += ", ";
                    }
                    textview.Text = tagtext;
                    try
                    {
                        showBitmap(card.path);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }


                textview.Click += async delegate
                {
                    textview.Text = "現在解析中です。しばらくお待ちください。";

                    string[] resulttags = new string[0];
                    string inputfilename;
                    inputfilename = card.path;
                    try
                    {
                        resulttags = await CV2Tag.getTag(inputfilename);
                        textview.Text = "";
                        foreach (string s in resulttags)
                        {
                            textview.Text += s;
                            textview.Text += ", ";
                        }
                    }
                    catch (WebException ex)
                    {
                        // 通信エラー
                        Log.WriteLine(LogPriority.Info, ex.Source, ex.Message);
                        textview.Text = "通信エラーのため取得できませんでした。";
                    }
                    // ひとまずDBに書き込み
                    _db.SaveImgTagInfo(card.imageId, resulttags);
                };

                button.Click += delegate
                {
                    //Intent intent = new Intent(Intent.ActionMediaShared, uri);

                    // StartActivity(intent);
                    // http://kittoworks.hateblo.jp/entry/2015/07/02/171103
                    Java.IO.File file = new Java.IO.File(card.path);
                    Intent shareIntent = new Intent();
                    // Intent.ACTION_VIEWを指定すると表示、
                    // Intent.ACTION_SHAREを指定すると共有、
                    // Intent.ACTION_EDITを指定すると編集を行うアプリを呼び出せる。
                    shareIntent.SetAction(Intent.ActionSend);
                    shareIntent.SetDataAndType(Android.Net.Uri.FromFile(file), "image/jpeg");
                    shareIntent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(file));
                    StartActivity(shareIntent);
                };
            }
        }

        /// <summary>単一画面を表示</summary>
        /// <param name="imgpath"></param>
        async void showBitmap(string imgpath)
        {
            //Bitmap bmp = MediaStore.Images.Media.GetBitmap(cr, (Android.Net.Uri)card.path);
            ///imageView.SetImageBitmap(bmp);
            using (Bitmap bmp = await BitmapFactory.DecodeFileAsync(imgpath))
            {
                ImageButton button = FindViewById<ImageButton>(Resource.Id.imageButton1);
                button.SetImageBitmap(bmp);
                //button.SetImageURI((Android.Net.Uri)imgpath);
            }
        }
    }
}