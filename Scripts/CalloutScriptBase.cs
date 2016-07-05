using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace ScriptManager.Scripts
{
    internal abstract class CalloutScriptBase : BaseScript, ICalloutScript
    {
        private const double TIME_CALL_NOT_ACCEPTED = 10000;
        private System.Windows.Forms.Keys _keyAccept = System.Windows.Forms.Keys.Y;
        private System.Timers.Timer _callNotAcceptedTimer = new System.Timers.Timer(TIME_CALL_NOT_ACCEPTED);

        public CalloutScriptBase()
        {
            AddStage(Initialize);
            AddStage(Accepted);
            AddStage(NotAccepted);

            ActivateStage(Initialize);
        }

        public override void Initialize()
        {
            SwapStages(Initialize, WaitForAcceptKey);
            _callNotAcceptedTimer.Start();
        }

        public void DisplayCalloutInfo(string text)
        {
            //TODO: add an overload ftc to handle textures
            Game.DisplayNotification(text);
        }

        private void WaitForAcceptKey()
        {
            if(Game.IsKeyDown(_keyAccept))
            {
                SwapStages(WaitForAcceptKey, Accepted);
                return;
            }
            
            _callNotAcceptedTimer.Elapsed += (s, args) =>
            {
                SwapStages(WaitForAcceptKey, NotAccepted);
                _callNotAcceptedTimer.Dispose();
            };
        }

        public virtual void Accepted()
        {
            SwapStages(Accepted, Process);
        }

        public virtual void NotAccepted()
        {
            SetScriptFinished(false);
        }
    }
}
