using Core.Unity;
using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/* Implementation of LocalizedText may change.  But the Text getter and GetText() should stay.
 */

namespace Core.Unity.Assets {
    [CreateAssetMenu(fileName = "New LocalizedText", menuName = "LocalizedText", order = 51)]
    public class LocalizedText : ScriptableObject {

        [Tooltip("TextAsset to use for the English - United States locale.")]
        public TextAsset enUS;

        [Tooltip("TextAsset to use for the Japanese locale.")]
        public TextAsset ja;

        /// <summary>
        /// Gets the text of the text asset for the current localization.
        /// </summary>
        public string Text {
            get {
                return this.GetText(LocalizationSettings.Localization);
            }
        }

        /// <summary>
        /// Gets the text of the text asset for a given localization.
        /// </summary>
        /// <param name="localization">Localization code to use.</param>
        /// <returns>text</returns>
        public string GetText(LocalizationCode localization) {
            TextAsset textAsset = this.GetTextAsset(localization);
            if (textAsset == null)
                return null;
            
            return textAsset.text;
        }

        /// <summary>
        /// Gets the value for a property defined in this text.  Assumes text contains lines in the form of 'prop_key: val'.
        /// Uses the current localization.
        /// </summary>
        /// <param name="propertyKey">Key of the property.</param>
        /// <returns>Property value</returns>
        public string GetPropertyValue(string propertyKey) {
            return this.GetPropertyValue(LocalizationSettings.Localization, propertyKey);
        }

        /// <summary>
        /// Gets the value for a property defined in this text.  Assumes text contains lines in the form of 'prop_key: val'.
        /// </summary>
        /// <param name="localization">Localization code to use.</param>
        /// <param name="propertyKey">Key of the property.</param>
        /// <returns>Property value</returns>
        public string GetPropertyValue(LocalizationCode localization, string propertyKey) {
            string text = this.GetText(localization);
            if (text == null || string.IsNullOrEmpty(propertyKey))
                return null;

            // TODO: this isn't a great way of finding the property value
            int keyIndex = text.IndexOf($"{propertyKey}:");
            if (keyIndex == -1) {
                return null;
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

        /// <summary>
        /// Gets the text asset for the current localization.
        /// </summary>
        protected TextAsset TextAsset {
            get {
                return this.GetTextAsset(LocalizationSettings.Localization);
            }
        }

        /// <summary>
        /// Gets the text asset for a given localization.
        /// </summary>
        /// <param name="localization">Localization code to use.</param>
        /// <returns>text asset</returns>
        protected TextAsset GetTextAsset(LocalizationCode localization) {
            switch (localization) {
            case LocalizationCode.en_US:
                return this.enUS;
            case LocalizationCode.ja:
                return this.ja;
            }

            Debug.LogError($"Localization {localization} not supported");
            return null;
        }

    }
}