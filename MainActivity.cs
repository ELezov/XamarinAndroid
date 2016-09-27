using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using AndroidSQLite.Resources.Model;
using AndroidSQLite.Resources.DataHelper;
using AndroidSQLite.Resources;
using Android.Util;
using System.Xml;
using Android.Content;
using System.Xml.Linq;
using System.Threading;
using Java.Lang;

namespace AndroidSQLite
{
	[Activity(Label = "AndroidSQLite", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		DateTime TimeCreateCache;
		string xmlUrl = "http://andrei-chernyavskii.name/steam/?t=17acff5c5dad004d67a342f5bbfaa3ef";
		XmlTextReader reader;
		DataBase db;
		bool flag;
		Button btnParse;
		Button btnHome;
		Button btnExit;
		ProgressDialog progress;
		TimeSpan ts;

		protected override void OnCreate(Bundle bundle)
		{

			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

			initialize();

			if (bundle != null)
			{
				flag = bundle.GetBoolean("flag");
				string Data = bundle.GetString("TimeCreateCache");
				TimeCreateCache = DateTime.Parse(Data);
			}

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
			new System.Threading.Thread(new ThreadStart(() =>
					  {
						  //System.Threading.Thread.Sleep(3000);
						  MakeCacheFromXML();
						  this.RunOnUiThread(() =>
							  {
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
				int differenceInHours = ts.Hours;
				if (differenceInHours >= 10)
				{
					//запоминание времени записи таблицы
					TimeCreateCache = DateTime.Now;
					CacheThread();
				}
				else
				{
					progress.Dismiss();
					StartActivity(typeof(MyListViewActivity));
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

	}
}