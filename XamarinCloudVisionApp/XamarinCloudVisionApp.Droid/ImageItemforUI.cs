using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using Java.Interop;

namespace XamarinCloudVisionApp.Droid
{
    // ImageItem はDBで使用するための最低限の記述。こちらはUIのページの遷移に必要な情報
    public class ImageItemforUI : Java.Lang.Object, IParcelable
    {
        public long imageId { get; set; }
        public string path { get; set; }
        public List<string> tags { get; set; }
        public ImageItemforUI()
        {
            tags = new List<string>();
        }
        public ImageItemforUI(ImageItem imageitem)
        {
            imageId = imageitem.ImageId;
            path = imageitem.Path;
            // 本来ならここでDB読んでタグを収集
            tags = new List<string>();
            tags.Add("テストタグ1");
            tags.Add("テストタグ2");
            tags.Add("テストタグ3");
        }
        public ImageItemforUI(long ImageId, string Path, List<string> Tags)
        {
            imageId = ImageId;
            path = Path;
            tags = Tags;
            tags = new List<string>();
            tags.Add("テストタグ1");
            tags.Add("テストタグ2");
            tags.Add("テストタグ3");
        }

        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            dest.WriteLong(this.imageId);
            dest.WriteString(this.path);
            dest.WriteStringList(tags);
        }

        // public static final Parcelable.Creator の代わり
        [ExportField("CREATOR")]
        public static IParcelableCreator GetCreator()
        {
            return new ImageItemParcelableCreator();
        }

        public int DescribeContents()
        {
            return 0;
        }


        class ImageItemParcelableCreator : Java.Lang.Object, IParcelableCreator
        {
            Java.Lang.Object IParcelableCreator.CreateFromParcel(Parcel source)
            {
                var imageId = source.ReadLong();
                var path = source.ReadString();
                List<string> tags = new List<string>();
                source.ReadStringList(tags);

                return new ImageItemforUI(imageId, path, tags);
            }

            Java.Lang.Object[] IParcelableCreator.NewArray(int size)
            {
                return new Java.Lang.Object[size];
            }
        }
    }
}
