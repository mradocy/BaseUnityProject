using Core.Unity.Assets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Core.Unity.UI {
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class BindTextMeshUGUI : MonoBehaviour {

        [SerializeField]
        [Tooltip("Localized text asset to bind to the content of the TextMeshProUGUI component.")]
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
            if (!Application.isPlaying || this._bindWhenPlaying) {
                this.UpdateText();
            }
        }

        /// <summary>
        /// Updates the text in the text component.
        /// </summary>
        private void UpdateText() {
            TextMeshProUGUI textComponent = this.GetComponent<TextMeshProUGUI>();
            if (textComponent == null)
                return;

            textComponent.text = BindText.GetComponentText(this._text, this._forceLocalization, this._propertyKey);
        }

    }
}