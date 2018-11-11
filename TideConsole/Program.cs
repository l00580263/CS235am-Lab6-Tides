using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using TideLibrary;

namespace TideConsole
{
    class Program
    {

        static string currentDir;



        static void Main(string[] args)
        {
            Console.WriteLine("Hello Losers!");

            // get current dir
            currentDir = Directory.GetCurrentDirectory();
            
            // get path to database
            string dbPath = currentDir + @"/../../../../TideApp/Assets/Tides.db3";

            // get db
            SQLiteConnection db = new SQLiteConnection(dbPath);

            // drop Tides table
            db.DropTable<Tide>();

            // create Tides table if no table xists
            if (db.CreateTable<Tide>() == 0)
            {
                // deletes all entries if table exists already
                db.DeleteAll<Tide>();
            }

            // add to db
            AddTidesToTable(db, "Florence", "/../../../../TideLibrary/Florence_annual.xml");
            AddTidesToTable(db, "Florence USCG Pier", "/../../../../TideLibrary/FlorenceUSCGPier_annual.xml");
            AddTidesToTable(db, "Suislaw River Entrance", "/../../../../TideLibrary/SuislawRiverEntrance_annual.xml");

            Console.ReadLine();
        }



        static void AddTidesToTable(SQLiteConnection db, string location, string file)
        {
            // open tides
            XmlTideFileParser noaa = new XmlTideFileParser( File.Open(currentDir + file, FileMode.Open));

            int rows = 0;

            //  make tides objects
            foreach (IDictionary<string, object> o in noaa.TideList)
            {
                Tide t = new Tide();
                
                // give location
                t.Location = location;

                // get date and time in a string format
                string dateFromXML = (string)o[XmlTideFileParser.DATE];
                int day = int.Parse(dateFromXML.Substring(8, 2));
                int month = int.Parse(dateFromXML.Substring(5, 2));
                int year = int.Parse(dateFromXML.Substring(0, 4));
                string time = (string)o[XmlTideFileParser.TIME];

                // set Date
                t.Date = DateTime.Parse(month + "/" + day + "/" + year + " " + time ).Ticks;
                // set Day
                t.Day = (string)o["day"];
                // set Height 
                t.Height = float.Parse((string)o[XmlTideFileParser.HEIGHT]);
                // set HighLow
                t.HighLow = ((string)o[XmlTideFileParser.HI_LOW] == "H" ? "High" : "Low");


                // add to db
                rows += db.Insert(t);

                if (rows % 50 == 0)
                {
                    Console.WriteLine("Inserted " + rows + " so far, most recent is: " + t);
                }
            }

            Console.WriteLine("Done with " + location);

        }
    }
}
