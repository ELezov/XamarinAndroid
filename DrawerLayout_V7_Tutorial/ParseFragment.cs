using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;

namespace MyFirstProject
{
	
	class ParseFragment: Android.Support.V4.App.Fragment
	{
		ListView lstData;
		DataBase db;
		List<Entry> lstSource = new List<Entry>();

		public ParseFragment()
		{

		}
		public static Android.Support.V4.App.Fragment newInstance(Context context)
		{
			ParseFragment busrouteFragment = new ParseFragment();
			return busrouteFragment;
		}
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.RetainInstance = true;

		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.parsefragment, null);

			lstData = (ListView)root.FindViewById(Resource.Id.listView);
			db = new DataBase();
			lstSource = db.selectTableEntry();
			var adapter = new ListViewAdapter(this.Activity, lstSource);
			lstData.Adapter = adapter;
			return root;

		}
	}

}


