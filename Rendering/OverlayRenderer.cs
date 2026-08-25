using System.Collections.Generic;
using ImGuiNET;
using JointDebugger.Config;
using JointDebugger.Data;
using JointDebugger.Math;

namespace JointDebugger.Rendering
{
    /// <summary>
    /// Per-frame ImGui rendering: config window + debug joint table + joint
    /// markers via <see cref="DrawSkeleton"/>.
    /// </summary>
    public class OverlayRenderer
    {
        private readonly Settings _settings;
        private Dictionary<JointType, Vector2> _lastPositions = new Dictionary<JointType, Vector2>();

        public OverlayRenderer(Settings settings)
        {
            _settings = settings;
        }

        public void Render(Entity entity, CameraMatrix camera, int screenW, int screenH)
        {
            DrawConfigWindow();
            _lastPositions = DrawSkeleton.Draw(entity, camera, screenW, screenH, _settings);
            if (_settings.DebugJointPosition) DrawDebugTableWindow();
        }

        private void DrawConfigWindow()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(360, 0), ImGuiCond.Once);
            if (ImGui.Begin("Joint Position Debugger"))
            {
                bool showMarkers = _settings.ShowJointMarkers;
                if (ImGui.Checkbox("Joint Markers ON/OFF", ref showMarkers))
                    _settings.ShowJointMarkers = showMarkers;

                bool showLabels = _settings.ShowBoneLabels;
                if (ImGui.Checkbox("Joint Labels ON/OFF", ref showLabels))
                    _settings.ShowBoneLabels = showLabels;

                float markerRadius = _settings.MarkerRadius;
                if (ImGui.SliderFloat("Marker Radius", ref markerRadius, 1.0f, 8.0f))
                    _settings.MarkerRadius = markerRadius;

                float outlineRadius = _settings.OutlineRadius;
                if (ImGui.SliderFloat("Outline Radius", ref outlineRadius, 2.0f, 12.0f))
                    _settings.OutlineRadius = outlineRadius;

                var markerRgb = new System.Numerics.Vector4(
                    _settings.MarkerColor.X,
                    _settings.MarkerColor.Y,
                    _settings.MarkerColor.Z,
                    _settings.MarkerColor.W);
                if (ImGui.ColorEdit4("Marker Color", ref markerRgb))
                {
                    _settings.MarkerColor = new Vector4(markerRgb.X, markerRgb.Y, markerRgb.Z, markerRgb.W);
                }

                float outlineAlpha = _settings.OutlineAlpha;
                if (ImGui.SliderFloat("Outline Alpha", ref outlineAlpha, 0.0f, 1.0f))
                    _settings.OutlineAlpha = outlineAlpha;

                float labelAlpha = _settings.LabelAlpha;
                if (ImGui.SliderFloat("Label Alpha", ref labelAlpha, 0.0f, 1.0f))
                    _settings.LabelAlpha = labelAlpha;

                bool debug = _settings.DebugJointPosition;
                if (ImGui.Checkbox("Debug Joint Position", ref debug))
                    _settings.DebugJointPosition = debug;

                ImGui.Separator();
                if (ImGui.Button("Reset Settings"))
                {
                    _settings.Reset();
                }
            }
            ImGui.End();
        }

        private void DrawDebugTableWindow()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(360, 0), ImGuiCond.Once);
            if (ImGui.Begin("Debug — Joint Positions"))
            {
                if (ImGui.BeginTable("joints", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn("Joint");
                    ImGui.TableSetupColumn("Screen X");
                    ImGui.TableSetupColumn("Screen Y");
                    ImGui.TableSetupColumn("Abbr");
                    ImGui.TableHeadersRow();

                    foreach (var joint in DrawSkeleton.AllJoints)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text(joint.ToString());

                        ImGui.TableSetColumnIndex(1);
                        if (_lastPositions.TryGetValue(joint, out var p))
                            ImGui.Text($"{p.X:F0}");
                        else
                            ImGui.TextDisabled("--");

                        ImGui.TableSetColumnIndex(2);
                        if (_lastPositions.TryGetValue(joint, out var p2))
                            ImGui.Text($"{p2.Y:F0}");
                        else
                            ImGui.TextDisabled("--");

                        ImGui.TableSetColumnIndex(3);
                        ImGui.Text(DrawSkeleton.Abbreviations.TryGetValue(joint, out var ab) ? ab : "--");
                    }
                    ImGui.EndTable();
                }
            }
            ImGui.End();
        }
    }
}
