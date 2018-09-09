using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class PipeMesh : MonoBehaviour {

    [Header("Pipe Dimensions")]

    [Tooltip("Thickness of the pipe")]
    public float thickness = 1f;
    [Tooltip("arcLengths[i] is the arc length for the curve at point i.  Note for pipes that don't wrap is first curve is at index 1.")]
    public float[] arcLengths;
    [Tooltip("Arc distance between each step in a curve")]
    public float arcStepLength = .1f;

    [Header("Obtaining Points")]

    [Tooltip("How the points for this line are obtained")]
    public PointsMode pointsMode = PointsMode.MANUAL_LOCAL;
    [Tooltip("If the collider points are taken from should be disabled when the game starts.")]
    public bool disablePtCollider = true;
    [Tooltip("For setting the points manually (pointsMode is MANUAL_LOCAL or MANUAL_GLOBAL)")]
    public Vector2[] manualPoints;
    [Tooltip("For setting the wrap mode manually (pointsMode is MANUAL_LOCAL or MANUAL_GLOBAL)")]
    public bool manualWrap = false;

    [Header("Texture Scale (applied in play mode)")]
    
    [Tooltip("Inverse of this becomes the material's texture scale x.")]
    public float textureWidth = 1;
    [Tooltip("When true, will offset texture scale x a bit to make sure it fits with the length of the pipe.")]
    public bool textureScaleFit = true;


    [Header("Updating")]

    [Tooltip("If MeshFilter is set every frame.  If not, will need to call updateMesh() after modifying points")]
    public bool updateEachFrame = false;

    /// <summary>
    /// Gets the number of segments.  This is the number of curves and straights, and includes ignored segments.
    /// Is equal to the number of points used to define the pipe multiplied by 2.
    /// </summary>
    public int numSegments {
        get {
            return accumulatedSegmentEndLengths.Length;
        }
    }

    /// <summary>
    /// Gets if the segment at the given index is a curve.  If not, it's a straight.
    /// </summary>
    public bool segmentIsCurve(int segmentIndex) {
        return segmentIndex % 2 == 0;
    }

    /// <summary>
    /// Gets the length of the segment at the given index.  Is 0 if the segment is ignored.
    /// </summary>
    public float segmentLength(int segmentIndex) {
        return accumultedLengthAt(segmentIndex + 1) - accumultedLengthAt(segmentIndex);
    }

    /// <summary>
    /// Gets if the segment at the given index is ignored.
    /// </summary>
    public bool segmentIsIgnored(int segmentIndex) {
        if (segmentIndex < 0 || segmentIndex >= numSegments) return true;
        if (segmentIndex % 2 == 0) {
            if (segmentIndex / 2 >= curves.Length) return true; // failsafe
            return curves[segmentIndex / 2].ignore;
        } else {
            if (segmentIndex / 2 >= straights.Length) return true; // failsafe
            return straights[segmentIndex / 2].ignore;
        }
    }

    /// <summary>
    /// Gets the point at a position in the segment.
    /// </summary>
    /// <param name="segmentIndex">Index of the segment.</param>
    /// <param name="distanceAlongSegment">Distance from the start of the segment.</param>
    /// <param name="centerOffset">Offset in a perpendicular direction from the center.  Positive value is to the left, negative is to the right.</param>
    public Vector2 segmentPointAt(int segmentIndex, float distanceAlongSegment, float centerOffset) {
        if (segmentIndex < 0) return new Vector2();
        if (segmentIndex >= numSegments) return new Vector2();
        if (segmentIndex % 2 == 0) {
            // curve
            Curve curve = curves[segmentIndex / 2];
            if (curve.ignore || curve.arcLength < .00001f) {
                return curve.getPoint(0, centerOffset);
            }
            return curve.getPoint(distanceAlongSegment / curve.arcLength, centerOffset);
        } else {
            // straight
            Straight straight = straights[segmentIndex / 2];
            if (straight.ignore || straight.length < .00001f) {
                return straight.getPoint(0, centerOffset);
            }
            return straight.getPoint(distanceAlongSegment / straight.length, centerOffset);
        }
    }
    
    /// <summary>
    /// Gets the point at a position in pipe.
    /// </summary>
    /// <param name="distanceAlongPipe">Distance from the start of the pipe.</param>
    /// <param name="centerOffset">Offset in a perpendicular direction from the center.  Positive value is to the left, negative is to the right.</param>
    public Vector2 pointAt(float distanceAlongPipe, float centerOffset) {
        float dist = distanceAlongPipe;
        if (wrap) {
            dist -= Mathf.Floor(dist / length) * length;
        } else {
            dist = Mathf.Clamp(dist, 0, length - .00001f);
        }

        int segmentIndex = getSegmentIndex(distanceAlongPipe);
        return segmentPointAt(segmentIndex, dist - segmentLength(segmentIndex), centerOffset);
    }

    /// <summary>
    /// Gets the index of the segment at the given length along the pipe.  Ignored segments are not included, since their lengths would be 0.
    /// </summary>
    /// <param name="distanceAlongPipe"></param>
    public int getSegmentIndex(float distanceAlongPipe) {
        float dist = distanceAlongPipe;
        if (wrap) {
            dist -= Mathf.Floor(dist / length) * length;
        } else {
            dist = Mathf.Clamp(dist, 0, length - .00001f);
        }
        for (int i=0; i < numSegments; i++) {
            if (segmentIsIgnored(i))
                continue;
            if (dist < accumultedLengthAt(i + 1))
                return i;
        }
        return numSegments - 1;
    }

    /// <summary>
    /// Total length (or perimeter) of the center of the pipe.
    /// </summary>
    public float length {
        get {
            return accumulatedSegmentEndLengths[numSegments - 1];
        }
    }

    /// <summary>
    /// Accumulated length at the start of the given segment.  Ignored segments (which have a length of 0) are included.
    /// 0 returns 0, 1 returns length of the first segment, 2 returns length of the first 2 segments, and so on.
    /// numSegments returns the length of the entire pipe.
    /// </summary>
    public float accumultedLengthAt(int segmentIndex) {
        
        if (segmentIndex <= 0)
            return 0;
        if (segmentIndex >= numSegments) {
            return length;
        }

        return accumulatedSegmentEndLengths[segmentIndex - 1];
    }

    /// <summary>
    /// If this pipe wraps around to the beginning.
    /// </summary>
    public bool wrap {
        get; private set;
    }
    
    


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
    

    public void updateMesh() {

        //// debug:
        //if (!Application.isPlaying)
        //    return;

        if (!enabled) {
            clearMesh();
            return;
        }

        Vector2[] pts = null;
        switch (pointsMode) {
        case PointsMode.MANUAL_LOCAL:
            setMeshFilter(manualPoints, manualWrap, thickness, arcLengths);
            break;

        case PointsMode.MANUAL_GLOBAL:
            if (manualPoints != null) {
                pts = new Vector2[manualPoints.Length];
                for (int i = 0; i < pts.Length; i++) {
                    pts[i] = transform.InverseTransformPoint(manualPoints[i]);
                }
            }
            setMeshFilter(pts, manualWrap, thickness, arcLengths);
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
            setMeshFilter(pts, false, thickness, arcLengths);
            if (disablePtCollider && Application.isPlaying) {
                ec2d.enabled = false;
            }
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
            setMeshFilter(pts, true, thickness, arcLengths);
            if (disablePtCollider && Application.isPlaying) {
                pc2d.enabled = false;
            }
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
            setMeshFilter(box, true, thickness, arcLengths);
            if (disablePtCollider && Application.isPlaying) {
                bc2d.enabled = false;
            }
            break;

        }


        if (Application.isPlaying) {
            updateTextureScale();
        }

    }


    void updateTextureScale() {

        // modifying material should only be done in play mode.
        if (!Application.isPlaying) return;

        Material material = GetComponent<MeshRenderer>().material;

        // applying texture scale
        float scaleX = Mathf.Approximately(textureWidth, 0) ? 1 : length / textureWidth;
        if (textureScaleFit) {
            bool sign = scaleX > 0;
            scaleX = Mathf.Round(scaleX);
            if (scaleX == 0)
                scaleX = sign ? 1 : -1;
        }
        material.mainTextureScale = new Vector2(scaleX, material.mainTextureScale.y);

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

        // inspector properties bounds
        arcStepLength = Mathf.Max(.001f, arcStepLength);
        textureWidth = Mathf.Max(.001f, textureWidth);


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
        
        if (!Application.isPlaying ||
            updateEachFrame)
            updateMesh();

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

        straights = new Straight[0];
        curves = new Curve[0];
        accumulatedSegmentEndLengths = new float[0];
    }

    struct Straight {
        /// <summary>
        /// set to true to ignore this part.
        /// </summary>
        public bool ignore;
        /// <summary>
        /// index of the point from
        /// </summary>
        public int indexFrom;
        /// <summary>
        /// index of the point to
        /// </summary>
        public int indexTo;

        /// <summary>
        /// point from, offset by previous curve's dist.
        /// </summary>
        public Vector2 vFrom;
        /// <summary>
        /// point to, offset by next curve's dist.
        /// </summary>
        public Vector2 vTo;

        public float length {
            get {
                return Vector2.Distance(vFrom, vTo);
            }
        }

        /// <summary>
        /// Returns the normalized normal (pointing to the left)
        /// </summary>
        public Vector2 normal {
            get {
                return normalVector(vTo - vFrom);
            }
        }

        /// <summary>
        /// Gets point a distance away from the center line.
        /// </summary>
        /// <param name="t">Distance in [0, 1] up the line from vFrom.</param>
        /// <param name="radius">Distance along the normal of the line.  Positive will be to the left, negative to the right.</param>
        public Vector2 getPoint(float t, float radius) {
            return vFrom + (vTo - vFrom) * t + normal * radius;
        }

    }

    struct Curve {
        /// <summary>
        /// set to true to ignore this part.
        /// </summary>
        public bool ignore;
        /// <summary>
        /// index of the point
        /// </summary>
        public int pointIndex;
        /// <summary>
        /// angle formed between the point and its two adjacent points (radians)
        /// </summary>
        public float angle;
        /// <summary>
        /// angle of the arc of the curve (radians)
        /// </summary>
        public float theta {
            get {
                return Mathf.PI - angle;
            }
        }
        /// <summary>
        /// radius of the arc
        /// </summary>
        public float radius {
            get {
                return arcLength / theta;
            }
        }
        /// <summary>
        /// length of the arc
        /// </summary>
        public float arcLength;

        /// <summary>
        /// distance subtracted from adjacent straight pieces to place the curve
        /// </summary>
        public float dist {
            get {
                return radius / Mathf.Tan(angle / 2);
            }
        }

        /// <summary>
        /// center point of the circle the arc is a part of
        /// </summary>
        public Vector2 center;

        
        public float startAngle;

        public bool clockwise;

        /// <summary>
        /// Gets point a distance away from the center arc.
        /// </summary>
        /// <param name="t">Distance in [0, 1] up the arc from startAngle.</param>
        /// <param name="radiusOffset">Added/subtracted to the radius of the arc.  Positive will always go in left direction.</param>
        public Vector2 getPoint(float t, float radiusOffset) {
            float angle = startAngle;
            if (clockwise) {
                angle -= theta * t;
            } else {
                angle += theta * t;
            }
            float c = Mathf.Cos(angle);
            float s = Mathf.Sin(angle);
            if (clockwise) {
                return center + new Vector2(c, s) * (radius + radiusOffset);
            } else {
                return center + new Vector2(c, s) * (radius - radiusOffset);
            }
        }

    }

    void setMeshFilter(Vector2[] points, bool wrap, float thickness, float[] arcLengths) {

        this.wrap = wrap;

        if (points == null || points.Length < 2) {
            clearMesh();
            return;
        }
        
        // straights[i] connects points points[i] and points[i+1].
        // curves[i] is at point points[i] and is followed by straight[i].  It has an arcLength of arcLengths[i].
        // even indexed segments are curves, odd indexed segmetns are straights.
        // curves[i] is segments[i * 2], straights[i] is segments[i * 2 + 1].
        // accumulatedSegmentLengths[i] is the accumulated length up until segment i.

        int numPoints = points.Length;
        straights = new Straight[numPoints];
        curves = new Curve[numPoints];
        accumulatedSegmentEndLengths = new float[numPoints * 2];
        float halfWidth = thickness / 2;

        // create curves
        for (int i=0; i < numPoints; i++) {

            int next = (i + 1) % numPoints;
            int prev = i == 0 ? numPoints - 1 : i - 1;
            Vector2 point = points[i];
            Vector2 pointNext = points[next];
            Vector2 pointPrev = points[prev];
            float angle = angleBetweenVectors(pointPrev - point, pointNext - point);
            
            Curve curve = new Curve();
            curve.pointIndex = i;
            if ((wrap || (i > 0 && i < numPoints - 1)) && // ignore if curve is on edge and not wrapping.
                !Mathf.Approximately(angle, 0) && !Mathf.Approximately(angle, Mathf.PI)) { // ignore if angle is 0 or pi.
                curve.angle = angle;
                curve.arcLength = i < arcLengths.Length ? arcLengths[i] : 0;
                // radius can't be smaller than the pipe half-width.  This restricts how small the arc length can be.
                curve.arcLength = Mathf.Max(curve.arcLength, halfWidth * curve.theta);
                
                curve.center = point + vectorBisector(pointPrev - point, pointNext - point) * Mathf.Sqrt(curve.dist * curve.dist + curve.radius * curve.radius);
                Vector2 startPt = point - (point - pointPrev).normalized * curve.dist;
                curve.startAngle = Mathf.Atan2(startPt.y - curve.center.y, startPt.x - curve.center.x);
                curve.clockwise = pointToLeft(point, pointPrev, pointNext);
                
            } else {
                curve.ignore = true;
            }
            curves[i] = curve;

        }

        // create straights
        for (int i=0; i < numPoints; i++) {

            int next = (i + 1) % numPoints;

            Straight straight = new Straight();
            straight.indexFrom = i;
            straight.indexTo = next;
            if (!wrap && i == numPoints - 1) { // at edge and not wrapping
                straight.ignore = true;
                straights[i] = straight;
                continue;
            }

            Vector2 pFrom = points[i];
            Vector2 pTo = points[next];
            Vector2 diff = pTo - pFrom;
            float fullDist = diff.magnitude; // distance between points without curve dist

            float distFrom = curves[i].ignore ? 0 : curves[i].dist;
            if (distFrom > fullDist) {
                // error, dist of curve can't extent past the length of the straight.
                // resolve by ignoring previous curve
                Curve curve = curves[i];
                curve.ignore = true;
                curves[i] = curve;
                distFrom = 0;
            }
            float distTo = curves[next].ignore ? 0 : curves[next].dist;
            if (distFrom + distTo > fullDist) {
                // error, dist of curve can't extent past the length of the straight.
                // resolve by ignoring next curve
                Curve curve = curves[next];
                curve.ignore = true;
                curves[next] = curve;
                distTo = 0;
            }

            // calculate points
            diff.Normalize();
            straight.vFrom = pFrom + diff * distFrom;
            straight.vTo = pTo - diff * distTo;
            
            straights[i] = straight;
            
        }

        // get segment lengths
        float accLength = 0;
        for (int i=0; i < accumulatedSegmentEndLengths.Length; i++) {
            if (i % 2 == 0) {
                // curve
                if (!curves[i / 2].ignore) {
                    accLength += curves[i / 2].arcLength;
                }
            } else {
                // straight
                if (!straights[i / 2].ignore) {
                    accLength += straights[i / 2].length;
                }
            }
            accumulatedSegmentEndLengths[i] = accLength;
        }

        
        // vertices are divided into "left" and "right"
        // odd indexed vertices are left, even indexed vertices are right
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        
        
        float currentLength = 0;
        for (int i = 0; i < numSegments; i++) {

            if (segmentIsIgnored(i))
                continue;

            if (segmentIsCurve(i)) { // segment is curve
                float arcLength = curves[i / 2].arcLength;

                // create vertices for each step of the arc
                int numArcSteps = Mathf.CeilToInt(arcLength / arcStepLength);
                for (int j = 0; j <= numArcSteps; j++) {

                    if (j == 0 && vertices.Count > 0) {
                        // previous vertices exist, just use those
                        continue;
                    }

                    // left point
                    vertices.Add(segmentPointAt(i, j * arcLength / numArcSteps, halfWidth));
                    // right point
                    vertices.Add(segmentPointAt(i, j * arcLength / numArcSteps, -halfWidth));
                    
                    // adding uvs
                    float curPerim = currentLength + arcLength * j / numArcSteps;
                    // left
                    uvs.Add(new Vector2(curPerim / length, 1));
                    // right
                    uvs.Add(new Vector2(curPerim / length, 0));

                }

                currentLength += arcLength;

            } else { // segment is straight
                float straightLength = straights[i / 2].length;

                if (vertices.Count == 0) {
                    // previous vertices don't exist, create vertices at vFrom
                    // left
                    vertices.Add(segmentPointAt(i, 0, halfWidth));
                    // right
                    vertices.Add(segmentPointAt(i, 0, -halfWidth));
                    
                    // adding uvs
                    // left
                    uvs.Add(new Vector2(currentLength / length, 1));
                    // right
                    uvs.Add(new Vector2(currentLength / length, 0));
                }

                // create vertices at vTo
                // left
                vertices.Add(segmentPointAt(i, straightLength, halfWidth));
                // right
                vertices.Add(segmentPointAt(i, straightLength, -halfWidth));
                
                // adding uvs
                currentLength += straightLength;
                // left
                uvs.Add(new Vector2(currentLength / length, 1));
                // right
                uvs.Add(new Vector2(currentLength / length, 0));
            }
            

        }


        if (wrap) {
            // end and start vertices are the same

            // ...

        }


        // create triangles
        for (int i=0; i < vertices.Count - 2; i += 2) {

            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + 2);

            triangles.Add(i + 2);
            triangles.Add(i + 1);
            triangles.Add(i + 3);
        }

        
        // create mesh
        Mesh mesh;
        if (Application.isPlaying) {
            mesh = meshFilter.mesh;
        } else {
            mesh = new Mesh();
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

    }

    MeshFilter meshFilter;
    EdgeCollider2D ec2d;
    PolygonCollider2D pc2d;
    BoxCollider2D bc2d;

    private Straight[] straights = new Straight[0];
    private Curve[] curves = new Curve[0];
    private float[] accumulatedSegmentEndLengths = new float[0];

    private PointsMode prevPointsMode = PointsMode.MANUAL_LOCAL;


    // math functions:

    /// <summary>
    /// Returns the angle (in radians) formed between the two given vectors.
    /// </summary>
    /// <param name="v0">First vector to compare.</param>
    /// <param name="v1">Second vector to compare.</param>
    private static float angleBetweenVectors(Vector2 v0, Vector2 v1) {
        return Mathf.Acos(Vector2.Dot(v0, v1) / (v0.magnitude * v1.magnitude));
    }

    /// <summary>
    /// Returns a normalized vector that bisects the angle formed between the two given vectors.
    /// </summary>
    /// <param name="v0">First vector to compare.</param>
    /// <param name="v1">Second vector to compare.</param>
    private static Vector2 vectorBisector(Vector2 v0, Vector2 v1) {
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
    private static bool pointToLeft(Vector2 lineP0, Vector2 lineP1, Vector2 point) {
        return ((lineP1.x - lineP0.x) * (point.y - lineP0.y) - (lineP1.y - lineP0.y) * (point.x - lineP0.x)) > 0;
    }

    /// <summary>
    /// Returns the normalized normal of the given vector.  If looking in the direction of the given vector, the normal will face to the "left".
    /// </summary>
    /// <param name="v">The vector whose normal to calculate.</param>
    private static Vector2 normalVector(Vector2 v) {
        Vector2 ret = new Vector2(-v.y, v.x);
        return ret.normalized;
    }

}
