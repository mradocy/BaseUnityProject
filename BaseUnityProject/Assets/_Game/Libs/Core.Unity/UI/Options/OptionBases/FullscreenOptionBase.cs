using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI.Options.OptionBases {

    /// <summary>
    /// Option for switching fullscreen modes.  Abstract so implementation can specify names.
    /// </summary>
    public abstract class FullscreenOptionBase : IPropertyOption {

        #region Abstract Properties

        /// <summary>
        /// Gets the display name for this option.
        /// </summary>
        /// <remarks>e.g. "Full Screen"</remarks>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets the display value for when <see cref="FullScreenMode.ExclusiveFullScreen"/> is the fullscreen mode.
        /// </summary>
        /// <remarks>e.g. "Enabled - Exclusive"</remarks>
        protected abstract string ExclusiveFullScreenDisplayValue { get; }

        /// <summary>
        /// Gets the display value for when <see cref="FullScreenMode.FullScreenWindow"/> is the fullscreen mode.
        /// </summary>
        /// <remarks>e.g. "Enabled - Windowed"</remarks>
        protected abstract string FullScreenWindowDisplayValue { get; }

        /// <summary>
        /// Gets the display value for when <see cref="FullScreenMode.Windowed"/> is the fullscreen mode.
        /// </summary>
        /// <remarks>e.g. "Disabled"</remarks>
        protected abstract string WindowedDisplayValue { get; }

        /// <summary>
        /// Gets the display value for when the fullscreen mode is unknown.
        /// </summary>
        /// <remarks>e.g. "Unknown"</remarks>
        protected abstract string UnknownDisplayValue { get; }

        #endregion

        public virtual string DisplayValue {
            get {
                switch (GraphicsSettings.FullScreenMode) {
                case FullScreenMode.ExclusiveFullScreen:
                    return this.ExclusiveFullScreenDisplayValue;
                case FullScreenMode.FullScreenWindow:
                    return this.FullScreenWindowDisplayValue;
                case FullScreenMode.Windowed:
                    return this.WindowedDisplayValue;
                }

                return this.UnknownDisplayValue;
            }
        }

        public bool CanIncrement { get { return Screen.fullScreenMode != FullScreenMode.ExclusiveFullScreen; } }

        public bool CanDecrement { get { return Screen.fullScreenMode != FullScreenMode.Windowed; } }

        public bool CanHoldChange { get { return false; } }

        public void Increment() {

            switch (GraphicsSettings.FullScreenMode) {
            case FullScreenMode.Windowed:
                GraphicsSettings.FullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case FullScreenMode.FullScreenWindow:
                GraphicsSettings.FullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            default:
                GraphicsSettings.FullScreenMode = FullScreenMode.Windowed;
                break;
            }
        }

        public void Decrement() {

            switch (GraphicsSettings.FullScreenMode) {
            case FullScreenMode.ExclusiveFullScreen:
                GraphicsSettings.FullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case FullScreenMode.FullScreenWindow:
                GraphicsSettings.FullScreenMode = FullScreenMode.Windowed;
                break;
            default:
                GraphicsSettings.FullScreenMode = FullScreenMode.Windowed;
                break;
            }
        }
    }
}