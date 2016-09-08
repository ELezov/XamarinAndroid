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

namespace AndroidSQLite
{
    [Activity(Label = "AndroidSQLite", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //List<Entry> lstSource = new List<Entry>();
        DataBase db;
		//ListView lstData;
		DateTime TimeCreateCache;
		string xmlUrl = "http://andrei-chernyavskii.name/steam/?t=17acff5c5dad004d67a342f5bbfaa3ef";
		XmlTextReader reader;
		bool flag;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Create DataBase
            db = new DataBase();
            db.createDataBase();
			string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Log.Info("DB_PATH", folder);

			//lstData = FindViewById<ListView>(Resource.Id.listView1);

			var btnParse = FindViewById<Button>(Resource.Id.btnParse);
			var btnHome = FindViewById<Button>(Resource.Id.btnHome);
			var btnExit = FindViewById<Button>(Resource.Id.btnExit);

			//Console.WriteLine(TimeCreateCache);
			//Console.WriteLine("Checccckk 1");

            //Event
			btnParse.Click += delegate
            {
				//Console.WriteLine(TimeCreateCache);
				if (flag == false)
				{
					flag = true;
					//Console.WriteLine("Checccckk 2");
					//запоминание времени записи таблицы
					TimeCreateCache = DateTime.Now;
					MakeCache();
				}
				else //(TimeCreateCache != null)
				{
					//Console.WriteLine("Checccckk 3");
					//Console.WriteLine(DateTime.Now);
					TimeSpan ts = DateTime.Now - TimeCreateCache;
					//Console.WriteLine(DateTime.Now);
					//Console.WriteLine(TimeCreateCache);
					//Console.WriteLine(ts.Hours);
					//Console.WriteLine(ts.Minutes);

					int differenceInHours = ts.Hours;
					if (differenceInHours >= 10)
					{
						//Console.WriteLine("My Check WTF!");
						//запоминание времени записи таблицы
						TimeCreateCache = DateTime.Now;
						MakeCache();
					}
				}

				StartActivity(typeof(MyListViewActivity));
			};

			btnHome.Click += delegate {
				StartActivity(typeof(HomeActivity));
            };

			btnExit.Click += delegate {
				
				Finish();
            };

        }

		private void MakeCache()
		{

			reader= new XmlTextReader(xmlUrl);
			//очищение таблицы
			db.deleteAllTableEntry();
			//перезапись таблицы
			//Console.WriteLine("CHEECCKKKK");
			while (reader.Read())
			{
				if (reader.Name == "place")
				{
					Entry entry = new Entry();
					entry.Place = Convert.ToInt32(reader.ReadString());
					while (reader.Read())
					{
						if (reader.Name == "appid")
						{
							entry.Appid = Convert.ToInt32(reader.ReadString());
							goto First;
						}
					}
				First:
					while (reader.Read())
					{
						if (reader.Name == "title")
						{
							entry.Title = reader.ReadString();
							//Console.WriteLine("Place: " + entry.getPlace() + "  Appid: " + entry.getAppid() + "  Title: " + entry.getTitle());
							db.insertIntoTableEntry(entry);
							//LoadData();
							goto Second;
						}
					}
				}
			Second:
				GC.Collect();

			}
		}
				
    }
}