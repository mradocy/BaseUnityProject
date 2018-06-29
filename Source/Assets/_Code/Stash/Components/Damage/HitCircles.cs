using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(HitObject))]
public class HitCircles : MonoBehaviour {
    
    public Circle[] circles;

    [Header("UI")]
    [Tooltip("Show position handles.")]
    public bool showPositionHandles = false;
    [Tooltip("Show radius handles.")]
    public bool showRadiusHandles = false;
    [Tooltip("Draw gizmos when not selected.")]
    public bool drawGizmosWhenNotSelected = true;
    [Tooltip("Draw gizmos when HitObject component is disabled.")]
    public bool drawGizmosWhenHitObjectIsDisabled = false;
    public Color gizmoColor = Color.red;

    [System.Serializable]
    public struct Circle {
        public Vector2 localOffset;
        public float localRadius;
        public Circle(Vector2 localOffset, float localRadius) {
            this.localOffset = localOffset;
            this.localRadius = localRadius;
        }
    }

    public float lossyScale {
        get {
            //return (Mathf.Abs(transform.lossyScale.x) + Mathf.Abs(transform.lossyScale.y)) / 2;
            return calculateGlobalScale(transform);
        }
    }

    /// <summary>
    /// Recersively finds a "good enough" value for the global scale of the given transform.
    /// </summary>
    float calculateGlobalScale(Transform transform) {
        float scale = (Mathf.Abs(transform.localScale.x) + Mathf.Abs(transform.localScale.y)) / 2;
        if (transform.parent != null) {
            scale *= calculateGlobalScale(transform.parent);
        }
        return scale;
    }

    /// <summary>
    /// Updates current transform, and sets previous transform to this transform.
    /// </summary>
    public void teleport() {
        setCurrentTransformCircles();
        setPrevTransformCircles();
    }
    
    void Awake() {
        hitObject = GetComponent<HitObject>();
        if (circles != null) {
            currentTransformedCircles = new Circle[circles.Length];
            prevTransformedCircles = new Circle[circles.Length];
        }
	}

    void Start() {
        teleport();
    }
    
    void Update() {

    }

    void OnEnable() {
        teleport();
    }

    void FixedUpdate() {

        if (currentTransformedCircles == null) return;
        if (!enabled) return;

        setCurrentTransformCircles();

        // do collision here
        List<HurtObject> hurtObjectsHit = new List<HurtObject>();
        for (int i=0; i<currentTransformedCircles.Length; i++) {

            Circle prevCircle = prevTransformedCircles[i];
            Circle currentCircle = currentTransformedCircles[i];

            // perform circle cast
            Vector2 origin = prevCircle.localOffset;
            float radius = prevCircle.localRadius;
            Vector2 diff = currentCircle.localOffset - prevCircle.localOffset;
            float distance = diff.magnitude;
            Vector2 direction = new Vector2();
            if (distance > .0001f) {
                direction = diff / distance;
            }
            int layerMask = getLayerCollisionMask(gameObject.layer);
            int numRH2D = Physics2D.CircleCastNonAlloc(origin, radius, direction, resultRH2D, distance, layerMask);
            for (int j=0; j<numRH2D; j++) {
                if (resultRH2D[j]) {
                    if (resultRH2D[j].collider == null) continue;
                    HurtObject hurtObject = resultRH2D[j].collider.GetComponent<HurtObject>();
                    if (hurtObject == null) continue;
                    if (hurtObjectsHit.Contains(hurtObject)) continue;
                    
                    hitObject.hitHurtObject(hurtObject);
                    hurtObjectsHit.Add(hurtObject);
                }
            }

        }
        setPrevTransformCircles();

        // remove objects not hit from hurt record
        foreach (int hurtObjectID in hurtObjectsHitLastFrame) {
            HurtObject hurtObject = HurtObject.getHurtObject(hurtObjectID);
            if (hurtObject == null) continue;
            if (!hurtObjectsHit.Contains(hurtObject)) {
                hitObject.hurtRecordRemove(hurtObject.globalHurtID);
            }
        }
        hurtObjectsHitLastFrame.Clear();
        foreach (HurtObject hurtObject in hurtObjectsHit) {
            hurtObjectsHitLastFrame.Add(hurtObject.globalHurtID);
        }
        
    }
    List<int> hurtObjectsHitLastFrame = new List<int>();


    /// <summary>
    /// Sets currentTransformedCircles[i] to the transformed version of circles[i].  Assumes arrays have already been made.
    /// </summary>
    void setCurrentTransformCircles() {
        for (int i=0; i<circles.Length; i++) {
            currentTransformedCircles[i].localOffset = transform.TransformPoint(circles[i].localOffset);
            currentTransformedCircles[i].localRadius = circles[i].localRadius * lossyScale;
        }
    }

    /// <summary>
    /// Sets prevTransformedCircles to match currentTransformedCircles.
    /// </summary>
    void setPrevTransformCircles() {
        for (int i = 0; i < currentTransformedCircles.Length; i++) {
            prevTransformedCircles[i] = currentTransformedCircles[i];
        }
    }

    Circle[] currentTransformedCircles = null;
    Circle[] prevTransformedCircles = null;

    RaycastHit2D[] resultRH2D = new RaycastHit2D[10];

    void OnDrawGizmos() {

        if (drawGizmosWhenNotSelected) {
            drawGizmosF();
        }
        
    }

    void OnDrawGizmosSelected() {

        if (!drawGizmosWhenNotSelected) {
            drawGizmosF();
        }

    }

    void drawGizmosF() {
        if (!enabled)
            return;

        if (circles == null) return;

        if (!drawGizmosWhenHitObjectIsDisabled) {
            if (hitObject == null) {
                hitObject = GetComponent<HitObject>();
            }
            if (hitObject == null || !hitObject.enabled)
                return;
        }
        
        foreach (Circle circle in circles) {
            Vector2 pos = transform.TransformPoint(circle.localOffset);
            float r = circle.localRadius * lossyScale;
            drawCircleGizmo(gizmoColor, pos, r);
        }
    }

    void drawCircleGizmo(Color color, Vector2 position, float radius) {
        int numPoints = 20;
        Vector3 from = new Vector3();
        Vector3 to = new Vector3();
        Gizmos.color = color;
        for (int i = 0; i < numPoints; i++) {
            float fromAngle = i * Mathf.PI * 2f / numPoints;
            float toAngle = (i+1) * Mathf.PI * 2f / numPoints;
            from = new Vector2(position.x + radius * Mathf.Cos(fromAngle), position.y + radius * Mathf.Sin(fromAngle));
            to = new Vector2(position.x + radius * Mathf.Cos(toAngle), position.y + radius * Mathf.Sin(toAngle));
            Gizmos.DrawLine(from, to);
        }
    }

    public HitObject hitObject { get; private set; }

    /// <summary>
    /// Get the collision layer mask that indicates which layer(s) the specified layer can collide with.
    /// </summary>
    int getLayerCollisionMask(int layer) {
        int ret = 0;
        for (int i = 0; i < 32; i++) {
            if (!Physics2D.GetIgnoreLayerCollision(layer, i)) {
                ret = ret | 1 << i;
            }
        }
        return ret;
    }

}
