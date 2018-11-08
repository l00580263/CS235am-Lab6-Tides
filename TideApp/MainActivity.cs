using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using TideTableUsingXmlFile;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TideApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : ListActivity
    {





        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // read xml
            XmlTideFileParser noaa = new XmlTideFileParser(Assets.Open("tides_annual.xml"));

            // change each L and H to Low and High
            foreach (IDictionary<string, object> o in noaa.TideList)
            {
                string v = (string)o["highlow"];
                o["highlow"] = (v == "H" ? "High" : "Low");
            }

                

            // adapter
            ListAdapter = new TideAdapter(this, noaa.TideList, Resource.Layout.rowLayout, 
                new string[] {XmlTideFileParser.DATE, XmlTideFileParser.HI_LOW, XmlTideFileParser.TIME, XmlTideFileParser.DAY}, 
                new int[] {Resource.Id.dateText, Resource.Id.hiLowText, Resource.Id.timeText, Resource.Id.dayText } );
           
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