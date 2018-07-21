using UnityEngine;
using System.Collections;

public static class TransformExtensions {
    
    /// <summary>
    /// Sets the x component of the transform's local scale.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="scaleX"></param>
    public static void SetLocalScaleX(this Transform transform, float scaleX) {
        transform.localScale = new Vector3(
            scaleX,
            transform.localScale.y,
            transform.localScale.z);
    }

}
