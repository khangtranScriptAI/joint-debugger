using JointDebugger.Math;

namespace JointDebugger.Config
{
    /// <summary>
    /// User-tunable knobs for the joint debugger. Lives at process scope so
    /// both the UI window and the renderer read the same instance.
    /// </summary>
    public class Settings
    {
        public bool ShowJointMarkers { get; set; } = true;
        public bool ShowBoneLabels { get; set; } = true;
        public float MarkerRadius { get; set; } = 3.5f;
        public float OutlineRadius { get; set; } = 4.5f;

        /// <summary>LimeGreen by default. RGB channels in [0,1].</summary>
        public Vector4 MarkerColor { get; set; } = new Vector4(0.2f, 1.0f, 0.2f, 1.0f);

        /// <summary>Outline alpha in [0,1]. Default 80/255.</summary>
        public float OutlineAlpha { get; set; } = 80f / 255f;

        /// <summary>Label alpha in [0,1]. Default 200/255.</summary>
        public float LabelAlpha { get; set; } = 200f / 255f;

        public bool DebugJointPosition { get; set; } = true;

        /// <summary>Restore factory defaults.</summary>
        public void Reset()
        {
            ShowJointMarkers = true;
            ShowBoneLabels = true;
            MarkerRadius = 3.5f;
            OutlineRadius = 4.5f;
            MarkerColor = new Vector4(0.2f, 1.0f, 0.2f, 1.0f);
            OutlineAlpha = 80f / 255f;
            LabelAlpha = 200f / 255f;
            DebugJointPosition = true;
        }
    }
}
