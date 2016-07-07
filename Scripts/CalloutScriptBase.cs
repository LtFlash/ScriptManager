using Rage;

namespace ScriptManager.Scripts
{
    internal abstract class CalloutScriptBase : BaseScript, ICalloutScript
    {
        private const double TIME_CALL_NOT_ACCEPTED = 10000;
        private System.Windows.Forms.Keys _keyAccept = System.Windows.Forms.Keys.Y;
        private System.Timers.Timer _callNotAcceptedTimer = new System.Timers.Timer(TIME_CALL_NOT_ACCEPTED);
        private bool _timeElapsed = false;
        private Blip _blipArea;
        private readonly System.Drawing.Color _blipAreaColor = System.Drawing.Color.Blue;

        public CalloutScriptBase()
        {
            AddStage(Initialize);
            AddStage(WaitForAcceptKey);
            AddStage(Accepted);
            AddStage(NotAccepted);

            ActivateStage(Initialize);
        }

        public override void Initialize()
        {
            SwapStages(Initialize, WaitForAcceptKey);
            _callNotAcceptedTimer.Start();
            _callNotAcceptedTimer.Elapsed += (s, args) =>
            {
                _timeElapsed = true;
            };
        }

        public void DisplayCalloutInfo(string text)
        {
            Game.DisplayNotification(text);
        }

        public void ShowAreaBlip(Vector3 position, float radius)
        {
            _blipArea = new Blip(position, radius);
            _blipArea.Color = _blipAreaColor;
        }

        private void RemoveAreaBlip()
        {
            if (_blipArea.Exists()) _blipArea.Delete();
        }

        public void DisplayCalloutInfo(string textureDictionaryName, string textureName,
            string title, string subtitle, string text)
        {
            Game.DisplayNotification(textureDictionaryName, textureName, title, subtitle, text);
        }

        private void WaitForAcceptKey()
        {
            if(Game.IsKeyDown(_keyAccept))
            {
                RemoveAreaBlip();
                SwapStages(WaitForAcceptKey, Accepted);
                return;
            }
            
            if(_timeElapsed)
            {
                RemoveAreaBlip();
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
