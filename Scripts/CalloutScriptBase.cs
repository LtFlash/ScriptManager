﻿using Rage;
using System.Media;

namespace ScriptManager.Scripts
{
    public abstract class CalloutScriptBase : ScriptBase, ICalloutScript
    {
        //PUBLIC
        public float DistanceSoundPlayerClosingIn { get; set; } = 80f;
        public bool PlaySoundPlayerClosingIn
        {
            set
            {
                if (value) ActivateStage(PlaySoundWhenPlayerNearby);
                else DeactivateStage(PlaySoundWhenPlayerNearby);
            }
        }
        public SoundPlayer SoundPlayerClosingIn
        {
            set
            {
                _soundPlayerClosingIn = value;
            }
        }

        //PROTECTED
        protected System.Windows.Forms.Keys KeyAcceptCallout { get; set; } = System.Windows.Forms.Keys.Y;
        protected double Timeout { set { _callNotAcceptedTimer.Interval = value; } }

        //PRIVATE
        private const double TIME_CALL_NOT_ACCEPTED = 10000;
        private const float RADAR_ZOOM_LEVEL = 200f;
        private const float BLIP_ALPHA = 0.3f;

        private SoundPlayer _soundPlayerClosingIn = new SoundPlayer(Properties.Resources.CaseApproach);
        private System.Timers.Timer _callNotAcceptedTimer = new System.Timers.Timer(TIME_CALL_NOT_ACCEPTED);
        private bool _timeElapsed = false;
        private bool _zoomOutMinimap = false;
        private Blip _blipArea;
        private Blip _blipRoute;
        private float _blipRouteRadius;
        private Vector3 _blipRoutePosition;
        private Vector3 _callPosition;
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

            AddStage(PlaySoundWhenPlayerNearby);

            ActivateStage(InternalInitialize);
        }

        private void PlaySoundWhenPlayerNearby()
        {
            if (Vector3.Distance(Game.LocalPlayer.Character.Position, _callPosition) <= DistanceSoundPlayerClosingIn)
            {
                _soundPlayerClosingIn.Play();
                DeactivateStage(PlaySoundWhenPlayerNearby);
            }
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

        protected void DisplayCalloutInfo(string text)
        {
            Game.DisplayNotification(text);
        }

        protected void ShowAreaBlip(Vector3 position, float radius, bool zoomOutMinimap = true, bool flashMinimap = true)
        {
            _blipArea = new Blip(position, radius);
            _blipArea.Color = _blipAreaColor;
            _blipArea.Alpha = BLIP_ALPHA; //max val == 1f

            _callPosition = position;

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
            => Rage.Native.NativeFunction.Natives.SetRadarZoom(zoomLevel);

        private void FlashMinimap()
            => Rage.Native.NativeFunction.Natives.FlashMinimapDisplay();

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
            if(Game.IsKeyDown(KeyAcceptCallout))
            {
                RemoveAreaBlip();
                SwapStages(WaitForAcceptKey, InternalAccepted);
                return;
            }

            if(_blipArea.Exists() && _zoomOutMinimap)
            {
                Game.SetRadarZoomLevelThisFrame(RADAR_ZOOM_LEVEL);
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
            _soundPlayerClosingIn.Dispose();
            RemoveAreaBlipWithRoute();
            RemoveAreaBlip();
            End();
            Stop(); 
        }

        public void SetScriptFinished(bool completed)
        {
            HasFinished = true;
            Completed = completed;
            InternalEnd();
        }

        protected abstract bool Accepted();
        protected abstract void NotAccepted();
    }
}
