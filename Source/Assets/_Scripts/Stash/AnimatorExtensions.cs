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
    
    /// <summary>
    /// Gets if animator is currently at the end of the given state.
    /// </summary>
    /// <param name="stateName">Name of the state expected to be running.  If the current state is different, this function returns false.</param>
    public static bool IsAtEnd(this Animator animator, string stateName) {
        if (!animator.CurrentStateIs(stateName)) return false;
        return animator.GetStateNormalizedTime() >= 1;
    }

}
