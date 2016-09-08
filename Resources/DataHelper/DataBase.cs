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
using SQLite;
using Android.Util;
using AndroidSQLite.Resources.Model;

namespace AndroidSQLite.Resources.DataHelper
{
	[Serializable]
    public class DataBase
    {
		string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        public bool createDataBase()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Entrys.db")))
                {
                    connection.CreateTable<Entry>();
                    return true;
                }
            }
            catch(SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

		public bool insertIntoTableEntry(Entry entry)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Entrys.db")))
                {
                    connection.Insert(entry);
                    return true;
                }
            }
            catch(SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<Entry> selectTableEntry()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Entrys.db")))
                {
                    return connection.Table<Entry>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool updateTableEntry(Entry entry)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Entrys.db")))
                {
					connection.Query<Entry>("UPDATE Entry set Name=?,Age=?,Email=? Where Id=?",entry.Place,entry.Appid,entry.Title,entry.Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool deleteTableEntry(Entry entry)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Entrys.db")))
                {
                    connection.Delete(entry);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool selectQueryTableEntry(int Id)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Entrys.db")))
                {
                    connection.Query<Entry>("SELECT * FROM Entry Where Id=?", Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

		public void deleteAllTableEntry()
		{
			var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "Entrys.db"));
			connection.DeleteAll<Entry>();
		}


    }
}