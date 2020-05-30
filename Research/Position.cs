using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP
{
    namespace Research
    {
        //Declare an enumeration
        public enum EmploymentLevel { ALL,A, B, C, D, E, Student };

        public class Position
        {
            public DateTime start_date { get; set; }

            public DateTime end_date { get; set; }

            public EmploymentLevel Level { get; set; }

            public string Title()
            {
                string title;
                switch (Level)
                {
                    case EmploymentLevel.A:
                        title = "Postdoc";
                        break;
                    case EmploymentLevel.B:
                        title = "Lecturer";
                        break;
                    case EmploymentLevel.C:
                        title = "Senior Lecturer";
                        break;
                    case EmploymentLevel.D:
                        title = "Associate Professor";
                        break;
                    case EmploymentLevel.E:
                        title = "Professor";
                        break;
                    case EmploymentLevel.Student:
                        title = "Student";
                        break;
                    default:
                        title = "";
                        break;
                }

                return title;
            }

            public string ToTitle(EmploymentLevel l)
            {
                string title;
                switch (l)
                {
                    case EmploymentLevel.A:
                        title = "Postdoc";
                        break;
                    case EmploymentLevel.B:
                        title = "Lecturer";
                        break;
                    case EmploymentLevel.C:
                        title = "Senior Lecturer";
                        break;
                    case EmploymentLevel.D:
                        title = "Associate Professor";
                        break;
                    case EmploymentLevel.E:
                        title = "Professor";
                        break;
                    case EmploymentLevel.Student:
                        title = "Student";
                        break;
                    default:
                        title = "";
                        break;
                }

                return title;
            }

            public EmploymentLevel GetEmploymentLevel(string level)
            {
                Research.EmploymentLevel l;
                switch (level)
                {
                    case "A":
                        l = Research.EmploymentLevel.A;
                        break;
                    case "B":
                        l = Research.EmploymentLevel.B;
                        break;
                    case "C":
                        l = Research.EmploymentLevel.C;
                        break;
                    case "D":
                        l = Research.EmploymentLevel.D;
                        break;
                    case "E":
                        l = Research.EmploymentLevel.E;
                        break;
                    case "Student":
                    case "":
                        l = Research.EmploymentLevel.Student;
                        break;
                    default:
                        l = Research.EmploymentLevel.ALL;
                        break;
                }
                return l;
            }
        }
    }
}
