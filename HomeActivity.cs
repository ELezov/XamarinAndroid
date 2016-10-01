using Android.App;
using Android.OS;
using Android.Widget;

namespace MyFirstProject
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
		}
	}
}

