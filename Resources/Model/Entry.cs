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

namespace AndroidSQLite.Resources.Model
{
	public class Entry
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Place { get; set; }
        public int Appid { get; set; }
    }
}