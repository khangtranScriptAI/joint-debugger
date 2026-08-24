using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Widget;
using JointDebugger.Services;

namespace JointDebugger
{
    /// <summary>
    /// Single-screen activity that asks for the SYSTEM_ALERT_WINDOW permission
    /// and starts <see cref="OverlayService"/> once it's granted.
    /// </summary>
    [Activity(Label = "Joint Position Debugger", MainLauncher = true, Exported = true)]
    public class MainActivity : Activity
    {
        private const int RequestOverlayPermission = 1001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var layout = new LinearLayout(this) { Orientation = LinearLayout.Orientation.Vertical };
            var status = new TextView(this) { Text = "Joint Position Debugger" };
            var hint = new TextView(this) { Text = "Permission required to draw over other apps." };
            var grant = new Button(this) { Text = "Grant overlay permission" };
            var launch = new Button(this) { Text = "Start overlay" };

            layout.AddView(status);
            layout.AddView(hint);
            layout.AddView(grant);
            layout.AddView(launch);
            SetContentView(layout);

            grant.Click += (s, e) => RequestOverlay();
            launch.Click += (s, e) => StartOverlay();

            if (Settings.CanDrawOverlays(this))
            {
                StartOverlay();
            }
            else
            {
                RequestOverlay();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (Settings.CanDrawOverlays(this))
            {
                StartOverlay();
            }
        }

        private void RequestOverlay()
        {
            try
            {
                var intent = new Intent(
                    Settings.ActionManageOverlayPermission,
                    Android.Net.Uri.Parse("package:" + PackageName));
                StartActivityForResult(intent, RequestOverlayPermission);
            }
            catch (Exception)
            {
                // Older OEM path: best-effort.
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode != RequestOverlayPermission) return;
            if (Settings.CanDrawOverlays(this))
            {
                StartOverlay();
            }
            else
            {
                Toast.MakeText(this, "Overlay permission denied — grant it in Settings.", ToastLength.Long).Show();
            }
        }

        private void StartOverlay()
        {
            if (!Settings.CanDrawOverlays(this)) return;
            var intent = new Intent(this, typeof(OverlayService));
            StartService(intent);
        }
    }
}
