using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Settings {

    /// <summary>
    /// Static class for accessing and setting graphics settings.
    /// </summary>
    public static class GraphicsSettings {

        #region Constants

        /// <summary>
        /// Key for accessing the vSync count from <see cref="Initialization.Settings"/>.
        /// </summary>
        public const string VSyncCountInitializationKey = "v_sync_count";

        /// <summary>
        /// Key for accessing the max queued frames from <see cref="Initialization.Settings"/>.
        /// </summary>
        public const string MaxQueuedFramesInitializationKey = "max_queued_frames";

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

            QualitySettings.vSyncCount = Initialization.Settings.GetInt(VSyncCountInitializationKey, 0);
            QualitySettings.maxQueuedFrames = Initialization.Settings.GetInt(MaxQueuedFramesInitializationKey, 0);
            
            _isInitialized = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of VSyncs that should pass between each frame.  Use 'Don't Sync' (0) to not wait for VSync.  Value must be 0, 1, 2, 3, or 4.
        /// This value will be saved to the .ini file.
        /// </summary>
        public static int VSyncCount {
            get { return QualitySettings.vSyncCount; }
            set {
                if (QualitySettings.vSyncCount == value)
                    return;

                QualitySettings.vSyncCount = value;
                Initialization.Settings.SetInt(VSyncCountInitializationKey, QualitySettings.vSyncCount);
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
                Initialization.Settings.SetInt(MaxQueuedFramesInitializationKey, QualitySettings.maxQueuedFrames);
                Initialization.Settings.Save();
            }
        }

        #endregion

        #region Private Fields

        private static bool _isInitialized = false;

        #endregion
    }
}