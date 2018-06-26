using UnityEngine;
using System.Collections;

public class TestMove : MonoBehaviour {
    
    void Awake() {
        startPos = transform.position;
        rb2d = GetComponent<Rigidbody2D>();
	}
    
    void FixedUpdate() {

        time += Time.fixedDeltaTime;

        float periodX = 4;
        float magnitudeX = 3;
        float periodY = 3;
        float magnitudeY = 2;
        float rotationSpeed = 20;

        Vector2 pos = startPos + new Vector2(
            Mathf.Sin(time / periodX * Mathf.PI * 2) * magnitudeX,
            Mathf.Sin(time / periodY * Mathf.PI * 2) * magnitudeY
        );

        rb2d.velocity = (pos - rb2d.position) / Time.fixedDeltaTime;
        rb2d.angularVelocity = rotationSpeed;

    }

    Vector2 startPos;

    float time = 0;

    Rigidbody2D rb2d;

}
