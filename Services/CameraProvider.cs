using JointDebugger.Math;

namespace JointDebugger.Services
{
    /// <summary>
    /// Hands out the current view-projection matrix each frame.
    /// <para>
    /// The current implementation returns a sensible identity-ish camera so
    /// the overlay is visually verifiable on first install. Swap in a real
    /// game-data source (memory read, file dump, in-process hook) by
    /// replacing <see cref="Acquire"/>.
    /// </para>
    /// </summary>
    public class CameraProvider
    {
        public CameraMatrix Acquire()
        {
            // TODO: replace with real game view-projection matrix read.
            return BuildSampleMatrix();
        }

        private static CameraMatrix BuildSampleMatrix()
        {
            // A simple right-handed perspective-style mapping that keeps the
            // sample humanoid on screen. Anything that produces a valid
            // column-major 4x4 will work for the W2S pipeline.
            var m = new CameraMatrix();
            m.M[0]  =  1.0f; m.M[1]  =  0.0f; m.M[2]  = 0.0f; m.M[3]  = 0.0f;
            m.M[4]  =  0.0f; m.M[5]  = -1.0f; m.M[6]  = 0.0f; m.M[7]  = 0.0f;
            m.M[8]  =  0.0f; m.M[9]  =  0.0f; m.M[10] = 1.0f; m.M[11] = 0.0f;
            m.M[12] =  0.0f; m.M[13] =  0.0f; m.M[14] = 0.0f; m.M[15] = 1.0f;
            return m;
        }
    }
}
