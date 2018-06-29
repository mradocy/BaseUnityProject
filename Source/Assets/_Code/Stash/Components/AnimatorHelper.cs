
using UnityEngine;
using System.Collections;

/// <summary>
/// Put this component next to the associating Animator component
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatorHelper : MonoBehaviour {

    public Animator animator {
        get {
            if (_animator == null)
                _animator = GetComponent<Animator>();
            return _animator;
        }
    }

    /// <summary>
    /// A smoother way to play a state on the animator.  Uses a crossfade.  If a crossfade is already happening, the animation is interrupted.
    /// </summary>
    /// <param name="stateName">state to play.</param>
    /// <param name="transitionDuration">duration of the crossfade.  Set to 0 to immediately start the given state.</param>
    /// <param name="normalizedTime">start time (normalized) of state.  Set to float.NegativeInfinity state will either be played from beginning or will continue playing from current time.</param>
    public void play(string stateName, float transitionDuration = .1f, float normalizedTime = float.NegativeInfinity) {
        
        if (animator.IsInTransition(0)) {
            animator.Rebind(); // need to do this, otherwise animation will be ignored thanks to an oversight of the crossfade.
            animator.Play(stateName, 0, normalizedTime);
        } else {
            if (transitionDuration == 0) {
                animator.Rebind();
                animator.Play(stateName, 0, normalizedTime);
            } else {
                animator.CrossFade(stateName, transitionDuration, 0, normalizedTime);
            }
        }
    }

    /// <summary>
    /// Gets if the current state matches the given name.
    /// </summary>
    /// <param name="stateName">Name of the state.</param>
    public bool currentStateIs(string stateName) {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    public float stateDuration {
        get {
            return animator.GetCurrentAnimatorStateInfo(0).length;
        }
    }
    public float stateNormalizedTime {
        get {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
    }
    /// <summary>
    /// Gets if animator is currently at the end of the given state.
    /// </summary>
    /// <param name="stateName">Name of the state expected to be running.  If the current state is different, this function returns false.</param>
    public bool isAtEnd(string stateName) {
        if (!currentStateIs(stateName)) return false;
        return stateNormalizedTime >= 1;
    }

    public bool hasParameter(string paramName) {
        foreach (AnimatorControllerParameter param in animator.parameters) {
            if (param.name == paramName) return true;
        }
        return false;
    }
    public bool inTransition {
        get { return animator.IsInTransition(0); }
    }

    #region Animation Events

    public void AE_sendMessage(string message) {
        SendMessageUpwards(message, SendMessageOptions.DontRequireReceiver);
    }
    public void AE_sendMessage_AttackBegin() {
        AE_sendMessage("AttackBegin");
    }
    public void AE_sendMessage_AttackEnd() {
        AE_sendMessage("AttackEnd");
    }
    public void AE_playSFX(AudioClip clip) {
        SFXManager.instance.play(clip);
    }
    public void AE_playSFXRandPitchBend(AudioClip clip) {
        SFXManager.instance.playRandPitchBend(clip);
    }

    #endregion

    void Awake() {
        _animator = GetComponent<Animator>();
        if (animator == null) {
            Debug.LogWarning("AnimatorHelper does not have associating Animator");
        }
    }

    private Animator _animator;


}
