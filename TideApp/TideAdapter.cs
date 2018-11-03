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
using Java.Lang;
using TideTableUsingXmlFile;

namespace TideApp
{
    class TideAdapter : SimpleAdapter, ISectionIndexer
    {

        string[] sections;
        Java.Lang.Object[] sectionsInfo;
        Dictionary<string, int> keyPosition = new Dictionary<string, int>();
        string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};



        public TideAdapter(Context c, List<IDictionary<string, object>> data, int r, string[] f, int[] t) : base(c, data, r, f, t)
        {
            BuildSectionsInfo(data);
        }



        public int GetPositionForSection(int sectionIndex)
        {
            // get the index where a section starts
            return keyPosition[sections[sectionIndex]];
        }



        public int GetSectionForPosition(int position)
        {
            // find what section a position is in
            return 1;
        }



        public Java.Lang.Object[] GetSections()
        {
            return sectionsInfo;
        }



        void BuildSectionsInfo(List<IDictionary<string, object>> data)
        {
            int position = 0;

            // create dict of keys and positions
            foreach (IDictionary<string, object> o in data)
            {
                // get month
                int month = int.Parse(((string)o[XmlTideFileParser.DATE]).Substring(5, 2));
                // get key month
                string key = months[month - 1];

                // if this is the first entry of the key
                if (!keyPosition.ContainsKey(key))
                {
                    // add to key position dict
                    keyPosition[key] = position;
                }

                position++;
            }

            // create sections array
            sections = new string[keyPosition.Keys.Count];
            keyPosition.Keys.CopyTo(sections, 0);

            // put into Java object array
            sectionsInfo = new Java.Lang.Object[sections.Length];

            int i = 0;
            foreach(string s in sections)
            {
                sectionsInfo[i] = new Java.Lang.String(s);
                i++;
            }
        }
    }
}