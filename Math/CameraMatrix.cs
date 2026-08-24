namespace JointDebugger.Math
{
    /// <summary>
    /// Column-major 4x4 view-projection matrix. Layout matches OpenGL / Unity / most engines.
    /// <para>
    /// Stored as 16 floats in column-major order:
    /// m[0..3]  = column 0
    /// m[4..7]  = column 1
    /// m[8..11] = column 2
    /// m[12..15]= column 3
    /// </para>
    /// </summary>
    public class CameraMatrix
    {
        public readonly float[] M = new float[16];

        public static CameraMatrix Identity => new CameraMatrix
        {
            M = {
                1f, 0f, 0f, 0f,
                0f, 1f, 0f, 0f,
                0f, 0f, 1f, 0f,
                0f, 0f, 0f, 1f
            }
        };

        /// <summary>
        /// Transform a 3D point as a homogeneous (x,y,z,1) vector.
        /// </summary>
        public Vector3 MultiplyPoint(Vector3 v)
        {
            // result = M * (x, y, z, 1)
            float x = M[0] * v.X + M[4] * v.Y + M[8]  * v.Z + M[12];
            float y = M[1] * v.X + M[5] * v.Y + M[9]  * v.Z + M[13];
            float z = M[2] * v.X + M[6] * v.Y + M[10] * v.Z + M[14];
            return new Vector3(x, y, z);
        }
    }
}
