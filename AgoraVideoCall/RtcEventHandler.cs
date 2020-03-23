using DT.Xamarin.Agora;

namespace AgoraVideoCall
{
    public class RtcEventHandler : IRtcEngineEventHandler
    {
        private readonly VideoCallActivity _activitiy;

        public RtcEventHandler(VideoCallActivity activity)
        {
            _activitiy = activity;
        }

        public override void OnFirstRemoteVideoDecoded(int uid, int width, int height, int elapsed)
        {
            _activitiy.OnFirstRemoteVideoDecoded(uid, width, height, elapsed);
        }
    }
}