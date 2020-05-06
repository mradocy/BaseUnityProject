using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Settings {

    /// <summary>
    /// Interface that sets the volumes of AudioGroups in an AudioMixer.  To be set with <see cref="SoundSettings.SetAudioMixer(ISoundSettingsAudioMixer)"/>.
    /// </summary>
    public interface ISoundSettingsAudioMixer {

        /// <summary>
        /// Called when the value of <see cref="SoundSettings.EffectsVolume"/> changes.
        /// </summary>
        /// <param name="audioMixerEffectsVolume">Volume to be set to the Attenuation effect of an effects AudioMixerGroup.</param>
        void OnEffectsVolumeSettingChanged(float audioMixerEffectsVolume);
    }
}