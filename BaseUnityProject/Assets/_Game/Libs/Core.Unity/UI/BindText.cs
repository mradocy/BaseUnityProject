using Core.Unity.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unity.UI {
    [ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class BindText : MonoBehaviour {

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

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            this.UpdateText();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {
            if (!Application.isPlaying || _bindWhenPlaying) {
                this.UpdateText();
            }
        }

        /// <summary>
        /// Updates the text in the text component.
        /// </summary>
        private void UpdateText() {
            Text textComponent = this.GetComponent<Text>();
            if (textComponent == null)
                return;

            if (_forceLocalization == LocalizationCode.None) {
                textComponent.text = GetComponentText(_text, _propertyKey);
            } else {
                textComponent.text = GetComponentText(_text, _forceLocalization, _propertyKey);
            }
        }

        /// <summary>
        /// Gets the text that will be set to the Text component, using the default localization.
        /// </summary>
        /// <param name="localizedTextAsset">Localized text asset to pull the text from.</param>
        /// <param name="propertyKey">If given, will pull the value of a property instead of the text file's entire content.</param>
        /// <returns>text</returns>
        public static string GetComponentText(LocalizedText localizedTextAsset, string propertyKey) {
            return GetComponentText(localizedTextAsset, Localization.Current, propertyKey);
        }

        /// <summary>
        /// Gets the text that will be set to the Text component.
        /// </summary>
        /// <param name="localizedTextAsset">Localized text asset to pull the text from.</param>
        /// <param name="localization">Localization code.</param>
        /// <param name="propertyKey">If given, will pull the value of a property instead of the text file's entire content.</param>
        /// <returns>text</returns>
        public static string GetComponentText(LocalizedText localizedTextAsset, LocalizationCode localization, string propertyKey) {

            if (localizedTextAsset == null) {
                return $"<Localized text asset not defined>";
            }

            if (localization == LocalizationCode.None) {
                localization = Localization.Current;
            }
            string text = localizedTextAsset.GetText(localization);
            if (text == null) {
                return $"<Text for localization \"{Localization.CodeToString(localization)}\" not defined>";
            }

            if (string.IsNullOrEmpty(propertyKey)) {
                return text;
            } else {

                // TODO: this isn't a great way of finding the property value
                int keyIndex = text.IndexOf($"{propertyKey}:");
                if (keyIndex == -1) {
                    return $"<String \"{propertyKey}:\" not found for localization \"{Localization.CodeToString(localization)}\">";
                }
                int valueStartIndex = keyIndex + propertyKey.Length + 1; // +1 for ":"
                int valueEndIndex = text.IndexOf('\n', valueStartIndex);
                string value;
                if (valueEndIndex == -1) {
                    value = text.Substring(valueStartIndex).Trim();
                } else {
                    value = text.Substring(valueStartIndex, valueEndIndex - valueStartIndex).Trim();
                }

                return value;
            }
        }

    }
}