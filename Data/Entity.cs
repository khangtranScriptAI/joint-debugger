using System.Collections.Generic;
using JointDebugger.Math;

namespace JointDebugger.Data
{
    /// <summary>
    /// A skeletal entity. Holds world-space positions for the joints the
    /// debugger knows about. Missing keys are simply skipped during draw.
    /// </summary>
    public class Entity
    {
        public uint Id { get; set; }
        public string Name { get; set; } = "entity";
        public Dictionary<JointType, Vector3> Joints { get; } = new Dictionary<JointType, Vector3>();

        public bool TryGet(JointType joint, out Vector3 pos) => Joints.TryGetValue(joint, out pos);

        public void Set(JointType joint, Vector3 pos) => Joints[joint] = pos;
    }
}
