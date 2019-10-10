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
        /// Clamps the magnitude of this Vector2 to the given value.  Does nothing if the given magnitude is larger than the current magnitude.
        /// </summary>
        /// <param name="vector2">This Vector2</param>
        /// <param name="magnitude">The magnitude to clamp to.</param>
        public static void ClampMagnitude(this Vector2 vector2, float magnitude) {
            if (vector2.sqrMagnitude <= magnitude * magnitude || vector2.sqrMagnitude == 0)
                return;

            vector2.Normalize();
            vector2.x *= magnitude;
            vector2.y *= magnitude;
        }

        /// <summary>
        /// Gets this Vector2 as a Vector3 (setting z to 0).
        /// </summary>
        /// <param name="vector2">This Vector2</param>
        /// <returns>Vector3</returns>
        public static Vector3 AsVector3(this Vector2 vector2) {
            return new Vector3(vector2.x, vector2.y);
        }

    }
}