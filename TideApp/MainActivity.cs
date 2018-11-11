using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using TideLibrary;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using SQLite;

namespace TideApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : ListActivity
    {

        List<Tide> tides;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // path to db in assets
            string dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Tides.db3");

            using (Stream inStream = Assets.Open("Tides.db3"))
            using (Stream outStream = File.Create(dbPath))
                inStream.CopyTo(outStream);

            // get db
            var db = new SQLiteConnection(dbPath);

            var dayStart = new DateTime(2018, 6, 15).Ticks;
            var dayEnd = new DateTime(2018, 6, 16).Ticks;

            // get a tides at a location
            tides = (from t in db.Table<Tide>()
                         where (t.Location == "Florence")
                             && (t.Date >= dayStart)
                             && (t.Date <= dayEnd)
                     select t).ToList();



            // adapter
            ListAdapter = new ArrayAdapter<Tide>(this, Android.Resource.Layout.SimpleListItem1, tides);
           
            // fast scroll
            ListView.FastScrollEnabled = true;
        }



        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            // toast message
            string message = (string) ((JavaDictionary<string, object>)ListView.GetItemAtPosition(position))[XmlTideFileParser.HEIGHT];
            message += " cm";
            // show toast
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }
    }
}