using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAP.Control
{   
    public abstract class AbstractController
    {   //  Observer Pattern
        public abstract void Update(object sender, EventArgs arg);
    }
}
