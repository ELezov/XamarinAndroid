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

		Android.Support.V4.App.FragmentTransaction tx;

		
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

				//homeFragment = SupportFragmentManager.GetFragment(bundle,"home");
				//parseFragment= SupportFragmentManager.GetFragment(bundle, "parse");
			}

			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
			homeFragment = new HomeFragment();

			parseFragment = new ParseFragment();
			mStackFragments = new Stack<SupportFragment>();
		

			mLeftDrawer.Tag = 0;
			

			SetSupportActionBar(mToolbar);
			SupportActionBar.SetTitle(Resource.String.app_name);
		
			mLeftDataSet = new List<string>();
			mLeftDataSet.Add ("Home");
			mLeftDataSet.Add ("Parse");
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

			tx = SupportFragmentManager.BeginTransaction();

			tx.Add(Resource.Id.main, homeFragment,"home");
			tx.Add(Resource.Id.main, parseFragment,"parse");
			tx.Hide(parseFragment); 

			mCurrentFragment = homeFragment;
			tx.Commit();
		}

		void MenuListView_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			Android.Support.V4.App.Fragment fragment = null;

			switch (e.Id)
			{
			case 0:
				ShowFragment(homeFragment);
				break;
			case 1:
				UpdCache();
				check = 2;
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
			else
				if ((fragment == null) || (!fragment.IsVisible))
			{
				var trans = SupportFragmentManager.BeginTransaction();


				fragment.View.BringToFront();
				mCurrentFragment.View.BringToFront();

				trans.Hide(mCurrentFragment);
				trans.Show(fragment);
				trans.AddToBackStack(null);
				mStackFragments.Push(mCurrentFragment);
				trans.Commit();

				mCurrentFragment = fragment;
			}

		}
		public override bool OnOptionsItemSelected (IMenuItem item)
		{		
			switch (item.ItemId)
			{

			case Android.Resource.Id.Home:
				//The hamburger icon was clicked which means the drawer toggle will handle the event
				
				
				mDrawerToggle.OnOptionsItemSelected(item);
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
			//SupportFragmentManager.PutFragment(outState, "home", homeFragment);
			//SupportFragmentManager.PutFragment(outState, "parse", parseFragment);

			 
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
				Toast.MakeText(this, "Отсутствует подключение к интернету. Выполните подключение для обновления информации",
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
			progress = ProgressDialog.Show(this, "Loading...", "Please Wait....", true);

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

		public Boolean checkInternet()
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
			return isOnline;
		}
	}
}


