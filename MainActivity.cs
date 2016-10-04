using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Util;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using Android.Net;
using Android.Content;
using Android.Views;

namespace MyFirstProject
{
	[Activity(Label = "MyFirstProject", MainLauncher = true)]
	public class MainActivity : Activity
	{
		DateTime TimeCreateCache;
		string xmlUrl = "http://andrei-chernyavskii.name/steam/?t=17acff5c5dad004d67a342f5bbfaa3ef";
		XmlTextReader reader;
		DataBase db;
		bool flag;
		bool Internet;
		Button btnParse;
		Button btnHome;
		Button btnExit;
		ProgressDialog progress;
		TimeSpan ts;
		TextView txtV;

		protected override void OnCreate(Bundle bundle)
		{

			base.OnCreate(bundle);

			ActionBar.Hide();

			SetContentView(Resource.Layout.Main);
			txtV = FindViewById<TextView>(Resource.Id.txtV);
			initialize();


			if (bundle != null)
			{
				flag = bundle.GetBoolean("flag");
				string Data = bundle.GetString("TimeCreateCache");
				TimeCreateCache = DateTime.Parse(Data);
			}
			txtV.Text = Convert.ToString(TimeCreateCache);
			btnParse.Click += delegate
			{

				UpdCache();
			};

			btnHome.Click += delegate
			{
				StartActivity(typeof(HomeActivity));
			};

			btnExit.Click += delegate
			{
				Finish();
			};

		}


		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutBoolean("flag", flag);
			outState.PutString("TimeCreateCache", Convert.ToString(TimeCreateCache));

			base.OnSaveInstanceState(outState);
		}

		public void initialize()
		{
			//Create DataBase
			db = new DataBase();
			db.createDataBase();
			string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			Log.Info("DB_PATH", folder);

			btnParse = FindViewById<Button>(Resource.Id.btnParse);
			btnHome = FindViewById<Button>(Resource.Id.btnHome);
			btnExit = FindViewById<Button>(Resource.Id.btnExit);
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
				StartActivity(typeof(MyListViewActivity));
			}
			else 
				new System.Threading.Thread(new ThreadStart(() =>
					  {
						  //System.Threading.Thread.Sleep(3000);
						  MakeCacheFromXML();
						  this.RunOnUiThread(() =>
							  {
								  txtV.Text = Convert.ToString(TimeCreateCache);
								  Internet = false;
								  progress.Dismiss();
								  StartActivity(typeof(MyListViewActivity));
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
						txtV.Text = Convert.ToString(TimeCreateCache);
						progress.Dismiss();
						StartActivity(typeof(MyListViewActivity));
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