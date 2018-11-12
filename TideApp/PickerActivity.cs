using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SQLite;
using TideLibrary;

namespace TideApp
{
    [Activity(Label = "PickerActivity", Theme = "@style/AppTheme", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    public class PickerActivity : AppCompatActivity
    {

        string location;
        long date;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.picker_layout);

            // path to db in assets
            string dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Tides.db3");

            using (Stream inStream = Assets.Open("Tides.db3"))
            using (Stream outStream = File.Create(dbPath))
                inStream.CopyTo(outStream);

            // get db
            var db = new SQLiteConnection(dbPath);


            // set up spinner
            // get a list of only the first of each tide object at a location, then get the location names to a list
            var locations = db.Table<Tide>().GroupBy(t => t.Location).Select(t => t.First()).Select(t => t.Location).ToList();

            // make adapter with list of locations
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, locations);

            // set spinner to use adapter
            var tideSpinner = FindViewById<Spinner>(Resource.Id.locationSpinner);
            tideSpinner.Adapter = adapter;


            // get location
            tideSpinner.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e) {
                Spinner spinner = (Spinner)sender;
                location = (string)spinner.GetItemAtPosition(e.Position);
            };


            FindViewById<Button>(Resource.Id.confirmButton).Click += (sender, o) =>
            {
                Intent i = new Intent(this, typeof(MainActivity));

                // pass location
                i.PutExtra(MainActivity.LOCATION_KEY, location);

                // get date
                date = FindViewById<DatePicker>(Resource.Id.datePicker).DateTime.Ticks;

                // pass date
                i.PutExtra(MainActivity.DATE_KEY, date);

                // next activity
                StartActivity(i);
            };
        }
    }
}