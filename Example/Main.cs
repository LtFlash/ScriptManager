using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptManager.Example
{
    class Main
    {
        ScriptManager _cases = new ScriptManager();

        public void AddCases()
        {
            _cases.AddScript("homicide", new Homicide(), true);
        }
    }
}
