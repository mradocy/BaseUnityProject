using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class LineMesh : MonoBehaviour {

    [Tooltip("Thickness of the line")]
    public float thickness = .1f;

    [Tooltip("Limits how \"sharp\" points can be")]
    public float taperThreshold = .05f;
    
    [Tooltip("If MeshFilter is set every frame.  If not, will need to call updateMesh() after modifying points")]
    public bool updateEachFrame = true;

    [Tooltip("How the points for this line are obtained")]
    public PointsMode pointsMode = PointsMode.MANUAL_LOCAL;

    [Tooltip("For setting the points manually (pointsMode is MANUAL_LOCAL or MANUAL_GLOBAL)")]
    public Vector2[] manualPoints;

    [Tooltip("How uvs for the mesh are created")]
    public UVMode uvMode = UVMode.TRAIL;
    

    public enum PointsMode {
        MANUAL_LOCAL,
        MANUAL_GLOBAL,
        EDGE_COLLIDER_2D,
        POLYGON_COLLIDER_2D,
        BOX_COLLIDER_2D,
        PARENT_EDGE_COLLIDER_2D,
        PARENT_POLYGON_COLLIDER_2D,
        PARENT_BOX_COLLIDER_2D,
    }

    public enum UVMode {
        TRAIL,
        BOX,
    }
    
    public void updateMesh() {
        if (!enabled) {
            clearMesh();
            return;
        }

        Vector2[] pts = null;
        switch (pointsMode) {
        case PointsMode.MANUAL_LOCAL:
            setMeshFilter(manualPoints, false, thickness);
            break;

        case PointsMode.MANUAL_GLOBAL:
            if (manualPoints != null) {
                pts = new Vector2[manualPoints.Length];
                for (int i = 0; i < pts.Length; i++) {
                    pts[i] = transform.InverseTransformPoint(manualPoints[i]);
                }
            }
            setMeshFilter(pts, false, thickness);
            break;

        case PointsMode.EDGE_COLLIDER_2D:
        case PointsMode.PARENT_EDGE_COLLIDER_2D:
            if (ec2d == null) {
                if (pointsMode == PointsMode.PARENT_EDGE_COLLIDER_2D) {
                    if (transform.parent != null)
                        ec2d = transform.parent.GetComponent<EdgeCollider2D>();
                } else
                    ec2d = GetComponent<EdgeCollider2D>();
                if (ec2d == null) {
                    clearMesh();
                    return;
                }
            }
            Vector2[] edgePoints = ec2d.points;
            if (edgePoints.Length < 2)
                return;
            pts = new Vector2[edgePoints.Length];
            for (int i = 0; i < edgePoints.Length; i++) {
                pts[i] = edgePoints[i] + ec2d.offset;
            }
            setMeshFilter(pts, false, thickness);
            break;

        case PointsMode.POLYGON_COLLIDER_2D:
        case PointsMode.PARENT_POLYGON_COLLIDER_2D:
            if (pc2d == null) {
                if (pointsMode == PointsMode.PARENT_POLYGON_COLLIDER_2D) {
                    if (transform.parent != null)
                        pc2d = transform.parent.GetComponent<PolygonCollider2D>();
                } else
                    pc2d = GetComponent<PolygonCollider2D>();
                if (pc2d == null) {
                    clearMesh();
                    return;
                }
            }
            Vector2[] path = pc2d.GetPath(0);
            if (path.Length < 2)
                return;
            pts = new Vector2[path.Length];
            for (int i = 0; i < path.Length; i++) {
                pts[i] = path[i] + pc2d.offset;
            }
            setMeshFilter(pts, true, thickness);
            break;

        case PointsMode.BOX_COLLIDER_2D:
        case PointsMode.PARENT_BOX_COLLIDER_2D:
            if (bc2d == null) {
                if (pointsMode == PointsMode.PARENT_BOX_COLLIDER_2D) {
                    if (transform.parent != null)
                        bc2d = transform.parent.GetComponent<BoxCollider2D>();
                } else
                    bc2d = GetComponent<BoxCollider2D>();
                if (bc2d == null) {
                    clearMesh();
                    return;
                }
            }
            Vector2[] box = new Vector2[4];
            box[0] = bc2d.offset + new Vector2(-bc2d.size.x / 2, -bc2d.size.y / 2);
            box[1] = bc2d.offset + new Vector2(bc2d.size.x / 2, -bc2d.size.y / 2);
            box[2] = bc2d.offset + new Vector2(bc2d.size.x / 2, bc2d.size.y / 2);
            box[3] = bc2d.offset + new Vector2(-bc2d.size.x / 2, bc2d.size.y / 2);
            setMeshFilter(box, true, thickness);
            break;

        }
    }

    void Awake() {
        meshFilter = GetComponent<MeshFilter>();

    }

    void Start() {
        updateMesh();
    }

    void OnEnable() {
        updateMesh();
    }
    void OnDisable() {
        updateMesh();
    }

    /// <summary>
    /// If this component will force the gameObject to have the identity transform at all times.
    /// </summary>
    public bool forceIdentityTransform {
        get {
            return pointsMode == PointsMode.PARENT_EDGE_COLLIDER_2D || pointsMode == PointsMode.PARENT_POLYGON_COLLIDER_2D;
        }
    }
    
    void Update() {

        if (prevPointsMode != pointsMode) {
            ec2d = null;
            pc2d = null;
            prevPointsMode = pointsMode;
        }

        if (forceIdentityTransform) {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
            updateMesh();
#else
        if (updateEachFrame) {
            updateMesh();
        }
#endif
        
    }

    void clearMesh() {
#if UNITY_EDITOR
        Mesh mesh = new Mesh();
#else
        Mesh mesh = meshFilter.mesh;
#endif
        mesh.Clear();
        meshFilter.mesh = mesh;
    }

    void setMeshFilter(Vector2[] points, bool wrap, float thickness) {

        if (points == null || points.Length < 2) {
            clearMesh();
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        float h = thickness / 2; // half-thickness

        float lineLength = 0;
        for (int j=1; j<points.Length; j++) {
            lineLength += (points[j] - points[j-1]).magnitude;
        }
        if (wrap) {
            lineLength += (points[0] - points[points.Length - 1]).magnitude;
        }

        Vector2 pt, prevPt, nextPt, prevNormal, nextNormal, b; // b for bisector
        float a; // angle between line and bisector
        int rightIndex , leftIndex;

        // first point
        pt = points[0];
        nextPt = points[1];
        nextNormal = normalVector(nextPt - pt);

        vertices.Add(pt - nextNormal * h);
        uvs.Add(new Vector2(0, 0));
        rightIndex = 0;

        vertices.Add(pt + nextNormal * h);
        uvs.Add(new Vector2(0, 1));
        leftIndex = 1;


        // middle points
        float currentLength = 0;
        float inter = 0;
        int i = 1;
        for ( ; ; i++) {

            if (i == 0) {
                prevPt = points[points.Length - 1];
                pt = points[0];
                nextPt = points[1];
            } else if (i >= points.Length - 1) {
                if (wrap && i == points.Length - 1) {
                    prevPt = points[i - 1];
                    pt = points[i];
                    nextPt = points[0];
                } else if (wrap && i == points.Length) {
                    // when wrapping, set what'll be the last point in the middle points loop
                    prevPt = points[i - 1];
                    pt = points[0];
                    nextPt = points[1];
                } else
                    break;
            } else {
                prevPt = points[i - 1];
                pt = points[i];
                nextPt = points[i + 1];
            }
            
            prevNormal = normalVector(pt - prevPt);
            nextNormal = normalVector(nextPt - pt);
            currentLength += (pt - prevPt).magnitude;
            inter = currentLength / lineLength;
            b = vectorBisector(prevPt - pt, nextPt - pt);
            a = angleBetweenVectors(b, prevPt - pt);
            float taperLength = 0;
            if (a != 0) {
                taperLength = h / Mathf.Sin(a);
                if (taperLength*taperLength > Mathf.Min((pt - prevPt).sqrMagnitude, (nextPt - pt).sqrMagnitude)) {
                    // blamming a stupid edge case when the angle between vectors gets too small.  Don't feel like making nicer.
                    b *= Mathf.Min(taperLength, Mathf.Min((pt - prevPt).magnitude, (nextPt - pt).magnitude));
                } else {
                    b *= taperLength;
                }
                taperLength -= h;
            }

            if (Mathf.Abs(a - Mathf.PI / 2) < .0001f) { // straight line
                // no cap

                vertices.Add(pt - prevNormal * h);
                uvs.Add(new Vector2(inter, 0));
                triangles.AddRange(new int[] { leftIndex, rightIndex, vertices.Count - 1 });
                rightIndex = vertices.Count - 1;

                vertices.Add(pt + prevNormal * h);
                uvs.Add(new Vector2(inter, 1));
                triangles.AddRange(new int[] { leftIndex, vertices.Count - 2, vertices.Count - 1 });
                leftIndex = vertices.Count - 1;

            } else if (Mathf.Abs(a) < .0001f) { // turns 180 degrees
                // no cap, then flip left and right indicies

                vertices.Add(pt - prevNormal * h);
                uvs.Add(new Vector2(inter, 0));
                triangles.AddRange(new int[] { leftIndex, rightIndex, vertices.Count - 1 });
                rightIndex = vertices.Count - 1;

                vertices.Add(pt + prevNormal * h);
                uvs.Add(new Vector2(inter, 1));
                triangles.AddRange(new int[] { leftIndex, vertices.Count - 2, vertices.Count - 1 });

                leftIndex = rightIndex;
                rightIndex = vertices.Count - 1;

            } else if (taperLength <= taperThreshold) {
                // pointy cap

                Vector2 leftPt, rightPt;
                if (pointToLeft(prevPt, pt, pt + b)) {
                    // line bends to the left
                    leftPt = pt + b;
                    rightPt = pt - b;
                } else {
                    // line bends to the right
                    leftPt = pt - b;
                    rightPt = pt + b;
                }

                vertices.Add(rightPt);
                uvs.Add(new Vector2(inter, 0));
                triangles.AddRange(new int[] { leftIndex, rightIndex, vertices.Count - 1 });
                rightIndex = vertices.Count - 1;

                vertices.Add(leftPt);
                uvs.Add(new Vector2(inter, 1));
                triangles.AddRange(new int[] { leftIndex, vertices.Count - 2, vertices.Count - 1 });
                leftIndex = vertices.Count - 1;
                
            } else {
                // tapered cap
                
                Vector2 insidePt;
                Vector2 outsidePt0;
                Vector2 outsidePt1;
                if (pointToLeft(prevPt, pt, pt + b)) {
                    // line bends to the left
                    insidePt = pt + b; // inside is to the left
                    outsidePt0 = pt - prevNormal * h; // ouside is to the right
                    outsidePt1 = pt - nextNormal * h;

                    vertices.Add(outsidePt0);
                    uvs.Add(new Vector2(inter, 0));
                    triangles.AddRange(new int[] { leftIndex, rightIndex, vertices.Count - 1 });

                    vertices.Add(insidePt);
                    uvs.Add(new Vector2(inter, 1));
                    triangles.AddRange(new int[] { leftIndex, vertices.Count - 2, vertices.Count - 1 });
                    leftIndex = vertices.Count - 1; // leftIndex now set to the index of insidePt

                    vertices.Add(outsidePt1);
                    uvs.Add(new Vector2(inter, 0));
                    triangles.AddRange(new int[] { leftIndex, vertices.Count - 3, vertices.Count - 1 });
                    rightIndex = vertices.Count - 1; // rightIndex now set to the index of outsidePt1

                } else {
                    // line bends to the right
                    insidePt = pt + b; // inside is to the right
                    outsidePt0 = pt + prevNormal * h; // outside is to the left
                    outsidePt1 = pt + nextNormal * h;

                    vertices.Add(insidePt);
                    uvs.Add(new Vector2(inter, 0));
                    triangles.AddRange(new int[] { leftIndex, rightIndex, vertices.Count - 1 });
                    rightIndex = vertices.Count - 1; // rightIndex now set to the index of insidePt

                    vertices.Add(outsidePt0);
                    uvs.Add(new Vector2(inter, 1));
                    triangles.AddRange(new int[] { leftIndex, rightIndex, vertices.Count - 1 });

                    vertices.Add(outsidePt1);
                    uvs.Add(new Vector2(inter, 1));
                    triangles.AddRange(new int[] { rightIndex, vertices.Count - 2, vertices.Count - 1 });
                    leftIndex = vertices.Count - 1; // leftIndex now set to the index of outsidePt1

                }

            }

        }

        
        if (wrap) {            
            // last point was set during the middle points loop.  Instead, change the first point to match the current last point
            vertices[0] = vertices[rightIndex];
            vertices[1] = vertices[leftIndex];

        } else {
            // last point
            prevPt = points[points.Length - 2];
            pt = points[points.Length - 1];
            prevNormal = normalVector(prevPt - pt);

            vertices.Add(pt - nextNormal * h);
            uvs.Add(new Vector2(1, 0));
            triangles.AddRange(new int[] { leftIndex, rightIndex, vertices.Count - 1 });

            vertices.Add(pt + nextNormal * h);
            uvs.Add(new Vector2(1, 1));
            triangles.AddRange(new int[] { leftIndex, vertices.Count - 2, vertices.Count - 1 });
        }

        // create mesh
#if UNITY_EDITOR
        Mesh mesh = new Mesh();
#else
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();
#endif

        mesh.vertices = vertices.ToArray();
        if (uvMode == UVMode.TRAIL) {
            mesh.uv = uvs.ToArray();
        } else if (uvMode == UVMode.BOX) {
            // uvs as a bounding box
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            foreach (Vector3 vertex in vertices) {
                minX = Mathf.Min(minX, vertex.x);
                maxX = Mathf.Max(maxX, vertex.x);
                minY = Mathf.Min(minY, vertex.y);
                maxY = Mathf.Max(maxY, vertex.y);
            }
            for (int j=0; j<uvs.Count; j++) {
                uvs[j] = new Vector2((vertices[j].x - minX) / (maxX - minX), (vertices[j].y - minY) / (maxY - minY));
            }
            mesh.uv = uvs.ToArray();
        }
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

    }

    MeshFilter meshFilter;
    EdgeCollider2D ec2d;
    PolygonCollider2D pc2d;
    BoxCollider2D bc2d;

    private PointsMode prevPointsMode = PointsMode.MANUAL_LOCAL;


    // math functions:

    /// <summary>
    /// Returns the angle (in radians) formed between the two given vectors.
    /// </summary>
    /// <param name="v0">First vector to compare.</param>
    /// <param name="v1">Second vector to compare.</param>
    private float angleBetweenVectors(Vector2 v0, Vector2 v1) {
        return Mathf.Acos(Vector2.Dot(v0, v1) / (v0.magnitude * v1.magnitude));
    }

    /// <summary>
    /// Returns a normalized vector that bisects the angle formed between the two given vectors.
    /// </summary>
    /// <param name="v0">First vector to compare.</param>
    /// <param name="v1">Second vector to compare.</param>
    private Vector2 vectorBisector(Vector2 v0, Vector2 v1) {
        Vector2 b = v0.normalized + v1.normalized;
        float m = b.magnitude;
        if (Mathf.Abs(m) < .0001f) {
            return normalVector(v0);
        }
        return b / m;
    }

    /// <summary>
    /// Given a line defined by p0 and p1, returns if the point is to the left of the line ("left" when facing p1 from p0).
    /// </summary>
    /// <param name="lineP0">The first point that defines the line.</param>
    /// <param name="lineP1">The other point that defines the line.</param>
    /// <param name="point">The point to test.</param>
    private bool pointToLeft(Vector2 lineP0, Vector2 lineP1, Vector2 point) {
        return ((lineP1.x - lineP0.x) * (point.y - lineP0.y) - (lineP1.y - lineP0.y) * (point.x - lineP0.x)) > 0;
    }

    /// <summary>
    /// Returns the normalized normal of the given vector.  If looking in the direction of the given vector, the normal will face to the "left".
    /// </summary>
    /// <param name="v">The vector whose normal to calculate.</param>
    private Vector2 normalVector(Vector2 v) {
        Vector2 ret = new Vector2(-v.y, v.x);
        return ret.normalized;
    }

}
