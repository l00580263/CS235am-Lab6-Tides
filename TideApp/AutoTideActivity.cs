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
using Plugin.Geolocator;

namespace TideApp
{
    [Activity(Label = "AutoTideActivity", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleInstance)]
    public class AutoTideActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.auto_tide_layout);

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
        }
    }
}