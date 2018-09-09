using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Provides collision tools without modifying anything.
/// Performs short shape casts to check collision with nearby objects.  Triggers are not detected.
/// Shapes are defined by non-trigger colliders attached to the gameObject and its children.
/// </summary>
public class CollisionCaster : MonoBehaviour {

    /// <summary>
    /// One of four directions.
    /// </summary>
    public enum Direction {
        NONE,
        RIGHT,
        UP,
        LEFT,
        DOWN
    }
    
    /// <summary>
    /// Performs casts in the given direction, and return if any of them hit something.
    /// </summary>
    public bool touch(Direction direction) {
        int numResults = 0;
        checkCasts(direction, out numResults, touchCastDistance, Vector2.zero);
        return numResults > 0;
    }

    /// <summary>
    /// Performs casts in the given direction, and returns the results in an array.
    /// </summary>
    public RaycastHit2D[] touchResults(Direction direction) {
        int numResults = 0;
        checkCasts(direction, out numResults, touchCastDistance, Vector2.zero);
        RaycastHit2D[] ret = new RaycastHit2D[numResults];
        Array.Copy(tempResults, ret, numResults);
        return ret;
    }

    /// <summary>
    /// Performs casts in the given direction, and stores the results in the given array.  Returns the number of results.
    /// </summary>
    public int touchResultsNonAlloc(Direction direction, RaycastHit2D[] results) {
        int numResults = 0;
        checkCasts(direction, out numResults, touchCastDistance, Vector2.zero);
        Array.Copy(tempResults, results, numResults);
        return numResults;
    }

    /// <summary>
    /// Performs casts in the given direction, and returns the first result.  If nothing was hit, the collider property of the returned RaycastHit2D will be null.
    /// </summary>
    public RaycastHit2D touchResult(Direction direction) {
        int numResults = 0;
        checkCasts(direction, out numResults, touchCastDistance, Vector2.zero);
        if (numResults == 0) {
            RaycastHit2D noHit = new RaycastHit2D();
            return noHit;
        }

        // return earliest hit
        return earliestOfTempCastResults(numResults);
    }

    /// <summary>
    /// Performs casts in the given direction, and returns the normal of the collision.  Normal is averaged on multiple colliders.  Returns 0,0 if nothing was hit.
    /// </summary>
    public Vector2 touchNormal(Direction direction) {
        int numResults = 0;
        checkCasts(direction, out numResults, touchCastDistance, Vector2.zero);
        if (numResults == 0) {
            return Vector2.zero;
        }
        Vector2 n = new Vector2();
        for (int i=0; i < numResults; i++) {
            n += tempResults[i].normal;
        }
        return n.normalized;
    }
    
    /// <summary>
    /// Performs casts from all the colliders and returns the earliest result.  If the returned raycastHit2D.collider == null, then nothing was hit.
    /// </summary>
    /// <param name="directionVec">Normalized direction of the cast.</param>
    /// <param name="distance">Distance of the cast.</param>
    /// <param name="originOffset">Offsets the origin of all the shapes colliders.</param>
    /// <param name="normalRestriction">If given, cast will ignore all results with normals that go in a different direction.</param>
    public RaycastHit2D cast(Vector2 directionVec, float distance, Vector2 originOffset = new Vector2(), Direction normalRestriction = Direction.NONE) {

        int numResults;
        checkCasts(directionVec, normalRestriction, out numResults, distance, originOffset);
        if (numResults > 0) {
            return earliestOfTempCastResults(numResults);
        }
        return new RaycastHit2D();
    }

    /// <summary>
    /// Returns an axis aligned rectangle in world space that contains all the colliders used by casts.
    /// </summary>
    /// <param name="border">Adds an additional border to the returned rectangle.</param>
    public Rect getBounds(float border = 0) {
        if (colliders.Count <= 0)
            return new Rect(transform.position.x, transform.position.y, 0, 0);

        float left = float.MaxValue;
        float right = float.MinValue;
        float down = float.MaxValue;
        float up = float.MinValue;

        foreach (Collider2D c2d in colliders) {

            if (c2d is BoxCollider2D) {
                BoxCollider2D bc2d = c2d as BoxCollider2D;

                Vector2 offset = bc2d.offset;
                Vector2 size = bc2d.size;

                Vector2 bl = new Vector2(offset.x - size.x / 2, offset.y - size.y / 2);
                Vector2 br = new Vector2(offset.x + size.x / 2, offset.y - size.y / 2);
                Vector2 tl = new Vector2(offset.x - size.x / 2, offset.y + size.y / 2);
                Vector2 tr = new Vector2(offset.x + size.x / 2, offset.y + size.y / 2);

                Vector2 gbl = bc2d.transform.TransformPoint(bl);
                Vector2 gbr = bc2d.transform.TransformPoint(br);
                Vector2 gtl = bc2d.transform.TransformPoint(tl);
                Vector2 gtr = bc2d.transform.TransformPoint(tr);

                left = Mathf.Min(left, gbl.x, gbr.x, gtl.x, gtr.x);
                right = Mathf.Max(right, gbl.x, gbr.x, gtl.x, gtr.x);
                down = Mathf.Min(down, gbl.y, gbr.y, gtl.y, gtr.y);
                up = Mathf.Max(up, gbl.y, gbr.y, gtl.y, gtr.y);

            } else if (c2d is CircleCollider2D) {

                Vector2 center;
                float radius;
                getCircleCollider2DProperties(c2d as CircleCollider2D, out center, out radius);
                left = Mathf.Min(left, center.x - radius);
                right = Mathf.Max(right, center.x + radius);
                down = Mathf.Min(down, center.y - radius);
                up = Mathf.Max(up, center.y + radius);
                
            }
        }

        left -= border;
        right += border;
        down -= border;
        up += border;
        
        return new Rect(left, down, right - left, up - down);
    }

    /// <summary>
    /// If the left bottom side of the bounds isn't immediately over a platform.
    /// layerMask of the raycast is a union of the layer masks of all the colliders.
    /// </summary>
    /// <param name="depthCheck">How far down to check if there's a platform (distance of the raycast).</param>
    /// <param name="leftOffset">x offset from the left side of the bounds.</param>
    public bool walkoffToLeft(float depthCheck, float leftOffset = 0) {

        Rect bounds = getBounds(.01f);
        Vector2 origin = new Vector2(bounds.xMin + leftOffset, bounds.yMin);
        int layerMask = getUnionLayerMask();

        return Physics2D.Raycast(origin, Vector2.down, depthCheck, layerMask).collider == null;
    }

    /// <summary>
    /// If the right bottom side of the bounds isn't immediately over a platform.
    /// layerMask of the raycast is a union of the layer masks of all the colliders.
    /// </summary>
    /// <param name="depthCheck">How far down to check if there's a platform (distance of the raycast).</param>
    /// <param name="rightOffset">x offset from the right side of the bounds.</param>
    public bool walkoffToRight(float depthCheck, float rightOffset = 0) {

        Rect bounds = getBounds(.01f);
        Vector2 origin = new Vector2(bounds.xMax + rightOffset, bounds.yMin);
        int layerMask = getUnionLayerMask();

        return Physics2D.Raycast(origin, Vector2.down, depthCheck, layerMask).collider == null;
    }
    
    /// <summary>
    /// Simulates "projection" repositioning upon collision.  Returns the new position if projection reposition occurred.
    /// rigidbody2d.SetPosition(pos) can then be used to attempt smoother movement along sloped surfaces.
    /// </summary>
    /// <param name="velocity">Velocity of the moving object (e.g. rigidbody2d.velocity).</param>
    /// <param name="time">The amount of time passed in the collision test (e.g. Time.fixedDeltaTime).</param>
    /// <param name="directionRestriction">If UP or DOWN, projection will be along the x axis (i.e. the x value will be the same as if no collision happened).  Vise-versa if direction is LEFT or RIGHT.  If set to NONE, all casts are tested and the projection direction is automatically determined.</param>
    public Vector2 projectReposition(Vector2 velocity, float time, Direction directionRestriction = Direction.NONE) {

        Vector2 pos0 = transform.position; // position at the start
        Vector2 displacement = velocity * time;
        if (displacement == Vector2.zero) return pos0;

        Vector2 pos1 = pos0 + displacement; // position transform would be at next frame if there was no collision.
        float castDistance = displacement.magnitude;
        Vector2 directionVec = displacement / castDistance;

        int numResults = 0;
        RaycastHit2D rh2d;
        Vector2 hitPt; // where collider hit object
        Vector2 slope = new Vector2(); // slope of object hit.  Is perpendicular to the normal
        Vector2 ret = new Vector2();
        
        Direction normalDirection = Direction.NONE;
        switch (directionRestriction) {
        case Direction.RIGHT:
            normalDirection = Direction.LEFT;
            break;
        case Direction.UP:
            normalDirection = Direction.DOWN;
            break;
        case Direction.LEFT:
            normalDirection = Direction.RIGHT;
            break;
        case Direction.DOWN:
            normalDirection = Direction.UP;
            break;
        }

        checkCasts(directionVec, normalDirection, out numResults, castDistance, Vector2.zero);
        if (numResults > 0) {
            rh2d = earliestOfTempCastResults(numResults);
            hitPt = pos0 + displacement * rh2d.fraction;
            slope.Set(rh2d.normal.y, -rh2d.normal.x);

            Direction nDir = getNormalDirection(rh2d.normal, slopeAngle);
            if (nDir == Direction.LEFT || nDir == Direction.RIGHT) {
                // project horizontally
                ret = new Vector2(
                    hitPt.x + slope.x / slope.y * (pos1.y - hitPt.y),
                    pos1.y);
            } else {
                // project vertically
                ret = new Vector2(
                    pos1.x,
                    hitPt.y + slope.y / slope.x * (pos1.x - hitPt.x));
            }

        } else { // if didn't hit anything
            ret = pos1;
        }

        return ret;
    }
    

    /// <summary>
    /// Returns the Direction of the normal angle.
    /// </summary>
    /// <param name="thresholdAngleDegrees">Angle in (0, 90).  The returned direction will be RIGHT iff the normal angle is in between -(90 - threshold) and (90 - threshold).</param>
    public static Direction getNormalDirection(Vector2 normal, float slopeAngleDegrees = 45) {
        float thresholdAngle = (90 - slopeAngleDegrees) * Mathf.Deg2Rad;
        float normalAngle = Mathf.Atan2(normal.y, normal.x);
        if (normalAngle < 0) normalAngle += Mathf.PI * 2; // keep angle in [0, 2pi)
        if (normalAngle < thresholdAngle) {
            return Direction.RIGHT;
        } else if (normalAngle < Mathf.PI - thresholdAngle) {
            return Direction.UP;
        } else if (normalAngle < Mathf.PI + thresholdAngle) {
            return Direction.LEFT;
        } else if (normalAngle < Mathf.PI * 2 - thresholdAngle) {
            return Direction.DOWN;
        }
        return Direction.RIGHT;
    }

    /// <summary>
    /// Returns a mask combining all the layers that collide with the given layer.
    /// </summary>
    public static int layerMaskFromLayer(int layer) {
        int mask = 0;
        for (int i = 0; i < 32; i++) {
            if (!Physics2D.GetIgnoreLayerCollision(layer, i)) mask = mask | (1 << i);
        }
        return mask;
    }

    /// <summary>
    /// Returns a union of the layer masks of the layers of all the colliders.
    /// </summary>
    public int getUnionLayerMask() {
        int ret = 0;
        foreach (Collider2D c2d in colliders) {
            ret |= layerMaskFromLayer(c2d.gameObject.layer);
        }
        return ret;
    }
    
    [Range(.1f, 89.9f), Tooltip("Determines which of 4 directions a normal angle will be.  The higher the value, the steeper the slope needed for a collision to be considered RIGHT instead of DOWN.")]
    public float slopeAngle = 50;
    [Tooltip("Distance of a cast for colliders to be considered touching.")]
    public float touchCastDistance = .04f;
    
    /// <summary>
    /// Updates collection of Colliders by searching components of this gameObject and children.
    /// For now only collects BoxCollider2D and CircleCollider2D.
    /// Ignores colliders marked as triggers.
    /// This is automatically called during Start().
    /// </summary>
    public void updateColliders() {

        this.colliders.Clear();
        
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D c2d in colliders) {
            if (c2d.isTrigger) continue;
            if ((c2d is BoxCollider2D) ||
                (c2d is CircleCollider2D)) {
                this.colliders.Add(c2d);
            } else {
                Debug.LogWarning("CollisionCaster only recognizes BoxCollider2D and CircleCollider2D");
            }
        }

    }

    #region Private

    void Awake() { }

    void Start() {
        updateColliders();
    }

    void OnDestroy() {
        colliders.Clear();
    }

    /// <summary>
    /// Performs raycasts.  Stores results in this.tempResults.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="totalResults"></param>
    private void checkCasts(Direction direction, out int totalResults, float castDistance, Vector2 originOffset) {

        Vector2 directionVec = new Vector2();
        Direction oppositeDirection = Direction.NONE;
        switch (direction) {
        case Direction.RIGHT:
            directionVec = Vector2.right;
            oppositeDirection = Direction.LEFT;
            break;
        case Direction.UP:
            directionVec = Vector2.up;
            oppositeDirection = Direction.DOWN;
            break;
        case Direction.LEFT:
            directionVec = Vector2.left;
            oppositeDirection = Direction.RIGHT;
            break;
        case Direction.DOWN:
            directionVec = Vector2.down;
            oppositeDirection = Direction.UP;
            break;
        default:
            Debug.LogWarning("Must specify direction in the CollisionCaster.");
            totalResults = 0;
            return;
        }

        checkCasts(directionVec, oppositeDirection, out totalResults, castDistance, originOffset);
        
    }
    
    /// <summary>
    /// Performs casts using shapes defined by this.colliders.  Stores results in this.tempResults.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="totalResults"></param>
    /// <param name="normalRestriction">Cast hits will only count if their normal is in this direction.  Set to NONE for all cast hits to count.</param>
    /// <param name="originOffset">Offset to apply to the origins of the casts (in global space) just before performing the cast.</param>
    private void checkCasts(Vector2 directionVec, Direction normalRestriction, out int totalResults, float castDistance, Vector2 originOffset) {

        // disable casts hitting triggers
        bool prevQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        
        totalResults = 0;

        foreach (Collider2D c2d in colliders) {

            if (c2d == null || !c2d.isActiveAndEnabled) continue;

            int numResults = 0;
            Vector2 origin = c2d.transform.TransformPoint(c2d.offset);
            origin += originOffset;
            int layerMask = layerMaskFromLayer(c2d.gameObject.layer);

            if (c2d is CircleCollider2D) {

                Vector2 center;
                float radius;
                getCircleCollider2DProperties(c2d as CircleCollider2D, out center, out radius);
                
                numResults = Physics2D.CircleCastNonAlloc(origin, radius, directionVec, tempCastResults, castDistance, layerMask);

            } else if (c2d is BoxCollider2D) {

                Vector2 center;
                Vector2 size;
                float angle;
                getBoxCollider2DProperties(c2d as BoxCollider2D, out center, out size, out angle);

                numResults = Physics2D.BoxCastNonAlloc(origin, size, angle, directionVec, tempCastResults, castDistance, layerMask);

            }
            
            for (int i = 0; i < numResults; i++) {
                // skip colliders belonging to this gameObject
                if (tempCastResults[i].collider.gameObject == gameObject) continue;

                if (normalRestriction == Direction.NONE ||
                    getNormalDirection(tempCastResults[i].normal, slopeAngle) == normalRestriction) {
                    // only count if normal points in given direction
                    if (totalResults < tempResults.Length) { // failsafe
                        tempResults[totalResults] = tempCastResults[i];
                        totalResults++;
                    }
                }
            }

        }


        // reset casts hitting triggers
        Physics2D.queriesHitTriggers = prevQueriesHitTriggers;

    }

    private void getCircleCollider2DProperties(CircleCollider2D cc2d, out Vector2 center, out float radius) {
        center = cc2d.transform.TransformPoint(cc2d.offset);
        radius = cc2d.radius * Mathf.Max(Mathf.Abs(cc2d.transform.lossyScale.x), Mathf.Abs(cc2d.transform.lossyScale.y));
    }

    private void getBoxCollider2DProperties(BoxCollider2D bc2d, out Vector2 center, out Vector2 size, out float angleDegrees) {
        Vector2 offset = bc2d.offset;
        size = bc2d.size;

        Vector2 bl = new Vector2(offset.x - size.x / 2, offset.y - size.y / 2);
        Vector2 br = new Vector2(offset.x + size.x / 2, offset.y - size.y / 2);
        Vector2 tl = new Vector2(offset.x - size.x / 2, offset.y + size.y / 2);

        Vector2 gbl = bc2d.transform.TransformPoint(bl);
        Vector2 gbr = bc2d.transform.TransformPoint(br);
        Vector2 gtl = bc2d.transform.TransformPoint(tl);

        center = bc2d.transform.TransformPoint(offset);
        size = new Vector2((gbr - gbl).magnitude, (gtl - gbl).magnitude);
        angleDegrees = Mathf.Atan2(gbr.y - gbl.y, gbr.x - gbl.x) * Mathf.Rad2Deg;
    }

    private RaycastHit2D earliestOfTempCastResults(int numResults) {
        if (numResults <= 0) {
            return new RaycastHit2D();
        }

        int retIndex = 0;
        for (int i = 1; i < numResults; i++) {
            if (tempResults[i].fraction < tempResults[retIndex].fraction) {
                retIndex = i;
            }
        }
        return tempResults[retIndex];
    }
    
    private List<Collider2D> colliders = new List<Collider2D>();
    
    private RaycastHit2D[] tempCastResults = new RaycastHit2D[10];
    private RaycastHit2D[] tempResults = new RaycastHit2D[10];

    #endregion
    
}
