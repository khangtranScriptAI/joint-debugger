using System.Collections.Generic;
using ImGuiNET;
using JointDebugger.Config;
using JointDebugger.Data;
using JointDebugger.Math;
using JointDebugger.Util;

namespace JointDebugger.Rendering
{
    /// <summary>
    /// Per-frame ImGui rendering: config window + debug joint table + joint
    /// markers via <see cref="DrawSkeleton"/>.
    /// </summary>
    public class OverlayRenderer
    {
        private readonly Config _config;
        private Dictionary<JointType, Vector2> _lastPositions = new Dictionary<JointType, Vector2>();

        public OverlayRenderer(Config config)
        {
            _config = config;
        }

        public void Render(Entity entity, CameraMatrix camera, int screenW, int screenH)
        {
            DrawConfigWindow();
            _lastPositions = DrawSkeleton.Draw(entity, camera, screenW, screenH, _config);
            if (_config.DebugJointPosition) DrawDebugTableWindow();
        }

        private void DrawConfigWindow()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(360, 0), ImGuiCond.Once);
            if (ImGui.Begin("Joint Position Debugger"))
            {
                ImGui.Checkbox("Joint Markers ON/OFF",  ref _config.ShowJointMarkers);
                ImGui.Checkbox("Joint Labels ON/OFF",    ref _config.ShowBoneLabels);

                ImGui.SliderFloat("Marker Radius",  ref _config.MarkerRadius,  1.0f, 8.0f);
                ImGui.SliderFloat("Outline Radius", ref _config.OutlineRadius, 2.0f, 12.0f);

                var markerRgb = new System.Numerics.Vector4(
                    _config.MarkerColor.X,
                    _config.MarkerColor.Y,
                    _config.MarkerColor.Z,
                    _config.MarkerColor.W);
                if (ImGui.ColorEdit4("Marker Color", ref markerRgb))
                {
                    _config.MarkerColor = new Vector4(markerRgb.X, markerRgb.Y, markerRgb.Z, markerRgb.W);
                }

                ImGui.SliderFloat("Outline Alpha", ref _config.OutlineAlpha, 0.0f, 1.0f);
                ImGui.SliderFloat("Label Alpha",   ref _config.LabelAlpha,   0.0f, 1.0f);

                ImGui.Checkbox("Debug Joint Position", ref _config.DebugJointPosition);

                ImGui.Separator();
                if (ImGui.Button("Reset Settings"))
                {
                    _config.Reset();
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
