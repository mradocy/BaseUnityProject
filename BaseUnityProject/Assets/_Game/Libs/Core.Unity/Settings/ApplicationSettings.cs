using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Settings {

    /// <summary>
    /// Static class for accessing and setting application settings.
    /// </summary>
    public static class ApplicationSettings {

        #region Initialization Keys

        /// <summary>
        /// Key for accessing the run in background option from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _runInBackgroundInitializationKey = "run_in_background";

        #endregion

        #region Initialization

        /// <summary>
        /// Called by Unity before any scene is loaded.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            Initialization.CallOnInitialize(Initialize);
        }

        private static void Initialize() {
            if (_isInitialized)
                return;

            Application.runInBackground = Initialization.Settings.GetBool(_runInBackgroundInitializationKey, false);

            _isInitialized = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if the application runs in the background
        /// </summary>
        public static bool RunInBackground {
            get { return Application.runInBackground; }
            set {
                if (Application.runInBackground == value)
                    return;

                Application.runInBackground = value;
                Initialization.Settings.SetBool(_runInBackgroundInitializationKey, value);
            }
        }

        #endregion

        #region Private Fields

        private static bool _isInitialized = false;

        #endregion
    }
}