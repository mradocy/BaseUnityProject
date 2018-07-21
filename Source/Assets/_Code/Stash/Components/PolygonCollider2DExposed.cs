using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allows user to directly set points of PolygonCollider2D in the inspector.
/// Can also create multiple paths.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonCollider2DExposed : MonoBehaviour {
    
    [System.Serializable]
    public class Points {
        [System.Serializable]
        public class Path {
            public List<Vector2> points = new List<Vector2>();
        }
        public List<Path> paths = new List<Path>();
    }
    public Points points = new Points();

    /// <summary>
    /// Update inspector properties from polygonCollider2D.
    /// </summary>
    public void updateInspector() {
        PolygonCollider2D pc2d = GetComponent<PolygonCollider2D>();
        if (pc2d == null) return;

        // set same number of paths
        while (points.paths.Count < pc2d.pathCount) {
            points.paths.Add(new Points.Path());
        }
        while (points.paths.Count > pc2d.pathCount) {
            points.paths.RemoveAt(points.paths.Count - 1);
        }

        // match points
        for (int i=0; i < pc2d.pathCount; i++) {
            Vector2[] path = pc2d.GetPath(i);
            // set same number of points
            while (points.paths[i].points.Count < path.Length) {
                points.paths[i].points.Add(new Vector2());
            }
            while (points.paths[i].points.Count > path.Length) {
                points.paths[i].points.RemoveAt(points.paths[i].points.Count - 1);
            }
            // match points
            for (int j=0; j < path.Length; j++) {
                points.paths[i].points[j] = path[j];
            }
        }
        
    }

    /// <summary>
    /// Update polygonCollider2D from the inspector properties.
    /// </summary>
    public void updateCollider() {
        PolygonCollider2D pc2d = GetComponent<PolygonCollider2D>();
        if (pc2d == null) return;

        // set same number of paths
        int pathCount = Mathf.Max(1, points.paths.Count);
        pc2d.pathCount = pathCount;
        for (int i=0; i < pathCount; i++) {
            
            if (i >= points.paths.Count ||
                points.paths[i].points.Count < 3) {

                // do edge case stuff
                List<Vector2> pts = new List<Vector2>();
                if (i < points.paths.Count) {
                    pts.AddRange(points.paths[i].points.ToArray());
                }
                while (pts.Count < 3) {
                    if (pts.Count == 0) {
                        pts.Add(Vector2.zero);
                    } else {
                        pts.Add(pts[pts.Count - 1]);
                    }
                }
                pc2d.SetPath(i, pts.ToArray());
                
            } else {
                pc2d.SetPath(i, points.paths[i].points.ToArray());
            }
            
        }
        
    }
    
    void Start() {
        updateInspector();
    }
    
    void Update() {
        updateInspector();
    }
    
    void OnValidate() {
        updateCollider();
    }

}
