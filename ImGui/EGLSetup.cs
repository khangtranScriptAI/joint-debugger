using System;
using Android.Opengl;
using EGL10 = Javax.Microedition.Khronos.Egl.IEGL10;
using EGLDisplay = Javax.Microedition.Khronos.Egl.EGLDisplay;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;
using EGLContext = Javax.Microedition.Khronos.Egl.EGLContext;
using EGLSurface = Javax.Microedition.Khronos.Egl.EGLSurface;

namespace JointDebugger.ImGuiGL
{
    /// <summary>
    /// Minimal EGL + OpenGL ES 3.0 setup for an Android SurfaceView.
    /// The host (OverlayService) supplies the EGL10 display / config / context.
    /// </summary>
    public class EGLSetup
    {
        public EGL10 Egl { get; private set; }
        public EGLDisplay Display { get; private set; }
        public EGLConfig EglConfig { get; private set; }
        public EGLContext Context { get; private set; }
        public EGLSurface Surface { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Initialize(Java.Lang.Object surface)
        {
            var eglInstance = EGLContext.EGL;
            if (eglInstance == null) return false;
            Egl = (EGL10)eglInstance;
            if (Egl == null) return false;

            Display = Egl.EglGetDisplay(EGL10.EglDefaultDisplay);
            if (Display == null) return false;

            int[] version = new int[2];
            if (!Egl.EglInitialize(Display, version)) return false;

            int[] attribs =
            {
                (int)EGL10.EglRedSize, 8,
                (int)EGL10.EglGreenSize, 8,
                (int)EGL10.EglBlueSize, 8,
                (int)EGL10.EglAlphaSize, 8,
                (int)EGL10.EglDepthSize, 16,
                (int)EGL10.EglStencilSize, 8,
                (int)EGL10.EglRenderableType, 4 /* EGL_OPENGL_ES2_BIT */,
                (int)EGL10.EglNone
            };
            EGLConfig[] configs = new EGLConfig[1];
            int[] numConfigs = new int[1];
            if (!Egl.EglChooseConfig(Display, attribs, configs, 1, numConfigs)) return false;
            EglConfig = configs[0];

            int[] ctxAttribs =
            {
                0x3098 /* EGL_CONTEXT_CLIENT_VERSION */, 3,
                (int)EGL10.EglNone
            };
            Context = Egl.EglCreateContext(Display, EglConfig, EGL10.EglNoContext, ctxAttribs);
            if (Context == null) return false;

            Surface = Egl.EglCreateWindowSurface(Display, EglConfig, surface, null);
            if (Surface == null) return false;

            if (!Egl.EglMakeCurrent(Display, Surface, Surface, Context)) return false;

            int[] qWidth  = new int[1];
            int[] qHeight = new int[1];
            Egl.EglQuerySurface(Display, Surface, (int)EGL10.EglWidth,  qWidth);
            Egl.EglQuerySurface(Display, Surface, (int)EGL10.EglHeight, qHeight);
            Width  = qWidth[0];
            Height = qHeight[0];

            GLES30.GlViewport(0, 0, Width, Height);
            return true;
        }

        public bool SwapBuffers() =>
            Egl != null && Surface != null && Egl.EglSwapBuffers(Display, Surface);

        public void Destroy()
        {
            if (Egl == null) return;
            if (Surface != null) Egl.EglMakeCurrent(Display, EGL10.EglNoSurface, EGL10.EglNoSurface, EGL10.EglNoContext);
            if (Surface != null) { Egl.EglDestroySurface(Display, Surface); Surface = null; }
            if (Context != null) { Egl.EglDestroyContext(Display, Context); Context = null; }
        }

        public void Clear(float r, float g, float b, float a)
        {
            GLES30.GlClearColor(r, g, b, a);
            GLES30.GlClear((int)GLES30.GlColorBufferBit);
        }
    }
}
