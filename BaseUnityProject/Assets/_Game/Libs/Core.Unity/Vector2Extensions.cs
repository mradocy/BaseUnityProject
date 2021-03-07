using Core.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Extensions for <see cref="Vector2"/>.
    /// </summary>
    public static class Vector2Extensions {

        /// <summary>
        /// Gets this Vector2 as an angle, in degrees.
        /// </summary>
        /// <param name="vector2">This Vector2</param>
        /// <returns>angle</returns>
        public static float ToAngle(this Vector2 vector2) {
            return Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Gets this Vector2 as an angle, in radians.
        /// </summary>
        /// <param name="vector2">This Vector2</param>
        /// <returns>angle</returns>
        public static float ToAngleRadians(this Vector2 vector2) {
            return Mathf.Atan2(vector2.y, vector2.x);
        }

        /// <summary>
        /// Returns the result of clamping the magnitude of this Vector2 to the given value.  Does nothing if the given magnitude is larger than the current magnitude.
        /// </summary>
        /// <param name="vector2">This Vector2</param>
        /// <param name="magnitude">The magnitude to clamp to.</param>
        public static Vector2 ClampMagnitude(this Vector2 vector2, float magnitude) {
            if (vector2.sqrMagnitude <= magnitude * magnitude || vector2.sqrMagnitude == 0)
                return vector2;

            vector2.Normalize();
            vector2.x *= magnitude;
            vector2.y *= magnitude;
            return vector2;
        }

        /// <summary>
        /// Gets this Vector2 as a Vector3 (setting z to 0).
        /// </summary>
        /// <param name="vector2">This Vector2</param>
        /// <returns>Vector3</returns>
        public static Vector3 AsVector3(this Vector2 vector2) {
            return new Vector3(vector2.x, vector2.y);
        }

        /// <summary>
        /// Gets if the values of this vector are approximately the values of the given vector.
        /// </summary>
        /// <param name="vector2">This Vector2</param>
        /// <param name="other">The vector to compare.</param>
        public static bool Approximately(this Vector2 vector2, Vector2 other) {
            return Mathf.Approximately(vector2.x, other.x) && Mathf.Approximately(vector2.y, other.y);
        }

    }
}