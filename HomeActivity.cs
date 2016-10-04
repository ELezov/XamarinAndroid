using Android.App;
using Android.OS;
using Android.Views;
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

