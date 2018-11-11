using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TideLibrary
{
    [Table("Tides")]
    public class Tide
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Location { get; set; }
        public long Date { get; set; }
        public string Day { get; set; }
        public float Height { get; set; }
        public string HighLow { get; set; }



        public string GetMonthAsName()
        {
            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            int month = new DateTime(Date).Month;
            return months[month];
        }



        public override string ToString()
        {
            return "Location = " + Location + ", Date = " + Date + ", Day = " + Day + ", Height = " + Height + ", HighLow = " + HighLow;
        }
    }
}
