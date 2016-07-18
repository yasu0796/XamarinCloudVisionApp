using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Provider;

// https://developer.xamarin.com/recipes/android/layout/grid_view/create_a_grid_view/
// を参考にしてる
namespace XamarinCloudVisionApp.Droid
{
    public class ImageAdapter : BaseAdapter
    {
        Context context;
        ContentResolver cr;
        private List<ImageItem> items;

        public ImageAdapter(Context c, ContentResolver cr)
        {
            items = new List<ImageItem>();
            this.cr = cr;
            context = c;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public void AddImageIds(List<ImageItem> imageItems)
        {
            items = imageItems;
        }

        // create a new ImageView for each item referenced by the Adapter
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if (convertView == null)
            {  // if it's not recycled, initialize some attributes
                imageView = new ImageView(context);
                imageView.LayoutParameters = new GridView.LayoutParams(230, 230);
                //imageView.LayoutParameters = new GridView.LayoutParams(190, 190);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                imageView.SetPadding(4, 4, 4, 4);
            }
            else
            {
                imageView = (ImageView)convertView;
            }

            Bitmap bmp = MediaStore.Images.Thumbnails.GetThumbnail(cr, items[position].ImageId, ThumbnailKind.MicroKind, null);
            imageView.SetImageBitmap(bmp);
            return imageView;
        }
        
        
    }
}