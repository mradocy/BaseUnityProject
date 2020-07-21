using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Core.Unity.Collision {
    /// <summary>
    /// Provides collision tools without modifying anything.
    /// Performs short shape casts to check collision with nearby objects.  Triggers are not detected.
    /// Shapes are defined by non-trigger colliders attached to the gameObject and its children.
    /// </summary>
    public class CollisionCaster : MonoBehaviour {

        #region Inspector Fields

        [Range(.1f, 89.9f), Tooltip("Determines which of 4 directions a normal angle will be.  The higher the value, the steeper the slope needed for a collision to be considered Right instead of Down.")]
        public float SlopeAngle = 50;

        [Tooltip("Distance of a cast for colliders to be considered touching.")]
        public float TouchCastDistance = .04f;

        #endregion

        #region Static Public Methods

        /// <summary>
        /// Returns the Direction of the normal angle.
        /// </summary>
        /// <param name="thresholdAngleDegrees">Angle in (0, 90).  The returned direction will be RIGHT iff the normal angle is in between -(90 - threshold) and (90 - threshold).</param>
        public static Direction GetNormalDirection(Vector2 normal, float slopeAngleDegrees = 45) {
            float thresholdAngle = (90 - slopeAngleDegrees) * Mathf.Deg2Rad;
            float normalAngle = Mathf.Atan2(normal.y, normal.x);
            if (normalAngle < 0) normalAngle += Mathf.PI * 2; // keep angle in [0, 2pi)
            if (normalAngle < thresholdAngle) {
                return Direction.Right;
            } else if (normalAngle < Mathf.PI - thresholdAngle) {
                return Direction.Up;
            } else if (normalAngle < Mathf.PI + thresholdAngle) {
                return Direction.Left;
            } else if (normalAngle < Mathf.PI * 2 - thresholdAngle) {
                return Direction.Down;
            }
            return Direction.Right;
        }

        /// <summary>
        /// Returns a mask combining all the layers that collide with the given layer.
        /// </summary>
        public static int LayerMaskFromLayer(int layer) {
            int mask = 0;
            for (int i = 0; i < 32; i++) {
                if (!Physics2D.GetIgnoreLayerCollision(layer, i)) mask |= (1 << i);
            }
            return mask;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Performs casts in the given direction, and return if any of them hit something.
        /// </summary>
        public bool Touch(Direction direction) {
            int numResults = 0;
            this.CheckCasts(direction, out numResults, this.TouchCastDistance, Vector2.zero);
            return numResults > 0;
        }

        /// <summary>
        /// Performs casts in the given direction, and returns the results in an array.
        /// </summary>
        public RaycastHit2D[] TouchResults(Direction direction) {
            int numResults = 0;
            this.CheckCasts(direction, out numResults, this.TouchCastDistance, Vector2.zero);
            RaycastHit2D[] ret = new RaycastHit2D[numResults];
            Array.Copy(_tempResults, ret, numResults);
            return ret;
        }

        /// <summary>
        /// Performs casts in the given direction, and stores the results in the given array.  Returns the number of results.
        /// </summary>
        public int TouchResultsNonAlloc(Direction direction, RaycastHit2D[] results) {
            int numResults = 0;
            this.CheckCasts(direction, out numResults, this.TouchCastDistance, Vector2.zero);
            Array.Copy(_tempResults, results, numResults);
            return numResults;
        }

        /// <summary>
        /// Performs casts in the given direction, and returns the first result.  If nothing was hit, the collider property of the returned RaycastHit2D will be null.
        /// </summary>
        public RaycastHit2D TouchResult(Direction direction) {
            int numResults = 0;
            this.CheckCasts(direction, out numResults, this.TouchCastDistance, Vector2.zero);
            if (numResults == 0) {
                RaycastHit2D noHit = new RaycastHit2D();
                return noHit;
            }

            // return earliest hit
            return this.EarliestOfTempCastResults(numResults);
        }

        /// <summary>
        /// Performs casts in the given direction, and returns the normal of the collision.  Normal is averaged on multiple colliders.  Returns 0,0 if nothing was hit.
        /// </summary>
        public Vector2 TouchNormal(Direction direction) {
            int numResults = 0;
            this.CheckCasts(direction, out numResults, this.TouchCastDistance, Vector2.zero);
            if (numResults == 0) {
                return Vector2.zero;
            }
            Vector2 n = new Vector2();
            for (int i = 0; i < numResults; i++) {
                n += _tempResults[i].normal;
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
        public RaycastHit2D Cast(Vector2 directionVec, float distance, Vector2 originOffset = new Vector2(), Direction normalRestriction = Direction.None) {

            int numResults;
            this.CheckCasts(directionVec, normalRestriction, out numResults, distance, originOffset);
            if (numResults > 0) {
                return this.EarliestOfTempCastResults(numResults);
            }
            return new RaycastHit2D();
        }

        /// <summary>
        /// Returns an axis aligned rectangle in world space that contains all the colliders used by casts.
        /// </summary>
        /// <param name="border">Adds an additional border to the returned rectangle.</param>
        public Rect GetBounds(float border = 0) {
            if (_colliders.Count <= 0)
                return new Rect(this.transform.position.x, this.transform.position.y, 0, 0);

            float left = float.MaxValue;
            float right = float.MinValue;
            float down = float.MaxValue;
            float up = float.MinValue;

            foreach (Collider2D c2d in _colliders) {

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
                    this.GetCircleCollider2DProperties(c2d as CircleCollider2D, out center, out radius);
                    left = Mathf.Min(left, center.x - radius);
                    right = Mathf.Max(right, center.x + radius);
                    down = Mathf.Min(down, center.y - radius);
                    up = Mathf.Max(up, center.y + radius);

                } else if (c2d is CapsuleCollider2D) {

                    Vector2 center;
                    Vector2 size;
                    CapsuleDirection2D capsuleDirection;
                    float angleDegrees;
                    this.GetCapsuleCollider2DProperties(c2d as CapsuleCollider2D, out center, out size, out capsuleDirection, out angleDegrees);
                    Vector2 c0, c1;
                    float sizeMin = Mathf.Min(size.x, size.y);
                    if (capsuleDirection == CapsuleDirection2D.Vertical) {
                        c0 = new Vector2(0, -(size.y - sizeMin) / 2);
                        c1 = new Vector2(0, (size.y - sizeMin) / 2);
                    } else {
                        c0 = new Vector2(-(size.x - sizeMin) / 2, 0);
                        c1 = new Vector2((size.x - sizeMin) / 2, 0);
                    }
                    c0 = MathUtils.RotateAroundPoint(c0, Vector2.zero, angleDegrees * Mathf.Deg2Rad) + center;
                    c1 = MathUtils.RotateAroundPoint(c1, Vector2.zero, angleDegrees * Mathf.Deg2Rad) + center;
                    left = Mathf.Min(c0.x, c1.x) - sizeMin / 2;
                    right = Mathf.Max(c0.x, c1.x) + sizeMin / 2;
                    down = Mathf.Min(c0.y, c1.y) - sizeMin / 2;
                    up = Mathf.Max(c0.y, c1.y) + sizeMin / 2;

                }
            }

            left -= border;
            right += border;
            down -= border;
            up += border;

            return new Rect(left, down, right - left, up - down);
        }
        
        /// <summary>
        /// Gets if the left bottom side of the bounds isn't immediately over a platform.
        /// layerMask of the raycast is a union of the layer masks of all the colliders.
        /// </summary>
        /// <param name="depthCheck">How far down to check if there's a platform (distance of the raycast).</param>
        /// <param name="leftOffset">x offset from the left side of the bounds.</param>
        public bool WalkoffToLeft(float depthCheck, float leftOffset) {

            Rect bounds = this.GetBounds(.01f);
            Vector2 origin = new Vector2(bounds.xMin - leftOffset, bounds.yMin + .02f);
            int layerMask = this.GetUnionLayerMask();

            bool prevQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
            bool walkoff = Physics2D.Raycast(origin, Vector2.down, depthCheck, layerMask).collider == null;
            Physics2D.queriesHitTriggers = prevQueriesHitTriggers;
            return walkoff;
        }

        /// <summary>
        /// Gets if the right bottom side of the bounds isn't immediately over a platform.
        /// layerMask of the raycast is a union of the layer masks of all the colliders.
        /// </summary>
        /// <param name="depthCheck">How far down to check if there's a platform (distance of the raycast).</param>
        /// <param name="rightOffset">x offset from the right side of the bounds.</param>
        public bool WalkoffToRight(float depthCheck, float rightOffset) {

            Rect bounds = this.GetBounds(.01f);
            Vector2 origin = new Vector2(bounds.xMax + rightOffset, bounds.yMin + .02f);
            int layerMask = this.GetUnionLayerMask();

            bool prevQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
            bool walkoff = Physics2D.Raycast(origin, Vector2.down, depthCheck, layerMask).collider == null;
            Physics2D.queriesHitTriggers = prevQueriesHitTriggers;
            return walkoff;
        }

        /// <summary>
        /// Simulates "projection" repositioning upon collision.  Returns the new position if projection reposition occurred.
        /// rigidbody2d.SetPosition(pos) can then be used to attempt smoother movement along sloped surfaces.
        /// </summary>
        /// <param name="velocity">Velocity of the moving object (e.g. rigidbody2d.velocity).</param>
        /// <param name="time">The amount of time passed in the collision test (e.g. Time.fixedDeltaTime).</param>
        /// <param name="directionRestriction">If Up or Down, projection will be along the x axis (i.e. the x value will be the same as if no collision happened).  Vise-versa if direction is Left or Right.  If set to None, all casts are tested and the projection direction is automatically determined.</param>
        public Vector2 ProjectReposition(Vector2 velocity, float time, Direction directionRestriction = Direction.None) {

            Vector2 pos0 = this.transform.position; // position at the start
            Vector2 displacement = velocity * time;
            if (displacement == Vector2.zero) return pos0;

            Vector2 pos1 = pos0 + displacement; // position transform would be at next frame if there was no collision.
            float castDistance = displacement.magnitude;
            Vector2 directionVec = displacement / castDistance;

            int numResults;
            RaycastHit2D rh2d;
            Vector2 hitPt; // where collider hit object
            Vector2 slope = new Vector2(); // slope of object hit.  Is perpendicular to the normal
            Vector2 ret;

            Direction normalDirection = Direction.None;
            switch (directionRestriction) {
            case Direction.Right:
                normalDirection = Direction.Left;
                break;
            case Direction.Up:
                normalDirection = Direction.Down;
                break;
            case Direction.Left:
                normalDirection = Direction.Right;
                break;
            case Direction.Down:
                normalDirection = Direction.Up;
                break;
            }

            this.CheckCasts(directionVec, normalDirection, out numResults, castDistance, Vector2.zero);
            if (numResults > 0) {
                rh2d = this.EarliestOfTempCastResults(numResults);
                hitPt = pos0 + displacement * rh2d.fraction;
                slope.Set(rh2d.normal.y, -rh2d.normal.x);

                Direction nDir = GetNormalDirection(rh2d.normal, SlopeAngle);
                if (nDir == Direction.Left || nDir == Direction.Right) {
                    // project horizontally
                    ret = new Vector2(
                        hitPt.x + slope.x / slope.y * (pos1.y - hitPt.y),
                        pos1.y);

                    // add a tiny bit of distance between the projected position and the wall
                    if (nDir == Direction.Left) {
                        ret.x -= TouchCastDistance / 2;
                    } else {
                        ret.x += TouchCastDistance / 2;
                    }
                } else {
                    // project vertically
                    ret = new Vector2(
                        pos1.x,
                        hitPt.y + slope.y / slope.x * (pos1.x - hitPt.x));

                    // add a tiny bit of distance between the projected position and the wall
                    if (nDir == Direction.Down) {
                        ret.y -= TouchCastDistance / 2;
                    } else {
                        ret.y += TouchCastDistance / 2;
                    }
                }

            } else { // if didn't hit anything
                ret = pos1;
            }

            return ret;
        }

        /// <summary>
        /// Returns a union of the layer masks of the layers of all the colliders.
        /// </summary>
        public int GetUnionLayerMask() {
            int ret = 0;
            foreach (Collider2D c2d in _colliders) {
                ret |= LayerMaskFromLayer(c2d.gameObject.layer);
            }
            return ret;
        }

        /// <summary>
        /// Tells caster to ignore casts with the given collider.  Unity physics will ignore collisions between this object and the given collider as well.
        /// </summary>
        /// <param name="collider2D">Collider to ignore.</param>
        public void IgnoreCollision(Collider2D collider2D) {
            if (collider2D == null)
                return;

            _ignoreCollisionColliders.Add(collider2D.GetInstanceID());
            foreach (Collider2D selfCollider in _colliders) {
                Physics2D.IgnoreCollision(selfCollider, collider2D, true);
            }
        }

        /// <summary>
        /// Tells caster to stop ignoring casts with the given collider.  Unity physics will unignore collisions between this object and the given collider as well.
        /// </summary>
        /// <param name="collider2D">Collider to stop ignoring.</param>
        public void UnignoreCollision(Collider2D collider2D) {
            if (collider2D == null)
                return;

            _ignoreCollisionColliders.Remove(collider2D.GetInstanceID());
            foreach (Collider2D selfCollider in _colliders) {
                Physics2D.IgnoreCollision(selfCollider, collider2D, false);
            }
        }

        /// <summary>
        /// Updates collection of Colliders by searching components of this gameObject and children.
        /// For now only collects BoxCollider2D and CircleCollider2D.
        /// Ignores colliders marked as triggers.
        /// This is automatically called during Start().
        /// </summary>
        public void UpdateColliders() {

            _colliders.Clear();

            Collider2D[] colliders = this.GetComponentsInChildren<Collider2D>(true);
            foreach (Collider2D c2d in colliders) {
                if (c2d.isTrigger) continue;
                if ((c2d is BoxCollider2D) ||
                    (c2d is CircleCollider2D) ||
                    (c2d is CapsuleCollider2D)) {
                    _colliders.Add(c2d);
                } else {
                    Debug.LogError("CollisionCaster only recognizes BoxCollider2D, CircleCollider2D, CapsuleCollider2D");
                }
            }
        }

        #endregion

        #region Private

        void Awake() { }

        void Start() {
            this.UpdateColliders();
        }

        void OnDestroy() {
            _colliders.Clear();
        }

        /// <summary>
        /// Performs raycasts.  Stores results in this.tempResults.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="totalResults"></param>
        private void CheckCasts(Direction direction, out int totalResults, float castDistance, Vector2 originOffset) {

            Vector2 directionVec;
            Direction oppositeDirection;
            switch (direction) {
            case Direction.Right:
                directionVec = Vector2.right;
                oppositeDirection = Direction.Left;
                break;
            case Direction.Up:
                directionVec = Vector2.up;
                oppositeDirection = Direction.Down;
                break;
            case Direction.Left:
                directionVec = Vector2.left;
                oppositeDirection = Direction.Right;
                break;
            case Direction.Down:
                directionVec = Vector2.down;
                oppositeDirection = Direction.Up;
                break;
            default:
                Debug.LogWarning("Must specify direction in the CollisionCaster.");
                totalResults = 0;
                return;
            }

            this.CheckCasts(directionVec, oppositeDirection, out totalResults, castDistance, originOffset);
        }

        /// <summary>
        /// Performs casts using shapes defined by this.colliders.  Stores results in this.tempResults.
        /// </summary>
        /// <param name="directionVec">Direction (vector) of the cast</param>
        /// <param name="normalRestriction">Cast hits will only count if their normal is in this direction.  Set to NONE for all cast hits to count.</param>
        /// <param name="totalResults"></param>
        /// <param name="originOffset">Offset to apply to the origins of the casts (in global space) just before performing the cast.</param>
        private void CheckCasts(Vector2 directionVec, Direction normalRestriction, out int totalResults, float castDistance, Vector2 originOffset) {

            // disable casts hitting triggers
            bool prevQueriesHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;


            totalResults = 0;

            foreach (Collider2D c2d in _colliders) {

                if (c2d == null || !c2d.isActiveAndEnabled) continue;

                int numResults = 0;
                Vector2 origin;
                int layerMask = LayerMaskFromLayer(c2d.gameObject.layer);

                if (c2d is CircleCollider2D) {

                    float radius;
                    this.GetCircleCollider2DProperties(c2d as CircleCollider2D, out origin, out radius);
                    origin += originOffset;

                    numResults = Physics2D.CircleCastNonAlloc(origin, radius, directionVec, _tempCastResults, castDistance, layerMask);

                } else if (c2d is BoxCollider2D) {

                    Vector2 size;
                    float angle;
                    this.GetBoxCollider2DProperties(c2d as BoxCollider2D, out origin, out size, out angle);
                    origin += originOffset;

                    numResults = Physics2D.BoxCastNonAlloc(origin, size, angle, directionVec, _tempCastResults, castDistance, layerMask);
                    
                } else if (c2d is CapsuleCollider2D) {

                    Vector2 size;
                    CapsuleDirection2D capsuleDirection;
                    float angle;
                    this.GetCapsuleCollider2DProperties(c2d as CapsuleCollider2D, out origin, out size, out capsuleDirection, out angle);
                    origin += originOffset;

                    numResults = Physics2D.CapsuleCastNonAlloc(origin, size, capsuleDirection, angle, directionVec, _tempCastResults, castDistance, layerMask);

                }

                for (int i = 0; i < numResults; i++) {
                    RaycastHit2D castResult = _tempCastResults[i];
                    if (castResult.collider == null)
                        continue;

                    // skip colliders belonging to this gameObject
                    if (castResult.collider.gameObject == gameObject)
                        continue;

                    // skip colliders that are being ignored
                    if (_ignoreCollisionColliders.Count > 0 &&
                        _ignoreCollisionColliders.Contains(castResult.collider.GetInstanceID()))
                        continue;
                    
                    // check one-way platform effector, if used
                    if (castResult.collider.usedByEffector) {
                        PlatformEffector2D platformEffector = castResult.collider.GetComponent<PlatformEffector2D>();
                        if (platformEffector != null && platformEffector.enabled && platformEffector.useOneWay) {
                            // ignore if cast is not contained in surface arc
                            float castAngle = Mathf.Atan2(castResult.normal.y, castResult.normal.x) * Mathf.Rad2Deg;
                            float effectorAngle = platformEffector.rotationalOffset + 90;
                            float angleDiff = MathUtils.Wrap180(effectorAngle - castAngle);
                            if (Mathf.Abs(angleDiff) >= platformEffector.surfaceArc / 2)
                                continue;

                            // ignore if colliders were already in the platform
                            if (castResult.fraction == 0)
                                continue;
                        }
                    }

                    if (normalRestriction == Direction.None ||
                        GetNormalDirection(castResult.normal, this.SlopeAngle) == normalRestriction) {
                        // only count if normal points in given direction
                        if (totalResults < _tempResults.Length) { // failsafe
                            _tempResults[totalResults] = castResult;
                            totalResults++;
                        }
                    }
                }

            }

            // reset casts hitting triggers
            Physics2D.queriesHitTriggers = prevQueriesHitTriggers;
        }

        private void GetCircleCollider2DProperties(CircleCollider2D cc2d, out Vector2 origin, out float radius) {
            origin = cc2d.transform.TransformPoint(cc2d.offset);
            radius = cc2d.radius * Mathf.Max(Mathf.Abs(cc2d.transform.lossyScale.x), Mathf.Abs(cc2d.transform.lossyScale.y));
        }

        private void GetBoxCollider2DProperties(BoxCollider2D bc2d, out Vector2 origin, out Vector2 size, out float angleDegrees) {
            Vector2 offset = bc2d.offset;
            size = bc2d.size;

            Vector2 bl = new Vector2(offset.x - size.x / 2, offset.y - size.y / 2);
            Vector2 br = new Vector2(offset.x + size.x / 2, offset.y - size.y / 2);
            Vector2 tl = new Vector2(offset.x - size.x / 2, offset.y + size.y / 2);

            Vector2 gbl = bc2d.transform.TransformPoint(bl);
            Vector2 gbr = bc2d.transform.TransformPoint(br);
            Vector2 gtl = bc2d.transform.TransformPoint(tl);

            origin = bc2d.transform.TransformPoint(offset);
            size = new Vector2((gbr - gbl).magnitude, (gtl - gbl).magnitude);
            angleDegrees = Mathf.Atan2(gbr.y - gbl.y, gbr.x - gbl.x) * Mathf.Rad2Deg;
        }

        private void GetCapsuleCollider2DProperties(CapsuleCollider2D cc2d, out Vector2 origin, out Vector2 size, out CapsuleDirection2D capsuleDirection, out float angleDegrees) {
            origin = cc2d.transform.TransformPoint(cc2d.offset);
            size = new Vector2(
                cc2d.size.x * Mathf.Abs(cc2d.transform.lossyScale.x),
                cc2d.size.y * Mathf.Abs(cc2d.transform.lossyScale.y));
            capsuleDirection = cc2d.direction;

            Quaternion quat = cc2d.transform.rotation;
            if (Mathf.Abs(quat.eulerAngles.z) < .001f && Mathf.Abs(quat.eulerAngles.y) > .0001f) { // remove dependency from M
                angleDegrees = quat.eulerAngles.y;
            } else {
                angleDegrees = quat.eulerAngles.z;
            }
        }

        private RaycastHit2D EarliestOfTempCastResults(int numResults) {
            if (numResults <= 0) {
                return new RaycastHit2D();
            }

            int retIndex = 0;
            for (int i = 1; i < numResults; i++) {
                if (_tempResults[i].fraction < _tempResults[retIndex].fraction) {
                    retIndex = i;
                }
            }
            return _tempResults[retIndex];
        }

        private List<Collider2D> _colliders = new List<Collider2D>();

        private RaycastHit2D[] _tempCastResults = new RaycastHit2D[10];
        private RaycastHit2D[] _tempResults = new RaycastHit2D[10];

        /// <summary>
        /// List of instance IDs of colliders to ignore when checking casts.
        /// </summary>
        private HashSet<int> _ignoreCollisionColliders = new HashSet<int>();

        #endregion

    }
}