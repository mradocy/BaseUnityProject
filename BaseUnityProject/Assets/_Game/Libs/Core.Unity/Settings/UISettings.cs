using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Settings {

    /// <summary>
    /// Static class for accessing and setting ui settings.
    /// </summary>
    public static class UISettings {

        #region Initialization Keys

        /// <summary>
        /// Key for accessing the ui scale from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _uIScaleInitializationKey = "ui_scale";

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

            _uiScale = Initialization.Settings.GetFloat(_uIScaleInitializationKey, 1);

            _isInitialized = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the global UI scale.
        /// </summary>
        public static float UIScale {
            get { return _uiScale; }
            set {
                if (_uiScale == value)
                    return;

                _uiScale = value;
                Initialization.Settings.SetFloat(_uIScaleInitializationKey, _uiScale);
            }
        }

        #endregion

        #region Private Fields

        private static bool _isInitialized = false;

        private static float _uiScale = 1;

        #endregion
    }
}