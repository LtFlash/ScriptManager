using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptManager.Example
{
    class Homicide : Scripts.BaseScript
    {
        ScriptManager _stagesOfInvestigation = new ScriptManager();

        public Homicide()
        {
            _stagesOfInvestigation.AddScript("csi", new Homicide_CSI(), true);
        }

        public override void Process()
        {
            base.Process();
        }

        public override void End()
        {
            base.End();
        }
    }
}
