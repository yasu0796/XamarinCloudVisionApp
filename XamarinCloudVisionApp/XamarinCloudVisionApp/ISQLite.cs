using System;
using SQLite.Net;

// http://furuya02.hatenablog.com/entry/2015/04/27/171843
namespace XamarinCloudVisionApp
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
