using UnityEngine;
using System.Collections;

/// <summary>
/// Destroys itself once an amount of time has passed.
/// </summary>
public class VisualEffect : MonoBehaviour {

    [Tooltip("Determines when this gameObject will be destroyed.\nMANUAL - when gameObject has existed longer than manualDuration.\nANIMATION_END - when animation of the attached animator ends.")]
    public DestroyMode destroyMode;
    [Tooltip("How long until gameObject is destroyed, if destroyMode is set to MANUAL.")]
    public float manualDuration = 1;

    public Animator animator { get; private set; }

    public enum DestroyMode {
        MANUAL,
        ANIMATION_END,
    }

    /// <summary>
    /// Destroys the gameObject.  Can be overridden (e.g. to recycle GameObject instead).
    /// </summary>
    protected virtual void destroyThis() {
        Destroy(gameObject);
    }

    void Awake() {
        animator = GetComponent<Animator>();

        if (destroyMode == DestroyMode.ANIMATION_END && animator == null) {
            Debug.LogError("If VisualEffect's destroyMode is ANIMATION_END, an Animator component must be attached.");
        }
	}
    
    void Update() {

        time += Time.deltaTime;

        switch (destroyMode) {
        case DestroyMode.MANUAL:
            if (time >= manualDuration) {
                destroyThis();
                return;
            }
            break;
        case DestroyMode.ANIMATION_END:
            if (animator.GetStateNormalizedTime() >= 1) {
                destroyThis();
            }
            break;
        }

    }

    float time = 0;
    
}
