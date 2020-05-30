using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP
{
    namespace Research
    {
        public class Student : Researcher
        {//to show the details of students
            public string Degree { get; set; }
            public Staff Supervisor { get; set; }
            public string SupervisorName { get; set; }

        }
    }
}
