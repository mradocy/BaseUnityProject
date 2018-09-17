
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    #region Event Sounds

    public List<EventSound> eventSounds;

    [Tooltip("AudioSource the sounds are played in.  If null, will look for a sibling AudioSource")]
    public AudioSource audioSource;
    
    /// <summary>
    /// In the Animator, add an animation event that calls this function.
    /// </summary>
    /// <param name="soundName">The name of the sound to play (specified in the event sounds)</param>
    public virtual void PlaySound(string soundName) {

        if (eventSounds == null) return;
        EventSound soundEvent = eventSounds.Find(item => item.name == soundName);

        if (soundEvent != null) {

            // match the AnimatorSpeed with the Sound Pitch
            if (animator) audioSource.pitch = animator.speed;

            soundEvent.PlayAudio(audioSource);
        }
    }

    ///// <summary>
    ///// In the Animator, add an animation event that calls this function.
    ///// </summary>
    ///// <param name="ae">The AnimationEvent.  This is specified for the animation in the Animator.  The string value is the name of the sound to play.</param>
    //public virtual void PlaySoundx(AnimationEvent ae) {
    //    PlaySound(ae.stringParameter);
    //}

    [System.Serializable]
    public class EventSound {

        [Tooltip("Name of the sound, as specified in the string parameter of the AnimationEvent.")]
        public string name = "";
        [Tooltip("AudioClip to play.  If multiple are specified, a random clip will be chosen.")]
        public AudioClip[] clips;
        public float volume = 1;
        public float pitch = 1;
        public float pitchRandRange = .05f;

        public void PlayAudio(AudioSource audioSource) {
            if (audioSource == null) {
                Debug.LogWarning("Cannot play audio because audioSource is null");
                return;
            }
            if (clips == null || clips.Length == 0) return;

            audioSource.clip = clips[Random.Range(0, clips.Length)];
            audioSource.pitch = pitch + Random.Range(-pitchRandRange, pitchRandRange);
            audioSource.volume = volume;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    #endregion


    void Awake() {
        _animator = GetComponent<Animator>();
        if (animator == null) {
            Debug.LogError("AnimatorHelper does not have associating Animator");
        }

        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }

    private Animator _animator;


}
