using System;
using System.Collections.Generic;
using SQLite.Net;
using System.Text.RegularExpressions;
// SQLiteDroid 使用中につき、移植可能に移植不可

// http://furuya02.hatenablog.com/entry/2015/04/27/171843
// https://components.xamarin.com/gettingstarted/sqlite-net

namespace XamarinCloudVisionApp.Droid
{
    public class ImageRepository
    {
        #region 変数
        static readonly object Locker = new object();
        readonly SQLiteConnection _db;
        SQLiteDroid connection;
        #endregion

        #region コンストラクタ
        public ImageRepository()
        {
            connection = new SQLiteDroid();
            _db = connection.GetConnection();//データベース接続
            _db.CreateTable<TagItem>();//テーブル作成
            _db.CreateTable<ImageTag>();//テーブル作成
            _db.CreateTable<ImageItem>();//テーブル作成
        }
        #endregion

        #region ImgaeItem (写真の管理情報)
        /// <summary>一覧(新しい順に)</summary>
        /// <param name="queryTags">絞り込む条件となるタグ・現時点で一つ対応</param>
        /// <returns></returns>
        public IEnumerable<ImageItem> GetItems(TagItem[] queryTags = null)
        {
            if (queryTags == null)
            {
                lock (Locker)
                {
                    var result = _db.Table<ImageItem>().OrderByDescending(m => m.ImageId);
                    return result;
                }
            }
            else
            {
                // クエリを指定
                // 
                //
                List<ImageItem> result;
                lock (Locker)
                {
                    result = _db.Query<ImageItem>("select i.ImageId, i.Path, DateModified from ImageItem as i INNER JOIN ImageTag as t on i.ImageId = t.ImageId where t.TagId = " + queryTags[0].TagId);
                }
                return result.ToArray();
            }
        }
        
        // 特定のID
        public IEnumerable<ImageItem> GetItem(long id)
        {
            lock (Locker)
            {
                return _db.Table<ImageItem>().Where(m => m.ImageId == id);
            }
        }

        // 写真のDBの個数と最終更新日時を返す（同一の場合は情報を更新しない）
        public int GetImageItemCount()
        {
            lock (Locker)
            {
                // データ数
                int datacount = _db.Table<ImageItem>().Count();
                return datacount;
            }
        }

        //更新・追加
        public long SaveItem(ImageItem item)
        {
            lock (Locker)
            {
                return _db.Insert(item);//追加
            }
        }

        /// <summary>画像が削除された場合など、DBから特定のIDの画像情報を削除します。</summary>
        /// <param name="imageId">画像ID</param>
        public void DeleteItem(long imageId)
        {
            // ImageItemの削除
            // ImageTagの削除
        }
        #endregion

        #region ImageTag（タグIDと写真の関連付け）
        public void SaveImgTagInfo(long imageid, string[] tags)
        {
            ImageTag imgtag = new ImageTag
            {
                ImageId = imageid
            };
            long tagid;
            // タグIDを取得
            foreach (string tag in tags)
            {
                tagid = getandWriteTagId(tag);
                imgtag.TagId = tagid;
                _db.Insert(imgtag);
            }
            //long id
            //long id = getandWriteTagId("GMOテスト" + DateTime.Now.Second);
            //long id2 = getandWriteTagId("GMOテスト" + DateTime.Now.Minute);
            // 書き込み
        }

        /// <summary>画像のタグ情報</summary>
        /// <param name="imageid"></param>
        /// <returns></returns>
        public TagItem[] GetImgTagInfo(long imageid)
        {
            List<TagItem> result = _db.Query<TagItem>("SELECT t.TagId, t.TagPath, t.TagStr FROM ImageTag as i INNER JOIN TagItem AS t ON i.TagId = t.TagId WHERE ImageId = " + imageid　+ " ORDER BY TagStr DESC ;");
            return result.ToArray();
        }


        /// <summary>頻繁に使われているタグを出力します。一覧表示用のため、日本語のみとします。</summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public TagItem[] GetPopularTags(bool onlyJp = true,int limit = 100) {
            List<TagItem> result = _db.Query<TagItem>("SELECT t.TagId, t.TagPath, t.TagStr FROM ImageTag as i INNER JOIN TagItem AS t ON i.TagId = t.TagId GROUP BY i.TagId ORDER BY COUNT(*) DESC, TagStr DESC; ");

            if (onlyJp)
            {
                List<TagItem> temp = new List<TagItem>();
                // 英語の項目を削除
                Regex reg = new Regex("^[0-9a-zA-Z ]+$");
                foreach (TagItem s in result)
                {
                    //if (!s.TagStr.StartsWith("*"))
                    if(!reg.IsMatch(s.TagStr))
                    {
                        temp.Add(s);
                    }
                }
                result = temp;
            }
            return result.ToArray();
        }
        #endregion

        #region TagItem（タグ名）
        /// <summary>タグIDを取得・ない場合は作成する。</summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public long getandWriteTagId(string tag)
        {
            //
            long result = 0;
            var taginfo = _db.Table<TagItem>().Where(m => m.TagStr == tag);
            foreach (TagItem tagitem in taginfo)
            {
                result = tagitem.TagId;
            }
            if (result == 0)
            {
                // タグ情報がない場合は作成
                TagItem tagitem = new TagItem
                {
                    TagStr = tag
                };
                // 書き込むとIDが付与される
                _db.Insert(tagitem);
                result = tagitem.TagId;
            }
            return result;
        }
        #endregion

        #region トランザクション（SQLiteConnectionそのまま）　https://github.com/praeclarum/sqlite-net/wiki/Transactions
        /// <summary>トランザクションを開始します。</summary>
        public void BeginTransaction()
        {
            _db.BeginTransaction();
        }

        /// <summary>結果にコミットする</summary>
        public void Commit()
        {
            _db.Commit();
        }
        #endregion
    }
}
