using UnityEngine;
using System.Collections;

public static class TransformExtensions {
    
    /// <summary>
    /// Sets the x component of the transform's local scale.
    /// </summary>
    public static void SetLocalScaleX(this Transform transform, float scaleX) {
        transform.localScale = new Vector3(
            scaleX,
            transform.localScale.y,
            transform.localScale.z);
    }
    /// <summary>
    /// Sets the y component of the transform's local scale.
    /// </summary>
    public static void SetLocalScaleY(this Transform transform, float scaleY) {
        transform.localScale = new Vector3(
            transform.localScale.x,
            scaleY,
            transform.localScale.z);
    }
    /// <summary>
    /// Sets the z component of the transform's local scale.
    /// </summary>
    public static void SetLocalScaleZ(this Transform transform, float scaleZ) {
        transform.localScale = new Vector3(
            transform.localScale.x,
            transform.localScale.y,
            scaleZ);
    }

}
