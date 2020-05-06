﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Unity.Settings {

    /// <summary>
    /// Static class for accessing and setting sound settings.
    /// </summary>
    public static class SoundSettings {

        #region Constants

        /// <summary>
        /// Default volume on a range in [0, 1]
        /// </summary>
        public const float DefaultVolume = 0.7f;

        #endregion

        #region Initialization Keys

        /// <summary>
        /// Key for accessing the effect volume from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _effectsVolumeInitializationKey = "effects_volume";

        #endregion

        #region Initialization

        /// <summary>
        /// Called by Unity before any scene is loaded.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            Initialization.CallOnInitialize(Initialize);
        }

        private static void Initialize() {
            if (_isInitialized)
                return;

            // get initial values from .ini file
            _effectsVolume = Initialization.Settings.GetFloat(_effectsVolumeInitializationKey, DefaultVolume);

            _isInitialized = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current effects volume.
        /// </summary>
        public static float EffectsVolume {
            get { return _effectsVolume; }
            set {
                if (_effectsVolume == value)
                    return;

                _effectsVolume = value;
                Initialization.Settings.SetFloat(_effectsVolumeInitializationKey, value);
                if (_audioMixer == null) {
                    LogAudioMixerNotSet();
                    return;
                }

                SetAudioMixerValue(_effectsVolumeParameterKey, VolumeToAudioMixerVolume(_effectsVolume));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the AudioMixer whose values to change with the settings change
        /// </summary>
        /// <param name="audioMixer">The AudioMixer to modify.</param>
        /// <param name="effectsVolumeParameterKey">The name of the parameter in the AudioMixer to change when the <see cref="EffectsVolume"/> changes.</param>
        public static void SetAudioMixer(AudioMixer audioMixer, string effectsVolumeParameterKey) {
            if (!_isInitialized) {
                Debug.LogError("SoundSettings has not been initialized yet");
                return;
            }
            if (_audioMixer != null) {
                Debug.LogError("AudioMixer for SoundSettings has already been set.");
                return;
            }

            _audioMixer = audioMixer;
            _effectsVolumeParameterKey = effectsVolumeParameterKey;

            // set initial values
            SetAudioMixerValue(_effectsVolumeParameterKey, VolumeToAudioMixerVolume(_effectsVolume));
        }

        /// <summary>
        /// Converts a volume in [0, 1] to a value to be set as the volume in the Attenuation effect of an AudioMixerGroup.
        /// </summary>
        /// <param name="volume02">Volume</param>
        /// <returns></returns>
        public static float VolumeToAudioMixerVolume(float volume01) {
            if (volume01 <= 0) {
                return -80;
            } else if (volume01 < DefaultVolume) {
                return Easing.QuadOut(-40, 0, volume01, DefaultVolume);
            } else {
                return Easing.Linear(0, 10, volume01 - DefaultVolume, 1 - DefaultVolume);
            }
        }

        #endregion

        #region Private

        private static void SetAudioMixerValue(string name, float value) {
            if (!_audioMixer.SetFloat(name, value)) {
                Debug.LogError($"Could not set value with name \"{name}\" to the AudioMixer.");
            }
        }

        private static void LogAudioMixerNotSet() {
            Debug.LogError("Cannot set sound setting because AudioMixer has not been set.");
        }

        private static bool _isInitialized = false;

        private static float _effectsVolume;

        private static AudioMixer _audioMixer = null;
        private static string _effectsVolumeParameterKey = null;

        #endregion
    }
}