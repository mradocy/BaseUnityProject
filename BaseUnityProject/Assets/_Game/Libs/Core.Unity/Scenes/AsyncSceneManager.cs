using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core.Unity.Scenes {

    /// <summary>
    /// Improved version of <see cref="SceneManager"/>.
    /// </summary>
    public static class AsyncSceneManager {

        #region Constants

        private static string _sceneNotInBuildError = "Scene with name {0} is not included in the build";

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the full path for the scene with the given name.
        /// <para/>
        /// Returns null if the scene does not exist in the build settings.
        /// </summary>
        /// <param name="sceneName">Name of the scene.</param>
        /// <returns>Scene's full path.</returns>
        public static string GetFullScenePath(string sceneName) {
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
            if (buildIndex == -1)
                return null;
            return SceneUtility.GetScenePathByBuildIndex(buildIndex);
        }

        /// <summary>
        /// Gets if the scene with the given name exists in the build.
        /// </summary>
        /// <param name="sceneName">Name of the scene.</param>
        /// <returns>Is scene in build.</returns>
        public static bool IsSceneInBuild(string sceneName) {
            return SceneUtility.GetBuildIndexByScenePath(sceneName) != -1;
        }

        /// <summary>
        /// Gets the current state of the scene with the given name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        public static SceneLoadState GetSceneLoadState(string sceneName) {
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
            if (buildIndex == -1)
                throw new System.ArgumentException(string.Format(_sceneNotInBuildError, sceneName));

            return GetSceneLoadState(buildIndex);
        }

        /// <summary>
        /// Gets the current state of the scene with the given build index.
        /// </summary>
        /// <param name="buildIndex">The build index of the scene.</param>
        public static SceneLoadState GetSceneLoadState(int buildIndex) {
            if (_sceneStates.TryGetValue(buildIndex, out SceneLoadState loadState)) {
                return loadState;
            }

            // add scene to _sceneStates since not added already
            Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            SceneLoadState sceneLoadState = scene.IsValid() ? SceneLoadState.Loaded : SceneLoadState.NotLoaded;
            SetSceneLoadState(buildIndex, sceneLoadState);
            return sceneLoadState;
        }

        /// <summary>
        /// Gets if the list of scenes with the given names are all loaded.
        /// </summary>
        public static bool AreScenesLoaded(IEnumerable<string> sceneNames) {
            if (sceneNames == null)
                return true;

            foreach (string sceneName in sceneNames) {
                if (GetSceneLoadState(sceneName) != SceneLoadState.Loaded)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets if the list of scenes with the given build indices are all loaded.
        /// </summary>
        public static bool AreScenesLoaded(IEnumerable<int> sceneBuildIndices) {
            if (sceneBuildIndices == null)
                return true;

            foreach (int buildIndex in sceneBuildIndices) {
                if (GetSceneLoadState(buildIndex) != SceneLoadState.Loaded)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the build indices of all the scenes that are currently loaded.
        /// </summary>
        /// <param name="loadedSceneBuildIndices">Array to set loaded scenes into</param>
        /// <returns>The number of loaded scenes.</returns>
        public static int GetLoadedScenes(int[] loadedSceneBuildIndices) {
            int i = 0;
            for (;i < _loadedScenes.Count; i++) {
                if (i >= loadedSceneBuildIndices.Length) {
                    Debug.LogError($"Given array cannot contain all {_loadedScenes.Count} loaded scenes");
                    return i - 1;
                }

                loadedSceneBuildIndices[i] = _loadedScenes[i];
            }

            return i;
        }

        /// <summary>
        /// Loads the scene with the given name asyncronously.
        /// The scene is not loaded if it is already loading, or unloading.
        /// It CAN be loaded again if the scene was already loaded.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <param name="loadSceneMode">Load scene mode.  If <see cref="LoadSceneMode.Single"/>, then all scenes will be unloaded before loading.</param>
        /// <param name="callbackAction">Optional callback invoked when the scene is loaded.  Can be null.</param>
        public static void LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, UnityAction<int> callbackAction) {
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
            if (buildIndex == -1) {
                throw new System.ArgumentException(string.Format(_sceneNotInBuildError, sceneName));
            }

            LoadSceneAsync(buildIndex, loadSceneMode, callbackAction);
        }

        /// <summary>
        /// Loads the scene with the given build index asyncronously.
        /// The scene is not loaded if it is already loading, or unloading.
        /// It CAN be loaded again if the scene was already loaded.
        /// </summary>
        /// <param name="buildIndex">The build index of the scene.</param>
        /// <param name="loadSceneMode">Load scene mode.  If <see cref="LoadSceneMode.Single"/>, then all scenes will be unloaded before loading.</param>
        /// <param name="callbackAction">Optional callback invoked when the scene is loaded.  Can be null.</param>
        public static void LoadSceneAsync(int buildIndex, LoadSceneMode loadSceneMode, UnityAction<int> callbackAction) {
            string sceneName = SceneUtility.GetScenePathByBuildIndex(buildIndex);

            // don't load scene if it's already loading or unloading
            SceneLoadState currentLoadState = GetSceneLoadState(sceneName);
            if (currentLoadState == SceneLoadState.Loading) {
                if (callbackAction == null) {
                    Debug.LogWarning($"Cannot start loading scene {sceneName} because it is already loading.");
                } else {
                    Debug.LogWarning($"Cannot start loading scene {sceneName} because it is already loading, but the given callback will be invoked");
                    AddSceneLoadedCallback(buildIndex, callbackAction);
                }
                return;
            }
            if (currentLoadState == SceneLoadState.Unloading) {
                if (callbackAction == null) {
                    Debug.LogWarning($"Cannot start loading scene {sceneName} because it is currently unloading.");
                } else {
                    Debug.LogWarning($"Cannot start loading scene {sceneName} because it is currently unloading.  The given callback will be ignored");
                }
                return;
            }

            // ensure best background loading
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            // start scene load
            SetSceneLoadState(buildIndex, SceneLoadState.Loading);
            if (callbackAction != null) {
                AddSceneLoadedCallback(buildIndex, callbackAction);
            }
            Debug.Log($"Start scene load: {sceneName}");
            SceneManager.LoadSceneAsync(buildIndex, loadSceneMode);
        }

        /// <summary>
        /// Unloads the scene with the given name asyncronously.
        /// Does nothing if it's the only scene currently loaded (Unity's rule).
        /// The scene is not unloaded if it is already not loaded or unloading.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        public static void UnloadSceneAsync(string sceneName) {
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
            if (buildIndex == -1) {
                throw new System.ArgumentException(string.Format(_sceneNotInBuildError, sceneName));
            }

            UnloadSceneAsync(buildIndex);
        }

        /// <summary>
        /// Unloads the scene with the given build index asyncronously.
        /// Does nothing if it's the only scene currently loaded (Unity's rule).
        /// The scene is not unloaded if it is already not loaded or unloading.
        /// </summary>
        /// <param name="buildIndex">The build index of the scene.</param>
        public static void UnloadSceneAsync(int buildIndex) {
            string sceneName = SceneUtility.GetScenePathByBuildIndex(buildIndex);

            // don't unload scene if it's already not loaded or unloading
            SceneLoadState currentLoadState = GetSceneLoadState(buildIndex);
            if (currentLoadState == SceneLoadState.NotLoaded || currentLoadState == SceneLoadState.Unloading)
                return;

            // don't unload scene if it's the only scene loaded (Unity's rule)
            if (SceneManager.sceneCount <= 1)
                return;

            // start scene unload
            SetSceneLoadState(buildIndex, SceneLoadState.Unloading);
            Debug.Log($"Start scene unload: {sceneName}");
            SceneManager.UnloadSceneAsync(buildIndex);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes this scene manager.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void AddSceneLoadedCallback(int buildIndex, UnityAction<int> callbackAction) {
            if (!_sceneLoadedCallbacks.TryGetValue(buildIndex, out List<UnityAction<int>> callbacks)) {
                callbacks = new List<UnityAction<int>>();
                _sceneLoadedCallbacks.Add(buildIndex, callbacks);
            }
            callbacks.Add(callbackAction);
        }

        /// <summary>
        /// Called when a scene finishes loading.
        /// </summary>
        /// <param name="scene">The scene loaded.</param>
        /// <param name="loadSceneMode">The load scene mode of the scene loaded.</param>
        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            if (!scene.IsValid()) {
                throw new System.ArgumentException($"Loaded invalid scene {scene.path}");
            }

            Debug.Log($"Scene loaded: {GetFullScenePath(scene.path)}");

            SetSceneLoadState(scene.buildIndex, SceneLoadState.Loaded);

            // invoke callbacks
            if (_sceneLoadedCallbacks.TryGetValue(scene.buildIndex, out List<UnityAction<int>> callbacks)) {
                foreach (UnityAction<int> callback in callbacks) {
                    callback.Invoke(scene.buildIndex);
                }
                callbacks.Clear();
            }
        }

        /// <summary>
        /// Called when a scene finishes unloading.
        /// </summary>
        /// <param name="scene">The unloaded scene.</param>
        private static void OnSceneUnloaded(Scene scene) {
            Debug.Log($"Scene unloaded: {GetFullScenePath(scene.path)}");

            SetSceneLoadState(scene.buildIndex, SceneLoadState.NotLoaded);
        }

        /// <summary>
        /// Sets the load state in <see cref="_sceneStates"/> for the given scene.
        /// </summary>
        /// <param name="buildIndex">Build index of the scene.</param>
        /// <param name="loadState">Load state</param>
        private static void SetSceneLoadState(int buildIndex, SceneLoadState loadState) {
            // update _loadedScenes
            SceneLoadState prevState;
            if (_sceneStates.TryGetValue(buildIndex, out prevState)) {
                if (prevState == loadState) {
                    return;
                }

                if (prevState == SceneLoadState.Loaded) {
                    _loadedScenes.Remove(buildIndex);
                }
            }

            if (loadState == SceneLoadState.Loaded) {
                _loadedScenes.Add(buildIndex);
            }

            // update _sceneStates
            _sceneStates[buildIndex] = loadState;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Maps the build indices of scenes to their load state
        /// </summary>
        private static Dictionary<int, SceneLoadState> _sceneStates = new Dictionary<int, SceneLoadState>();

        /// <summary>
        /// List of the build indices of all the scenes that are currently loaded.
        /// </summary>
        private static List<int> _loadedScenes = new List<int>();

        private static Dictionary<int, List<UnityAction<int>>> _sceneLoadedCallbacks = new Dictionary<int, List<UnityAction<int>>>();

        #endregion
    }
}