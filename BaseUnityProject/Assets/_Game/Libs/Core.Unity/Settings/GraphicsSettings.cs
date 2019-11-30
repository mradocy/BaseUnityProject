using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Settings {

    /// <summary>
    /// Static class for accessing and setting graphics settings.
    /// </summary>
    public static class GraphicsSettings {

        #region Initialization Keys

        /// <summary>
        /// Key for accessing the fullscreen mode from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _fullScreenModeInitializationKey = "fullscreen_mode";

        /// <summary>
        /// Key for accessing the vSync count from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _vSyncCountInitializationKey = "v_sync_count";

        /// <summary>
        /// Key for accessing the max queued frames from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _maxQueuedFramesInitializationKey = "max_queued_frames";

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

            int fullscreenModeInt = Initialization.Settings.GetInt(_fullScreenModeInitializationKey, (int)FullScreenMode.Windowed);
            if (System.Enum.IsDefined(typeof(FullScreenMode), fullscreenModeInt)) {
                Screen.fullScreenMode = (FullScreenMode)fullscreenModeInt;
            } else {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }

            QualitySettings.vSyncCount = Initialization.Settings.GetInt(_vSyncCountInitializationKey, 0);
            QualitySettings.maxQueuedFrames = Initialization.Settings.GetInt(_maxQueuedFramesInitializationKey, 0);
            
            _isInitialized = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current fullscreen mode.
        /// </summary>
        public static FullScreenMode FullScreenMode {
            get { return Screen.fullScreenMode; }
            set {
                if (Screen.fullScreenMode == value)
                    return;

                Screen.fullScreenMode = value;
                Initialization.Settings.SetInt(_fullScreenModeInitializationKey, (int)value);
                Initialization.Settings.Save();
            }
        }

        /// <summary>
        /// Gets or sets the number of VSyncs that should pass between each frame.  Use 'Don't Sync' (0) to not wait for VSync.  Value must be 0, 1, 2, 3, or 4.
        /// </summary>
        public static int VSyncCount {
            get { return QualitySettings.vSyncCount; }
            set {
                if (QualitySettings.vSyncCount == value)
                    return;

                QualitySettings.vSyncCount = value;
                Initialization.Settings.SetInt(_vSyncCountInitializationKey, QualitySettings.vSyncCount);
                Initialization.Settings.Save();
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of frames queued up by graphics driver.
        /// </summary>
        public static int MaxQueuedFrames {
            get { return QualitySettings.maxQueuedFrames; }
            set {
                if (QualitySettings.maxQueuedFrames == value)
                    return;

                QualitySettings.maxQueuedFrames = value;
                Initialization.Settings.SetInt(_maxQueuedFramesInitializationKey, QualitySettings.maxQueuedFrames);
                Initialization.Settings.Save();
            }
        }

        #endregion

        #region Private Fields

        private static bool _isInitialized = false;

        #endregion
    }
}