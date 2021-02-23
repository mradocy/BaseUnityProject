using Core.Unity;
using Core.Unity.Settings;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Core.Unity.TextMeshPro {

    [CreateAssetMenu(fileName = "New LocalizedTMProFont", menuName = "Localization/LocalizedTMProFontAsset", order = 52)]
    public class LocalizedTMProFontAsset : ScriptableObject {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("TMP_FontAsset to use for the English - United States locale.")]
        private TMP_FontAsset _enUS;

        [SerializeField]
        [Tooltip("TMP_FontAsset to use for the Japanese locale.")]
        private TMP_FontAsset _ja;

        #endregion

        public TMP_FontAsset FontAsset {
            get => this.GetFontAsset(LocalizationSettings.Localization);
        }

        public TMP_FontAsset GetFontAsset(LocalizationCode localization) {
            switch (localization) {
            case LocalizationCode.en_US:
                return _enUS;
            case LocalizationCode.ja:
                return _ja;
            }

            Debug.LogError($"Localization {localization} not supported");
            return null;
        }
    }
}