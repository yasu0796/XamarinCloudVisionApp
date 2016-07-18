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

namespace XamarinCloudVisionApp.Droid
{
    public class TagAdapter : BaseAdapter
    {
        Context context;
        ContentResolver cr;
        private List<TagItem> items;

        public TagAdapter(Context c, ContentResolver cr)
        {
            items = new List<TagItem>();
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

        public void AddTagIds(List<TagItem> tagItems)
        {
            items = tagItems;
        }

        public void AddTagIds(TagItem[] tagItems)
        {
            items = new List<TagItem>(tagItems);
        }

        // create a new ImageView for each item referenced by the Adapter
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //Button tagButton;
            TextView tagButton;

            if (convertView == null)
            {  // if it's not recycled, initialize some attributes
                //tagButton = new Button(context);
                tagButton = new TextView(context);
                //tagButton.LayoutParameters = new GridView.LayoutParams(190, 190);
                tagButton.LayoutParameters = new GridView.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                tagButton.SetTextColor(Color.Rgb(34, 191, 185));
                //tagButton.SetPadding(5, 2, 5, 2);// ltrb
                tagButton.SetPadding(10, 4, 10, 4);// ltrb
                tagButton.SetBackgroundResource(Resource.Drawable.flame_style);

                //imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            }
            else
            {
                //tagButton = (Button)convertView;
                tagButton = (TextView)convertView;
            }

            // Bitmap bmp = MediaStore.Images.Thumbnails.GetThumbnail(cr, items[position].ImageId, ThumbnailKind.MicroKind, null);
            //imageView.SetImageBitmap(bmp);
            tagButton.Text = items[position].TagStr;
            return tagButton;
        }
        
        
    }
}