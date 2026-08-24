using System;
using ImGuiNET;
using JointDebugger.Math;

namespace JointDebugger.ImGuiGL
{
    /// <summary>
    /// Owns the ImGui context, font atlas, and the per-frame NewFrame / Render
    /// wiring. The Android side calls <see cref="NewFrame"/> and
    /// <see cref="Render"/> from the render loop.
    /// </summary>
    public class ImGuiController : IDisposable
    {
        public IntPtr Context { get; private set; }

        public void Initialize(int width, int height, float displayScale = 1f)
        {
            Context = ImGui.CreateContext();
            ImGui.SetCurrentContext(Context);

            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(width, height);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(displayScale, displayScale);
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags |= ImGuiConfigFlags.DpiEnableScaleViewports;

            // Touch / mouse / kbd inputs are deliberately NOT wired up here:
            // the overlay window is FLAG_NOT_TOUCHABLE, so the user can keep
            // interacting with the app underneath unimpeded. The config window
            // is read-only / toggle-only from the in-app menu activity.
        }

        public void Resize(int width, int height)
        {
            if (Context == IntPtr.Zero) return;
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(width, height);
        }

        public void NewFrame()
        {
            ImGui.NewFrame();
        }

        public void Render()
        {
            ImGui.Render();
            // The actual GL draw is done by ImGui's render handler (set up
            // by the Android binding at startup); the OpenTK / ImGui.NET
            // packages do this internally. We just need to call Render() and
            // then SwapBuffers.
        }

        public void Dispose()
        {
            if (Context != IntPtr.Zero)
            {
                ImGui.DestroyContext(Context);
                Context = IntPtr.Zero;
            }
        }
    }
}
