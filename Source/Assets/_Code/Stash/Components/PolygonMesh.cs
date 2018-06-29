using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
/// <summary>
/// Builds a Mesh for a gameObject using the PolygonCollider2D's path
/// </summary>
public class PolygonMesh : MonoBehaviour {

    [Tooltip("If MeshFilter is set every frame.  If not, will need to call updateMesh() after modifying points")]
    public bool updateEachFrame = true;

    [Tooltip("How the points for this line are obtained")]
    public PointsMode pointsMode = PointsMode.MANUAL_LOCAL;

    [Tooltip("For setting the points manually (pointsMode is MANUAL_LOCAL or MANUAL_GLOBAL)")]
    public Vector2[] manualPoints;

    public enum PointsMode {
        MANUAL_LOCAL,
        MANUAL_GLOBAL,
        POLYGON_COLLIDER_2D,
        BOX_COLLIDER_2D,
    }

    /// <summary>
    /// Updates the mesh based on the points of the attached PolygonCollider2D.
    /// </summary>
    public void updateMesh() {

        switch (pointsMode) {
        case PointsMode.MANUAL_LOCAL:
            setMeshFilter(manualPoints);
            break;

        case PointsMode.MANUAL_GLOBAL:
            Vector2[] pts = new Vector2[manualPoints.Length];
            for (int i = 0; i < pts.Length; i++) {
                pts[i] = transform.InverseTransformPoint(manualPoints[i]);
            }
            setMeshFilter(pts);
            break;

        case PointsMode.POLYGON_COLLIDER_2D:
            if (pc2d == null) {
                pc2d = GetComponent<PolygonCollider2D>();
                if (pc2d == null) {
                    clearMesh();
                    return;
                }
            }
            Vector2[] path = pc2d.GetPath(0);
            Vector2[] pathPoints = new Vector2[path.Length];
            for (int i = 0; i < path.Length; i++) {
                pathPoints[i] = path[i] + pc2d.offset;
            }
            setMeshFilter(pathPoints);
            break;

        case PointsMode.BOX_COLLIDER_2D:
            if (bc2d == null) {
                bc2d = GetComponent<BoxCollider2D>();
                if (bc2d == null) {
                    clearMesh();
                    return;
                }
            }
            Vector2[] boxPoints = new Vector2[4];
            boxPoints[0] = bc2d.offset + new Vector2(-bc2d.size.x / 2, -bc2d.size.y / 2);
            boxPoints[1] = bc2d.offset + new Vector2(bc2d.size.x / 2, -bc2d.size.y / 2);
            boxPoints[2] = bc2d.offset + new Vector2(bc2d.size.x / 2, bc2d.size.y / 2);
            boxPoints[3] = bc2d.offset + new Vector2(-bc2d.size.x / 2, bc2d.size.y / 2);            
            setMeshFilter(boxPoints);
            break;
        }
        
    }

    void Awake() {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        pc2d = gameObject.GetComponent<PolygonCollider2D>();
        bc2d = gameObject.GetComponent<BoxCollider2D>();
    }

    void clearMesh() {

        Mesh mesh;
        if (Application.isPlaying) {
            mesh = meshFilter.mesh;
        } else {
            mesh = new Mesh();
        }

        mesh.Clear();
        meshFilter.mesh = mesh;
    }

    void setMeshFilter(Vector2[] points) {
        if (points == null || points.Length < 2) {
            clearMesh();
            return;
        }

        Mesh mesh;
        if (Application.isPlaying) {
            mesh = meshFilter.mesh;
            mesh.Clear();
        } else {
            mesh = new Mesh();
        }

        Vector3[] vertices = new Vector3[points.Length];
        Vector2[] uvs = new Vector2[points.Length];
        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        float yMin = float.MaxValue;
        float yMax = float.MinValue;
        for (int i = 0; i < points.Length; i++) {
            Vector2 pt = points[i];
            vertices[i] = pt;
            xMin = Mathf.Min(xMin, pt.x);
            xMax = Mathf.Max(xMax, pt.x);
            yMin = Mathf.Min(yMin, pt.y);
            yMax = Mathf.Max(yMax, pt.y);
        }
        for (int i = 0; i < points.Length; i++) {
            Vector2 pt = points[i];
            uvs[i].x = (pt.x - xMin) / (xMax - xMin);
            uvs[i].y = (pt.y - yMin) / (yMax - yMin);
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = new Triangulator(points).Triangulate();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
    }

    void Start() {
        updateMesh();
    }
    
    void Update() {

        if (!Application.isPlaying ||
            updateEachFrame)
            updateMesh();

    }

    protected PolygonCollider2D pc2d;
    protected BoxCollider2D bc2d;
    protected MeshFilter meshFilter;
}

class Triangulator {
    private List<Vector2> mPoints = new List<Vector2>();

    public Triangulator(Vector2[] points) {
        mPoints = new List<Vector2>(points);
    }

    public int[] Triangulate() {
        List<int> indices = new List<int>();

        int n = mPoints.Count;
        if (n < 3) return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0) {
            for (int v = 0; v < n; v++)
                V[v] = v;
        } else {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;) {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V)) {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area() {
        int n = mPoints.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++) {
            Vector2 pval = mPoints[p];
            Vector2 qval = mPoints[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V) {
        int p;
        Vector2 A = mPoints[V[u]];
        Vector2 B = mPoints[V[v]];
        Vector2 C = mPoints[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++) {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = mPoints[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P) {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}