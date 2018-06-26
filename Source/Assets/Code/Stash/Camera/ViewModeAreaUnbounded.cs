using UnityEngine;
using System.Collections;

/// <summary>
/// Component that defines an unbounded CameraControl.ViewMode.
/// The transform defines the position.  The inspector property size determines the size.
/// </summary>
[ExecuteInEditMode]
public class ViewModeAreaUnbounded : MonoBehaviour {

    #region Inspector Properties

    [Tooltip("The camera size of the view mode.")]
    public float size = 5;

    #endregion

    #region Getting ViewMode

    /// <summary>
    /// The ViewMode created by this ViewModeArea.
    /// </summary>
    public CameraControl.ViewMode viewMode {
        get {
            if (useCache) {
                return cachedViewMode;
            }
            return createViewMode();
        }
    }

    #endregion

    #region Creating ViewMode (to override)

    /// <summary>
    /// Creates viewMode from the components attached to the GameObject.
    /// </summary>
    protected virtual CameraControl.ViewMode createViewMode() {
        return new CameraControl.ViewMode(transform.position, size);
    }

    #endregion

    #region Drawing Gizmos (to override)

    protected virtual void OnDrawGizmos() {

    }

    protected virtual void OnDrawGizmosSelected() {

        drawFullBox(Color.gray, Vector2.zero, size * Screen.currentResolution.width / Screen.currentResolution.height, size);

    }

    protected void drawFullBox(Color color, Vector2 localCenter, float localHalfWidth, float localHalfHeight) {
        
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(localCenter.x - localHalfWidth, localCenter.y - localHalfHeight);
        points[1] = new Vector2(localCenter.x + localHalfWidth, localCenter.y - localHalfHeight);
        points[2] = new Vector2(localCenter.x + localHalfWidth, localCenter.y + localHalfHeight);
        points[3] = new Vector2(localCenter.x - localHalfWidth, localCenter.y + localHalfHeight);
        for (int i = 0; i < points.Length; i++) {
            points[i] = transform.TransformPoint(points[i]);
        }

        Gizmos.color = color;
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[1], points[2]);
        Gizmos.DrawLine(points[2], points[3]);
        Gizmos.DrawLine(points[3], points[0]);

    }

    protected void drawCorners(Color color, Vector2 localCenter, float localHalfWidth, float localHalfHeight) {
        
        float size = 1;
        float cornerW = Mathf.Min(size, localHalfWidth);
        float cornerH = Mathf.Min(size, localHalfHeight);

        Vector2[] points = new Vector2[12];

        points[0] = new Vector2(localCenter.x - localHalfWidth, localCenter.y - localHalfHeight);
        points[1] = points[0] + new Vector2(cornerW, 0);
        points[2] = points[0] + new Vector2(0, cornerH);

        points[3] = new Vector2(localCenter.x + localHalfWidth, localCenter.y - localHalfHeight);
        points[4] = points[3] - new Vector2(cornerW, 0);
        points[5] = points[3] + new Vector2(0, cornerH);

        points[6] = new Vector2(localCenter.x + localHalfWidth, localCenter.y + localHalfHeight);
        points[7] = points[6] - new Vector2(cornerW, 0);
        points[8] = points[6] - new Vector2(0, cornerH);

        points[9] = new Vector2(localCenter.x - localHalfWidth, localCenter.y + localHalfHeight);
        points[10] = points[9] + new Vector2(cornerW, 0);
        points[11] = points[9] - new Vector2(0, cornerH);

        for (int i = 0; i < points.Length; i++) {
            points[i] = transform.TransformPoint(points[i]);
        }

        Gizmos.color = color;
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[0], points[2]);

        Gizmos.DrawLine(points[3], points[4]);
        Gizmos.DrawLine(points[3], points[5]);

        Gizmos.DrawLine(points[6], points[7]);
        Gizmos.DrawLine(points[6], points[8]);

        Gizmos.DrawLine(points[9], points[10]);
        Gizmos.DrawLine(points[9], points[11]);

    }

    #endregion

    #region Unity Functions

    protected void Awake() {
        
#if !UNITY_EDITOR
        // cache viewMode first
        cachedViewMode = createViewMode();
        useCache = true;
#endif

    }

    protected void Update() {

        transform.localScale = new Vector3(1, 1, 1);

    }

    #endregion

    #region Private

    private bool useCache = false;
    private CameraControl.ViewMode cachedViewMode = new CameraControl.ViewMode();

    #endregion

}
