using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using JointDebugger.Config;
using JointDebugger.ImGuiGL;
using JointDebugger.Rendering;

namespace JointDebugger.Services
{
    /// <summary>
    /// Foreground service that owns the SurfaceView, the EGL context, the ImGui
    /// controller, and the per-frame render loop.
    /// <para>
    /// The overlay is <b>strictly</b> visualization. The surface is set up
    /// with <c>FLAG_NOT_FOCUSABLE | FLAG_NOT_TOUCHABLE</c> so taps fall
    /// through to whatever app is below.
    /// </para>
    /// </summary>
    [Service(Label = "Joint Position Debugger", Exported = false)]
    public class OverlayService : Service
    {
        private IWindowManager _windowManager;
        private SurfaceView _surfaceView;
        private WindowManagerLayoutParams _layoutParams;
        private OverlayRenderThread _renderThread;

        public override void OnCreate()
        {
            base.OnCreate();
            _windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            BuildSurface();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _renderThread ??= new OverlayRenderThread(this, _surfaceView.Holder, new Settings());
            if (!_renderThread.IsAlive) _renderThread.Start();
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            _renderThread?.Stop();
            _renderThread?.Join(500);
            _renderThread = null;

            if (_surfaceView != null && _windowManager != null)
            {
                try { _windowManager.RemoveView(_surfaceView); } catch { /* ignored */ }
            }
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent) => null;

        private void BuildSurface()
        {
            _surfaceView = new SurfaceView(this);

            _layoutParams = new WindowManagerLayoutParams(
                WindowManagerLayoutParams.MatchParent,
                WindowManagerLayoutParams.MatchParent,
                Android.Views.WindowManagerType.ApplicationOverlay,
                WindowManagerFlags.NotFocusable
                | WindowManagerFlags.NotTouchable
                | WindowManagerFlags.LayoutNoLimits
                | WindowManagerFlags.WatchOutsideTouch,
                Format.Translucent);

            _layoutParams.Gravity = GravityFlags.Top | GravityFlags.Start;
            _windowManager.AddView(_surfaceView, _layoutParams);
        }
    }

    /// <summary>
    /// Per-frame render thread. Owns the EGL context, the ImGui controller, and
    /// the overlay renderer.
    /// </summary>
    internal class OverlayRenderThread : Java.Lang.Thread
    {
        private readonly Context _ctx;
        private readonly ISurfaceHolder _holder;
        private readonly Settings _settings;
        private readonly OverlayRenderer _renderer;
        private readonly EntityProvider _entityProvider = new EntityProvider();
        private readonly CameraProvider _cameraProvider = new CameraProvider();

        private volatile bool _running = true;

        public OverlayRenderThread(Context ctx, ISurfaceHolder holder, Settings settings)
        {
            _ctx = ctx;
            _holder = holder;
            _settings = settings;
            _renderer = new OverlayRenderer(_settings);
        }

        public new void Stop() => _running = false;

        public override void Run()
        {
            var egl = new EGLSetup();
            var controller = new ImGuiController();
            int width = 1, height = 1;

            try
            {
                while (_running)
                {
                    var surface = _holder.Surface;
                    if (surface == null || !surface.IsValid)
                    {
                        Thread.Sleep(8);
                        continue;
                    }

                    if (width <= 1 || height <= 1)
                    {
                        if (!egl.Initialize(surface))
                        {
                            Thread.Sleep(16);
                            continue;
                        }
                        width = egl.Width;
                        height = egl.Height;
                        controller.Initialize(width, height, 1f);
                    }

                    egl.Clear(0f, 0f, 0f, 0f);

                    controller.NewFrame();
                    var entity = _entityProvider.Acquire();
                    var camera = _cameraProvider.Acquire();
                    _renderer.Render(entity, camera, width, height);
                    controller.Render();

                    egl.SwapBuffers();

                    // 60 fps cap.
                    Thread.Sleep(16);
                }
            }
            catch (Exception)
            {
                // Surface destroyed / context lost — clean up and exit.
            }
            finally
            {
                controller.Dispose();
                egl.Destroy();
            }
        }
    }
}
