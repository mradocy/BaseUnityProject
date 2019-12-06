using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI.Options.OptionBases {

    /// <summary>
    /// Option for specifying the UI scale.  Abstract so implementation can specify names.
    /// </summary>
    public abstract class UIScaleOptionBase : IPropertyOption {

        #region Abstract Properties

        /// <summary>
        /// Gets the display name for this option.
        /// </summary>
        /// <remarks>e.g. "UI Scale"</remarks>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets the display value format for displaying the ui scale
        /// </summary>
        /// <remarks>e.g. "{0:0.0}"</remarks>
        protected abstract string DisplayValueFormat { get; }

        /// <summary>
        /// Gets the min UI scale.
        /// </summary>
        /// <remarks>e.g. 0.5f</remarks>
        protected abstract float MinScale { get; }

        /// <summary>
        /// Gets the max UI scale.
        /// </summary>
        /// <remarks>e.g. 2.0f</remarks>
        protected abstract float MaxScale { get; }

        /// <summary>
        /// Gets the step value when changing the UI scale.
        /// </summary>
        /// <remarks>e.g. 0.1f</remarks>
        protected abstract float OptionStep { get; }

        #endregion

        public string DisplayValue {
            get {
                return string.Format(this.DisplayValueFormat, UISettings.UIScale);
            }
        }

        public bool CanHoldChange { get { return true; } }

        public bool CanIncrement { get { return UISettings.UIScale < this.MaxScale; } }

        public void Increment() {
            UISettings.UIScale = MathUtils.RoundTo(Mathf.Min(MaxScale, UISettings.UIScale + this.OptionStep), this.OptionStep);
        }

        public bool CanDecrement { get { return UISettings.UIScale > this.MinScale; } }

        public void Decrement() {
            UISettings.UIScale = MathUtils.RoundTo(Mathf.Max(this.MinScale, UISettings.UIScale - this.OptionStep), this.OptionStep);
        }
    }
}