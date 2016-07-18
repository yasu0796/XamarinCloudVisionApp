using System;
using SQLite.Net.Attributes;

namespace XamarinCloudVisionApp
{
    public class TagItem
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public long TagId { get; set; }
        //[Unique, NotNull]
        [Unique]
        public string TagPath { get; set; }
        [NotNull]
        public string TagStr { get; set; }
    }
}
