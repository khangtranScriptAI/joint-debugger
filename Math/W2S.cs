namespace JointDebugger.Math
{
    /// <summary>
    /// World-to-Screen projection helpers.
    /// </summary>
    public static class W2S
    {
        /// <summary>
        /// Project a world-space point through a view-projection camera matrix and
        /// convert the result to screen-space pixel coordinates (top-left origin).
        /// </summary>
        /// <param name="camera">Column-major 4x4 view-projection matrix.</param>
        /// <param name="worldPos">World-space joint position.</param>
        /// <param name="screenWidth">Render-target width in pixels.</param>
        /// <param name="screenHeight">Render-target height in pixels.</param>
        /// <returns>
        /// Screen-space position in pixels, or <c>null</c> if the point is behind /
        /// on the camera plane (clip.w &lt;= 0) or fully off-screen.
        /// </returns>
        public static Vector2? WorldToScreen(
            CameraMatrix camera,
            Vector3 worldPos,
            int screenWidth,
            int screenHeight)
        {
            if (camera == null) return null;

            // Step 1: clip-space transform. column-major: clip.xyz = M * (x,y,z,1).
            // We re-derive clip.w as the homogeneous divisor.
            float cx = camera.M[0] * worldPos.X + camera.M[4] * worldPos.Y +
                       camera.M[8]  * worldPos.Z + camera.M[12];
            float cy = camera.M[1] * worldPos.X + camera.M[5] * worldPos.Y +
                       camera.M[9]  * worldPos.Z + camera.M[13];
            float cz = camera.M[2] * worldPos.X + camera.M[6] * worldPos.Y +
                       camera.M[10] * worldPos.Z + camera.M[14];
            float cw = camera.M[3] * worldPos.X + camera.M[7] * worldPos.Y +
                       camera.M[11] * worldPos.Z + camera.M[15];

            // Behind the camera or on the near plane.
            if (cw <= 0.0001f) return null;

            // Step 2: NDC ([-1, 1] x [-1, 1])
            float ndcX = cx / cw;
            float ndcY = cy / cw;

            // Step 3: viewport map. ImGui draw lists use top-left origin, so flip Y.
            float screenX = (ndcX * 0.5f + 0.5f) * screenWidth;
            float screenY = (1.0f - (ndcY * 0.5f + 0.5f)) * screenHeight;

            return new Vector2(screenX, screenY);
        }
    }
}
