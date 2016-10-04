using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MyFirstProject
{
	[Activity(Label = "MyListViewActivity")]
	public class MyListViewActivity : Activity
	{
		ListView lstData;
		DataBase db;
		List<Entry> lstSource = new List<Entry>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.MyListView);

			lstData = FindViewById<ListView>(Resource.Id.listView);
			db = new DataBase();
			lstSource = db.selectTableEntry();
			var adapter = new ListViewAdapter(this, lstSource);
			lstData.Adapter = adapter;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.actionbar, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.back:
					//Add button clicked
					OnBackPressed();
					return true;
			}
			return base.OnOptionsItemSelected(item);
		}


	}
}

