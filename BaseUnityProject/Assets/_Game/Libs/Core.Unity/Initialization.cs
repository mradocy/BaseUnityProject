using Core.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity {

    // TODO: Get rid of LocalizationProperty, specific properties should be in their own classes.

    /// <summary>
    /// Handles game initialization.
    /// </summary>
    public static class Initialization {

        /// <summary>
        /// Registers event function to be called when the game initializes.  Cleanup is done automatically.
        /// If the game has already initialized, the function is called immediately.
        /// </summary>
        /// <param name="callback">Function to call on initialize.</param>
        public static void CallOnInitialize(UnityAction callback) {
            if (callback == null) {
                throw new System.ArgumentNullException();
            }
            if (_isInitialized) {
                callback();
                return;
            }

            _callbacks.Add(callback);
        }

        /// <summary>
        /// The .ini file containing the settings.
        /// </summary>
        public static InitializationFile Settings { get { return _settings; } }

        /// <summary>
        /// Called by Unity before any scene is loaded.
        /// https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute-ctor.html
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {

            if (_isInitialized)
                return;

            Initialize();
        }

        private const string LocalizationProperty = "localization";

        /// <summary>
        /// Initializes the initialization settings.
        /// </summary>
        private static void Initialize() {
            _isInitialized = true;

            // load ini file
            _settings = new InitializationFile();
            _settings.Load();

            // call callback functions
            foreach (UnityAction callback in _callbacks) {
                callback();
            }
            _callbacks.Clear();
        }

        /// <summary>
        /// Saves settings (such as music volume, localization) to an .ini file.
        /// </summary>
        public static void SaveInitializationSettings() {
            // pull properties into .ini settings
            _settings.SetString(LocalizationProperty, Localization.CodeToString(Localization.Current));

            // save to .ini file
            _settings.Save();
        }

        #region Private Fields

        private static bool _isInitialized = false;
        private static InitializationFile _settings = null;
        private static List<UnityAction> _callbacks = new List<UnityAction>();

        #endregion
    }
}