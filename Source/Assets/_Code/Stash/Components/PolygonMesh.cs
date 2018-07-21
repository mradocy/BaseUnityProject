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
            List<Vector2[]> paths = new List<Vector2[]>();
            for (int i = 0; i < pc2d.pathCount; i++) {
                Vector2[] path = pc2d.GetPath(i);
                paths.Add(new Vector2[path.Length]);
                for (int j = 0; j < path.Length; j++) {
                    paths[i][j] = path[j] + pc2d.offset;
                }
            }
            setMeshFilter(paths.ToArray());
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
        if (points == null) {
            clearMesh();
            return;
        }
        List<Vector2[]> paths = new List<Vector2[]>();
        paths.Add(points);
        setMeshFilter(paths.ToArray());
    }

    void setMeshFilter(Vector2[][] paths) {
        // clear mesh if too few points
        if (paths == null) {
            clearMesh();
            return;
        }
        int numPoints = 0;
        for (int i = 0; i < paths.Length; i++) {
            if (paths[i] == null) continue;
            numPoints += paths[i].Length;
        }
        if (numPoints < 3) {
            clearMesh();
            return;
        }

        // select mesh
        Mesh mesh;
        if (Application.isPlaying) {
            mesh = meshFilter.mesh;
            mesh.Clear();
        } else {
            mesh = new Mesh();
        }
        
        // set vertices and find bounds
        Vector3[] vertices = new Vector3[numPoints];
        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        float yMin = float.MaxValue;
        float yMax = float.MinValue;
        int pointCount = 0;
        for (int i = 0; i < paths.Length; i++) {
            if (paths[i] == null || paths[i].Length < 3) continue;
            for (int j=0; j < paths[i].Length; j++) {
                Vector2 pt = paths[i][j];
                vertices[pointCount] = pt;
                xMin = Mathf.Min(xMin, pt.x);
                xMax = Mathf.Max(xMax, pt.x);
                yMin = Mathf.Min(yMin, pt.y);
                yMax = Mathf.Max(yMax, pt.y);
                pointCount++;
            }
        }
        mesh.vertices = vertices;

        // set uvs point positions relative to the bounds
        Vector2[] uvs = new Vector2[numPoints];
        pointCount = 0;
        for (int i = 0; i < paths.Length; i++) {
            if (paths[i] == null || paths[i].Length < 3) continue;
            for (int j = 0; j < paths[i].Length; j++) {
                Vector2 pt = paths[i][j];
                uvs[pointCount].x = (pt.x - xMin) / (xMax - xMin);
                uvs[pointCount].y = (pt.y - yMin) / (yMax - yMin);
                pointCount++;
            }
        }
        mesh.uv = uvs;

        // create triangles from paths
        List<int> allTris = new List<int>();
        pointCount = 0;
        for (int i=0; i < paths.Length; i++) {
            if (paths[i] == null || paths[i].Length < 3) continue;

            Triangulator triangulator = new Triangulator(paths[i]);
            int[] tris = triangulator.Triangulate();
            for (int j=0; j < tris.Length; j++) {
                tris[j] += pointCount;
            }

            allTris.AddRange(tris);

            pointCount += paths[i].Length;
        }
        mesh.triangles = allTris.ToArray();

        // finalize mesh
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