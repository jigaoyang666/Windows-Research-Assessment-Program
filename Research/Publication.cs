using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP
{
    namespace Research
    {
        //declare a euum
        public enum Outputtype { Conference, Journal, Other};
        public class Publication
        {
            //Atrribute
            public string DOI { get; set; }
            public string Title { get; set; }
            public string Authors { get; set; }
            //Publication year
            public int Year { get; set; }
            //one of 'conference','journal'and 'other'
            public Outputtype Type { get; set; }
            //
            public string CiteAs { get; set; }
            //the date when the publication was first available
            public DateTime Availability { get; set; }
            //the days elapsed since the publication became available
            public int Age { get; set; }
            //Method
            public  int GetAge()
            {
                TimeSpan ts = DateTime.Now.Subtract(Availability);
                return ts.Days;
            }
        }
    }
}
