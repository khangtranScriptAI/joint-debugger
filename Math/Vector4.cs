using System;

namespace JointDebugger.Math
{
    /// <summary>
    /// Minimal 4D vector / RGBA color. Channels are in [0,1] range when used as a color.
    /// </summary>
    public struct Vector4 : IEquatable<Vector4>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vector4 Zero => new Vector4(0f, 0f, 0f, 0f);
        public static Vector4 One => new Vector4(1f, 1f, 1f, 1f);

        public bool Equals(Vector4 other) =>
            X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        public override bool Equals(object obj) => obj is Vector4 v && Equals(v);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);
        public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2}, {W:F2})";
    }
}
