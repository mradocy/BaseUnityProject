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
        /// Number of straight lines made when drawing a circle
        /// </summary>
        public const int CircleSteps = 20;

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

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="transform">Transforms the given circle.</param>
        /// <param name="center">The center (offset) of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public static void DrawCircle(Transform transform, Vector2 center, float radius) {
            Vector2[] circlePoints = new Vector2[CircleSteps];
            for (int i=0; i < CircleSteps; i++) {
                float angle = Mathf.PI * 2 * i / CircleSteps;
                circlePoints[i] = new Vector2(
                    center.x + Mathf.Cos(angle) * radius,
                    center.y + Mathf.Sin(angle) * radius);
            }

            Vector3 firstPt, pt = new Vector3(), prevPt;
            firstPt = transform.TransformPoint(circlePoints[0]);
            prevPt = firstPt;
            for (int i = 1; i < circlePoints.Length; i++) {
                pt = transform.TransformPoint(circlePoints[i]);
                Gizmos.DrawLine(prevPt, pt);
                prevPt = pt;
            }
            Gizmos.DrawLine(pt, firstPt);
        }

        /// <summary>
        /// Draws a capsule.
        /// </summary>
        /// <param name="transform">Transform to transform the capsule given.</param>
        /// <param name="center">The center (offset) of the capsule.</param>
        /// <param name="size">The size of the capsule.</param>
        /// <param name="direction">Direction of the capsule.</param>
        public static void DrawCapsule(Transform transform, Vector2 center, Vector2 size, CapsuleDirection2D direction) {

            if (direction == CapsuleDirection2D.Vertical) {
                float radius = size.x / 2;
                Vector2 topCenter = new Vector2(center.x, center.y + size.y / 2 - radius);
                Vector2 bottomCenter = new Vector2(center.x, center.y - size.y / 2 + radius);
                Vector2[] topCirclePoints = new Vector2[CircleSteps / 2 + 1];
                for (int i=0; i < CircleSteps / 2 + 1; i++) {
                    float angle = Mathf.PI * i / (CircleSteps / 2);
                    topCirclePoints[i] = new Vector2(
                        topCenter.x + Mathf.Cos(angle) * radius,
                        topCenter.y + Mathf.Sin(angle) * radius);
                }
                Vector2[] bottomCirclePoints = new Vector2[CircleSteps / 2 + 1];
                for (int i = 0; i < CircleSteps / 2 + 1; i++) {
                    float angle = Mathf.PI + Mathf.PI * i / (CircleSteps / 2);
                    bottomCirclePoints[i] = new Vector2(
                        bottomCenter.x + Mathf.Cos(angle) * radius,
                        bottomCenter.y + Mathf.Sin(angle) * radius);
                }

                Vector3 firstPt, pt, prevPt;
                firstPt = transform.TransformPoint(topCirclePoints[0]);
                prevPt = firstPt;
                for (int i = 1; i < topCirclePoints.Length; i++) {
                    pt = transform.TransformPoint(topCirclePoints[i]);
                    Gizmos.DrawLine(prevPt, pt);
                    prevPt = pt;
                }
                pt = transform.TransformPoint(bottomCirclePoints[0]);
                Gizmos.DrawLine(prevPt, pt);
                prevPt = pt;
                for (int i = 1; i < bottomCirclePoints.Length; i++) {
                    pt = transform.TransformPoint(bottomCirclePoints[i]);
                    Gizmos.DrawLine(prevPt, pt);
                    prevPt = pt;
                }
                Gizmos.DrawLine(prevPt, firstPt);

            } else if (direction == CapsuleDirection2D.Horizontal) {
                float radius = size.y / 2;
                Vector2 leftCenter = new Vector2(center.x - size.x / 2 + radius, center.y);
                Vector2 rightCenter = new Vector2(center.x + size.x / 2 - radius, center.y);
                Vector2[] leftCirclePoints = new Vector2[CircleSteps / 2 + 1];
                for (int i = 0; i < CircleSteps / 2 + 1; i++) {
                    float angle = Mathf.PI / 2 + Mathf.PI * i / (CircleSteps / 2);
                    leftCirclePoints[i] = new Vector2(
                        leftCenter.x + Mathf.Cos(angle) * radius,
                        leftCenter.y + Mathf.Sin(angle) * radius);
                }
                Vector2[] rightCirclePoints = new Vector2[CircleSteps / 2 + 1];
                for (int i = 0; i < CircleSteps / 2 + 1; i++) {
                    float angle = Mathf.PI * 3 / 2 + Mathf.PI * i / (CircleSteps / 2);
                    rightCirclePoints[i] = new Vector2(
                        rightCenter.x + Mathf.Cos(angle) * radius,
                        rightCenter.y + Mathf.Sin(angle) * radius);
                }

                Vector3 firstPt, pt, prevPt;
                firstPt = transform.TransformPoint(leftCirclePoints[0]);
                prevPt = firstPt;
                for (int i = 1; i < leftCirclePoints.Length; i++) {
                    pt = transform.TransformPoint(leftCirclePoints[i]);
                    Gizmos.DrawLine(prevPt, pt);
                    prevPt = pt;
                }
                pt = transform.TransformPoint(rightCirclePoints[0]);
                Gizmos.DrawLine(prevPt, pt);
                prevPt = pt;
                for (int i = 1; i < rightCirclePoints.Length; i++) {
                    pt = transform.TransformPoint(rightCirclePoints[i]);
                    Gizmos.DrawLine(prevPt, pt);
                    prevPt = pt;
                }
                Gizmos.DrawLine(prevPt, firstPt);

            }
        }


        /// <summary>
        /// Draws a capsule.
        /// </summary>
        /// <param name="center">The center of the capsule.</param>
        /// <param name="size">The size of the capsule.</param>
        /// <param name="direction">Direction of the capsule.</param>
        public static void DrawCapsule(Vector2 center, Vector2 size, CapsuleDirection2D direction) {

            if (direction == CapsuleDirection2D.Vertical) {
                float radius = size.x / 2;
                Vector2 topCenter = new Vector2(center.x, center.y + size.y / 2 - radius);
                Vector2 bottomCenter = new Vector2(center.x, center.y - size.y / 2 + radius);
                Vector2[] topCirclePoints = new Vector2[CircleSteps / 2 + 1];
                for (int i = 0; i < CircleSteps / 2 + 1; i++) {
                    float angle = Mathf.PI * i / (CircleSteps / 2);
                    topCirclePoints[i] = new Vector2(
                        topCenter.x + Mathf.Cos(angle) * radius,
                        topCenter.y + Mathf.Sin(angle) * radius);
                }
                Vector2[] bottomCirclePoints = new Vector2[CircleSteps / 2 + 1];
                for (int i = 0; i < CircleSteps / 2 + 1; i++) {
                    float angle = Mathf.PI + Mathf.PI * i / (CircleSteps / 2);
                    bottomCirclePoints[i] = new Vector2(
                        bottomCenter.x + Mathf.Cos(angle) * radius,
                        bottomCenter.y + Mathf.Sin(angle) * radius);
                }

                Vector3 firstPt, pt, prevPt;
                firstPt = topCirclePoints[0];
                prevPt = firstPt;
                for (int i = 1; i < topCirclePoints.Length; i++) {
                    pt = topCirclePoints[i];
                    Gizmos.DrawLine(prevPt, pt);
                    prevPt = pt;
                }
                pt = bottomCirclePoints[0];
                Gizmos.DrawLine(prevPt, pt);
                prevPt = pt;
                for (int i = 1; i < bottomCirclePoints.Length; i++) {
                    pt = bottomCirclePoints[i];
                    Gizmos.DrawLine(prevPt, pt);
                    prevPt = pt;
                }
                Gizmos.DrawLine(prevPt, firstPt);

            } else if (direction == CapsuleDirection2D.Horizontal) {
                float radius = size.y / 2;
                Vector2 leftCenter = new Vector2(center.x - size.x / 2 + radius, center.y);
                Vector2 rightCenter = new Vector2(center.x + size.x / 2 - radius, center.y);
                Vector2[] leftCirclePoints = new Vector2[CircleSteps / 2 + 1];
                for (int i = 0; i < CircleSteps / 2 + 1; i++) {
                    float angle = Mathf.PI / 2 + Mathf.PI * i / (CircleSteps / 2);
                    leftCirclePoints[i] = new Vector2(
                        leftCenter.x + Mathf.Cos(angle) * radius,
                        leftCenter.y + Mathf.Sin(angle) * radius);
                }
                Vector2[] rightCirclePoints = new Vector2[CircleSteps / 2 + 1];
                for (int i = 0; i < CircleSteps / 2 + 1; i++) {
                    float angle = Mathf.PI * 3 / 2 + Mathf.PI * i / (CircleSteps / 2);
                    rightCirclePoints[i] = new Vector2(
                        rightCenter.x + Mathf.Cos(angle) * radius,
                        rightCenter.y + Mathf.Sin(angle) * radius);
                }

                Vector3 firstPt, pt, prevPt;
                firstPt = leftCirclePoints[0];
                prevPt = firstPt;
                for (int i = 1; i < leftCirclePoints.Length; i++) {
                    pt = leftCirclePoints[i];
                    Gizmos.DrawLine(prevPt, pt);
                    prevPt = pt;
                }
                pt = rightCirclePoints[0];
                Gizmos.DrawLine(prevPt, pt);
                prevPt = pt;
                for (int i = 1; i < rightCirclePoints.Length; i++) {
                    pt = rightCirclePoints[i];
                    Gizmos.DrawLine(prevPt, pt);
                    prevPt = pt;
                }
                Gizmos.DrawLine(prevPt, firstPt);

            }
        }


    }
}