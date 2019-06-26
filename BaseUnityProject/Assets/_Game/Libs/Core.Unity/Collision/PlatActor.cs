using UnityEngine;
using System.Collections;

namespace Core.Unity.Collision {

    /// <summary>
    /// Uses CollisionCaster and late FixedUpdate() calls to attempt movement more suited for platformers.
    /// rb2d.velocity will be set and considered.
    /// There is always a MovePosition() call at the end of FixedUpdate with the next intended position.  This also means movement isn't affected much by rb2d's gravity or linear drag.
    /// </summary>
    [RequireComponent(typeof(CollisionCaster))]
    [RequireComponent(typeof(Rigidbody2D))]
    [ExecuteInEditMode] // only done to automatically set own script execution order
    public class PlatActor : MonoBehaviour {

        /// <summary>
        /// Value set to the script execution order of this script (should be late).
        /// </summary>
        public const int SCRIPT_EXECUTION_ORDER = 999;

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
            get { return this._projectReposition; }
            set { this._projectReposition = value; }
        }

        /// <summary>
        /// If custom gravity force should be applied.  Note that <see cref="Rigidbody2D"/>'s gravity scale is set to 0 regardless.
        /// </summary>
        public bool ApplyGravity {
            get { return this._applyGravity; }
            set { this._applyGravity = value; }
        }

        /// <summary>
        /// Gravity unique to this object.  <see cref="Rigidbody2D"/>.gravityScale is automatically set to 0.
        /// </summary>
        public Vector2 Gravity {
            get { return this._gravity; }
            set { this._gravity = value; }
        }

        /// <summary>
        /// If touching a platform below, the platform's movement will be added to this object's movement.
        /// <para/>
        /// NOTE: must set platform's velocity and angular velocity, this will not work if the platform is being moved with MovePosition() or MoveRotation().
        /// </summary>
        public bool MovedByDownPlatform {
            get { return this._movedByDownPlatform; }
            set { this._movedByDownPlatform = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// If there's ground a given distance below, will snap down to it so they're touching.
        /// <para/>
        /// Returns true the object was already on the ground or if a snap down occurred.  Returns false if object is not on the ground and couldn't be snapped down.
        /// </summary>
        /// <param name="distance"></param>
        public bool SnapDown(float distance) {

            Vector2 pos = transform.position;

            // check raycast from bottom center first
            Rect rect = this._collisionCaster.GetBounds(0);
            bool prevQSIC = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D raycastResult = Physics2D.Raycast(new Vector2(rect.center.x, rect.yMin), Vector2.down, distance, this._collisionCaster.GetUnionLayerMask());
            Physics2D.queriesStartInColliders = prevQSIC;

            if (raycastResult.collider != null) {
                // need to cast with all the colliders for a better diff
                RaycastHit2D rh2d = _collisionCaster.Cast(Vector2.down, distance, Vector2.zero, Direction.Up);
                if (rh2d.collider != null) {
                    // only apply snap down if distance away isn't trivial
                    Vector2 diff = Vector2.down * distance * rh2d.fraction;
                    if (diff.sqrMagnitude > _collisionCaster.TouchCastDistance * _collisionCaster.TouchCastDistance) {
                        transform.position = pos + diff;
                    }

                    return true;
                }
            }

            // return true if touching down anyway
            if (this._collisionCaster.Touch(Direction.Down)) {
                return true;
            }

            return false;
        }

        #endregion

        #region Private

        private void Awake() {

            // only run when playing
            if (!Application.isPlaying)
                return;

        }

        private void Start() {
            // set script execution order when not playing
            if (Application.isEditor && !Application.isPlaying) {
                UnityEditor.MonoScript thisScript = UnityEditor.MonoScript.FromMonoBehaviour(this);
                if (UnityEditor.MonoImporter.GetExecutionOrder(thisScript) != SCRIPT_EXECUTION_ORDER) {
                    UnityEditor.MonoImporter.SetExecutionOrder(thisScript, SCRIPT_EXECUTION_ORDER);
                    Debug.Log("PlatActor script execution order set to " + SCRIPT_EXECUTION_ORDER);
                }
            }

            this._collisionCaster = GetComponent<CollisionCaster>();
            this._rb2d = GetComponent<Rigidbody2D>();

            // only run when playing
            if (!Application.isPlaying)
                return;

        }

        private void Update() {
            // ensure rb2d's gravityScale is always set to 0.
            this._rb2d.gravityScale = 0;

            // only run when playing
            if (!Application.isPlaying)
                return;

        }

        /// <summary>
        /// PlatActor is set very late in the script execution order, so this should be the last thing that modifies rigidBody2D before the internal physics update
        /// </summary>
        private void FixedUpdate() {

            // only run when playing
            if (!Application.isPlaying)
                return;

            // getting properties
            Vector2 v = _rb2d.velocity;
            RaycastHit2D touchRight = this._collisionCaster.TouchResult(Direction.Right);
            RaycastHit2D touchUp = this._collisionCaster.TouchResult(Direction.Up);
            RaycastHit2D touchLeft = this._collisionCaster.TouchResult(Direction.Left);
            RaycastHit2D touchDown = this._collisionCaster.TouchResult(Direction.Down);

            // apply gravity
            if (this._applyGravity) {
                v += this._gravity * Time.fixedDeltaTime;
            }

            // zero velocity on touch
            if (this._stopOnTouchRight && touchRight) {
                v.x = Mathf.Min(0, v.x);
            }
            if (this._stopOnTouchUp && touchUp) {
                v.y = Mathf.Min(0, v.y);
            }
            if (this._stopOnTouchLeft && touchLeft) {
                v.x = Mathf.Max(0, v.x);
            }
            if (this._stopOnTouchDown && touchDown) {
                v.y = Mathf.Max(0, v.y);
            }

            // apply custom velocity
            this._rb2d.velocity = v;

            // setting new position with MovePosition()
            Vector2 pos = new Vector2(transform.position.x, transform.position.y) + this._rb2d.velocity * Time.fixedDeltaTime;

            // project reposition
            if (this._projectReposition) {
                pos = this._collisionCaster.ProjectReposition(this._rb2d.velocity, Time.fixedDeltaTime);
            }

            // moving platform
            if (this._movedByDownPlatform) {
                if (touchDown) {
                    Rigidbody2D downRb2d = touchDown.rigidbody;
                    if (downRb2d != null) { // can be null if collider hit wasn't attached to a rigidbody
                        Vector2 touchPoint = touchDown.point;
                        Vector2 p1 = new Vector2();

                        // applying platform's angular velocity
                        Vector2 centerPoint = downRb2d.worldCenterOfMass;
                        float rotationRad = downRb2d.angularVelocity * Time.fixedDeltaTime * Mathf.Deg2Rad;
                        float c = Mathf.Cos(rotationRad);
                        float s = Mathf.Sin(rotationRad);
                        p1.x = centerPoint.x + (touchPoint.x - centerPoint.x) * c - (touchPoint.y - centerPoint.y) * s;
                        p1.y = centerPoint.y + (touchPoint.x - centerPoint.x) * s + (touchPoint.y - centerPoint.y) * c;

                        // applying platform's velocity
                        p1 += downRb2d.velocity * Time.fixedDeltaTime;

                        // add to position
                        pos += p1 - touchPoint;
                    }
                }
            }

            // applying position
            this._rb2d.MovePosition(pos);
        }

        CollisionCaster _collisionCaster;
        Rigidbody2D _rb2d;
        #endregion
    }
}