using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {
    public static class Localization {

        #region Initialization

        /// <summary>
        /// Used when reading the .ini file.
        /// </summary>
        private const string LocalizationProperty = "localization";

        /// <summary>
        /// Ensures <see cref="Initialize"/>() is called when .ini file is loaded. 
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            Initialization.CallOnInitialize(Initialize);
        }

        /// <summary>
        /// Initializes <see cref="Localization"/>.
        /// </summary>
        private static void Initialize() {
            if (_isInitialized)
                return;

            string localizationStr = Initialization.Settings.GetString(LocalizationProperty, CodeToString(LocalizationCode.Default));
            Current = StringToCode(localizationStr);
        }

        /// <summary>
        /// If <see cref="Initialize"/> has been called already.
        /// </summary>
        private static bool _isInitialized = false;
        #endregion

        /// <summary>
        /// Gets or sets the currect localization.
        /// </summary>
        public static LocalizationCode Current { get; set; } = LocalizationCode.Default;

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

    }
}