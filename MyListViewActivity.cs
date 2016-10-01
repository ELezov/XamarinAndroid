using System.Collections.Generic;
using Android.App;
using Android.OS;
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

			var btnBack = FindViewById<ImageButton>(Resource.Id.imageButton2);

			btnBack.Click += delegate
			{
				OnBackPressed();
			};

			lstData = FindViewById<ListView>(Resource.Id.listView);
			db = new DataBase();
			lstSource = db.selectTableEntry();
			var adapter = new ListViewAdapter(this, lstSource);
			lstData.Adapter = adapter;


		}
	}
}

