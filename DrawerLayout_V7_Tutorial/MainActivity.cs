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
using System.Xml;
using System.Threading;
using Android.Net;
using System.Xml.Linq;

namespace MyFirstProject
{
	[Activity (Label = "DrawerLayout_V7_Tutorial", MainLauncher = true, Icon = "@drawable/icon", Theme="@style/MyTheme")]
	public class MainActivity : ActionBarActivity
	{
		private SupportToolbar mToolbar;
		private MyActionBarDrawerToggle mDrawerToggle;
		private DrawerLayout mDrawerLayout;
		private ListView mLeftDrawer;
		private HomeFragment homeFragment;
		private ParseFragment parseFragment;
		private SupportFragment mCurrentFragment = new SupportFragment();
		private Stack<SupportFragment> mStackFragments;

		DateTime TimeCreateCache;
		string xmlUrl = "http://andrei-chernyavskii.name/steam/?t=17acff5c5dad004d67a342f5bbfaa3ef";
		XmlTextReader reader;
		DataBase db;
		bool flag;
		bool Internet;
		ProgressDialog progress;
		TimeSpan ts;
		int check = 0;

		private Toast backtoast;

		private ArrayAdapter mLeftAdapter;
		
		private List<string> mLeftDataSet;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			initialize();

			if (bundle != null)
			{
				flag = bundle.GetBoolean("flag");
				string Data = bundle.GetString("TimeCreateCache");
				TimeCreateCache = DateTime.Parse(Data);
				check = bundle.GetInt("check");
			}

			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
			homeFragment = new HomeFragment();
			parseFragment = new ParseFragment();

			mLeftDrawer.Tag = 0;

			SetSupportActionBar(mToolbar);
			SupportActionBar.SetTitle(Resource.String.app_name);

			String parse = GetString(Resource.String.parseFr);
			String home = GetString(Resource.String.homeFr);
			String exit = GetString(Resource.String.exit);

			mLeftDataSet = new List<string>();
			mLeftDataSet.Add (home);
			mLeftDataSet.Add (parse);
			mLeftDataSet.Add (exit);
			mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;
			mLeftDrawer.ItemClick+= MenuListView_ItemClick;

			mDrawerToggle = new MyActionBarDrawerToggle(
				this,                           //Host Activity
				mDrawerLayout,//DrawerLayout
				Resource.String.openDrawer,		//Opened Message
				Resource.String.closeDrawer		//Closed Message
			);

			mDrawerLayout.SetDrawerListener(mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayShowTitleEnabled(true);
			mDrawerToggle.SyncState();

			//Check fragment after rotation
			if (check == 2)
				ShowFragment(parseFragment);
			else
				ShowFragment(homeFragment);

		}

		void MenuListView_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			//Android.Support.V4.App.Fragment fragment = null;
			switch (e.Id)
			{
			case 0:
				check = 1;
				ShowFragment(homeFragment);
				break;
			case 1:
				check = 2;
				UpdCache();
				break;
				
			case 2:
				Exit();
				break;
			}

			mDrawerLayout.CloseDrawers();
			mDrawerToggle.SyncState();

		}
		private void ShowFragment(SupportFragment fragment)
		{

			if (fragment.IsVisible)
			{
				return;
			}
			var trans = SupportFragmentManager.BeginTransaction();
				trans.Replace(Resource.Id.main, fragment);
				trans.AddToBackStack(null);;
				trans.Commit();
				mCurrentFragment = fragment;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{		
			switch (item.ItemId)
			{

			case Android.Resource.Id.Home:
				//The hamburger icon was clicked which means the drawer toggle will handle the event
				mDrawerToggle.OnOptionsItemSelected(item);
				return true;

				case Resource.Id.action_exit:
					Exit();
				return true;

				default:
				return base.OnOptionsItemSelected (item);
			}
		}
			
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.action_menu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutInt("check", check);
			outState.PutBoolean("flag", flag);
			outState.PutString("TimeCreateCache", Convert.ToString(TimeCreateCache));
			base.OnSaveInstanceState(outState);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			mDrawerToggle.OnConfigurationChanged(newConfig);
		}


		public void initialize()
		{
			//Create DataBase
			db = new DataBase();
			db.createDataBase();
			string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
		}

		private XDocument GetXml(string xmlUrl)
		{
			reader = new XmlTextReader(xmlUrl);
			XDocument doc = XDocument.Load(reader);
			return doc;
		}

		private void ParseXmlEntry(XDocument doc)
		{
			foreach (XElement element in doc.Root.Elements("entry"))
			{
				Entry entry = new Entry();
				entry.Place = int.Parse(element.Element("place").Value);
				entry.Appid = int.Parse(element.Element("appid").Value);
				entry.Title = element.Element("title").Value;
				db.insertIntoTableEntry(entry);
			}
		}

		public void MakeCacheFromXML()
		{
			db.deleteAllTableEntry();
			XDocument doc = GetXml(xmlUrl);
			ParseXmlEntry(doc);
		}

		public void CacheThread()
		{
			if (!checkInternet())
			{
				String withoutInternet = GetString(Resource.String.withoutInternet);
				Toast.MakeText(this, withoutInternet,
							   ToastLength.Long).Show();
				progress.Dismiss();
				Internet = true;
				ShowFragment(parseFragment);
			}
			else
				new System.Threading.Thread(new ThreadStart(() =>
					  {
						  MakeCacheFromXML();
						  this.RunOnUiThread(() =>
							  {
								  Internet = false;
								  progress.Dismiss();
								  ShowFragment(parseFragment);
							  });
					  })).Start();
		}

		private void UpdCache()
		{
			String loading = GetString(Resource.String.load);
			String wait = GetString(Resource.String.wait);



			progress = ProgressDialog.Show(this, loading, wait, true);

			if (flag == true)
			{
				ts = DateTime.Now - TimeCreateCache;
				int differenceInMinutes = ts.Minutes;
				if (differenceInMinutes >= 10)
				{
					//запоминание времени записи таблицы
					TimeCreateCache = DateTime.Now;
					CacheThread();
				}
				else
				{
					if (Internet == true)
						CacheThread();
					else
					{
						progress.Dismiss();
						ShowFragment(parseFragment);
					}
				}
			}
			else
			{
				flag = true;
				//запоминание времени записи таблицы
				TimeCreateCache = DateTime.Now;
				CacheThread();
			}
		}

		//Проверка доступа к сети интернет
		public Boolean checkInternet()
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
			return isOnline;
		}

		//Обработка выхода
		public void Exit()
		{
			// Build the dialog.
			var builder = new AlertDialog.Builder(this);

			String wantToExit=GetString(Resource.String.wantToExit);
			builder.SetTitle(wantToExit);

			String yes = GetString(Resource.String.yes);
			String no = GetString(Resource.String.no);
			// Create empty event handlers, we will override them manually instead of letting the builder handling the clicks.
			builder.SetPositiveButton(yes, (EventHandler<DialogClickEventArgs>)null);
			builder.SetNegativeButton(no, (EventHandler<DialogClickEventArgs>)null);
			var dialog = builder.Create();

			// Show the dialog. This is important to do before accessing the buttons.
			dialog.Show();

			// Get the buttons.
			var yesBtn = dialog.GetButton((int)DialogButtonType.Positive);
			var noBtn = dialog.GetButton((int)DialogButtonType.Negative);

			// Assign our handlers.
			yesBtn.Click += (sender, args) =>
			{
				// Don't dismiss dialog;
				Finish();
			};
			noBtn.Click += (sender, args) =>
			{
				dialog.Dismiss();
			};
				
		}


		public override void OnBackPressed()
		{
			//FragmentManager.
			//if (FragmentManager.BackStackEntryCount > 0)
			//	FragmentManager.PopBackStack();
			//else
			Exit();
		}


	}
}


