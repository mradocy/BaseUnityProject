using UnityEngine;
using System.Collections;

/// <summary>
/// Component that defines a bounded CameraControl.ViewMode.
/// If this gets extended, make sure to extend ViewModeAreaEditor too.
/// </summary>
[ExecuteInEditMode]
public class ViewModeArea : ViewModeAreaUnbounded {

    #region Inspector Properties

    public Vector2 boundsCenter = new Vector2();
    public float boundsWidth = 20;
    public float boundsHeight = 10;

    public CameraControl.ViewModePositionFunctions.ID positionFunction = CameraControl.ViewModePositionFunctions.ID.DEFAULT;

    [Header("Set by Scene")]
    public Transform followTransform = null;

    [Header("UI")]
    [Tooltip("Show bounds position handles.")]
    public bool showBoundsHandles = true;

    #endregion

    #region Creating ViewMode

    /// <summary>
    /// Creates viewMode from the components attached to the GameObject.
    /// </summary>
    protected override CameraControl.ViewMode createViewMode() {
        Vector2 position = transform.position;
        Rect bounds = new Rect(boundsCenter.x - boundsWidth / 2, boundsCenter.y - boundsHeight / 2, boundsWidth, boundsHeight);
        bounds.center += position;
        return new CameraControl.ViewMode(position, bounds, size, followTransform, CameraControl.ViewModePositionFunctions.functionFromID(positionFunction));
    }

    #endregion

    #region Drawing Gizmos

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

#if UNITY_EDITOR

        if (!UnityEditor.Selection.Contains(gameObject)) {
            drawCorners(Color.green, boundsCenter, boundsWidth / 2, boundsHeight / 2);
        }

#endif

    }

    protected override void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        drawFullBox(Color.green, boundsCenter, boundsWidth / 2, boundsHeight / 2);
    }

    #endregion
    
}
