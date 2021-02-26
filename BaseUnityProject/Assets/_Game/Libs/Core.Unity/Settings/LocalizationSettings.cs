﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Core.Unity.Settings {

    /// <summary>
    /// Static class for accessing and setting localization settings.
    /// </summary>
    public static class LocalizationSettings {

        #region Initialization Keys

        /// <summary>
        /// Key for accessing the current localization from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _localizationInitializationKey = "localization";

        #endregion

        #region Initialization

        /// <summary>
        /// Called by Unity before any scene is loaded.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            Initialization.CallWhenInitialized(Initialize);
        }

        private static void Initialize() {
            if (_isInitialized)
                return;

            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            string localizationStr = Initialization.Settings.GetString(_localizationInitializationKey, CodeToString(CultureInfoToCode(cultureInfo)));
            _localization = StringToCode(localizationStr);

            _isInitialized = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currect localization.
        /// </summary>
        public static LocalizationCode Localization {
            get { return _localization; }
            set {
                if (Localization == value)
                    return;

                _localization = value;
                Initialization.Settings.SetString(_localizationInitializationKey, CodeToString(_localization));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a <see cref="LocalizationCode"/> into its string representation.
        /// </summary>
        /// <param name="code">Code to convert.</param>
        /// <returns>string</returns>
        public static string CodeToString(LocalizationCode code) {

            switch (code) {
            case LocalizationCode.en_US:
                return "en-US";
            case LocalizationCode.ja:
                return "ja";
            }

            Debug.LogError($"Localization code {code} not implemented yet.");
            return null;
        }

        /// <summary>
        /// Converts the string representation of a <see cref="LocalizationCode"/> into a code.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>code</returns>
        public static LocalizationCode StringToCode(string str) {
            if (string.IsNullOrEmpty(str))
                return LocalizationCode.None;

            str = str.ToLower().Replace('_', '-');
            if (str == "en-us") {
                return LocalizationCode.en_US;
            } else if (str == "ja") {
                return LocalizationCode.ja;
            } else if (str == "default") {
                return LocalizationCode.Default;
            }

            Debug.LogError($"String {str} is not recognized as a localization code yet.");
            return LocalizationCode.None;
        }

        /// <summary>
        /// Gets the <see cref="LocalizationCode"/> for the given culture.
        /// </summary>
        /// <remarks>Localization codes: https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c </remarks>
        public static LocalizationCode CultureInfoToCode(CultureInfo cultureInfo) {
            if (string.Equals(cultureInfo.TwoLetterISOLanguageName, "en", System.StringComparison.OrdinalIgnoreCase)) {
                return LocalizationCode.en_US;
            }
            if (string.Equals(cultureInfo.TwoLetterISOLanguageName, "ja", System.StringComparison.OrdinalIgnoreCase)) {
                return LocalizationCode.ja;
            }

            Debug.LogWarning($"No localization code found for culture {cultureInfo.Name}.  Using default.");
            return LocalizationCode.Default;
        }

        #endregion

        #region Private Fields

        private static bool _isInitialized = false;

        private static LocalizationCode _localization = LocalizationCode.Default;

        #endregion
    }
}