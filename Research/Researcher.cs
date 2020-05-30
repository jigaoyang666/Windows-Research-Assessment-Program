using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace RAP
{
    namespace Research
    {
        public enum ResearcherType { Staff, Student };
        public class Researcher
        {
            public Researcher()
            {
                PublicationList = new List<Publication>();
                PositionList = new List<Position>();
            }
            public string ID { get; set; }
            public string GivenName { get; set; }
            public string FamilyName { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string School { get; set; }
            public string Campus { get; set; }
            public string Email { get; set; }
            public string Photo { get; set; }
            public string Level { get; set; }
            public string CurrentJob { get; set; }   //A label derived from their current position’s level
            public DateTime EarliestPosition { get; set; }      //The start date of their earliest position
            public DateTime CurrentPosition { get; set; }       //The start date of their current position
            public float Tenure { get; set; }       //Time in (fractional) years since the researcher commenced with the institution
            public int TotalPublications { get; set; }      //The total number of publications authored by the researcher

            public List<Publication> PublicationList { get; set; }
            public List<Position> PositionList { get; set; }

            /// <summary>
            /// get start time of current job
            /// </summary>
            /// <returns></returns>
            public Position GetCurrentJob()
            {
                Position p = new Position();
                p.Level = p.GetEmploymentLevel(Level);
                p.start_date = CurrentPosition;          // from database
                p.end_date = DateTime.Now;
                return p;
            }
            public string CurrentJobTitle() { return GetCurrentJob().Title();  }

            /// <summary>
            /// get the time of previous job
            /// </summary>
            /// <returns></returns>
            public Position GetEarliestJob()
            {
                Position p = new Position();
                if (PositionList.Count > 0)
                {
                    p = PositionList[PositionList.Count - 1];
                }
                return p;
            }
            
            public DateTime CurrentJobStart()
            {
                return CurrentPosition;
            }

            public DateTime EarliestStart()
            {
                return EarliestPosition;
            }

            public float GetTenure()
            {
                TimeSpan ts = DateTime.Now.Subtract(EarliestStart());
                float tenure = (float)(ts.Days * 1.0 / 365.0);
                return (float)Math.Round((double)tenure, 1);
            }

            public int PublicationsCount()
            {
                int count = 0;
                if(PublicationList != null && PublicationList.Count >0)
                    count = PublicationList.Count;

                return count;
            }

            public override string ToString()
            {
                return string.Format("{0}({1})", Name, Title);
            }
        }

    }
}
