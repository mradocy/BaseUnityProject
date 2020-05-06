using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                _audioMixer.OnEffectsVolumeSettingChanged(VolumeToAudioMixerVolume(_effectsVolume));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the <see cref="ISoundSettingsAudioMixer"/> whose values to modify when the settings change.
        /// </summary>
        /// <param name="audioMixer"></param>
        public static void SetAudioMixer(ISoundSettingsAudioMixer audioMixer) {
            if (!_isInitialized) {
                Debug.LogError("SoundSettings has not been initialized yet");
                return;
            }
            if (_audioMixer != null) {
                Debug.LogError("AudioMixer for SoundSettings has already been set.");
                return;
            }

            _audioMixer = audioMixer;
            _audioMixer.OnEffectsVolumeSettingChanged(VolumeToAudioMixerVolume(_effectsVolume));
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

        private static void LogAudioMixerNotSet() {
            Debug.LogError("Cannot set audio setting because audio mixer has not been set.");
        }

        private static bool _isInitialized = false;

        private static float _effectsVolume;
        private static ISoundSettingsAudioMixer _audioMixer = null;

        #endregion
    }
}