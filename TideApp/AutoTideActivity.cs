using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using SQLite;
using TideLibrary;

namespace TideApp
{
    [Activity(Label = "AutoTideActivity", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    public class AutoTideActivity : Activity
    {
     



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.auto_tide_layout);

            // get text views
            var locationText = FindViewById<TextView>(Resource.Id.locationText);
            var tideText = FindViewById<TextView>(Resource.Id.nextTideText);


            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            // get geolocator
            var locator = CrossGeolocator.Current;

            // set locator accuracy in meters
            locator.DesiredAccuracy = 200;


            // set up button to next activity
            FindViewById<Button>(Resource.Id.goToPickerButton).Click += (sender, o) => 
            {
                var picker = new Intent(this, typeof(PickerActivity));
                StartActivity(picker);
            };


            // set up button to update location
            FindViewById<Button>(Resource.Id.updateNextTideButton).Click += (sender, o) =>
            {
                // get current location
                locator.GetPositionAsync(timeout: TimeSpan.FromMilliseconds(10000)).ContinueWith(t =>
                {
                    try
                    {
                        var db = GetDB();

                        // get closest tide location
                        string closestLocationName = GetClosestLocation(t.Result, db);
                        // query parameters
                        var now = DateTime.Now.Ticks;

     
                        // get next tide
                        Tide nextTide = (from tide in db.Table<Tide>()
                                         where (tide.Location == closestLocationName)
                                         && (tide.Date > now)
                                         select tide).First();


                        var nextTideDate = new DateTime(nextTide.Date);
                        // update text
                        locationText.Text = nextTide.Location + ", " + nextTideDate.Month + "/" + nextTideDate.Day;
                        tideText.Text = nextTide.ToStringFormatForTextView();
                    }
                    catch (Exception e)
                    {
                        locationText.Text = "Couldn't get your location";
                        tideText.Text =  "Geo Available? " + locator.IsGeolocationAvailable.ToString() + 
                        ".  Geo Enabled? " + locator.IsGeolocationEnabled.ToString() + 
                        ".  Geo Supported? " + CrossGeolocator.IsSupported.ToString() + 
                        ".  Error: " + e.ToString();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            };
        }



        SQLiteConnection GetDB()
        {
            // path to db in assets
            string dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Tides.db3");

            using (Stream inStream = Assets.Open("Tides.db3"))
            using (Stream outStream = File.Create(dbPath))
                inStream.CopyTo(outStream);

            // get db
            return new SQLiteConnection(dbPath);
        }



        string GetClosestLocation(Position currentLocation, SQLiteConnection db)
        {
            var shortestDistance = double.MaxValue;
            var closetLocation = "";

            // get list of location objects
            var locations = (from loc in db.Table<TideLocation>() select loc).ToList<TideLocation>();

            foreach (TideLocation l in locations)
            {
                // get tide position
                var tidePosition = new Position() { Longitude = l.Longitude, Latitude = l.Latitude};

                // get distance to tide position
                var distance = currentLocation.CalculateDistance(tidePosition);

                // check if closest tide position
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closetLocation = l.Name;
                }
            }

            // return closest tide location name
            return closetLocation;
        }
    }
}