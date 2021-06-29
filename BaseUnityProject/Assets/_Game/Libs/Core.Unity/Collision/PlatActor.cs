using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Core.Unity.Collision {

    /// <summary>
    /// Uses CollisionCaster and late FixedUpdate() calls to attempt movement more suited for platformers.
    /// rb2d.velocity will be set and considered.
    /// There is always a MovePosition() call at the end of FixedUpdate with the next intended position.  This also means movement isn't affected much by rb2d's gravity or linear drag.
    /// </summary>
    [RequireComponent(typeof(CollisionCaster))]
    [RequireComponent(typeof(Rigidbody2D))]
    [DefaultExecutionOrder(999)] // this script should execute late.  NOTE: This attribute works, despite not showing up in the Script Execution Order window
    public class PlatActor : MonoBehaviour {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("Attempts smoother repositioning when colliding with sloped surfaces.")]
        private bool _projectReposition = true;

        [SerializeField]
        [Tooltip("If the custom gravity force (given below) should be applied.  Note that rb2d.gravityScale is automatically set to 0 regardless.")]
        private bool _applyGravity = true;

        [SerializeField]
        [Tooltip("Gravity unique to this object.  rb2d.gravityScale is automatically set to 0.")]
        private Vector2 _gravity = new Vector2(0, -10);

        [SerializeField]
        [Tooltip("When touching an object (detected by CollisionCaster) in the downward direction, zero y velocity.")]
        private bool _stopOnTouchDown = true;
        [SerializeField]
        [Tooltip("When touching an object (detected by CollisionCaster) in the upward direction, zero y velocity.")]
        private bool _stopOnTouchUp = true;
        [SerializeField]
        [Tooltip("When touching an object (detected by CollisionCaster) in the leftward direction, zero x velocity.")]
        private bool _stopOnTouchLeft = true;
        [SerializeField]
        [Tooltip("When touching an object (detected by CollisionCaster) in the rightward direction, zero x velocity.")]
        private bool _stopOnTouchRight = true;

        [SerializeField]
        [Tooltip("If touching a platform below, the platform's movement will be added to this object's movement.  NOTE: must set platform's velocity and angular velocity, this will not work if the platform is being moved with MovePosition() or MoveRotation()")]
        private bool _movedByDownPlatform = true;

        #endregion

        #region Properties

        /// <summary>
        /// If smoother repositioning should be applied when colliding with sloped surfaces.
        /// </summary>
        public bool ProjectReposition {
            get { return _projectReposition; }
            set { _projectReposition = value; }
        }

        /// <summary>
        /// If custom gravity force should be applied.  Note that <see cref="Rigidbody2D"/>'s gravity scale is set to 0 regardless.
        /// </summary>
        public bool ApplyGravity {
            get { return _applyGravity; }
            set { _applyGravity = value; }
        }

        /// <summary>
        /// Gravity unique to this object.  <see cref="Rigidbody2D"/>.gravityScale is automatically set to 0.
        /// </summary>
        public Vector2 Gravity {
            get { return _gravity; }
            set { _gravity = value; }
        }

        /// <summary>
        /// When touching an object (detected by CollisionCaster) in the downward direction, zero y velocity?
        /// </summary>
        public bool StopOnTouchDown {
            get { return _stopOnTouchDown; }
            set { _stopOnTouchDown = value; }
        }

        /// <summary>
        /// When touching an object (detected by CollisionCaster) in the upward direction, zero y velocity?
        /// </summary>
        public bool StopOnTouchUp {
            get { return _stopOnTouchUp; }
            set { _stopOnTouchUp = value; }
        }

        /// <summary>
        /// When touching an object (detected by CollisionCaster) in the leftward direction, zero x velocity?
        /// </summary>
        public bool StopOnTouchLeft {
            get { return _stopOnTouchLeft; }
            set { _stopOnTouchLeft = value; }
        }

        /// <summary>
        /// When touching an object (detected by CollisionCaster) in the rightward direction, zero x velocity?
        /// </summary>
        public bool StopOnTouchRight {
            get { return _stopOnTouchRight; }
            set { _stopOnTouchRight = value; }
        }

        /// <summary>
        /// If touching a platform below, the platform's movement will be added to this object's movement.
        /// <para/>
        /// NOTE: must set platform's velocity and angular velocity, this will not work if the platform is being moved with MovePosition() or MoveRotation().
        /// </summary>
        public bool MovedByDownPlatform {
            get { return _movedByDownPlatform; }
            set { _movedByDownPlatform = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// If there's ground a given distance below, will snap down to it so they're touching.
        /// <para/>
        /// Returns true the object was already on the ground or if a snap down occurred.  Returns false if object is not on the ground and couldn't be snapped down.
        /// </summary>
        public bool SnapDown(float distance) {
            return this.SnapDirection(distance, Direction.Down);
        }

        /// <summary>
        /// If there's a ceiling a given distance above, will snap up to it so they're touching.
        /// <para/>
        /// Returns true the object was already on the ceiling or if a snap up occurred.  Returns false if object is not on the ceiling and couldn't be snapped up.
        /// </summary>
        public bool SnapUp(float distance) {
            return this.SnapDirection(distance, Direction.Up);
        }

        /// <summary>
        /// If there's a wall a given distance to the left, will snap left to it so they're touching.
        /// <para/>
        /// Returns true the object was already on the left wall or if a snap left occurred.  Returns false if object is not on the wall and couldn't be snapped.
        /// </summary>
        public bool SnapLeft(float distance) {
            return this.SnapDirection(distance, Direction.Left);
        }

        /// <summary>
        /// If there's a wall a given distance to the right, will snap right to it so they're touching.
        /// <para/>
        /// Returns true the object was already on the right wall or if a snap right occurred.  Returns false if object is not on the wall and couldn't be snapped.
        /// </summary>
        public bool SnapRight(float distance) {
            return this.SnapDirection(distance, Direction.Right);
        }

        /// <summary>
        /// Gets the one-way platform collider the object is currently touching below.  Returns null if no such object is being touched below.
        /// <para/>
        /// To qualify, the collider must be using an enabled <see cref="PlatformEffector2D"/> the uses one way pointed up.
        /// </summary>
        /// <returns>Is touching platform.</returns>
        public Collider2D GetOneWayPlatformBelow() {

            int numResults = _collisionCaster.TouchResultsNonAlloc(Direction.Down, _raycastHitResults);
            if (numResults == 0)
                return null;

            for (int i=0; i < numResults; i++) {
                // does the collider use a one-way platform effector?
                Collider2D collider = _raycastHitResults[i].collider;
                if (collider == null)
                    continue;
                if (!collider.usedByEffector)
                    continue;
                PlatformEffector2D platformEffector = collider.GetComponent<PlatformEffector2D>();
                if (platformEffector == null || !platformEffector.enabled)
                    continue;
                if (!platformEffector.useOneWay)
                    continue;
                // does one-way platform effector allow objects to pass from below?
                float effectorAngle = MathUtils.Wrap360(platformEffector.rotationalOffset + 90);
                if (effectorAngle - platformEffector.surfaceArc / 2 < 0 || effectorAngle + platformEffector.surfaceArc / 2 > 180)
                    continue;
                // is touching one-way platform
                return collider;
            }

            return null;
        }

        /// <summary>
        /// Tells the collision caster to ignore collision with the given collider.  Then after the given duration is up, unignore collision with the collider.
        /// </summary>
        /// <param name="collider2D">The collider to ignore.</param>
        /// <param name="duration">The duration to wait until unignoring collision.</param>
        public void IgnoreCollisionTemporarily(Collider2D collider2D, float duration) {
            if (collider2D == null)
                return;
            if (duration < .001f)
                return;

            _collisionCaster.IgnoreCollision(collider2D);
            _temporaryIgnoreColliders[collider2D] = Time.fixedTime + duration;
        }

        #endregion

        #region Private

        private void Awake() {
            if (!Physics2D.autoSyncTransforms) {
                Debug.LogWarning("Physics2D.autoSyncTransforms is false.  Since Unity 2019.4 there have been issues when this is false, set this setting to true.");
                Physics2D.autoSyncTransforms = true;
            }

            _collisionCaster = this.EnsureComponent<CollisionCaster>();
            _rb2d = this.EnsureComponent<Rigidbody2D>();

            if (_rb2d.gravityScale != 0) {
                Debug.LogWarning("Attached RigidBody2D.gravityScale has been set to 0, as required by PlatActor");
                _rb2d.gravityScale = 0;
            }
        }


        private void Update() {

        }

        /// <summary>
        /// PlatActor is set very late in the script execution order, so this should be the last thing that modifies rigidBody2D before the internal physics update
        /// </summary>
        private void FixedUpdate() {

            // unignore temporary ignore colliders
            if (_temporaryIgnoreColliders.Keys.Count > 0) {
                Collider2D[] keys = _temporaryIgnoreColliders.Keys.ToArray();
                foreach (Collider2D key in keys) {
                    if (Time.fixedTime >= _temporaryIgnoreColliders[key]) {
                        _collisionCaster.UnignoreCollision(key);
                        _temporaryIgnoreColliders.Remove(key);
                    }
                }
            }

            // getting properties
            Vector2 v = _rb2d.velocity;
            RaycastHit2D touchRight = _collisionCaster.TouchResult(Direction.Right);
            RaycastHit2D touchUp = _collisionCaster.TouchResult(Direction.Up);
            RaycastHit2D touchLeft = _collisionCaster.TouchResult(Direction.Left);
            RaycastHit2D touchDown = _collisionCaster.TouchResult(Direction.Down);

            // apply gravity
            if (_applyGravity) {
                v += _gravity * Time.fixedDeltaTime;
            }

            // zero velocity on touch
            if (_stopOnTouchRight && touchRight) {
                v.x = Mathf.Min(0, v.x);
            }
            if (_stopOnTouchUp && touchUp) {
                v.y = Mathf.Min(0, v.y);
            }
            if (_stopOnTouchLeft && touchLeft) {
                v.x = Mathf.Max(0, v.x);
            }
            if (_stopOnTouchDown && touchDown) {
                v.y = Mathf.Max(0, v.y);
            }

            // apply custom velocity
            _rb2d.velocity = v;

            // setting new position with MovePosition()
            Vector2 currentPos = this.transform.position;
            Vector2 pos = currentPos + _rb2d.velocity * Time.fixedDeltaTime;

            // project reposition
            if (_projectReposition) {
                pos = _collisionCaster.ProjectReposition(_rb2d.velocity, Time.fixedDeltaTime);
            }

            // moving platform
            if (_movedByDownPlatform) {

                if (touchDown) {
                    Rigidbody2D downRb2d = touchDown.rigidbody;
                    Vector2 touchPoint = touchDown.point;
                    Vector2 p1 = touchPoint;

                    while (downRb2d != null) {

                        // apply platform's angular velocity
                        Vector2 centerPoint = downRb2d.worldCenterOfMass;
                        float rotationRad = downRb2d.angularVelocity * Time.fixedDeltaTime * Mathf.Deg2Rad;
                        p1 = MathUtils.RotateAroundPoint(p1, centerPoint, rotationRad);

                        // applying platform's velocity
                        p1 += downRb2d.velocity * Time.fixedDeltaTime;

                        // apply movement of parent RB2D if present
                        downRb2d = downRb2d.transform.parent == null ? null : downRb2d.transform.parent.GetComponentInParent<Rigidbody2D>();
                    }

                    // add to position
                    pos += p1 - touchPoint;
                }
            }

            // applying position
            _rb2d.MovePosition(pos);
        }

        /// <summary>
        /// If there's a wall the given distance away in the given direction, will snap to it so they're touching.
        /// <para/>
        /// Returns true the object was already on the wall or if a snap in the direction occurred.  Returns false if object is not on the wall and couldn't be snapped in the direction.
        /// </summary>
        private bool SnapDirection(float distance, Direction direction) {
            Vector2 pos = this.transform.position;
            Vector2 directionVec;
            Direction oppositeDirection;
            switch (direction) {
            case Direction.Left:
                directionVec = Vector2.left;
                oppositeDirection = Direction.Right;
                break;
            case Direction.Right:
                directionVec = Vector2.right;
                oppositeDirection = Direction.Left;
                break;
            case Direction.Up:
                directionVec = Vector2.up;
                oppositeDirection = Direction.Down;
                break;
            case Direction.Down:
            default:
                directionVec = Vector2.down;
                oppositeDirection = Direction.Up;
                break;
            }

            // check raycast
            bool prevQSIC = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D raycastResult = _collisionCaster.Cast(directionVec, distance, Vector2.zero, oppositeDirection);
            Physics2D.queriesStartInColliders = prevQSIC;

            if (raycastResult.collider != null) {
                // only apply snap if distance away isn't trivial
                Vector2 diff = directionVec * distance * raycastResult.fraction;
                if (diff.sqrMagnitude > _collisionCaster.TouchCastDistance * _collisionCaster.TouchCastDistance) {
                    // .01 added so that collider isn't intersecting the platform below
                    this.transform.position = pos + diff - (directionVec * 0.01f);
                }

                return true;
            }

            // return true if touching down anyway
            if (_collisionCaster.Touch(Direction.Down)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reference to the attached <see cref="CollisionCaster"/>.
        /// </summary>
        private CollisionCaster _collisionCaster;

        /// <summary>
        /// Reference to the attached <see cref="Rigidbody2D"/>.
        /// </summary>
        private Rigidbody2D _rb2d;

        private RaycastHit2D[] _raycastHitResults = new RaycastHit2D[10];

        /// <summary>
        /// Dictionary of colliders to ignore temporarily.  Value is the Time.fixedTime when the collider should be unignored.
        /// </summary>
        private Dictionary<Collider2D, float> _temporaryIgnoreColliders = new Dictionary<Collider2D, float>();
        #endregion
    }
}