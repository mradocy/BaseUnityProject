using Core.Unity;
using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// TODO: delete this

/* Implementation of LocalizedText may change.  But the Text getter and GetText() should stay.
 */

namespace Core.Unity.Assets {
    [CreateAssetMenu(fileName = "New LocalizedText", menuName = "Localization/LocalizedTextAsset", order = 51)]
    public class LocalizedTextAsset : ScriptableObject {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("TextAsset to use for the English - United States locale.")]
        private TextAsset _enUS;

        [SerializeField]
        [Tooltip("TextAsset to use for the Japanese locale.")]
        private TextAsset _ja;

        #endregion

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
        /// Tries to get the value for a property defined in this text.  Assumes text contains lines in the form of 'prop_key: val'.
        /// </summary>
        /// <param name="propertyKey">Key of the property.</param>
        /// <param name="value">out param representing the found value.</param>
        /// <returns>If property was found</returns>
        public bool TryGetPropertyValue(string propertyKey, out string value) {
            return this.TryGetPropertyValue(LocalizationSettings.Localization, propertyKey, out value);
        }

        /// <summary>
        /// Gets the value for a property defined in this text.  Assumes text contains lines in the form of 'prop_key: val'.
        /// </summary>
        /// <param name="localization">Localization code to use.</param>
        /// <param name="propertyKey">Key of the property.</param>
        /// <returns>Property value</returns>
        public string GetPropertyValue(LocalizationCode localization, string propertyKey) {
            if (this.TryGetPropertyValue(localization, propertyKey, out string value)) {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Tries to get the value for a property defined in this text.  Assumes text contains lines in the form of 'prop_key: val'.
        /// </summary>
        /// <param name="localization">Localization code to use.</param>
        /// <param name="propertyKey">Key of the property.</param>
        /// <param name="value">out param representing the found value.</param>
        /// <returns>If property was found</returns>
        public bool TryGetPropertyValue(LocalizationCode localization, string propertyKey, out string value) {
            string text = this.GetText(localization);
            if (text == null || string.IsNullOrEmpty(propertyKey)) {
                value = null;
                return false;
            }

            // TODO: this isn't a great way of finding the property value
            value = null;
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

            if (value == null) {
                return false;
            }

            return true;
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
                return this._enUS;
            case LocalizationCode.ja:
                return this._ja;
            }

            Debug.LogError($"Localization {localization} not supported");
            return null;
        }

    }
}