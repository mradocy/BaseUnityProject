﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Contains many general easing functions.
/// </summary>
public class Easing {
    
    /// <summary>
    /// Linearly interpolates between a and b.  t is clamped between 0 and d.
    /// </summary>
    public static float linear(float a, float b, float t, float d = 1) {
        return linearUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
    }
    /// <summary>
    /// Linearly interpolates between a and b.  No clamping is done.
    /// </summary>
    public static float linearUnclamp(float a, float b, float t, float d = 1) {
        return a + (b - a) * t / d;
    }
    /// <summary>
    /// Linearly interpolates between a and b.  t is clamped between 0 and d.
    /// </summary>
    public static Vector2 linear(Vector2 a, Vector2 b, float t, float d = 1) {
        return linearUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
    }
    /// <summary>
    /// Linearly interpolates between a and b.  No clamping is done.
    /// </summary>
    public static Vector2 linearUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
        return a + (b - a) * t / d;
    }
    
    /// <summary>
    /// Quadratic ease in interpolation between a and b.  t is clamped between 0 and d.
    /// </summary>
    public static float quadIn(float a, float b, float t, float d = 1) {
        return quadInUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
    }
    /// <summary>
    /// Quadratic ease in interpolation between a and b.  No clamping is done.
    /// </summary>
    public static float quadInUnclamp(float a, float b, float t, float d = 1) {
        t /= d;
        return (b - a) * t * t + a;
    }
    /// <summary>
    /// Quadratic ease in interpolation between a and b.  t is clamped between 0 and d.
    /// </summary>
    public static Vector2 quadIn(Vector2 a, Vector2 b, float t, float d = 1) {
        return quadInUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
    }
    /// <summary>
    /// Quadratic ease in interpolation between a and b.  No clamping is done.
    /// </summary>
    public static Vector2 quadInUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
        t /= d;
        return (b - a) * t * t + a;
    }

    /// <summary>
    /// Quadratic ease out interpolation between a and b.  t is clamped between 0 and d.
    /// </summary>
    public static float quadOut(float a, float b, float t, float d = 1) {
        return quadOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
    }
    /// <summary>
    /// Quadratic ease out interpolation between a and b.  No clamping is done.
    /// </summary>
    public static float quadOutUnclamp(float a, float b, float t, float d = 1) {
        t /= d;
        return (a - b) * t * (t - 2) + a;
    }
    /// <summary>
    /// Quadratic ease out interpolation between a and b.  t is clamped between 0 and d.
    /// </summary>
    public static Vector2 quadOut(Vector2 a, Vector2 b, float t, float d = 1) {
        return quadOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
    }
    /// <summary>
    /// Quadratic ease out interpolation between a and b.  No clamping is done.
    /// </summary>
    public static Vector2 quadOutUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
        t /= d;
        return (a - b) * t * (t - 2) + a;
    }

    /// <summary>
    /// Quadratic ease in-out interpolation between a and b.  t is clamped between 0 and d.
    /// </summary>
    public static float quadInOut(float a, float b, float t, float d = 1) {
        return quadInOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
    }
    /// <summary>
    /// Quadratic ease in-out interpolation between a and b.  No clamping is done.
    /// </summary>
    public static float quadInOutUnclamp(float a, float b, float t, float d = 1) {
        t /= d / 2;
        if (t < 1) return (b - a) / 2 * t * t + a;
        t--;
        return (a - b) / 2 * (t * (t - 2) - 1) + a;
    }
    /// <summary>
    /// Quadratic ease in-out interpolation between a and b.  t is clamped between 0 and d.
    /// </summary>
    public static Vector2 quadInOut(Vector2 a, Vector2 b, float t, float d = 1) {
        return quadInOutUnclamp(a, b, Mathf.Clamp(t, 0, d), d);
    }
    /// <summary>
    /// Quadratic ease in-out interpolation between a and b.  No clamping is done.
    /// </summary>
    public static Vector2 quadInOutUnclamp(Vector2 a, Vector2 b, float t, float d = 1) {
        t /= d / 2;
        if (t < 1) return (b - a) / 2 * t * t + a;
        t--;
        return (a - b) / 2 * (t * (t - 2) - 1) + a;
    }


    /// <summary>
    /// Given a bezier curve defined by a start point p0, control point, and end point p2, find the point on the curve at t in [0,1]
    /// </summary>
    /// /// <param name="t">in [0,1]</param>
    public static Vector2 bezierQuadratic(Vector2 p0, Vector2 controlPoint, Vector2 p2, float t) {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * controlPoint + t * t * p2;
    }
    /// <summary>
    /// Given a bezier curve defined by a start point p0, control point, and end point p2, find the derivative on the curve at t in [0,1]
    /// </summary>
    /// <param name="t">in [0,1]</param>
    public static Vector2 bezierDerivativeQuadratic(Vector2 p0, Vector2 controlPoint, Vector2 p2, float t) {
        return 2 * (1 - t) * (controlPoint - p0) + 2 * t * (p2 - controlPoint);
    }
    /// <summary>
    /// Given a bezier curve defined by a start point p0, two control points, and end point p3, find the point on the curve at t in [0,1]
    /// </summary>
    /// /// <param name="t">in [0,1]</param>
    public static Vector2 bezierCubic(Vector2 p0, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 p3, float t) {
        return (1 - t) * bezierQuadratic(p0, controlPoint1, controlPoint2, t) + t * bezierQuadratic(controlPoint1, controlPoint2, p3, t);
    }
    /// <summary>
    /// Given a bezier curve defined by a start point p0, two control points, and end point p3, find the derivative on the curve at t in [0,1]
    /// </summary>
    /// /// <param name="t">in [0,1]</param>
    public static Vector2 bezierDerivativeCubic(Vector2 p0, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 p3, float t) {
        return 3 * (1 - t) * (1 - t) * (controlPoint1 - p0) + 6 * (1 - t) * t * (controlPoint2 - controlPoint1) + 3 * t * t * (p3 - controlPoint2);
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
    public static void springFaster(ref float value, ref float velocity, float targetValue, float zeta, float omega, float dt) {
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
    public static void spring(ref float value, ref float velocity, float targetValue, float oscillationPeriod, float reductionRatio, float rrDuration, float dt) {
        float omega = 2 * Mathf.PI / oscillationPeriod;
        float zeta = Mathf.Log(reductionRatio) / (-omega * rrDuration);
        springFaster(ref value, ref velocity, targetValue, zeta, omega, dt);
    }

}