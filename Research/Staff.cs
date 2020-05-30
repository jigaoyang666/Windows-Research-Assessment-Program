using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP
{
    namespace Research
    {

        public class Staff : Researcher
        {//to show the details of staff
            public Dictionary<string, string> SupervisionList { get; set; }

            public int Supervisions { get; set; }
            public float Performance { get; set; }
            public float Average { get; set; }

            public Staff()
            {
                SupervisionList = new Dictionary<string, string>();
            }
            /// <summary>
            /// to get the number of publication for three years
            /// </summary>
            /// <returns></returns>
            public float ThreeYearAverage()
            {
                float average = (float)0.0;
                int end_year = DateTime.Now.Year;
                int start_year = end_year - 3;
                if (PublicationList != null && PublicationList.Count > 0)
                {
                    foreach (Publication item in PublicationList)
                    {
                        if (item.Year >= start_year && item.Year < end_year)
                        {
                            average = average + 1;
                        }
                    }
                }
                average = (float)(average / 3.0);
                return (float)Math.Round((double)average, 2);
            }
            /// <summary>
            /// calculate the researcher performance
            /// </summary>
            /// <returns></returns>
            public float GetPerformance()
            {
                EmploymentLevel l = GetCurrentJob().Level;
                float ave = ThreeYearAverage();
                float ratio = (float)0.0;
                switch (l)
                {
                    case EmploymentLevel.A:
                        ratio = (float)0.5;
                        break;
                    case EmploymentLevel.B:
                        ratio = 1;
                        break;
                    case EmploymentLevel.C:
                        ratio = 2;
                        break;
                    case EmploymentLevel.D:
                        ratio = (float)3.2;
                        break;
                    case EmploymentLevel.E:
                        ratio = 4;
                        break;
                    default:
                        ratio = (float)double.PositiveInfinity;
                        break;
                }
                ave = ave / ratio * 100;
                return (float)Math.Round((double)ave, 2);
            }
        }
    }
}
