namespace JointDebugger.ImGuiGL
{
    /// <summary>
    /// Placeholder for future input plumbing. The overlay is FLAG_NOT_TOUCHABLE
    /// by design, so this intentionally does not forward Android MotionEvents
    /// into ImGui — interaction happens from the in-app menu (MainActivity).
    /// </summary>
    public static class InputBridge
    {
        public static void OnTouchDown(float x, float y)  { /* no-op */ }
        public static void OnTouchMove(float x, float y)  { /* no-op */ }
        public static void OnTouchUp()                     { /* no-op */ }
    }
}
