using UnityEngine;
using System.Collections;

public static class AnimatorExtensions {

    /// <summary>
    /// Gets if the current state matches the given name.
    /// </summary>
    /// <param name="stateName">Name of the state.</param>
    public static bool CurrentStateIs(this Animator animator, string stateName) {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public static float GetStateDuration(this Animator animator) {
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
    public static float GetStateNormalizedTime(this Animator animator) {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    public static float GetStateSpeed(this Animator animator) {
        return animator.GetCurrentAnimatorStateInfo(0).speed;
    }
    
    /// <summary>
    /// Gets if animator is currently at the end of the given state.
    /// </summary>
    /// <param name="stateName">Name of the state expected to be running.  If the current state is different, this function returns false.</param>
    public static bool IsAtEnd(this Animator animator, string stateName) {
        if (!animator.CurrentStateIs(stateName)) return false;
        return animator.GetStateNormalizedTime() >= 1;
    }

    /// <summary>
    /// A smoother way to play a state on the animator.  Uses a crossfade.  If a crossfade is already happening, the animation is interrupted.
    /// </summary>
    /// <param name="stateName">state to play.</param>
    /// <param name="transitionDuration">duration of the crossfade.  Set to 0 to immediately start the given state.</param>
    /// <param name="normalizedTime">start time (normalized) of state.  Set to float.NegativeInfinity state will either be played from beginning or will continue playing from current time.</param>
    public static void PlaySmooth(this Animator animator, string stateName, float transitionDuration = .1f, float normalizedTime = float.NegativeInfinity) {

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

}
