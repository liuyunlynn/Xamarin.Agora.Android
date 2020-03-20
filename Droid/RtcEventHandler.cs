using System;
using DT.Xamarin.Agora;

namespace AgoraDemo.Droid
{
    public class RtcEventHandler : IRtcEngineEventHandler
    {
        private MainActivity _activitiy;

        public RtcEventHandler(MainActivity activity)
        {
            _activitiy = activity;
        }

        public override void OnFirstRemoteVideoDecoded(int uid, int width, int height, int elapsed)
        {
            _activitiy.OnFirstRemoteVideoDecoded(uid, width, height, elapsed);
        }
    }
}
