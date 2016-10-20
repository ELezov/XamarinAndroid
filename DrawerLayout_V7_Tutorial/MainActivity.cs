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
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System.Xml;
using System.Threading;
using Android.Net;
using System.Xml.Linq;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;

namespace MyFirstProject
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity :  AppCompatActivity
	{
		private DrawerLayout mDrawerLayout;
		private ListView mLeftDrawer;
		private HomeFragment homeFragment;
		private ParseFragment parseFragment;
		public AboutMeFragment aboutMeFragment;
		private SupportFragment mCurrentFragment = new SupportFragment();
		private MyActionBarDrawerToggle ActionDrawerToggle;

		ISharedPreferences sPref;
		String SAVED_TEXT = "saved_text";

		DateTime TimeCreateCache;
		string xmlUrl = "http://andrei-chernyavskii.name/steam/?t=17acff5c5dad004d67a342f5bbfaa3ef";
		XmlTextReader reader;
		DataBase db;
		bool flag;
		bool Internet;
		String parse;
		String home;
		String exit;
		String about;
		ProgressDialog progress;
		TimeSpan ts;
		int check = 0;


		private ArrayAdapter mLeftAdapter;
		
		private List<string> mLeftDataSet;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			Initialize();

			if (bundle != null)
			{
				flag = bundle.GetBoolean("flag");
				string Data = bundle.GetString("TimeCreateCache");
				TimeCreateCache = DateTime.Parse(Data);
				check = bundle.GetInt("check");
			}

			if (flag == false)
			{
				sPref = GetPreferences(FileCreationMode.Private);
				string savedText = sPref.GetString(SAVED_TEXT, "");
				if ((savedText != "")&&(savedText!=null))
				{
					TimeCreateCache = DateTime.Parse(savedText);
					flag = true;
				}
			}

			//Check fragment after rotation
			if (check == 2)
				ShowFragment(parseFragment);
			else
				if (check == 3)
					ShowFragment(aboutMeFragment);
				else 
					ShowFragment(homeFragment);

		}

		internal class MyActionBarDrawerToggle : ActionBarDrawerToggle
		{
			AppCompatActivity owner;

			public MyActionBarDrawerToggle(AppCompatActivity activity, DrawerLayout layout, int openRes, int closeRes)
				: base(activity, layout, openRes, closeRes)
			{
				owner = activity;
			}

			public override void OnDrawerClosed(View drawerView)
			{
				owner.InvalidateOptionsMenu();
			}

			public override void OnDrawerOpened(View drawerView)
			{
				owner.InvalidateOptionsMenu();
			}
		}


		void MenuListView_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
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
				check = 3;
				ShowFragment(aboutMeFragment);
				break;
			case 3:
				Exit();
				break;
			}

			mDrawerLayout.CloseDrawers();
			ActionDrawerToggle.SyncState();
		}

		private void ShowFragment(SupportFragment fragment)
		{
			if (fragment.IsVisible)
			{
				return;
			}
			var trans = SupportFragmentManager.BeginTransaction();
				trans.Replace(Resource.Id.main, fragment);
				trans.AddToBackStack(null);
				trans.Commit();
				mCurrentFragment = fragment;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{		
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					ActionDrawerToggle.OnOptionsItemSelected(item);
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
			ActionDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			ActionDrawerToggle.OnConfigurationChanged(newConfig);
		}


		public void Initialize()
		{
			//Init DataBase
			db = new DataBase();
			db.createDataBase();
			string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

			//Init item for menu
			parse = GetString(Resource.String.parseFr);
			home = GetString(Resource.String.homeFr);
			exit = GetString(Resource.String.exit);
			about = GetString(Resource.String.aboutMe);

			//init Drawer
			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
			mLeftDrawer.Tag = 0;

			mLeftDataSet = new List<string>();
			mLeftDataSet.Add(home);
			mLeftDataSet.Add(parse);
			mLeftDataSet.Add(about);
			mLeftDataSet.Add(exit);

			mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;
			mLeftDrawer.ItemClick += MenuListView_ItemClick;

			//Init Fragment
			homeFragment = new HomeFragment();
			parseFragment = new ParseFragment();
			aboutMeFragment = new AboutMeFragment();

			//Init ActionBar
			SupportActionBar.SetTitle(Resource.String.app_name);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetHomeButtonEnabled(true);

			//Init DrawerToggle
			ActionDrawerToggle = new MyActionBarDrawerToggle(
				this,
				mDrawerLayout,
				Resource.String.openDrawer,
				Resource.String.closeDrawer
			);

			mDrawerLayout.AddDrawerListener(ActionDrawerToggle);
			ActionDrawerToggle.SyncState();
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
				int differenceInMinutes = (ts.Days*24 + ts.Hours)*60 + ts.Minutes;
				if (differenceInMinutes>= 10)
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
			var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

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
			if (SupportFragmentManager.BackStackEntryCount > 1)
				SupportFragmentManager.PopBackStack();
			else
				Exit();
		}

		protected override void OnDestroy()
		{
			sPref = GetPreferences(FileCreationMode.Private);
			var ed = sPref.Edit();
			ed.PutString(SAVED_TEXT, Convert.ToString(TimeCreateCache));
			ed.Commit();
			base.OnDestroy();
		}


	}
}


