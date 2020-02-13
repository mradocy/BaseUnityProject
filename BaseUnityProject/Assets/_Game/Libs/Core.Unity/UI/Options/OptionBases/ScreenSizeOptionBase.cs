using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI.Options.OptionBases {

    /// <summary>
    /// Option for switching screen sizes.  Abstract so implementation can specify names.
    /// </summary>
    public abstract class ScreenSizeOptionBase : IPropertyOption {

        public ScreenSizeOptionBase() {
            _screenSizes = new System.Lazy<Vector2Int[]>(() => GraphicsSettings.GetScreenSizes());
        }

        #region Abstract Properties

        /// <summary>
        /// Gets the display name for this option.
        /// </summary>
        /// <remarks>e.g. "Screen Size"</remarks>
        public abstract string DisplayName { get; }

        #endregion

        public virtual string DisplayValue {
            get {
                Vector2Int screenSize = GraphicsSettings.ScreenSize;
                return $"{screenSize.x}x{screenSize.y}";
            }
        }

        public bool CanIncrement {
            get {
                return this.GetCurrentScreenSizeIndex() < _screenSizes.Value.Length - 1;
            }
        }

        public bool CanDecrement {
            get {
                return this.GetCurrentScreenSizeIndex() > 0;
            }
        }

        public bool CanHoldChange { get { return false; } }

        public void Increment() {
            int index = this.GetCurrentScreenSizeIndex();
            GraphicsSettings.ScreenSize = _screenSizes.Value[Mathf.Min(_screenSizes.Value.Length - 1, index + 1)];
        }

        public void Decrement() {
            int index = this.GetCurrentScreenSizeIndex();
            GraphicsSettings.ScreenSize = _screenSizes.Value[Mathf.Max(0, index - 1)];
        }

        private int GetCurrentScreenSizeIndex() {
            Vector2Int currentScreenSize = GraphicsSettings.ScreenSize;
            Vector2Int[] screenSizes = _screenSizes.Value;
            for (int i=0; i < screenSizes.Length; i++) {
                if (screenSizes[i].x == currentScreenSize.x &&
                    screenSizes[i].y == currentScreenSize.y)
                    return i;
            }

            return -1;
        }

        private System.Lazy<Vector2Int[]> _screenSizes = null;
    }
}