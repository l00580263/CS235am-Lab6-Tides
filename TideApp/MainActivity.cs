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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : ListActivity
    {

        List<Tide> tides;
        public const string LOCATION_KEY = "location";
        public const string DATE_KEY = "date";



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

            // query parameters
            var dayStart = Intent.GetLongExtra(DATE_KEY, new DateTime().Ticks);
            var dayEnd = dayStart + TimeSpan.TicksPerDay;
            var location = Intent.GetStringExtra(LOCATION_KEY);

            // get a tides at a location
            tides = (from t in db.Table<Tide>()
                         where (t.Location == location)
                             && (t.Date >= dayStart)
                             && (t.Date <= dayEnd)
                     select t).ToList();

            // make a formatted list
            List<string> convertedTides = new List<string>();

            foreach (Tide t in tides)
            {
                convertedTides.Add(t.ToStringFormatForListView());
            }

            // adapter
            ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, convertedTides);
           
            // fast scroll
            ListView.FastScrollEnabled = true;
        }
    }
}