using System;
using SQLite.Net.Attributes;

namespace XamarinCloudVisionApp
{
    public class ImageItem : System.Attribute
    {
        [PrimaryKey, NotNull]
        public long ImageId { get; set; }
        [Unique, NotNull]
        public string Path { get; set; }
        public DateTime DateModified { get; set; } //編集日時
        
        // 自前でコンストラクタを書くとSQLiteでエラーになる。
    }
}
