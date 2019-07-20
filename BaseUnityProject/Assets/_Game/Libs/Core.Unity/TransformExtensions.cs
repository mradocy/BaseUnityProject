using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Extension methods for <see cref="Transform"/>.
    /// </summary>
    public static class TransformExtensions {

        /// <summary>
        /// Sets the x component of this transform's local scale.
        /// </summary>
        /// <param name="transform">This transform.</param>
        /// <param name="scaleX">Scale x to set.</param>
        public static void SetLocalScaleX(this Transform transform, float scaleX) {
            Vector3 ls = transform.localScale;
            ls.x = scaleX;
            transform.localScale = ls;
        }

        /// <summary>
        /// Gets the local 2D rotation of this transform, in degrees.
        /// </summary>
        /// <param name="transform">This transform.</param>
        /// <returns>Rotation</returns>
        public static float GetLocalRotation2D(this Transform transform) {
            return MathUtils.QuatToRot(transform.localRotation);
        }

        /// <summary>
        /// Sets the local 2D rotation of this transform.
        /// </summary>
        /// <param name="transform">This transform.</param>
        /// <param name="rotationDegrees">Rotation (degrees) to set.</param>
        public static void SetLocalRotation2D(this Transform transform, float rotationDegrees) {
            transform.localRotation = MathUtils.RotToQuat(rotationDegrees);
        }

    }
}