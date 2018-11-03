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

            ListAdapter = new SimpleAdapter(this, noaa.TideList, Resource.Layout.rowLayout, 
                new string[] {XmlTideFileParser.DATE, XmlTideFileParser.HI_LOW, XmlTideFileParser.TIME}, 
                new int[] {Resource.Id.dateText, Resource.Id.hiLowText, Resource.Id.timeText } );

            ListView.FastScrollEnabled = true;
        }



        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            
        }
    }
}