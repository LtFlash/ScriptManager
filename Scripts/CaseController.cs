using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptManager.Scripts
{
    public class CaseController : BaseScript
    {
        private Managers.TimerBasedScriptManager _cases = new Managers.TimerBasedScriptManager();

        public override bool CanBeStarted()
        {
            return true;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Process()
        {
            //if all finished SetScriptFinished

            base.Process();
        }

        public override void End()
        {
        }
    }
}
