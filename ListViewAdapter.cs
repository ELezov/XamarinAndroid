using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace MyFirstProject
{
    public class ViewHolder : Java.Lang.Object
    {
        public TextView txtName { get; set; }
        public TextView txtAge { get; set; }
        public TextView txtEmail { get; set; }
    }
    public class ListViewAdapter:BaseAdapter
    {
        private Activity activity;
        private List<Entry> lstEntry;
        public ListViewAdapter(Activity activity, List<Entry> lstEntry)
        {
            this.activity = activity;
            this.lstEntry = lstEntry;
        }

        public override int Count
        {
            get
            {
                return lstEntry.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return lstEntry[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.list_view_dataTemplate, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.textView1);
            var txtAge = view.FindViewById<TextView>(Resource.Id.textView2);
            var txtEmail = view.FindViewById<TextView>(Resource.Id.textView3);

			txtName.Text = "Appid: "+lstEntry[position].Appid;
            txtAge.Text =  "Place: "+lstEntry[position].Place;
			txtEmail.Text ="Title: "+lstEntry[position].Title;

            return view;
        }
    }
}