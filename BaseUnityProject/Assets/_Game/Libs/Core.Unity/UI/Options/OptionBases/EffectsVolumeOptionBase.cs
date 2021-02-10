using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI.Options.OptionBases {

    /// <summary>
    /// Option for specifying the sound effect volume.  Abstract so implementation can specify names.
    /// </summary>
    public abstract class EffectsVolumeOptionBase : IPropertyOption {

        #region Abstract Properties

        /// <summary>
        /// Gets the display name for this option.
        /// </summary>
        /// <remarks>e.g. "Effects Volume"</remarks>
        public abstract string DisplayName { get; }

        #endregion

        public string DisplayValue {
            get {
                return Mathf.RoundToInt(SoundSettings.EffectsVolume * 100) + "%";
            }
        }

        public bool CanHoldChange { get { return true; } }

        public bool CanIncrement { get { return SoundSettings.EffectsVolume < 1; } }

        public void Increment() {
            SoundSettings.EffectsVolume = Mathf.Clamp01(MathUtils.RoundTo(SoundSettings.EffectsVolume + 0.1f, 0.1f));
        }

        public bool CanDecrement { get { return SoundSettings.EffectsVolume > 0; } }

        public void Decrement() {
            SoundSettings.EffectsVolume = Mathf.Clamp01(MathUtils.RoundTo(SoundSettings.EffectsVolume - 0.1f, 0.1f));
        }
    }
}