using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity.Settings;

namespace Core.Unity.UI {

    /// <summary>
    /// Binds the scale of the sibling RectTransform to the global <see cref="UISettings.UIScale"/> value.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIScaler : MonoBehaviour {

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            _rectTransform = this.EnsureComponent<RectTransform>();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {

            _rectTransform.localScale = new Vector3(UISettings.UIScale, UISettings.UIScale, 1);

        }

        private RectTransform _rectTransform = null;
    }
}