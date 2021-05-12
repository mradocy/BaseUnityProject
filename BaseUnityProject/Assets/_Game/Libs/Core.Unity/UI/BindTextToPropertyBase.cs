using Core.Unity;
using Core.Unity.Assets;
using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI {

    [ExecuteAlways]
    public abstract class BindTextToPropertyBase : MonoBehaviour {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("Localized property table asset to bind to the content of the text component.")]
        private LocalizedPropertyTableAsset _propertyTableAsset = null;

        [SerializeField]
        private string _propertyKey = null;

        [SerializeField]
        [Tooltip("Forces a specific localization to be used, if a value other than None is specified.  Useful for debugging.")]
        private LocalizationCode _forceLocalization = LocalizationCode.None;

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Updates the text in the text component.
        /// </summary>
        protected abstract void UpdateText();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected void Awake() {
            this.UpdateText();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected void Update() {
            if (!Application.isPlaying) {
                this.UpdateText();
            }
        }

        /// <summary>
        /// Gets the text that will be set to the component.
        /// </summary>
        /// <returns>text</returns>
        protected string GetComponentText() {
            if (_propertyTableAsset == null) {
                return $"<Property table asset not defined>";
            }

            LocalizationCode localization = _forceLocalization;
            if (localization == LocalizationCode.None) {
                localization = LocalizationSettings.Localization;
            }

            string value = _propertyTableAsset.GetValue(_propertyKey, localization);
            if (string.IsNullOrEmpty(value)) {
                return $"<Property \"{_propertyKey}:\" not found for localization \"{LocalizationSettings.CodeToString(localization)}\">";
            }

            return value;
        }

        #endregion
    }
}