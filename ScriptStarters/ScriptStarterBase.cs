using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptManager.ScriptStarters
{
    internal abstract class ScriptStarterBase
    {
        public bool FinishedSuccessfully { get; protected set; }
        public bool FinishedUnsuccessfully { get; protected set; }

        public ScriptStarterBase()
        {

        }
    }
}
