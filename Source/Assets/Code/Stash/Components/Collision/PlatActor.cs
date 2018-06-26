using UnityEngine;
using System.Collections;

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
    
    [Tooltip("Attempts smoother repositioning when colliding with sloped surfaces.")]
    public bool projectReposition = true;

    [Tooltip("If the custom gravity force (given below) should be applied.  Note that rb2d.gravityScale is automatically set to 0 regardless.")]
    public bool applyGravity = true;

    [Tooltip("Gravity unique to this object.  rb2d.gravityScale is automatically set to 0.")]
    public Vector2 gravity = new Vector2(0, -10);
    
    [Tooltip("When touching an object (detected by CollisionCaster) in the downward direction, zero y velocity.")]
    public bool stopOnTouchDown = true;
    [Tooltip("When touching an object (detected by CollisionCaster) in the upward direction, zero y velocity.")]
    public bool stopOnTouchUp = true;

    [Tooltip("When touching an object (detected by CollisionCaster) in the leftward direction, zero x velocity.")]
    public bool stopOnTouchLeft = true;
    [Tooltip("When touching an object (detected by CollisionCaster) in the rightward direction, zero x velocity.")]
    public bool stopOnTouchRight = true;

    [Tooltip("If touching a platform below, the platform's movement will be added to this object's movement.  NOTE: must set platform's velocity and angular velocity, this will not work if the platform is being moved with MovePosition() or MoveRotation()")]
    public bool movedByDownPlatform = true;

    
    /// <summary>
    /// If there's ground a given distance below, will snap down to it so they're touching.
    /// Returns if a snap down occurred.
    /// </summary>
    /// <param name="distance"></param>
    public bool snapDown(float distance) {

        Vector2 pos = transform.position;

        // check raycast from bottom center first
        Rect rect = collisionCaster.getBounds(.01f);
        if (Physics2D.Raycast(new Vector2(rect.center.x, rect.yMin), Vector2.down, distance, collisionCaster.getUnionLayerMask()).collider != null) {
            // need to cast with all the colliders for a better diff
            RaycastHit2D rh2d = collisionCaster.cast(Vector2.down, distance, Vector2.zero, CollisionCaster.Direction.UP);
            if (rh2d.collider != null) {
                Vector2 diff = Vector2.down * distance * rh2d.fraction;
                if (diff.sqrMagnitude > collisionCaster.touchCastDistance * collisionCaster.touchCastDistance) { // don't apply if it's so small that it doesn't matter
                    transform.position = pos + diff;
                    return true;
                }
            }
        }

        return false;
    }

    void Awake() {
        

        if (!Application.isPlaying) return; // only run when playing
        


    }

    void Start() {
        // set script execution order when not playing
        if (Application.isEditor && !Application.isPlaying) {
            UnityEditor.MonoScript thisScript = UnityEditor.MonoScript.FromMonoBehaviour(this);
            if (UnityEditor.MonoImporter.GetExecutionOrder(thisScript) != SCRIPT_EXECUTION_ORDER) {
                UnityEditor.MonoImporter.SetExecutionOrder(thisScript, SCRIPT_EXECUTION_ORDER);
                Debug.Log("PlatActor script execution order set to " + SCRIPT_EXECUTION_ORDER);
            }
        }

        collisionCaster = GetComponent<CollisionCaster>();
        rb2d = GetComponent<Rigidbody2D>();

        if (!Application.isPlaying) return; // only run when playing



    }

    void Update() {
        // ensure rb2d's gravityScale is always set to 0.
        rb2d.gravityScale = 0;

        if (!Application.isPlaying) return; // only run when playing


    }
    
    /// <summary>
    /// PlatActor is set very late in the script execution order, so this should be the last thing that modifies rigidBody2D before the internal physics update
    /// </summary>
    void FixedUpdate() {
        if (!Application.isPlaying) return; // only run when playing

        // getting properties
        Vector2 v = rb2d.velocity;
        RaycastHit2D touchRight = collisionCaster.touchResult(CollisionCaster.Direction.RIGHT);
        RaycastHit2D touchUp = collisionCaster.touchResult(CollisionCaster.Direction.UP);
        RaycastHit2D touchLeft = collisionCaster.touchResult(CollisionCaster.Direction.LEFT);
        RaycastHit2D touchDown = collisionCaster.touchResult(CollisionCaster.Direction.DOWN);

        // zero velocity on touch
        if (stopOnTouchRight && touchRight) {
            v.x = Mathf.Min(0, v.x);
        }
        if (stopOnTouchUp && touchUp) {
            v.y = Mathf.Min(0, v.y);
        }
        if (stopOnTouchLeft && touchLeft) {
            v.x = Mathf.Max(0, v.x);
        }
        if (stopOnTouchDown && touchDown) {
            v.y = Mathf.Max(0, v.y);
        }
        
        // custom gravity
        if (applyGravity) {
            v += gravity * Time.fixedDeltaTime;
        }

        // apply custom velocity
        rb2d.velocity = v;

        // setting new position with MovePosition()
        Vector2 pos = new Vector2(transform.position.x, transform.position.y) + rb2d.velocity * Time.fixedDeltaTime;

        // project reposition
        if (projectReposition) {
            pos = collisionCaster.projectReposition(rb2d.velocity, Time.fixedDeltaTime);
        }
        
        // moving platform
        if (movedByDownPlatform) {
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
        rb2d.MovePosition(pos);

    }

    CollisionCaster collisionCaster;
    Rigidbody2D rb2d;
    
}
