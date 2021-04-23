using UnityEngine;
using System.Collections;

namespace Core.Unity {
    /// <summary>
    /// Contains many general easing functions.
    /// </summary>
    public class Easing {

        /// <summary>
        /// Linearly interpolates between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static float Linear(float a, float b, float t, float d = 1) {
            return LinearUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Linearly interpolates between a and b.  No clamping is done.
        /// </summary>
        public static float LinearUnclamp(float a, float b, float t, float d = 1) {
            return a + (b - a) * t / d;
        }
        /// <summary>
        /// Linearly interpolates between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static Vector2 Linear(Vector2 a, Vector2 b, float t, float d = 1) {
            return LinearUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Linearly interpolates between a and b.  No clamping is done.
        /// </summary>
        public static Vector2 LinearUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
            return a + (b - a) * t / d;
        }

        /// <summary>
        /// Quadratic ease in interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static float QuadIn(float a, float b, float t, float d = 1) {
            return QuadInUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Quadratic ease in interpolation between a and b.  No clamping is done.
        /// </summary>
        public static float QuadInUnclamp(float a, float b, float t, float d = 1) {
            t /= d;
            return (b - a) * t * t + a;
        }
        /// <summary>
        /// Quadratic ease in interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static Vector2 QuadIn(Vector2 a, Vector2 b, float t, float d = 1) {
            return QuadInUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Quadratic ease in interpolation between a and b.  No clamping is done.
        /// </summary>
        public static Vector2 QuadInUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
            t /= d;
            return (b - a) * t * t + a;
        }

        /// <summary>
        /// Quadratic ease out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static float QuadOut(float a, float b, float t, float d = 1) {
            return QuadOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Quadratic ease out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static float QuadOutUnclamp(float a, float b, float t, float d = 1) {
            t /= d;
            return (a - b) * t * (t - 2) + a;
        }
        /// <summary>
        /// Quadratic ease out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static Vector2 QuadOut(Vector2 a, Vector2 b, float t, float d = 1) {
            return QuadOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Quadratic ease out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static Vector2 QuadOutUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
            t /= d;
            return (a - b) * t * (t - 2) + a;
        }

        /// <summary>
        /// Quadratic ease in-out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static float QuadInOut(float a, float b, float t, float d = 1) {
            return QuadInOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Quadratic ease in-out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static float QuadInOutUnclamp(float a, float b, float t, float d = 1) {
            t /= d / 2;
            if (t < 1) return (b - a) / 2 * t * t + a;
            t--;
            return (a - b) / 2 * (t * (t - 2) - 1) + a;
        }
        /// <summary>
        /// Quadratic ease in-out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static Vector2 QuadInOut(Vector2 a, Vector2 b, float t, float d = 1) {
            return QuadInOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Quadratic ease in-out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static Vector2 QuadInOutUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
            t /= d / 2;
            if (t < 1) return (b - a) / 2 * t * t + a;
            t--;
            return (a - b) / 2 * (t * (t - 2) - 1) + a;
        }
        /// <summary>
        /// Quadratic ease in-out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static Vector3 QuadInOut(Vector3 a, Vector3 b, float t, float d = 1) {
            return QuadInOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Quadratic ease in-out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static Vector3 QuadInOutUnclamp(Vector3 a, Vector3 b, float t, float d = 1) {
            t /= d / 2;
            if (t < 1) return (b - a) / 2 * t * t + a;
            t--;
            return (a - b) / 2 * (t * (t - 2) - 1) + a;
        }
        /// <summary>
        /// Quadratic ease in-out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static Quaternion QuadInOut(Quaternion a, Quaternion b, float t, float d = 1) {
            return QuadInOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Quadratic ease in-out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static Quaternion QuadInOutUnclamp(Quaternion a, Quaternion b, float t, float d = 1) {
            float inter = QuadInOutUnclamp(0, 1, t, d);
            return Quaternion.LerpUnclamped(a, b, inter);
        }

        /// <summary>
        /// Cubic ease out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static float CubicOut(float a, float b, float t, float d = 1) {
            return CubicOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Cubic ease out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static float CubicOutUnclamp(float a, float b, float t, float d = 1) {
            t = t / d - 1;
            return (b - a) * (t * t * t + 1) + a;
        }
        /// <summary>
        /// Cubic ease out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static Vector2 CubicOut(Vector2 a, Vector2 b, float t, float d = 1) {
            return CubicOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Cubic ease out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static Vector2 CubicOutUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
            t = t / d - 1;
            return (b - a) * (t * t * t + 1) + a;
        }

        /// <summary>
        /// Cubic ease in-out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static float CubicInOut(float a, float b, float t, float d = 1) {
            return CubicInOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Cubic ease in-out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static float CubicInOutUnclamp(float a, float b, float t, float d = 1) {
            t /= d / 2;
            if (t < 1) return (b - a) / 2 * t * t * t + a;
            t -= 2;
            return (b - a) / 2 * (t * t * t + 2) + a;
        }
        /// <summary>
        /// Cubic ease in-out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static Vector2 CubicInOut(Vector2 a, Vector2 b, float t, float d = 1) {
            return CubicInOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Cubic ease in-out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static Vector2 CubicInOutUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
            t /= d / 2;
            if (t < 1) return (b - a) / 2 * t * t * t + a;
            t -= 2;
            return (b - a) / 2 * (t * t * t + 2) + a;
        }

        /// <summary>
        /// Generalizes k-In where k is the power value.
        /// </summary>
        /// <param name="k">Power value (e.g. k=1 is linear, k=2 is quad, etc.)</param>
        public static float PowIn(float k, float a, float b, float t, float d = 1) {
            return PowInUnclamp(k, a, b, Mathf.Clamp(t, 0, d), d);
        }

        /// <summary>
        /// Generalizes k-InUnclamp where k is the power value.
        /// </summary>
        /// <param name="k">Power value (e.g. k=1 is linear, k=2 is quad, etc.)</param>
        public static float PowInUnclamp(float k, float a, float b, float t, float d = 1) {
            t /= d;
            return (b - a) * Mathf.Pow(t, k) + a;
        }

        /// <summary>
        /// Generalizes k-Out where k is the power value.
        /// </summary>
        /// <param name="k">Power value (e.g. k=1 is linear, k=2 is quad, etc.)</param>
        public static float PowOut(float k, float a, float b, float t, float d = 1) {
            return PowOutUnclamp(k, a, b, Mathf.Clamp(t, 0, d), d);
        }

        /// <summary>
        /// Generalizes k-OutUnclamp where k is the power value.
        /// </summary>
        /// <param name="k">Power value (e.g. k=1 is linear, k=2 is quad, etc.)</param>
        public static float PowOutUnclamp(float k, float a, float b, float t, float d = 1) {
            t /= d;
            return (b - a) * (1 - Mathf.Pow(1 - t, k)) + a;
        }

        /// <summary>
        /// Generalizes k-InOut where k is the power value.
        /// </summary>
        /// <param name="k">Power value (e.g. k=1 is linear, k=2 is quad, etc.)</param>
        public static float PowInOut(float k, float a, float b, float t, float d = 1) {
            return PowInOutUnclamp(k, a, b, Mathf.Clamp(t, 0, d), d);
        }

        /// <summary>
        /// Generalizes k-InOutUnclamp where k is the power value.
        /// </summary>
        /// <param name="k">Power value (e.g. k=1 is linear, k=2 is quad, etc.)</param>
        /// <remarks>I made this! https://www.desmos.com/calculator/4vvj51dsun</remarks>
        public static float PowInOutUnclamp(float k, float a, float b, float t, float d = 1) {
            t /= d;
            if (t < 0.5f) return (b - a) * Mathf.Pow(2, k - 1) * Mathf.Pow(t, k) + a;
            else return (b - a) * (1 - Mathf.Pow(2, k - 1) * Mathf.Pow(1 - t, k)) + a;
        }

        /// <summary>
        /// Ease out interpolation between a and b where the value overshoots a bit before returning to b.  t is clamped between 0 and d.
        /// </summary>
        public static float BackOut(float a, float b, float t, float d = 1) {
            return BackOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Ease out interpolation between a and b where the value overshoots a bit before returning to b.
        /// </summary>
        public static float BackOutUnclamp(float a, float b, float t, float d = 1) {
            float s = 1.70158f;
            t = t / d - 1;
            return (b - a) * (t * t * ((s + 1) * t + s) + 1) + a;
        }

        /// <summary>
        /// Elastic ease out interpolation between a and b.  t is clamped between 0 and d.
        /// </summary>
        public static float ElasticOut(float a, float b, float t, float d = 1) {
            return ElasticOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
        }
        /// <summary>
        /// Elastic ease out interpolation between a and b.  No clamping is done.
        /// </summary>
        public static float ElasticOutUnclamp(float a, float b, float t, float d = 1) {
            float k = float.NaN, p = float.NaN; // these can be given?  https://github.com/danro/tweenman-as3/blob/master/Easing.as
            float s;
            if (t == 0)
                return a;
            t /= d;
            if (Mathf.Approximately(t, 1))
                return b;
            if (float.IsNaN(p))
                p = d * 0.3f;
            if (float.IsNaN(k) || k < Mathf.Abs(b - a)) {
                k = b - a;
                s = p / 4;
            } else {
                s = p / Mathf.PI * 2 * Mathf.Asin((b - a) / k);
            }

            return k * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * Mathf.PI * 2 / p) + b;
        }

        /// <summary>
        /// Alternates between a and b with a sine wave.  t is able to go beyond the bounds.
        /// </summary>
        /// <param name="phaseShift">If true, value will be a at t = 0 (phase shifted -PI/2).  If false, value will be (a + b) / 2 at t = 0.</param>
        public static float SineWave(float a, float b, float t, float period = 1, bool phaseShift = true) {
            t /= period;
            if (phaseShift) t -= .25f;
            return a + (b - a) * (Mathf.Sin(t * Mathf.PI * 2) + 1) / 2;
        }

        /// <summary>
        /// Exponential growth from <paramref name="a"/> to <paramref name="b"/>.
        /// <paramref name="a"/> and <paramref name="b"/> must both be positive.
        /// </summary>
        public static float ExponentialGrowth(float a, float b, float t, float d = 1) {
            return ExponentialGrowthUnclamped(a, b, Mathf.Clamp(t, 0, d), d);
        }

        /// <summary>
        /// Exponential growth from <paramref name="a"/> to <paramref name="b"/>.
        /// <paramref name="a"/> and <paramref name="b"/> must both be positive.
        /// </summary>
        public static float ExponentialGrowthUnclamped(float a, float b, float t, float d = 1) {
            if (Mathf.Approximately(a, 0) || Mathf.Approximately(b, 0)) {
                return float.NaN;
            }
            if ((a > 0) != (b > 0)) {
                return float.NaN;
            }

            return a * Mathf.Pow(b / a, t / d);
        }

        /// <summary>
        /// Given a bezier curve defined by a start point p0, control point, and end point p2, find the point on the curve at t in [0,1]
        /// </summary>
        /// /// <param name="t">in [0,1]</param>
        public static Vector2 BezierQuadratic(Vector2 p0, Vector2 controlPoint, Vector2 p2, float t) {
            return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * controlPoint + t * t * p2;
        }
        /// <summary>
        /// Given a bezier curve defined by a start point p0, control point, and end point p2, find the derivative on the curve at t in [0,1]
        /// </summary>
        /// <param name="t">in [0,1]</param>
        public static Vector2 BezierDerivativeQuadratic(Vector2 p0, Vector2 controlPoint, Vector2 p2, float t) {
            return 2 * (1 - t) * (controlPoint - p0) + 2 * t * (p2 - controlPoint);
        }
        /// <summary>
        /// Given a bezier curve defined by a start point p0, two control points, and end point p3, find the point on the curve at t in [0,1]
        /// </summary>
        /// /// <param name="t">in [0,1]</param>
        public static Vector2 BezierCubic(Vector2 p0, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 p3, float t) {
            return (1 - t) * BezierQuadratic(p0, controlPoint1, controlPoint2, t) + t * BezierQuadratic(controlPoint1, controlPoint2, p3, t);
        }
        /// <summary>
        /// Given a bezier curve defined by a start point p0, two control points, and end point p3, find the derivative on the curve at t in [0,1]
        /// </summary>
        /// /// <param name="t">in [0,1]</param>
        public static Vector2 BezierDerivativeCubic(Vector2 p0, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 p3, float t) {
            return 3 * (1 - t) * (1 - t) * (controlPoint1 - p0) + 6 * (1 - t) * t * (controlPoint2 - controlPoint1) + 3 * t * t * (p3 - controlPoint2);
        }
        /// <summary>
        /// Given a bezier curve defined by a start point p0, two control points, and end point p3, find the normalized normal on the curve at t in [0,1]
        /// </summary>
        /// /// <param name="t">in [0,1]</param>
        public static Vector2 BezierNormalCubic(Vector2 p0, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 p3, float t) {
            Vector2 deriv = BezierDerivativeCubic(p0, controlPoint1, controlPoint2, p3, t);
            Vector2 normal = new Vector2(-deriv.y, deriv.x);
            return normal.normalized;
        }

        /// <summary>
        /// Gradually changes a value towards a desired goal over time.
        /// Taken from decompiled code for the Unity function of the same name.
        /// </summary>
        /// <param name="current">The current position.</param>
        /// <param name="target">The position we are trying to reach.</param>
        /// <param name="currentVelocity">The current velocity, this value is modified by the function every time you call it.</param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <param name="maxSpeed">Clamps the maximum speed.</param>
        /// <param name="deltaTime">The time since the last call to this function.</param>
        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime) {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            float num4 = current - target;
            float num6 = maxSpeed * smoothTime;
            num4 = Mathf.Clamp(num4, -num6, num6);
            target = current - num4;
            float num7 = (currentVelocity + num * num4) * deltaTime;
            currentVelocity = (currentVelocity - num * num7) * num3;
            float ret = target + (num4 + num7) * num3;
            if (target - current > 0f == ret > target) {
                ret = target;
                currentVelocity = (ret - target) / deltaTime;
            }
            return ret;
        }

        /// <summary>
        /// (THIS HAS NOT BEEN TESTED YET) Eases one value to another using a numeric spring.  To be called each frame.
        /// More info: http://allenchou.net/2015/04/game-math-numeric-springing-examples/
        /// </summary>
        /// <param name="value">Position to be updated.</param>
        /// <param name="velocity">Velocity to be update.</param>
        /// <param name="targetValue">the target position.</param>
        /// <param name="zeta">Dampening ratio.</param>
        /// <param name="omega">Angular frequency.</param>
        /// <param name="dt">Time step.</param>
        public static void SpringFaster(ref float value, ref float velocity, float targetValue, float zeta, float omega, float dt) {
            float f = 1.0f + 2.0f * dt * zeta * omega;
            float oo = omega * omega;
            float hoo = dt * oo;
            float hhoo = dt * hoo;
            float detInv = 1.0f / (f + hhoo);
            float detX = f * value + dt * velocity + hhoo * targetValue;
            float detV = velocity + hoo * (targetValue - value);
            value = detX * detInv;
            velocity = detV * detInv;
        }
        /// <summary>
        /// (THIS HAS NOT BEEN TESTED YET) A version of spring() with more intuitive parameters.  Eases one value to another using a numeric spring.  To be called each frame.
        /// More info: http://allenchou.net/2015/04/game-math-numeric-springing-examples/
        /// </summary>
        /// <param name="value">Position to be updated.</param>
        /// <param name="velocity">Velocity to be update.</param>
        /// <param name="targetValue">the target position.</param>
        /// <param name="oscillationPeriod">Period of one oscillation.</param>
        /// <param name="reductionRatio">Fraction of oscillation magnitude reduced over rrDuration.  e.g. .1 means oscillation magnitude reduces by 90% every rrDuration seconds.</param>
        /// <param name="rrDuration">Time in which the reductionRatio is applied.  e.g. .5 means oscillation magnitude reduces by reductionRatio every .5 seconds.</param>
        /// <param name="dt">Time step.</param>
        public static void Spring(ref float value, ref float velocity, float targetValue, float oscillationPeriod, float reductionRatio, float rrDuration, float dt) {
            float omega = 2 * Mathf.PI / oscillationPeriod;
            float zeta = Mathf.Log(reductionRatio) / (-omega * rrDuration);
            SpringFaster(ref value, ref velocity, targetValue, zeta, omega, dt);
        }

    }
}