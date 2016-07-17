using Rage;

namespace ScriptManager.Scripts
{
    public abstract class CalloutScriptBase : BaseScript, ICalloutScript
    {
        //PRIVATE
        private const double TIME_CALL_NOT_ACCEPTED = 10000;
        private System.Windows.Forms.Keys _keyAccept = System.Windows.Forms.Keys.Y;
        private System.Timers.Timer _callNotAcceptedTimer = new System.Timers.Timer(TIME_CALL_NOT_ACCEPTED);
        private bool _timeElapsed = false;
        private Blip _blipArea;
        private Blip _blipRoute;
        private float _blipRouteRadius;
        private Vector3 _blipRoutePosition;
        private const float BLIP_ALPHA = 0.3f;
        private bool _zoomOutMinimap = false;
        private readonly System.Drawing.Color _blipAreaColor = System.Drawing.Color.Blue;

        public CalloutScriptBase()
        {
            RegisterStages();
        }

        private void RegisterStages()
        {
            AddStage(InternalInitialize);
            AddStage(WaitForAcceptKey);
            AddStage(InternalAccepted);
            AddStage(Process);
            AddStage(InternalNotAccepted);
            AddStage(InternalEnd);
            AddStage(RemoveAreaWhenClose);

            ActivateStage(InternalInitialize);
        }

        public override bool CanBeStarted()
        {
            return true;
        }

        private void InternalInitialize()
        {
            if (!Initialize())
            {
                SwapStages(InternalInitialize, InternalEnd);
                return;
            }

            _callNotAcceptedTimer.Start();
            _callNotAcceptedTimer.Elapsed += (s, args) =>
            {
                _timeElapsed = true;
            };

            SwapStages(InternalInitialize, WaitForAcceptKey);
        }

        public void DisplayCalloutInfo(string text)
        {
            Game.DisplayNotification(text);
        }

        protected void ShowAreaBlip(Vector3 position, float radius, bool zoomOutMinimap = true, bool flashMinimap = true)
        {
            _blipArea = new Blip(position, radius);
            _blipArea.Color = _blipAreaColor;
            _blipArea.Alpha = BLIP_ALPHA; //max val == 1f

            _zoomOutMinimap = zoomOutMinimap;
            if (flashMinimap) FlashMinimap();
        }

        protected void ShowAreaWithRoute(Vector3 position, float radius, System.Drawing.Color color)
        {
            _blipRoute = new Blip(position, radius);
            _blipRoute.Alpha = BLIP_ALPHA;
            _blipRoute.Color = color;
            _blipRoute.EnableRoute(color);

            _blipRouteRadius = radius;
            _blipRoutePosition = position;
            ActivateStage(RemoveAreaWhenClose);
        }

        protected void RemoveAreaBlipWithRoute()
        {
            if (_blipRoute.Exists()) _blipRoute.Delete();
        }

        private void RemoveAreaWhenClose()
        {
            if(Game.LocalPlayer.Character.Position.DistanceTo(_blipRoutePosition) <= _blipRouteRadius)
            {
                RemoveAreaBlipWithRoute();
                DeactivateStage(RemoveAreaWhenClose);
            }
        }

        //TODO: to use with ShowAreaBlip()
        private void SetMinimapZoom(int zoomLevel)
        { //zoomLevel == 0..200
            const ulong SET_RADAR_ZOOM = 0x096EF57A0C999BBA;
            Rage.Native.NativeFunction.CallByHash<uint>(SET_RADAR_ZOOM, zoomLevel);
        }

        private void FlashMinimap()
        {
            const ulong FLASH_MINIMAP_DISPLAY = 0xF2DD778C22B15BDA;
            Rage.Native.NativeFunction.CallByHash<uint>(FLASH_MINIMAP_DISPLAY);
        }

        private void RemoveAreaBlip()
        {
            if (_blipArea.Exists()) _blipArea.Delete();
            SetMinimapZoom(0);
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
                SwapStages(WaitForAcceptKey, InternalAccepted);
                return;
            }

            if(_blipArea.Exists() && _zoomOutMinimap)
            {
                Game.SetRadarZoomLevelThisFrame(200f);
            }
            
            if(_timeElapsed)
            {
                RemoveAreaBlip();
                SwapStages(WaitForAcceptKey, InternalNotAccepted);
                _callNotAcceptedTimer.Dispose();
            }
        }

        private void InternalAccepted()
        {
            if (!Accepted())
            {
                SwapStages(InternalAccepted, InternalEnd);
                return;
            }

            SwapStages(InternalAccepted, Process);
        }


        private void InternalNotAccepted()
        {
            NotAccepted();
            SetScriptFinished(false);
        }

        private void InternalEnd()
        {
            RemoveAreaBlipWithRoute();
            RemoveAreaBlip();
            End();
            Stop(); 
        }

        public void SetScriptFinished(bool completed)
        {
            Completed = completed;
            InternalEnd();
        }

        protected abstract bool Accepted();
        protected abstract void NotAccepted();
    }
}
