using System;
using System.IO;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;

// http://furuya02.hatenablog.com/entry/2015/04/27/171843

namespace XamarinCloudVisionApp.Droid
{
    public class SQLiteDroid
    {
        public SQLiteConnection GetConnection()
        {
            // 保存フォルダ 以下Let's Note
            // Android 5以下
            // /data/data/XamarinBlankApp.Droid/files/CaterinaSQLite.db3
            // Let's Emulator 5
            // 
            // Android 6 エミュレータ：/data/user/0/XamarinBlankApp.Droid/files/CaterinaSQLite.db3
            const string sqliteFilename = "CaterinaSQLite.db3"; //データベース名
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documentsフォルダ
            var path = Path.Combine(documentsPath, sqliteFilename); // DBファイルのパス
            var plat = new SQLitePlatformAndroid();
            var conn = new SQLiteConnection(plat, path);
            return conn;
        }
    }
}