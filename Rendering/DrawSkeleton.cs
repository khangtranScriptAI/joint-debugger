using System.Collections.Generic;
using ImGuiNET;
using JointDebugger.Config;
using JointDebugger.Data;
using JointDebugger.Math;
using JointDebugger.Util;

namespace JointDebugger.Rendering
{
    /// <summary>
    /// Renders the joint markers for a single <see cref="Entity"/>.
    /// <para>
    /// STRICT RULES (per user spec):
    ///  - Only filled circles + optional short text labels are drawn.
    ///  - NO lines, NO bones, NO skeleton, NO bounding boxes, NO connectors.
    ///  - Only joints whose screen coordinates satisfy X &gt; 0 &amp;&amp; Y &gt; 0
    ///    are drawn.
    /// </para>
    /// <para>
    /// Flow:
    ///   Entity → Joint 3D → W2S → Validation → Swap Left/Right → Vector2[] joints
    ///   → screen-space check → Joint Marker → optional Label.
    /// </para>
    /// </summary>
    public static class DrawSkeleton
    {
        /// <summary>Master joint list — the user spec is explicit about this set.</summary>
        public static readonly JointType[] AllJoints =
        {
            JointType.Head,
            JointType.Neck,
            JointType.LeftWrist,
            JointType.Hip,
            JointType.Root,
            JointType.RightFoot,
            JointType.LeftFoot,
            JointType.RightWrist,
            JointType.LeftHand,
            JointType.LeftShoulder,
            JointType.RightShoulder,
            JointType.LeftElbow,
            JointType.RightElbow,
        };

        /// <summary>Short labels used next to each marker when ShowBoneLabels is on.</summary>
        public static readonly Dictionary<JointType, string> Abbreviations = new Dictionary<JointType, string>
        {
            { JointType.Head,          "HD" },
            { JointType.Neck,          "NK" },
            { JointType.Hip,           "HP" },
            { JointType.Root,          "RT" },
            { JointType.LeftShoulder,  "LS" },
            { JointType.RightShoulder, "RS" },
            { JointType.LeftElbow,     "LE" },
            { JointType.RightElbow,    "RE" },
            { JointType.LeftWrist,     "LW" },
            { JointType.RightWrist,    "RW" },
            { JointType.LeftHand,      "LH" },
            { JointType.RightFoot,     "RF" },
            { JointType.LeftFoot,      "LF" },
        };

        /// <summary>
        /// Draw joint markers for an entity. Returns the screen-space positions
        /// (so a debug table can list them in the UI).
        /// </summary>
        public static Dictionary<JointType, Vector2> Draw(
            Entity entity,
            CameraMatrix camera,
            int screenW,
            int screenH,
            Settings config)
        {
            var screenPositions = new Dictionary<JointType, Vector2>();

            if (entity == null || camera == null || config == null) return screenPositions;
            if (!config.ShowJointMarkers) return screenPositions;

            // ---- Step 1: project every joint, drop nulls / off-screen ----
            foreach (var joint in AllJoints)
            {
                if (!entity.TryGet(joint, out var worldPos)) continue;

                var sp = W2S.WorldToScreen(camera, worldPos, screenW, screenH);
                if (!sp.HasValue) continue;

                // User spec: only draw when strictly inside the screen.
                if (sp.Value.X <= 0f || sp.Value.Y <= 0f) continue;

                screenPositions[joint] = sp.Value;
            }

            // ---- Step 2: left/right swap (facing-away heuristic) ----
            // Per spec: if Left.X > Right.X AND both > 0, swap. We do this for
            // shoulder / elbow / wrist. The character is mirrored visually, so the
            // data labels follow what the camera actually sees.
            TrySwap(screenPositions, JointType.LeftShoulder,  JointType.RightShoulder);
            TrySwap(screenPositions, JointType.LeftElbow,     JointType.RightElbow);
            TrySwap(screenPositions, JointType.LeftWrist,     JointType.RightWrist);

            // ---- Step 3: draw ----
            // Foreground draw list so markers sit on top of any in-game content.
            var drawList = ImGui.GetForegroundDrawList();

            uint markerColor = ColorUtil.ColorToUint32(
                config.MarkerColor.X,
                config.MarkerColor.Y,
                config.MarkerColor.Z,
                1.0f);

            // Outline keeps the marker's RGB but uses the user-tweakable alpha.
            uint outlineColor = ColorUtil.ColorToUint32(
                config.MarkerColor.X,
                config.MarkerColor.Y,
                config.MarkerColor.Z,
                config.OutlineAlpha);

            uint labelColor = ColorUtil.ColorToUint32(1f, 1f, 1f, config.LabelAlpha);

            // 16 segments is plenty for a small circle and stays cheap to draw.
            const int segments = 16;

            // Draw outline first so the brighter inner marker sits cleanly on top.
            foreach (var kv in screenPositions)
            {
                var p = kv.Value;
                drawList.AddCircleFilled(p, config.OutlineRadius, outlineColor, segments);
            }

            foreach (var kv in screenPositions)
            {
                var p = kv.Value;
                drawList.AddCircleFilled(p, config.MarkerRadius, markerColor, segments);
            }

            if (config.ShowBoneLabels)
            {
                // Small offset so the text doesn't sit on top of the dot.
                var labelOffset = new Vector2(6f, -6f);
                foreach (var kv in screenPositions)
                {
                    if (!Abbreviations.TryGetValue(kv.Key, out var label)) continue;
                    var labelPos = kv.Value + labelOffset;
                    drawList.AddText(labelPos, labelColor, label);
                }
            }

            return screenPositions;
        }

        /// <summary>
        /// Swap the screen positions of two joints when the heuristic says the
        /// model is facing away from the camera. Both X coordinates must be &gt; 0.
        /// </summary>
        private static void TrySwap(
            Dictionary<JointType, Vector2> map,
            JointType left,
            JointType right)
        {
            if (!map.TryGetValue(left, out var lp)) return;
            if (!map.TryGetValue(right, out var rp)) return;
            if (lp.X <= 0f || rp.X <= 0f) return;
            if (lp.X <= rp.X) return;

            map[left]  = rp;
            map[right] = lp;
        }
    }
}
