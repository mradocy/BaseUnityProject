using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI.Options.OptionBases {

    /// <summary>
    /// Option for running application in the background.
    /// </summary>
    public abstract class RunInBackgroundOptionBase : IPropertyOption {

        #region Abstract Properties

        /// <summary>
        /// Gets the display name for this option.
        /// </summary>
        /// <remarks>e.g. "Run In Background"</remarks>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets the display value for when the game doesn't run in the background.
        /// </summary>
        /// <remarks>e.g. "Disabled"</remarks>
        protected abstract string DisabledDisplayValue { get; }

        /// <summary>
        /// Gets the display value for when the game runs in the background.
        /// </summary>
        /// <remarks>e.g. "Enabled"</remarks>
        protected abstract string EnabledDisplayValue { get; }

        #endregion

        public virtual string DisplayValue {
            get {
                return ApplicationSettings.RunInBackground ? this.EnabledDisplayValue : this.DisabledDisplayValue;
            }
        }

        public bool CanIncrement { get { return !ApplicationSettings.RunInBackground; } }

        public bool CanDecrement { get { return ApplicationSettings.RunInBackground; } }

        public bool CanHoldChange => false;

        public void Increment() {
            ApplicationSettings.RunInBackground = true;
        }

        public void Decrement() {
            ApplicationSettings.RunInBackground = false;
        }
    }
}