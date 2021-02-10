using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI.Options.OptionBases {

    /// <summary>
    /// Option for specifying the music volume.  Abstract so implementation can specify names.
    /// </summary>
    public abstract class MusicVolumeOptionBase : IPropertyOption {

        #region Abstract Properties

        /// <summary>
        /// Gets the display name for this option.
        /// </summary>
        /// <remarks>e.g. "Music Volume"</remarks>
        public abstract string DisplayName { get; }

        #endregion

        public string DisplayValue {
            get {
                return Mathf.RoundToInt(SoundSettings.MusicVolume * 100) + "%";
            }
        }

        public bool CanHoldChange { get { return true; } }

        public bool CanIncrement { get { return SoundSettings.MusicVolume < 1; } }

        public void Increment() {
            SoundSettings.MusicVolume = Mathf.Clamp01(MathUtils.RoundTo(SoundSettings.MusicVolume + 0.1f, 0.1f));
        }

        public bool CanDecrement { get { return SoundSettings.MusicVolume > 0; } }

        public void Decrement() {
            SoundSettings.MusicVolume = Mathf.Clamp01(MathUtils.RoundTo(SoundSettings.MusicVolume - 0.1f, 0.1f));
        }
    }
}