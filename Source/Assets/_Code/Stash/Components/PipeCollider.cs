using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(PolygonCollider2D))]
public class PipeCollider : MonoBehaviour {

    [Tooltip("Radius of the outer part of the collider.")]
    public float outerRadius = .3f;
    [Tooltip("Radius of the inner part of the collider.  Set to 0 to not leave a space in the middle.")]
    public float innerRadius = .25f;

    [Tooltip("Arc distance between each step in a curve")]
    public float arcStepLength = .1f;

    [Tooltip("If collider is updated every frame.  If not, will need to call updateCollider() whenever points are changed in the PipeMesh.")]
    public bool updateEachFrame = false;

    void Start() {
        pc2d = GetComponent<PolygonCollider2D>();
        updateCollider();
	}
    
    void Update() {

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // inspector properties bounds
        arcStepLength = Mathf.Max(.01f, arcStepLength);
        innerRadius = Mathf.Max(0, innerRadius);
        outerRadius = Mathf.Max(innerRadius + .01f, outerRadius);

        if (!Application.isPlaying || updateEachFrame) {
            updateCollider();
        }

    }

    void OnEnable() {
        updateCollider();
    }
    void OnDisable() {
        updateCollider();
    }

    public void updateCollider() {

        clearCollider();

        if (pc2d == null) return;

        // getting PipeMesh from parent
        pipeMesh = null;
        if (transform.parent != null) {
            pipeMesh = transform.parent.GetComponent<PipeMesh>();
        }
        if (pipeMesh == null) {
            Debug.LogWarning("PipeCollider's parent must have the PipeMesh component.");
            return;
        }
        clearCollider();

        if (!enabled) return;


        List<Vector2> leftPoints = new List<Vector2>();
        List<Vector2> leftInnerPoints = new List<Vector2>();
        List<Vector2> rightPoints = new List<Vector2>();
        List<Vector2> rightInnerPoints = new List<Vector2>();

        for (int i=0; i < pipeMesh.numSegments; i++) {
            if (pipeMesh.segmentIsIgnored(i)) continue;
            if (pipeMesh.segmentIsCurve(i)) {
                // is curve.  create points for each step of the curve's arc
                int numArcSteps = Mathf.CeilToInt(pipeMesh.segmentLength(i) / arcStepLength);
                for (int j = 0; j <= numArcSteps; j++) {

                    if (j == 0 && leftPoints.Count > 0) {
                        // previous points exist, just use those
                        continue;
                    }

                    // left point
                    leftPoints.Add(pipeMesh.segmentPointAt(i, j * pipeMesh.segmentLength(i) / numArcSteps, outerRadius));
                    leftInnerPoints.Add(pipeMesh.segmentPointAt(i, j * pipeMesh.segmentLength(i) / numArcSteps, innerRadius));

                    // right point
                    rightPoints.Add(pipeMesh.segmentPointAt(i, j * pipeMesh.segmentLength(i) / numArcSteps, -outerRadius));
                    rightInnerPoints.Add(pipeMesh.segmentPointAt(i, j * pipeMesh.segmentLength(i) / numArcSteps, -innerRadius));
                    
                }
            } else {
                // is straight.  create points at start and end
                if (leftPoints.Count == 0) {
                    // previous points don't exist, make them

                    // left point
                    leftPoints.Add(pipeMesh.segmentPointAt(i, 0, outerRadius));
                    leftInnerPoints.Add(pipeMesh.segmentPointAt(i, 0, innerRadius));

                    // right point
                    rightPoints.Add(pipeMesh.segmentPointAt(i, 0, -outerRadius));
                    rightInnerPoints.Add(pipeMesh.segmentPointAt(i, 0, -innerRadius));

                }

                // left point
                leftPoints.Add(pipeMesh.segmentPointAt(i, pipeMesh.segmentLength(i), outerRadius));
                leftInnerPoints.Add(pipeMesh.segmentPointAt(i, pipeMesh.segmentLength(i), innerRadius));

                // right point
                rightPoints.Add(pipeMesh.segmentPointAt(i, pipeMesh.segmentLength(i), -outerRadius));
                rightInnerPoints.Add(pipeMesh.segmentPointAt(i, pipeMesh.segmentLength(i), -innerRadius));

            }

        }

        // create collider
        if (leftPoints.Count < 2) return;

        bool prevEnabled = pc2d.enabled;
        pc2d.enabled = false; // disable collider to make setting paths faster...?


        if (innerRadius > 0) {
            // inner part is open
            pc2d.pathCount = 2;
            leftInnerPoints.Reverse();
            leftPoints.AddRange(leftInnerPoints);
            pc2d.SetPath(0, leftPoints.ToArray());
            rightInnerPoints.Reverse();
            rightPoints.AddRange(rightInnerPoints);
            pc2d.SetPath(1, rightPoints.ToArray());
        } else {
            // no inner points
            pc2d.pathCount = 1;
            rightPoints.Reverse();
            leftPoints.AddRange(rightPoints);
            pc2d.SetPath(0, leftPoints.ToArray());
        }
        
        pc2d.enabled = prevEnabled;
        
    }

    void clearCollider() {
        if (pc2d == null) return;
        pc2d.pathCount = 0;
    }

    PolygonCollider2D pc2d;
    PipeMesh pipeMesh;

}
