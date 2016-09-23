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

		protected override void OnCreate(Bundle bundle)
		{
			
			base.OnCreate(bundle);
			if (bundle != null)
			{
				flag = bundle.GetBoolean("flag");
			}
			SetContentView(Resource.Layout.Main);

			TextView txtV = FindViewById<TextView>(Resource.Id.txtV);

			initialize();

			txtV.Text = Convert.ToString(flag);
			btnParse.Click += delegate 
			{
				progress = ProgressDialog.Show(this, "Loading...", "Please Wait....", true);
				new System.Threading.Thread(new ThreadStart(() =>
				{
					UpdCache();
					this.RunOnUiThread(() =>
					{
						progress.Dismiss();
						txtV.Text = Convert.ToString(flag);
						StartActivity(typeof(MyListViewActivity));
					});
				})).Start();

			};

			btnHome.Click += delegate
			{
				txtV.Text = Convert.ToString(flag);
				StartActivity(typeof(HomeActivity));
			};

			btnExit.Click += delegate
			{
				Finish();
			};

		}

		//protected override void OnRestoreInstanceState(Bundle savedInstanceState)
		//{
		//	flag = savedInstanceState.GetBoolean("flag");
		//	base.OnRestoreInstanceState(savedInstanceState);
		//}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutBoolean("flag", flag);

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
				//Java.Lang.Thread.Sleep(2000);
				db.deleteAllTableEntry();
				XDocument doc = GetXml(xmlUrl);
				ParseXmlEntry(doc);
		}

		private void UpdCache()
		{
				if (flag == false)
				{
					flag = true;
					//запоминание времени записи таблицы
					TimeCreateCache = DateTime.Now;
					MakeCacheFromXML();
				}
				else
				{
					TimeSpan ts = DateTime.Now - TimeCreateCache;
					int differenceInHours = ts.Hours;
					if (differenceInHours >= 10)
					{
						//запоминание времени записи таблицы
						TimeCreateCache = DateTime.Now;
						MakeCacheFromXML();
					}
				}
		}	
	}
}