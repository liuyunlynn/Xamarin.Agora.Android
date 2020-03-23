using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using AlertDialog = Android.App.AlertDialog;

namespace AgoraVideoCall
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var login = FindViewById<Button>(Resource.Id.login);
            login.Click += Login_Click;
            var call = FindViewById<Button>(Resource.Id.call);
            call.Click += Call_Click;
           
        }

        private async void Call_Click(object sender, EventArgs e)
        {
            var callId= FindViewById<EditText>(Resource.Id.callId).Text;
            var client = SignalRHubClient.Instance;
            await client.Send(callId, "Video Call");
            var intent = new Intent(this, typeof(VideoCallActivity));
            StartActivity(intent);
        }

        private async void Login_Click(object sender, EventArgs e)
        {
            var client = SignalRHubClient.Instance;
            try
            {
                var userId = FindViewById<EditText>(Resource.Id.userid).Text;
                await client.Connect(userId);
                client.OnReceiveEvent += (id, message) =>
                {
                    RunOnUiThread(() =>
                    {
                        var content = message + id;
                        var alertDialog = new AlertDialog.Builder(this);
                        alertDialog.SetTitle("Video Call").SetMessage(content);
                        alertDialog.SetCancelable(true);
                        alertDialog.SetNegativeButton("Denial",
                            (s, d) =>
                            {

                            });
                        alertDialog.SetPositiveButton("Answer",
                            (s, d) =>
                            {
                                var intent = new Intent(this, typeof(VideoCallActivity));
                                StartActivity(intent);
                            });
                        alertDialog.Show();
                    });
                  
                };
            }
            catch (Exception ex)
            {
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.action_settings) return true;

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            var view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener) null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}