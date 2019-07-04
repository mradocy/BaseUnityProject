using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Extensions for <see cref="Vector3"/>.
    /// </summary>
    public static class Vector3Extensions {

        /// <summary>
        /// Gets this Vector3 as a Vector2 (ignoring z coordinate).
        /// </summary>
        /// <param name="vector3">This Vector3</param>
        /// <returns>Vector2</returns>
        public static Vector2 AsVector2(this Vector3 vector3) {
            return new Vector2(vector3.x, vector3.y);
        }
    }
}