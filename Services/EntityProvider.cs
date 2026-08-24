using JointDebugger.Data;
using JointDebugger.Math;

namespace JointDebugger.Services
{
    /// <summary>
    /// Hands out <see cref="Entity"/> instances each frame.
    /// <para>
    /// The current implementation returns a hard-coded sample so the overlay
    /// is visually verifiable on first install. Swap in a real game-data
    /// source (memory read, file dump, in-process hook) by replacing
    /// <see cref="Acquire"/>.
    /// </para>
    /// </summary>
    public class EntityProvider
    {
        public Entity Acquire()
        {
            // TODO: replace with real game memory read / hook.
            return BuildSampleEntity();
        }

        private static Entity BuildSampleEntity()
        {
            // Hard-coded humanoid skeleton in world space. Useful to verify the
            // W2S / swap / marker pipeline without a live game.
            var e = new Entity { Id = 1, Name = "sample" };

            e.Set(JointType.Head,          new Vector3( 0.0f,  1.80f,  0.0f));
            e.Set(JointType.Neck,          new Vector3( 0.0f,  1.55f,  0.0f));
            e.Set(JointType.Hip,           new Vector3( 0.0f,  0.95f,  0.0f));
            e.Set(JointType.Root,          new Vector3( 0.0f,  0.00f,  0.0f));

            e.Set(JointType.LeftShoulder,  new Vector3(-0.20f,  1.50f,  0.0f));
            e.Set(JointType.RightShoulder, new Vector3( 0.20f,  1.50f,  0.0f));
            e.Set(JointType.LeftElbow,     new Vector3(-0.45f,  1.20f,  0.0f));
            e.Set(JointType.RightElbow,    new Vector3( 0.45f,  1.20f,  0.0f));
            e.Set(JointType.LeftWrist,     new Vector3(-0.65f,  0.95f,  0.0f));
            e.Set(JointType.RightWrist,    new Vector3( 0.65f,  0.95f,  0.0f));
            e.Set(JointType.LeftHand,      new Vector3(-0.70f,  0.85f,  0.0f));
            e.Set(JointType.RightFoot,     new Vector3( 0.20f,  0.05f,  0.0f));
            e.Set(JointType.LeftFoot,      new Vector3(-0.20f,  0.05f,  0.0f));

            return e;
        }
    }
}
