using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity {

    /// <summary>
    /// Static utilities class for drawing gizmos.
    /// </summary>
    public static class GizmoUtils {

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="transform">Transform to transform the rectangle given.</param>
        /// <param name="center">The center (offset) of the rectangle.</param>
        /// <param name="size">The size of the rectangle.</param>
        public static void DrawRectangle(Transform transform, Vector2 center, Vector2 size) {
            Vector3 bl = transform.TransformPoint(new Vector2(center.x - size.x / 2, center.y - size.y / 2));
            Vector3 br = transform.TransformPoint(new Vector2(center.x + size.x / 2, center.y - size.y / 2));
            Vector3 tl = transform.TransformPoint(new Vector2(center.x - size.x / 2, center.y + size.y / 2));
            Vector3 tr = transform.TransformPoint(new Vector2(center.x + size.x / 2, center.y + size.y / 2));

            Gizmos.DrawLine(bl, br);
            Gizmos.DrawLine(br, tr);
            Gizmos.DrawLine(tr, tl);
            Gizmos.DrawLine(tl, bl);
        }

    }
}