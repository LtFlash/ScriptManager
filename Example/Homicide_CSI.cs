using System;

namespace ScriptManager.Example
{
    class Homicide_CSI : Scripts.CalloutScriptBase
    {
        protected override bool Initialize()
        {
            return true;
        }

        protected override bool Accepted()
        {
            return true;
        }

        protected override void NotAccepted()
        {
        }

        protected override void Process()
        {
        }

        protected override void End()
        {
        }
    }
}
