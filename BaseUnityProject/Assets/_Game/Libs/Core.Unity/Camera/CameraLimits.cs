using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Camera {

    /// <summary>
    /// Describes the limits of how far the game camera can move.
    /// </summary>
    [System.Serializable]
    public struct CameraLimits {

        /// <summary>
        /// Gets a <see cref="CameraLimits"/> where each value is the float min/max, essentially meaning no limit.
        /// </summary>
        public static CameraLimits NoLimit {
            get {
                return new CameraLimits(float.MinValue, float.MaxValue, float.MinValue, float.MaxValue);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="left">Left</param>
        /// <param name="right">Right</param>
        /// <param name="bottom">Bottom</param>
        /// <param name="top">Top</param>
        public CameraLimits(float left, float right, float bottom, float top) {
            this.Left = left;
            this.Right = right;
            this.Bottom = bottom;
            this.Top = top;
        }

        /// <summary>
        /// X of the left edge of the camera area (in meters).
        /// Set to <see cref="float.MinValue"/> for essentially no limit.
        /// </summary>
        public float Left { get; }

        /// <summary>
        /// X of the right edge of the camera area (in meters).
        /// Set to <see cref="float.MaxValue"/> for essentially no limit.
        /// </summary>
        public float Right { get; }

        /// <summary>
        /// Y of the bottom edge of the camera area (in meters).
        /// Set to <see cref="float.MinValue"/> for essentially no limit.
        /// </summary>
        public float Bottom { get; }

        /// <summary>
        /// Y of the top edge of the camera area (in meters).
        /// Set to <see cref="float.MaxValue"/> for essentially no limit.
        /// </summary>
        public float Top { get; }

        /// <summary>
        /// Clamps the given position to be within this camera limits, and returns the result.
        /// </summary>
        /// <param name="pos">Position to clamp.</param>
        /// <param name="camHalfWidth">Half width of the camera.  Is equal to <see cref="UnityEngine.Camera.orthographicSize"/> * <see cref="Screen.width"/> / <see cref="Screen.height"/></param>
        /// <param name="camHalfHeight">Half height of the camera.  Is equal to <see cref="UnityEngine.Camera.orthographicSize"/></param>
        /// <returns>Clamped position</returns>
        public Vector2 ClampPosition(Vector2 pos, float camHalfWidth, float camHalfHeight) {
            Vector2 v = new Vector2();
            float l = this.Left + camHalfWidth;
            float r = this.Right - camHalfWidth;
            if (l > r) {
                v.x = (l + r) / 2;
            } else {
                v.x = Mathf.Clamp(pos.x, l, r);
            }
            float b = this.Bottom + camHalfHeight;
            float t = this.Top - camHalfHeight;
            if (b > t) {
                v.y = (b + t) / 2;
            } else {
                v.y = Mathf.Clamp(pos.y, b, t);
            }

            return v;
        }

        /// <summary>
        /// Gets if this camera limits' values equal those of the given <paramref name="cameraLimits"/>.
        /// </summary>
        public bool Equals(CameraLimits cameraLimits) {
            return
                Mathf.Approximately(this.Left, cameraLimits.Left) &&
                Mathf.Approximately(this.Right, cameraLimits.Right) &&
                Mathf.Approximately(this.Top, cameraLimits.Top) &&
                Mathf.Approximately(this.Bottom, cameraLimits.Bottom);
        }
    }
}