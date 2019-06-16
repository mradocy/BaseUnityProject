using Core.Unity;
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
                return this.GetText(Localization.Current);
            }
        }

        /// <summary>
        /// Gets the text of the text asset for a given localization.
        /// </summary>
        /// <param name="localization">Localization code to use.</param>
        /// <returns>text</returns>
        public string GetText(LocalizationCode localization) {
            return this.GetTextAsset(localization)?.text;
        }

        /// <summary>
        /// Gets the text asset for the current localization.
        /// </summary>
        protected TextAsset TextAsset {
            get {
                return this.GetTextAsset(Localization.Current);
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

            Debug.LogError($"Localization {Localization.Current} not supported");
            return null;
        }

    }
}