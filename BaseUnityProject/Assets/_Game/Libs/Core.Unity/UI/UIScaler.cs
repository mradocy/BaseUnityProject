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

        #region Inspector Fields

        [SerializeField]
        private bool _scaleX = true;

        [SerializeField]
        private bool _scaleY = true;

        #endregion

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

            Vector3 scale = _rectTransform.localScale;
            if (_scaleX) {
                scale.x = UISettings.UIScale;
            }
            if (_scaleY) {
                scale.y = UISettings.UIScale;
            }
            _rectTransform.localScale = scale;

        }

        private RectTransform _rectTransform = null;
    }
}