using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TestSealimeAnimations : MonoBehaviour {
    
    void Awake() {
        animator = GetComponent<Animator>();
    }
    
    void Update() {

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            animator.Play("jump");
        }

    }

    Animator animator;

}
