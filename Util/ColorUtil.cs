using JointDebugger.Math;

namespace JointDebugger.Util
{
    /// <summary>
    /// Color conversion helpers. ImGui stores colors as 32-bit unsigned ints
    /// in IM_COL32 layout: A in the high byte, B in the next, G, then R.
    /// </summary>
    public static class ColorUtil
    {
        /// <summary>
        /// Pack an RGBA color (channels in [0,1]) into ImGui's IM_COL32 uint layout.
        /// </summary>
        public static uint ColorToUint32(Vector4 c)
        {
            uint r = (uint)(Clamp01(c.X) * 255f) & 0xFFu;
            uint g = (uint)(Clamp01(c.Y) * 255f) & 0xFFu;
            uint b = (uint)(Clamp01(c.Z) * 255f) & 0xFFu;
            uint a = (uint)(Clamp01(c.W) * 255f) & 0xFFu;
            return r | (g << 8) | (b << 16) | (a << 24);
        }

        /// <summary>Convenience: build a uint from RGB+A.</summary>
        public static uint ColorToUint32(float r, float g, float b, float a) =>
            ColorToUint32(new Vector4(r, g, b, a));

        private static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}
