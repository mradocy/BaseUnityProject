using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI.Options.OptionBases {

    /// <summary>
    /// Option for specifying the maximum number of frames queued up by graphics driver.  Abstract so implementation can specify names.
    /// </summary>
    public abstract class MaxQueuedFramesOptionBase : IPropertyOption {

        #region Abstract Properties

        /// <summary>
        /// Gets the display name for this option.
        /// </summary>
        /// <remarks>e.g. "VSync"</remarks>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets the max value.
        /// </summary>
        /// <remarks>e.g. 10</remarks>
        public abstract int MaxValue { get; }

        #endregion

        public string DisplayValue {
            get {
                return $"{GraphicsSettings.MaxQueuedFrames}";
            }
        }

        public bool CanHoldChange { get { return false; } }

        public bool CanIncrement { get { return GraphicsSettings.MaxQueuedFrames < this.MaxValue; } }

        public void Increment() {
            GraphicsSettings.MaxQueuedFrames = Mathf.Min(this.MaxValue, GraphicsSettings.MaxQueuedFrames + 1);
        }

        public bool CanDecrement { get { return GraphicsSettings.MaxQueuedFrames > 0; } }

        public void Decrement() {
            GraphicsSettings.MaxQueuedFrames = Mathf.Max(0, GraphicsSettings.MaxQueuedFrames - 1);
        }
    }
}