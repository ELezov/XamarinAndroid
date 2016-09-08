
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

namespace AndroidSQLite
{
	[Activity(Label = "HomeActivity")]
	public class HomeActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Home);

			var btnBack = FindViewById<ImageButton>(Resource.Id.imageButton1);

			btnBack.Click += delegate
			{
				OnBackPressed();
			};
			// Create your application here
		}
	}
}

