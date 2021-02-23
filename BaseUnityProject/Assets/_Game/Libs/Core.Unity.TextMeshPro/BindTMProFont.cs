using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using TMPro;
using Core.Unity.Settings;

namespace Core.Unity.TextMeshPro {

    [RequireComponent(typeof(TMP_Text))]
    [ExecuteAlways]
    public class BindTMProFont : MonoBehaviour {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("Localized font asset to bind to the text component.")]
        private LocalizedTMProFontAsset _localizedFont = null;

        [SerializeField]
        [Tooltip("Forces a specific localization to be used, if a value other than None is specified.  Useful for debugging.")]
        private LocalizationCode _forceLocalization = LocalizationCode.None;

        #endregion

        /// <summary>
        /// Gets the TMPro text component this helper is for.
        /// </summary>
        public TMP_Text TextComponent {
            get {
                if (_textComponent == null) {
                    _textComponent = this.EnsureComponent<TMP_Text>();
                }
                return _textComponent;
            }
        }

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            this.UpdateFont();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {
            if (!Application.isPlaying) {
                this.UpdateFont();
            }
        }

        private void UpdateFont() {
            if (_localizedFont == null)
                return;
            if (this.TextComponent == null)
                return;

            LocalizationCode localization = _forceLocalization;
            if (localization == LocalizationCode.None) {
                localization = LocalizationSettings.Localization;
            }

            TMP_FontAsset fontAsset = _localizedFont.GetFontAsset(localization);
            if (fontAsset == null && localization != LocalizationCode.Default) {
                // revert to default font
                fontAsset = _localizedFont.GetFontAsset(LocalizationCode.Default);
            }
            if (fontAsset == null) {
                return;
            }

            this.TextComponent.font = fontAsset;
        }

        private TMP_Text _textComponent;
    }
}