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
        /// Key for accessing the screen width from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _screenWidthInitializationKey = "screen_width";

        /// <summary>
        /// Key for accessing the screen height from <see cref="Initialization.Settings"/>.
        /// </summary>
        private const string _screenHeightInitializationKey = "screen_height";

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
            FullScreenMode fullscreenMode = FullScreenMode.Windowed;
            if (System.Enum.IsDefined(typeof(FullScreenMode), fullscreenModeInt)) {
                fullscreenMode = (FullScreenMode)fullscreenModeInt;
            }
            int screenWidth = Initialization.Settings.GetInt(_screenWidthInitializationKey, Screen.width);
            int screenHeight = Initialization.Settings.GetInt(_screenHeightInitializationKey, Screen.height);
            Screen.SetResolution(screenWidth, screenHeight, fullscreenMode);

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
            }
        }

        /// <summary>
        /// Gets or sets the screen size (windowed) or screen resolution (fullscreen).
        /// </summary>
        public static Vector2Int ScreenSize {
            get {
                return new Vector2Int(Screen.width, Screen.height);
            }
            set {
                if (value.x == Screen.width && value.y == Screen.height)
                    return;

                Screen.SetResolution(value.x, value.y, FullScreenMode);
                Initialization.Settings.SetInt(_screenWidthInitializationKey, value.x);
                Initialization.Settings.SetInt(_screenHeightInitializationKey, value.y);
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
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all screen sizes (resolutions) supported by the game and monitor.
        /// </summary>
        /// <returns>Screen sizes.</returns>
        public static Vector2Int[] GetScreenSizes() {
            List<Vector2Int> screenSizes = new List<Vector2Int>();

            Resolution[] resolutions = Screen.resolutions;
            for (int i=0; i < resolutions.Length; i++) {
                Vector2Int screenSize = new Vector2Int(resolutions[i].width, resolutions[i].height);
                if (screenSizes.Contains(screenSize))
                    continue;

                screenSizes.Add(screenSize);
            }

            return screenSizes.ToArray();
        }

        /// <summary>
        /// Gets all resolutions supported by the game and monitor.
        /// </summary>
        /// <param name="removeDuplicateDimensions">If resolutions with identical dimensions should be removed.  Only the resolution with the highest refresh rate will be included.</param>
        /// <returns></returns>
        public static Resolution[] GetResolutions(bool removeDuplicateDimensions) {
            Resolution[] resolutions = Screen.resolutions;

            if (removeDuplicateDimensions) {
                List<Resolution> filteredResolutions = new List<Resolution>();
                for (int i=0; i < resolutions.Length; i++) {
                    Resolution resolution = resolutions[i];

                    // get index of filtered resolution with the same dimensions
                    int filteredResolutionIndex = -1;
                    for (int j=0; j < filteredResolutions.Count; j++) {
                        Resolution filteredResolution = filteredResolutions[j];
                        if (filteredResolution.width == resolution.width &&
                            filteredResolution.height == resolution.height) {
                            filteredResolutionIndex = j;
                            break;
                        }
                    }

                    if (filteredResolutionIndex == -1) {
                        // doesn't have res of same dimensions, add
                        filteredResolutions.Add(resolution);
                    } else {
                        // filtered resolution already added, compare to see which gets kept
                        if (filteredResolutions[filteredResolutionIndex].refreshRate < resolution.refreshRate) {
                            // replace filtered res
                            filteredResolutions.RemoveAt(filteredResolutionIndex);
                            filteredResolutions.Add(resolution);
                        }
                    }
                }
                
                return filteredResolutions.ToArray();

            } else {
                return resolutions;
            }
        }

        #endregion

        #region Private Fields

        private static bool _isInitialized = false;

        #endregion
    }
}