using Core.Unity.Assets;
using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unity.UI {

    [ExecuteAlways]
    public abstract class BindTextBase : MonoBehaviour {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("Localized text asset to bind to the content of the Text component.")]
        private LocalizedText _text = null;

        [SerializeField]
        [Tooltip("If given, text will instead be set to whatever value is to the right of \"property_key: \" in the source file.")]
        private string _propertyKey = null;

        [SerializeField]
        [Tooltip("If content of Text component should continuously be bound when the game is playing.")]
        private bool _bindWhenPlaying = false;

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
            if (!Application.isPlaying || _bindWhenPlaying) {
                this.UpdateText();
            }
        }

        /// <summary>
        /// Gets the text that will be set to the component.
        /// </summary>
        /// <returns>text</returns>
        protected string GetComponentText() {
            if (_text == null) {
                return $"<Localized text asset not defined>";
            }

            LocalizationCode localization = _forceLocalization;
            if (localization == LocalizationCode.None) {
                localization = LocalizationSettings.Localization;
            }

            if (string.IsNullOrEmpty(_propertyKey)) {
                return _text.GetText(localization);
            }

            string value = _text.GetPropertyValue(localization, _propertyKey);
            if (string.IsNullOrEmpty(value)) {
                return $"<String \"{_propertyKey}:\" not found for localization \"{LocalizationSettings.CodeToString(localization)}\">";
            }

            return value;
        }

        #endregion
    }
}