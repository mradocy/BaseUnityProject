using UnityEngine;
using System.Collections;

public class TestCollision : MonoBehaviour {
    
    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collisionCaster = GetComponent<CollisionCaster>();
    }
    
    void Update() {

        float speed = 8;
        Vector2 v = rb2d.velocity;
        if (Input.GetKey(KeyCode.LeftArrow)) {
            v.x = -speed;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            v.x = speed;
        } else {
            v.x = 0;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            v.y = -speed;
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            v.y = speed;
        } else {
            //v.y = 0;
        }
        rb2d.velocity = v;


        Color c = Color.gray;

        if (collisionCaster.touch(CollisionCaster.Direction.DOWN)) {
            c = Color.red;
            UDeb.post("touch down normal", collisionCaster.touchNormal(CollisionCaster.Direction.DOWN));
        } else if (collisionCaster.touch(CollisionCaster.Direction.UP)) {
            c = Color.green;
        } else if (collisionCaster.touch(CollisionCaster.Direction.LEFT)) {
            c = Color.yellow;
        } else if (collisionCaster.touch(CollisionCaster.Direction.RIGHT)) {
            c = Color.blue;
        }

        spriteRenderer.color = c;

    }

    void FixedUpdate() {
        
    }

    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;
    CollisionCaster collisionCaster;

}
