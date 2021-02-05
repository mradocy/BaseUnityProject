using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Sound {

    /// <summary>
    /// A sibling MonoBehavior to <see cref="AudioSource"/> that provides fade functionality.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceHelper : MonoBehaviour {

        public AudioSource AudioSource { get; private set; }

        /// <summary>
        /// Plays the audio source at the given volume without fading.
        /// </summary>
        public void Play(float volume) {
            this.AudioSource.volume = volume;
            if (!this.AudioSource.isPlaying) {
                this.AudioSource.Play();
            }
            _isFading = false;
        }

        /// <summary>
        /// Fades the audio source to volume = 1 over time.
        /// </summary>
        public void FadeIn(float duration) {
            this.FadeTo(1, duration);
        }

        /// <summary>
        /// Fades the audio source to the given volume over time.
        /// </summary>
        public void FadeTo(float volume, float duration) {
            if (duration <= 0) {
                this.Play(volume);
                return;
            }

            // if not currently playing, treated as starting at volume = 0
            if (!this.AudioSource.isPlaying) {
                this.AudioSource.volume = 0;
                this.AudioSource.Play();
            }

            _isFading = true;
            _fadeStartUnscaledTime = Time.unscaledTime;
            _fadeVolume0 = this.AudioSource.volume;
            _fadeVolume1 = volume;
            _fadeDuration = duration;
            _fadeStopWhenComplete = false;
        }

        /// <summary>
        /// Stops the audio source immediately.
        /// </summary>
        public void Stop() {
            if (this.AudioSource.isPlaying) {
                this.AudioSource.Stop();
            }
            _isFading = false;
        }

        /// <summary>
        /// Fades the audio source out over time then stops it.
        /// </summary>
        public void FadeOutThenStop(float duration) {
            if (!this.AudioSource.isPlaying)
                return;

            if (duration <= 0) {
                this.Stop();
                return;
            }

            this.FadeTo(0, duration);
            _fadeStopWhenComplete = true;
        }

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            this.AudioSource = this.EnsureComponent<AudioSource>();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {
            // stop fading if audio source was stopped by outside means
            if (_isFading && !this.AudioSource.isPlaying) {
                _isFading = false;
            }
            
            // manage fading
            if (_isFading) {
                float fadeTime = Time.unscaledTime - _fadeStartUnscaledTime;
                if (fadeTime >= _fadeDuration) {
                    // end fading
                    if (_fadeStopWhenComplete) {
                        this.Stop();
                    } else {
                        this.Play(_fadeVolume1);
                    }
                } else {
                    // continue fading
                    float volume = Easing.Linear(_fadeVolume0, _fadeVolume1, fadeTime, _fadeDuration);
                    this.AudioSource.volume = volume;
                }
            }
        }

        private bool _isFading = false;
        private float _fadeStartUnscaledTime;
        private float _fadeDuration;
        private float _fadeVolume0;
        private float _fadeVolume1;
        private bool _fadeStopWhenComplete;
    }
}