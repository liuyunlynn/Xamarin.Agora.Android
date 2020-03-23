using System.Linq;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using DT.Xamarin.Agora;
using DT.Xamarin.Agora.Video;
using Java.Interop;

namespace AgoraVideoCall
{
    [Activity(Label = "VideoCallActivity")]
    public class VideoCallActivity : Activity
    {
        private readonly string[] _permissions =
        {
            Manifest.Permission.Camera,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.RecordAudio,
            Manifest.Permission.ModifyAudioSettings,
            Manifest.Permission.Internet,
            Manifest.Permission.AccessNetworkState
        };

        private bool _isVideoEnabled;
        private RtcEngine _rtcEngine;
        private RtcEventHandler _rtcEventHandler;
        private int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.videocall);
            CheckPermissions();
            _rtcEventHandler = new RtcEventHandler(this);
            _rtcEngine = RtcEngine.Create(BaseContext, "f02659d5a175465f96e45614761ddb07", _rtcEventHandler);
            _rtcEngine.SetVideoProfile(Constants.ChannelProfileCommunication, false);
            _rtcEngine.EnableVideo();
            SetVideoEncoder();
            SetupLocalVideo();
            JoinChannel();
        }

        [Export("SwitchCamera")]
        public void SwitchCamera(View view)
        {
            _rtcEngine.SwitchCamera();
        }

        [Export("MuteLocalVideo")]
        public void MuteLocalVideo(View view)
        {
            var iv = (ImageView) view;
            if (iv.Selected)
            {
                iv.Selected = false;
                iv.SetImageResource(Resource.Mipmap.ic_cam_active_call);
            }
            else
            {
                iv.Selected = true;
                iv.SetImageResource(Resource.Mipmap.ic_cam_disabled_call);
            }

            _rtcEngine.MuteLocalVideoStream(iv.Selected);
            _isVideoEnabled = !iv.Selected;
            FindViewById(Resource.Id.local_video_container).Visibility =
                _isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
        }

        [Export("MuteLocalAudio")]
        public void MuteLocalAudio(View view)
        {
            var iv = (ImageView) view;
            if (iv.Selected)
            {
                iv.Selected = false;
                iv.SetImageResource(Resource.Mipmap.ic_mic_active_call);
            }
            else
            {
                iv.Selected = true;
                iv.SetImageResource(Resource.Mipmap.ic_mic_inactive_call);
            }

            _rtcEngine.MuteLocalAudioStream(iv.Selected);
            var visibleMutedLayers = iv.Selected ? ViewStates.Visible : ViewStates.Invisible;
            FindViewById(Resource.Id.local_video_muted).Visibility = visibleMutedLayers;
        }

        [Export("EndCall")]
        public void EndCall(View view)
        {
            _rtcEngine.StopPreview();
            _rtcEngine.SetupLocalVideo(null);
            _rtcEngine.LeaveChannel();
            _rtcEngine.Dispose();
            _rtcEngine = null;
            Finish();
        }

        private bool CheckPermissions(bool requestPermissions = true)
        {
            var isGranted = _permissions
                .Select(permission => ContextCompat.CheckSelfPermission(this, permission) == (int) Permission.Granted)
                .All(granted => granted);
            if (requestPermissions && !isGranted) ActivityCompat.RequestPermissions(this, _permissions, 0);
            return isGranted;
        }

        public void OnFirstRemoteVideoDecoded(int uid, int width, int height, int elapsed)
        {
            RunOnUiThread(() => { SetupRemoteVideo(uid); });
        }

        private void JoinChannel()
        {
            _rtcEngine.JoinChannel(null, "DEMOCHANNEL1", "Extra Optional Data",
                0); // If you do not specify the uid, Agora will assign one.
        }

        private void SetVideoEncoder()
        {
            var
                orientationMode =
                    VideoEncoderConfiguration.ORIENTATION_MODE.OrientationModeFixedPortrait;

            var dimensions = new VideoEncoderConfiguration.VideoDimensions(360, 640);

            var videoEncoderConfiguration = new VideoEncoderConfiguration(dimensions,
                VideoEncoderConfiguration.FRAME_RATE.FrameRateFps15, VideoEncoderConfiguration.StandardBitrate,
                orientationMode);

            _rtcEngine.SetVideoEncoderConfiguration(videoEncoderConfiguration);
        }

        private void SetupLocalVideo()
        {
            var container = (FrameLayout) FindViewById(Resource.Id.local_video_view_container);
            var surfaceView = RtcEngine.CreateRendererView(BaseContext);
            surfaceView.SetZOrderMediaOverlay(true);
            container.AddView(surfaceView);
            _rtcEngine.SetupLocalVideo(new VideoCanvas(surfaceView, VideoCanvas.RenderModeAdaptive, 0));
        }

        private void SetupRemoteVideo(int uid)
        {
            var container = (FrameLayout) FindViewById(Resource.Id.remote_video_view_container);
            if (container.ChildCount >= 1) return;
            var surfaceView = RtcEngine.CreateRendererView(BaseContext);
            container.AddView(surfaceView);
            _rtcEngine.SetupRemoteVideo(new VideoCanvas(surfaceView, VideoCanvas.RenderModeAdaptive, uid));
            surfaceView.Tag = uid;
        }
    }
}