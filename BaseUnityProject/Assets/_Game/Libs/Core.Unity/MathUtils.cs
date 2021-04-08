using UnityEngine;
using System.Collections;

namespace Core.Unity {

    /// <summary>
    /// Math class with generic static functions.
    /// </summary>
    public static class MathUtils {

        /// <summary>
        /// Epsilon value used for floating point comparisons.
        /// </summary>
        public const float EPSILON = .0001f;

        /// <summary>
        /// The square root of 2.
        /// </summary>
        public const float Sqrt2 = 1.414213562373095f;

        /// <summary>
        /// Calculates the "floor mod", i.e. a mod b where the result has the same sign as b.
        /// </summary>
        /// <param name="a">The dividend.</param>
        /// <param name="b">The divisor.</param>
        public static float FMod(float a, float b) {
            return a - Mathf.Floor(a / b) * b;
        }

        /// <summary>
        /// Literally just the classic Atan2 function, but takes a Vector2 as an argument.  Returns angle in radians.
        /// </summary>
        /// <param name="v"></param>
        public static float Atan2(Vector2 v) {
            return Mathf.Atan2(v.y, v.x);
        }

        /// <summary>
        /// Wraps a value in [0, 2*PI)
        /// </summary>
        /// <param name="angleRadians">Value to wrap.</param>
        public static float Wrap2PI(float angleRadians) {
            return FMod(angleRadians, Mathf.PI * 2);
        }

        /// <summary>
        /// Wraps a value in [-PI, PI)
        /// </summary>
        /// <param name="angleRadians">Value to wrap.</param>
        public static float WrapPI(float angleRadians) {
            return FMod(angleRadians + Mathf.PI, Mathf.PI * 2) - Mathf.PI;
        }

        /// <summary>
        /// Wraps a value in [0, 360)
        /// </summary>
        /// <param name="angleDegrees">Value to wrap.</param>
        public static float Wrap360(float angleDegrees) {
            return FMod(angleDegrees, 360);
        }

        /// <summary>
        /// Wraps a value in [-180, 180)
        /// </summary>
        /// <param name="angleDegrees">Value to wrap.</param>
        public static float Wrap180(float angleDegrees) {
            return FMod(angleDegrees + 180, 360) - 180;
        }

        /// <summary>
        /// Returns the value (in radians) to be added to angleStart to reach angleEnd.  This value is wrapped in [-PI, PI)
        /// </summary>
        /// <param name="angleStartRadians">The starting angle, in radians.</param>
        /// <param name="angleEndRadians">The finishing angle, in radians.</param>
        public static float AngleDiffRadians(float angleStartRadians, float angleEndRadians) {
            return WrapPI(angleEndRadians - angleStartRadians);
        }

        /// <summary>
        /// Returns the value (in degrees) to be added to angleStart to reach angleEnd.  This value is wrapped in [-180, 180)
        /// </summary>
        /// <param name="angleStartDegrees">The starting angle, in degrees.</param>
        /// <param name="angleEndDegrees">The finishing angle, in degrees.</param>
        public static float AngleDiffDegrees(float angleStartDegrees, float angleEndDegrees) {
            return Wrap180(angleEndDegrees - angleStartDegrees);
        }

        /// <summary>
        /// Rounds the given value to the given place.  e.g. RoundTo(1.23, 0.1) = 1.2
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <param name="place">The place to round to.</param>
        public static float RoundTo(float value, float place) {
            return Mathf.Round(value / place) * place;
        }

        /// <summary>
        /// Gets rotation along the z axis from the given quaternion, in degrees.  Range is [0, 360).
        /// </summary>
        /// <param name="quaternion">Quaterion to extract z rotation from.</param>
        public static float QuatToRot(Quaternion quaternion) {
            // when setting quaternion to 180 degrees, changes y instead of z?  not sure what's going on
            if (Mathf.Abs(quaternion.eulerAngles.z) < .001f && Mathf.Abs(quaternion.eulerAngles.y) > .0001f)
                return quaternion.eulerAngles.y;
            return quaternion.eulerAngles.z;
        }

        /// <summary>
        /// Gets Quaternion from rotation along the z axis (in degrees).
        /// </summary>
        /// <param name="rotationZ">z rotation in degrees.</param>
        public static Quaternion RotToQuat(float rotationZ) {
            return Quaternion.Euler(new Vector3(0, 0, rotationZ));
        }

        /// <summary>
        /// Gets the point along a bezier curve with 3 control points.
        /// </summary>
        /// <param name="p0">Start point.</param>
        /// <param name="p1">Anchor point.</param>
        /// <param name="p2">End point.</param>
        /// <param name="t">Time, in [0, 1]</param>
        /// <returns>Point</returns>
        /// <remarks>https://javascript.info/bezier-curve#maths</remarks>
        public static Vector2 BezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
            return
                (1 - t) * (1 - t) * p0 +
                2 * (1 - t) * t * p1 +
                t * t * p2;
        }

        /// <summary>
        /// Gets the normal vector (facing the "left") along a bezier curve with 3 control points.
        /// </summary>
        /// <param name="p0">Start point.</param>
        /// <param name="p1">Anchor point.</param>
        /// <param name="p2">End point.</param>
        /// <param name="t">Time, in [0, 1]</param>
        /// <returns>Normal</returns>
        public static Vector2 BezierNormal(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
            Vector2 deriv = p0 * (2 * t - 2) + (2 * p2 - 4 * p1) * t + 2 * p1;
            Vector2 normal = new Vector2(-deriv.y, deriv.x);
            return normal.normalized;
        }

        /// <summary>
        /// Given a sector defined by a center angle and spread, return if the given test angle is contained in the sector (all in degrees).
        /// </summary>
        /// <param name="sectorAngle">Angle defining the center of the sector.</param>
        /// <param name="sectorAngleSpread">The angular size of the sector.</param>
        /// <param name="testAngle">The angle to test if it's contained in the sector.</param>
        public static bool AngleInSectorDegrees(float sectorAngle, float sectorAngleSpread, float testAngle) {
            return Mathf.Abs(AngleDiffDegrees(testAngle, sectorAngle)) <= sectorAngleSpread / 2;
        }

        /// <summary>
        /// Rotates a vector around the origin.
        /// </summary>
        /// <param name="v">Vector to rotate.</param>
        /// <param name="rotationRadians">Angle to rotate, in radians.</param>
        public static Vector2 RotateAroundOrigin(Vector2 v, float rotationRadians) {
            Vector2 ret = new Vector2();
            float c = Mathf.Cos(rotationRadians);
            float s = Mathf.Sin(rotationRadians);
            ret.x = v.x * c - v.y * s;
            ret.y = v.x * s + v.y * c;
            return ret;
        }

        /// <summary>
        /// Rotates a vector around a given point.
        /// </summary>
        /// <param name="v">Vector to rotate.</param>
        /// <param name="point">Point to rotate around.</param>
        /// <param name="rotationRadians">Angle to rotate, in radians.</param>
        public static Vector2 RotateAroundPoint(Vector2 v, Vector2 point, float rotationRadians) {
            Vector2 ret = new Vector2();
            float c = Mathf.Cos(rotationRadians);
            float s = Mathf.Sin(rotationRadians);
            ret.x = point.x + (v.x - point.x) * c - (v.y - point.y) * s;
            ret.y = point.y + (v.x - point.x) * s + (v.y - point.y) * c;
            return ret;
        }

        /// <summary>
        /// Calculates the intersection points between circle 1 and circle 2.  Returns array of the 2 Vector2 intersections.  If there are no intersections, the array is null.
        /// </summary>
        /// <param name="c1">Center of circle 1</param>
        /// <param name="r1">Radius of circle 1</param>
        /// <param name="c2">Center of circle 2</param>
        /// <param name="r2">Radius of circle 2</param>
        public static Vector2[] CircleCircleIntersection(Vector2 c1, float r1, Vector2 c2, float r2) {
            float d = Vector2.Distance(c1, c2);
            if (d > r1 + r2) return null;
            if (d < Mathf.Abs(r1 - r2)) return null;

            float a = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            float h = Mathf.Sqrt(r1 * r1 - a * a);
            Vector2 p2 = c1 + a * (c2 - c1) / d;

            return new Vector2[] {
                new Vector2(p2.x + h*(c2.y - c1.y) / d, p2.y - h*(c2.x - c1.x) / d),
                new Vector2(p2.x - h*(c2.y - c1.y) / d, p2.y + h*(c2.x - c1.x) / d),
            };
        }

        /// <summary>
        /// Calculates the 2 intersection points between circle 1 and circle 2.
        /// </summary>
        /// <param name="c1">Center of circle 1</param>
        /// <param name="r1">Radius of circle 1</param>
        /// <param name="c2">Center of circle 2</param>
        /// <param name="r2">Radius of circle 2</param>
        /// <param name="int1">Out param: the first intersection</param>
        /// <param name="int2">Out param: the second intersection</param>
        /// <returns>If the circles intersect</returns>
        public static bool CircleCircleIntersection(Vector2 c1, float r1, Vector2 c2, float r2, out Vector2 int1, out Vector2 int2) {
            float d = Vector2.Distance(c1, c2);
            if (d > r1 + r2 || d < Mathf.Abs(r1 - r2)) {
                int1 = Vector2.zero;
                int2 = Vector2.zero;
                return false;
            }

            float a = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            float h = Mathf.Sqrt(r1 * r1 - a * a);
            Vector2 p2 = c1 + a * (c2 - c1) / d;

            int1 = new Vector2(p2.x + h * (c2.y - c1.y) / d, p2.y - h * (c2.x - c1.x) / d);
            int2 = new Vector2(p2.x - h * (c2.y - c1.y) / d, p2.y + h * (c2.x - c1.x) / d);
            return true;
        }

        /// <summary>
        /// Calculates the intersection points between a line (defined by 2 points on the line) and a circle.  Returns array of the 2 Vector2 intersections.  If there are no intersections, the array is null.
        /// </summary>
        /// <param name="lineP0">First point that defines the line</param>
        /// <param name="lineP1">Second point that defines the line</param>
        /// <param name="c">Center of the circle</param>
        /// <param name="r">Radius of the circle</param>
        public static Vector2[] LineCircleIntersection(Vector2 lineP0, Vector2 lineP1, Vector2 c, float r) {
            Vector2 mid = PointOnLineClosestToPoint(lineP0, lineP1, c);
            float dist2 = r * r - (c - mid).sqrMagnitude;
            if (dist2 < 0) return null;
            Vector2 diff = (lineP1 - lineP0) * Mathf.Sqrt(dist2 / (lineP1 - lineP0).sqrMagnitude);
            return new Vector2[] {
                mid - diff,
                mid + diff
            };
        }

        /// <summary>
        /// Given elements in a line, all spaced a specified distance from each other, with the average displacement being 0, what's the position of each element?
        /// </summary>
        /// <param name="spacing">Distance between each element</param>
        /// <param name="index">The index of the given element</param>
        /// <param name="numElements">Total number of elements</param>
        public static float CenteredSpacing(float spacing, int index, int numElements) {
            if (numElements <= 1) return 0;
            return (index - (numElements - 1.0f) / 2) * spacing;
        }

        /// <summary>
        /// Returns if v2 is positioned clockwise to v1
        /// </summary>
        public static bool IsClockwise(Vector2 v1, Vector2 v2) {
            return -v1.x * v2.y + v1.y * v2.x > 0;
        }

        /// <summary>
        /// Returns the angle (in radians) formed between the two given vectors.
        /// </summary>
        /// <param name="v0">First vector to compare.</param>
        /// <param name="v1">Second vector to compare.</param>
        public static float AngleBetweenVectors(Vector2 v0, Vector2 v1) {
            return Mathf.Acos(Vector2.Dot(v0, v1) / (v0.magnitude * v1.magnitude));
        }

        /// <summary>
        /// Returns the normalized normal of the given vector.  If looking in the direction of the given vector, the normal will face to the "left".
        /// </summary>
        /// <param name="v">The vector whose normal to calculate.</param>
        public static Vector2 NormalVector(Vector2 v) {
            Vector2 ret = new Vector2(-v.y, v.x);
            return ret.normalized;
        }

        /// <summary>
        /// Returns if the two given lines are parallel.
        /// </summary>
        /// <param name="line0P0">The first point that difines the first line.</param>
        /// <param name="line0P1">The other point that defines the first line.</param>
        /// <param name="line1P0">The first point that defines the other line.</param>
        /// <param name="line1P1">The other point that defines the other line.</param>
        public static bool LinesParallel(Vector2 line0P0, Vector2 line0P1, Vector2 line1P0, Vector2 line1P1) {
            Vector2 n = line1P1 - line1P0;
            n.Set(-n.y, n.x); // n is perpendicular to the line
            return Mathf.Abs(Vector2.Dot(n, line0P1 - line0P0)) < EPSILON;
        }

        /// <summary>
        /// Returns t, which is "when" two lines intersect.  The point of intersection is line0P0 + (line0P1 - line0P0) * t.  Returns float.PositiveInfinity if the lines are parallel.
        /// </summary>
        /// <param name="line0P0">The first point that difines the first line.</param>
        /// <param name="line0P1">The other point that defines the first line.</param>
        /// <param name="line1P0">The first point that defines the other line.</param>
        /// <param name="line1P1">The other point that defines the other line.</param>
        public static float LineLineIntersection(Vector2 line0P0, Vector2 line0P1, Vector2 line1P0, Vector2 line1P1) {
            Vector2 n = line1P1 - line1P0;
            n.Set(-n.y, n.x); // n is perpendicular to the line
            float div = Vector2.Dot(n, line0P1 - line0P0);
            if (Mathf.Abs(div) < EPSILON) {
                // lines are parallel
                return float.PositiveInfinity;
            }
            return Vector2.Dot(n, line1P0 - line0P0) / div;
        }

        /// <summary>
        /// Returns the point where 2 lines intersect.  Returns new Vector2(float.PositiveInfinity, float.PositiveInflinity) if the lines are parallel.
        /// </summary>
        /// <param name="line0P0">The first point that difines the first line.</param>
        /// <param name="line0P1">The other point that defines the first line.</param>
        /// <param name="line1P0">The first point that defines the other line.</param>
        /// <param name="line1P1">The other point that defines the other line.</param>
        public static Vector2 LineLineIntersectionPoint(Vector2 line0P0, Vector2 line0P1, Vector2 line1P0, Vector2 line1P1) {
            float t = LineLineIntersection(line0P0, line0P1, line1P0, line1P1);
            if (float.IsInfinity(t))
                return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            return line0P0 + (line0P1 - line0P0) * t;
        }

        /// <summary>
        /// Returns time t > -EPSILON when a ray will intersect a line of infinite length.  Returns -1 if ray won't intersect the line.  The point of the intersection is rayOrigin + rayDirection * t.
        /// </summary>
        /// <param name="rayOrigin">Origin point of the ray.</param>
        /// <param name="rayDirection">Direction of the ray as a vector.  It's recommended this value be normalized, but not required.</param>
        /// <param name="lineP0">A point that defines the line.</param>
        /// <param name="lineP1">The other point that defines the line.</param>
        /// <param name="rayDistance">How long the ray is.  The ray is considered to be infinite length if this value is Infinity.</param>
        public static float RayLineIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 lineP0, Vector2 lineP1, float rayDistance = float.PositiveInfinity) {
            float t = LineLineIntersection(rayOrigin, rayOrigin + rayDirection, lineP0, lineP1);
            if (float.IsPositiveInfinity(t)) return -1; // lines are parallel
            if (t <= -EPSILON) return -1; // ray would hit before the origin

            if (float.IsPositiveInfinity(rayDistance)) {
                // ray is infinite, sure to hit line
                return t;
            }

            // ray isn't infinite, check if intersection would happen within the rayDistance
            float directionMagnitude = rayDirection.sqrMagnitude;
            if (Mathf.Abs(directionMagnitude - 1) >= EPSILON) {
                // direction wasn't normalized
                directionMagnitude = Mathf.Sqrt(directionMagnitude);
            }
            float intersectDistance = directionMagnitude * t;
            if (intersectDistance <= rayDistance + EPSILON)
                return t;

            return -1;
        }

        /// <summary>
        /// Returns time t > -EPSILON when a ray will intersect a segment.  Returns -1 if ray won't intersect the segment.  The point of the intersection is rayOrigin + rayDirection * t.
        /// </summary>
        /// <param name="rayOrigin">Origin point of the ray.</param>
        /// <param name="rayDirection">Direction of the ray as a vector.  It's recommended this value be normalized, but not required.</param>
        /// <param name="segmentP0">A point that defines the segment.</param>
        /// <param name="segmentP1">The other point that defines the segment.</param>
        /// <param name="rayDistance">How long the ray is.  The ray is considered to be infinite length if this value is Infinity.</param>
        public static float RaySegmentIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 segmentP0, Vector2 segmentP1, float rayDistance = float.PositiveInfinity) {
            float t = RayLineIntersection(rayOrigin, rayDirection, segmentP0, segmentP1, rayDistance);
            if (t == -1) return -1;
            // check that intersect point is contained in the segment
            Vector2 p = rayOrigin + rayDirection * t;
            if (Mathf.Min(segmentP0.x, segmentP1.x) - EPSILON < p.x && p.x < Mathf.Max(segmentP0.x, segmentP1.x) + EPSILON &&
                Mathf.Min(segmentP0.y, segmentP1.y) - EPSILON < p.y && p.y < Mathf.Max(segmentP0.y, segmentP1.y) + EPSILON) {
                return t;
            }
            return -1;
        }

        /// <summary>
        /// Returns t in (-EPSILON, 1] if the two segments intersect, and -1 if they don't.  The point of intersection is segment0P0 + (segment0P1 - segment0P0) * t.
        /// </summary>
        /// <param name="segment0P0">The first point that defines the first segment.</param>
        /// <param name="segment0P1">The other point that defines the first segment.</param>
        /// <param name="segment1P0">The first point that defines the other segment.</param>
        /// <param name="segment1P1">The other point that defines the other segment.</param>
        public static float SegmentSegmentIntersection(Vector2 segment0P0, Vector2 segment0P1, Vector2 segment1P0, Vector2 segment1P1) {
            float t = RaySegmentIntersection(segment0P0, segment0P1 - segment0P0, segment1P0, segment1P1);
            if (t == -1) return -1;
            if (t > 1) return -1;
            return t;
        }

        /// <summary>
        /// Given a line defined by p0 and p1, returns if the point is to the left of the line ("left" when starting at p0, facing p1).
        /// </summary>
        /// <param name="lineP0">The first point that defines the line.</param>
        /// <param name="lineP1">The other point that defines the line.</param>
        /// <param name="point">The point to test.</param>
        public static bool PointToLeft(Vector2 lineP0, Vector2 lineP1, Vector2 point) {
            return ((lineP1.x - lineP0.x) * (point.y - lineP0.y) - (lineP1.y - lineP0.y) * (point.x - lineP0.x)) > 0;
        }

        /// <summary>
        /// Given a line and 2 points, returns if the points are on the same side of the line.
        /// </summary>
        /// <param name="lineP0">The first point that defines the line.</param>
        /// <param name="lineP1">The other point that defines the line.</param>
        /// <param name="point0">The first point to test.</param>
        /// <param name="point1">The other point to test.</param>
        public static bool PointsOnSameSideOfLine(Vector2 lineP0, Vector2 lineP1, Vector2 point0, Vector2 point1) {
            return PointToLeft(lineP0, lineP1, point0) == PointToLeft(lineP0, lineP1, point1);
        }

        /// <summary>
        /// Returns the result of projecting vector a onto vector b.
        /// </summary>
        /// <param name="a">Vector a.</param>
        /// <param name="b">Vector b.</param>
        public static Vector2 VectorProject(Vector2 a, Vector2 b) {
            return Vector2.Dot(a, b) / Vector2.Dot(b, b) * b;
        }

        /// <summary>
        /// Returns the point on the line defined by lineP0 and lineP1 that is closest to the given point.
        /// </summary>
        /// <param name="lineP0">First point that defines the line.</param>
        /// <param name="lineP1">Other point that defines the line.</param>
        /// <param name="point">Point to consider.</param>
        public static Vector2 PointOnLineClosestToPoint(Vector2 lineP0, Vector2 lineP1, Vector2 point) {
            return lineP0 + VectorProject(point - lineP0, lineP1 - lineP0);
        }

        /// <summary>
        /// Gets the distance from a point to a line defined by lineP0 and lineP1.
        /// </summary>
        /// <param name="lineP0">First point that defines the line.</param>
        /// <param name="lineP1">Other point that defines the line.</param>
        /// <param name="point">Point to consider.</param>
        public static float DistanceFromLine(Vector2 lineP0, Vector2 lineP1, Vector2 point) {
            Vector2 closestPoint = PointOnLineClosestToPoint(lineP0, lineP1, point);
            return Vector2.Distance(closestPoint, point);
        }

        /// <summary>
        /// Like getting the distance from a point to a line, but is negative if going to the right of the line.  
        /// </summary>
        /// <param name="lineP0">First point that defines the line.</param>
        /// <param name="lineP1">Other point that defines the line.</param>
        /// <param name="point">Point to consider.</param>
        public static float DisplacementFromLine(Vector2 lineP0, Vector2 lineP1, Vector2 point) {
            Vector2 closestPoint = PointOnLineClosestToPoint(lineP0, lineP1, point);
            float distance = Vector2.Distance(closestPoint, point);
            if (PointToLeft(lineP0, lineP1, point)) {
                return distance;
            } else {
                return -distance;
            }
        }

        /// <summary>
        /// Given a line, returns the point on the line with the given x coordinate.  Returns float.PositiveInfinity if line is vertical.
        /// </summary>
        /// <param name="lineP0">First point that defines the line.</param>
        /// <param name="lineP1">Other point that defines the line.</param>
        /// <param name="x">The x coordinate to consider.</param>
        public static float YFromXOnLine(Vector2 lineP0, Vector2 lineP1, float x) {
            if (Mathf.Abs(lineP1.x - lineP0.x) < EPSILON)
                return float.PositiveInfinity;
            float slope = (lineP1.y - lineP0.y) / (lineP1.x - lineP0.x);
            float yInt = lineP0.y - slope * lineP0.x;
            return slope * x + yInt;
        }

        /// <summary>
        /// Given a line, returns the point on the line with the given y coordinate.  Returns float.PositiveInfinity if line is horizontal.
        /// </summary>
        /// <param name="lineP0">First point that defines the line.</param>
        /// <param name="lineP1">Other point that defines the line.</param>
        /// <param name="y">The y coordinate to consider.</param>
        public static float XFromYOnLine(Vector2 lineP0, Vector2 lineP1, float y) {
            if (Mathf.Abs(lineP1.y - lineP0.y) < EPSILON)
                return float.PositiveInfinity;
            float invSlope = (lineP1.x - lineP0.x) / (lineP1.y - lineP0.y);
            float xInt = lineP0.x - invSlope * lineP0.y;
            return invSlope * y + xInt;
        }

        /// <summary>
        /// Returns if a given point is contained inside the given triangle.
        /// </summary>
        /// <param name="triangleP0">Point 0 defining the triangle.</param>
        /// <param name="triangleP1">Point 1 defining the triangle.</param>
        /// <param name="triangleP2">Point 2 defining the triangle.</param>
        /// <param name="point">The point to test.</param>
        public static bool PointInTriangle(Vector2 triangleP0, Vector2 triangleP1, Vector2 triangleP2, Vector2 point) {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = triangleP2.x - triangleP1.x; ay = triangleP2.y - triangleP1.y;
            bx = triangleP0.x - triangleP2.x; by = triangleP0.y - triangleP2.y;
            cx = triangleP1.x - triangleP0.x; cy = triangleP1.y - triangleP0.y;
            apx = point.x - triangleP0.x; apy = point.y - triangleP0.y;
            bpx = point.x - triangleP1.x; bpy = point.y - triangleP1.y;
            cpx = point.x - triangleP2.x; cpy = point.y - triangleP2.y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }

        /// <summary>
        /// Returns if a point is contained in the given polygon.
        /// </summary>
        /// <param name="v">Point to check.</param>
        /// <param name="polygon">array of points defining the vertices of the polygon.</param>
        public static bool PointInPolygon(Vector2 v, Vector2[] polygon) {
            // RPI represent http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++) {
                if ((polygon[i].y > v.y) != (polygon[j].y > v.y) &&
                     v.x < (polygon[j].x - polygon[i].x) * (v.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x) {
                    inside = !inside;
                }
            }

            return inside;
        }

        /// <summary>
        /// Returns the center (centroid) of the given triangle.
        /// </summary>
        /// <param name="triangleP0">Point 0 defining the triangle.</param>
        /// <param name="triangleP1">Point 1 defining the triangle.</param>
        /// <param name="triangleP2">Point 2 defining the triangle.</param>
        public static Vector2 CenterOfTriangle(Vector2 triangleP0, Vector2 triangleP1, Vector2 triangleP2) {
            return (triangleP0 + triangleP1 + triangleP2) / 3;
        }

        /// <summary>
        /// Returns if the two given triangles intersect.
        /// </summary>
        /// <param name="triangle0P0">Point 0 defining triangle 0.</param>
        /// <param name="triangle0P1">Point 1 defining triangle 0.</param>
        /// <param name="triangle0P2">Point 2 defining triangle 0.</param>
        /// <param name="triangle1P0">Point 0 defining triangle 1.</param>
        /// <param name="triangle1P1">Point 1 defining triangle 1.</param>
        /// <param name="triangle1P2">Point 2 defining triangle 1.</param>
        public static bool TriangleTraingleIntersect(Vector2 triangle0P0, Vector2 triangle0P1, Vector2 triangle0P2, Vector2 triangle1P0, Vector2 triangle1P1, Vector2 triangle1P2) {
            if (PointInTriangle(triangle1P0, triangle1P1, triangle1P2, triangle0P0)) return true;
            if (PointInTriangle(triangle1P0, triangle1P1, triangle1P2, triangle0P1)) return true;
            if (PointInTriangle(triangle1P0, triangle1P1, triangle1P2, triangle0P2)) return true;
            if (PointInTriangle(triangle0P0, triangle0P1, triangle0P2, triangle1P0)) return true;
            if (PointInTriangle(triangle0P0, triangle0P1, triangle0P2, triangle1P1)) return true;
            if (PointInTriangle(triangle0P0, triangle0P1, triangle0P2, triangle1P2)) return true;
            return false;
        }

        /// <summary>
        /// Returns a normalized vector that bisects the angle formed between the two given vectors.
        /// </summary>
        /// <param name="v0">First vector to compare.</param>
        /// <param name="v1">Second vector to compare.</param>
        public static Vector2 VectorBisector(Vector2 v0, Vector2 v1) {
            Vector2 b = v0.normalized + v1.normalized;
            float m = b.magnitude;
            if (Mathf.Abs(m) < EPSILON) {
                return NormalVector(v0);
            }
            return b / m;
        }

        /// <summary>
        /// Returns the rectangle formed by the intersection of the two given rectangles.  The width and height of the intersection rectangle are non-negative.
        /// If the given rectangles do not intersect, then a rectangle with a negative width is returned.
        /// </summary>
        /// <param name="rect0">Rectangle 0 to compare.</param>
        /// <param name="rect1">Rectangle 1 to compare.</param>
        public static Rect RectRectIntersection(Rect rect0, Rect rect1) {
            float intXMin = Mathf.Max(rect0.xMin, rect1.xMin);
            float intXMax = Mathf.Min(rect0.xMax, rect1.xMax);
            if (intXMax < intXMin) {
                return new Rect(0, 0, -1, -1);
            }
            float intYMin = Mathf.Max(rect0.yMin, rect1.yMin);
            float intYMax = Mathf.Min(rect0.yMax, rect1.yMax);
            if (intYMax < intYMin) {
                return new Rect(0, 0, -1, -1);
            }

            return new Rect(intXMin, intYMin, intXMax - intXMin, intYMax - intYMin);
        }
    }
}