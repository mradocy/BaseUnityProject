using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI.Options.OptionBases {

    /// <summary>
    /// Option for specifying the VSync Count.  Abstract so implementation can specify names.
    /// </summary>
    public abstract class VSyncCountOptionBase : IPropertyOption {

        #region Abstract Properties

        /// <summary>
        /// Gets the display name for this option.
        /// </summary>
        /// <remarks>e.g. "VSync"</remarks>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets the value for VSync Count 0
        /// </summary>
        /// <remarks>e.g. "Don't Sync"</remarks>
        public abstract string Count0 { get; }

        /// <summary>
        /// Gets the value for VSync Count 1
        /// </summary>
        /// <remarks>e.g. "Every V Blank"</remarks>
        public abstract string Count1 { get; }

        /// <summary>
        /// Gets the value for VSync Count 2
        /// </summary>
        /// <remarks>e.g. "Every Second V Blank"</remarks>
        public abstract string Count2 { get; }

        /// <summary>
        /// Gets the value for VSync Count 3
        /// </summary>
        /// <remarks>e.g. "Every Third V Blank"</remarks>
        public abstract string Count3 { get; }

        /// <summary>
        /// Gets the value for VSync Count 4
        /// </summary>
        /// <remarks>e.g. "Every Fourth V Blank"</remarks>
        public abstract string Count4 { get; }

        /// <summary>
        /// Gets the value for an unknown VSync Count.
        /// </summary>
        /// <remarks>e.g. "Unknown"</remarks>
        public abstract string Unknown { get; }

        #endregion

        public string DisplayValue {
            get {
                switch (GraphicsSettings.VSyncCount) {
                case 0:
                    return this.Count0;
                case 1:
                    return this.Count1;
                case 2:
                    return this.Count2;
                case 3:
                    return this.Count3;
                case 4:
                    return this.Count4;
                }

                return this.Unknown;
            }
        }

        public bool CanHoldChange { get { return false; } }

        public bool CanIncrement { get { return GraphicsSettings.VSyncCount != 4; } }

        public void Increment() {
            if (0 <= GraphicsSettings.VSyncCount && GraphicsSettings.VSyncCount < 4) {
                GraphicsSettings.VSyncCount++;
            } else {
                GraphicsSettings.VSyncCount = 0;
            }
        }

        public bool CanDecrement { get { return GraphicsSettings.VSyncCount != 0; } }

        public void Decrement() {
            if (0 < GraphicsSettings.VSyncCount && GraphicsSettings.VSyncCount <= 4) {
                GraphicsSettings.VSyncCount--;
            } else {
                GraphicsSettings.VSyncCount = 0;
            }
        }
    }
}