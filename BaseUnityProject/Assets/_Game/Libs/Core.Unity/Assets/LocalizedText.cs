using Core.Unity;
using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            string value = null;
            using (StringReader stringReader = new StringReader(text)) {
                while (true) {
                    string line = stringReader.ReadLine();
                    if (line == null)
                        break;

                    int colonIndex = line.IndexOf(':');
                    if (colonIndex == -1)
                        continue;
                    string key = line.Substring(0, colonIndex).Trim();
                    if (!string.Equals(propertyKey, key, System.StringComparison.OrdinalIgnoreCase))
                        continue;

                    value = line.Substring(colonIndex + 1).Trim();
                    break;
                }
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