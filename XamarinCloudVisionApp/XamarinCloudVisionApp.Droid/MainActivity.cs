using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.Provider;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Database;
using CloudVisionSharedProject;
using System.Threading;

namespace XamarinCloudVisionApp.Droid
{
    [Activity(Label = "XamarinCloudVisionApp.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        readonly ImageRepository _db = new ImageRepository();
        DateTime lastUpdated;

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // タイトルバーを消す
            RequestWindowFeature(WindowFeatures.NoTitle);

            try
            {
                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.Main);
            }
            catch (Exception ex)
            {
                Console.WriteLine("初期化に失敗しました。"+ex.Message);
            }

            // Get our button from the layout resource,
            // and attach an event to it

            //暫定的にDB側のほうで書き込む（あとで実装は検討）
            await writeImageListToDB();
            viewTagGrid();
            viewImgGrid();

            // タグをクリックされた時の動作
            GridView tagGridView = FindViewById<GridView>(Resource.Id.tagGridView);
            TagItem[] test = _db.GetPopularTags();
            tagGridView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                int position = args.Position;
                string tagStr = test[position].TagStr;
                viewImgGrid(tagStr);
            };

            // タグの上ボタンをクリックされた時の動作（暫定ですべての画像）ListButton
            Button listButton = FindViewById<Button>(Resource.Id.ListButton);
            listButton.Click += delegate 
            {
                viewImgGrid(null);
            };
        }

        private void viewTagGrid()
        {
            ContentResolver cr = ContentResolver;
            //var tagEditBox = FindViewById<EditText>(Resource.Id.tagEditBox);
            //string[] projection = null;
            /*{
                MediaStore.Images.ImageColumns.Data,
                MediaStore.Images.ImageColumns.DisplayName
            };*/

            GridView tagGridView = FindViewById<GridView>(Resource.Id.tagGridView);
            var tdp = new TagAdapter(this, cr);
            TagItem[] test = _db.GetPopularTags();
            tdp.AddTagIds(test);
            tagGridView.Adapter = tdp;
        }

        private void viewImgGrid(string inputTag = null)
        {
            ContentResolver cr = ContentResolver;
            List<ImageItem> dbImageItems = new List<ImageItem>();
            GridView gridview = null;
            try
            {
                gridview = FindViewById<GridView>(Resource.Id.gridview);
            }
            catch
            {
                Console.WriteLine("GridViewの取得に失敗しました");
            }
            var adp = new ImageAdapter(this, cr);

            // DBのテスト

            /*ImageItem tempItem = new ImageItem()
            {
                ImageId = DateTime.Now.Millisecond,
                Path = "temp" + DateTime.Now.Millisecond.ToString(),
                DateModified = DateTime.Now
            };*/
            //_db.SaveItem(tempItem); 
            //string st = "";
            try
            {
                TagItem[] temp;
                if (inputTag != null)
                {
                    temp = new TagItem[1];
                    temp[0] = new TagItem { TagId = _db.getandWriteTagId(inputTag) };
                }
                else {
                    temp = null;
                }
                var ItemsSource = _db.GetItems(temp);
                //var ItemsSource = _db.GetItemList();
                foreach (ImageItem i in ItemsSource)
                {
                    dbImageItems.Add(i);
                    //st += i.Path + " ";
                }
                //tagEditBox.Text = st;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            adp.AddImageIds(dbImageItems);
            gridview.Adapter = adp;

            DateTime lastUpdatedlocal = DateTime.Now;
            lastUpdated = lastUpdatedlocal;
            gridview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                if (lastUpdated == lastUpdatedlocal)
                {
                    using (var intent = new Intent(this, typeof(SingleImageActivity)))
                    {
                        int position = args.Position;
                        ImageItemforUI intentData = new ImageItemforUI(dbImageItems[position]);
                        //Toast.MakeText(this, dbImageItems[position].Path, ToastLength.Short).Show();

                        intent.PutExtra("data", intentData);
                        StartActivity(intent);
                    }
                }else
                {
                    Console.WriteLine("過去に作成されたイベントが呼ばれました。");
                }
                //StartActivityForResult(intent,0);
            };
        }

        /// <summary>ひとまず初回起動時のみ、実際に保存されているファイルとDB上のファイルと同期する</summary>
        /// <returns></returns>
        private async Task writeImageListToDB()
        {
            //Toast.MakeText(this, "端末に保存されている画像を取得しています", ToastLength.Short).Show();
            // 新しい順に取得
            string sortorder = MediaStore.Images.ImageColumns.DateModified + " DESC";
            ICursor c = ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, null, null, null, sortorder);
            c.MoveToFirst();
            ContentResolver cr = ContentResolver;

            // 端末側の画像を格納するリスト
            //List<ImageItem> localImageItems = new List<ImageItem>();
            Dictionary<long, ImageItem> localImageItems = new Dictionary<long, ImageItem>();
            long id;
            string path;
            // いったんリストに溜めておく
            for (int i = 0; i < c.Count; i++)
            {
                id = c.GetLong(c.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.Id));
                path = c.GetString(c.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.Data)); // ファイルパス
                //var s = c.GetDouble(c.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.DateModified));// 最終更新日
                long a = c.GetLong(c.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.DateModified));// 最終更新日
                var s = DateTime.FromFileTime(a);

                ImageItem item = new ImageItem()
                {
                    Path = path,
                    ImageId = id,
                    DateModified = s
                };
                localImageItems.Add(id, item);
                c.MoveToNext();
            }
            bool isDbFound = false;
            IEnumerable<ImageItem> result;

            //Toast.MakeText(this, "端末に保存されている画像をDBに書き込んでいます", ToastLength.Short).Show();
            // DBに書き込み
            #region トランザクション
            _db.BeginTransaction();
            foreach (KeyValuePair<long, ImageItem> i in localImageItems)
            {
                isDbFound = false;
                // あれば書き込まない
                result = _db.GetItem(i.Value.ImageId);

                foreach (ImageItem dbresult in result)
                {
                    // 型の関係でforeachをしているが、一回でよいループ
                    isDbFound = true;
                    // 本来なら更新日時で判断すべき
                    break;
                }
                if(!isDbFound)
                    _db.SaveItem(i.Value);
            }
            _db.Commit();
            #endregion

            //Toast.MakeText(this, "DB上に不要な情報があれば削除します。", ToastLength.Short).Show();
            // 全ての情報を集める
            result = _db.GetItems();
            foreach(ImageItem it in result)
            {
                if (!localImageItems.ContainsKey(it.ImageId))
                {
                    // 存在しない画像は削除（予定）
                }
            }



            //await viewGrid();
        }
    }
}


