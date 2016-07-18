using System;
using SQLite.Net.Attributes;

namespace XamarinCloudVisionApp
{
    public class ImageTag
    {
        [Indexed, NotNull]
        public long TagId { get; set; }
        [Indexed, NotNull]
        public long ImageId { get; set; }
    }
}
