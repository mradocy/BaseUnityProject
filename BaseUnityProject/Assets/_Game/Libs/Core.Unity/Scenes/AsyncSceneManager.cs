using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        /// <returns>SceneLoadState</returns>
        public static SceneLoadState GetSceneLoadState(string sceneName) {
            if (!IsSceneInBuild(sceneName))
                throw new System.ArgumentException(string.Format(_sceneNotInBuildError, sceneName));

            string key = GetFullScenePath(sceneName);
            SceneLoadState loadState;
            if (_sceneStates.TryGetValue(key, out loadState)) {
                return loadState;
            }

            // add scene to _sceneStates since not added already
            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneLoadState sceneLoadState = scene.IsValid() ? SceneLoadState.Loaded : SceneLoadState.NotLoaded;
            _sceneStates[key] = sceneLoadState;
            return sceneLoadState;
        }

        /// <summary>
        /// Gets if the list of scenes with the given names are all loaded.
        /// </summary>
        /// <param name="sceneNames"></param>
        public static bool AreScenesLoaded(string[] sceneNames) {
            if (sceneNames == null || sceneNames.Length == 0)
                return true;

            foreach (string sceneName in sceneNames) {
                if (GetSceneLoadState(sceneName) != SceneLoadState.Loaded)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Loads the scene with the given name asyncronously.
        /// The scene is not loaded if it is already loading, loaded, or unloading.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        public static void LoadSceneAsync(string sceneName) {
            if (!IsSceneInBuild(sceneName))
                throw new System.ArgumentException(string.Format(_sceneNotInBuildError, sceneName));

            // don't load scene if it's already loading or loaded
            SceneLoadState currentLoadState = GetSceneLoadState(sceneName);
            if (currentLoadState == SceneLoadState.Loading || currentLoadState == SceneLoadState.Loaded || currentLoadState == SceneLoadState.Unloading)
                return;

            // start scene load
            _sceneStates[GetFullScenePath(sceneName)] = SceneLoadState.Loading;
            Debug.Log($"Start scene load: {GetFullScenePath(sceneName)}");
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        #endregion

        #region Private Methods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            Initialize();
        }

        /// <summary>
        /// Initializes this scene manager.
        /// </summary>
        private static void Initialize() {
            if (_isInitialized)
                return;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            _isInitialized = true;
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

            _sceneStates[GetFullScenePath(scene.path)] = SceneLoadState.Loaded;
        }

        /// <summary>
        /// Called when a scene finishes unloading.
        /// </summary>
        /// <param name="scene">The unloaded scene.</param>
        private static void OnSceneUnloaded(Scene scene) {
            Debug.Log($"Scene unloaded: {GetFullScenePath(scene.path)}");

            _sceneStates[GetFullScenePath(scene.path)] = SceneLoadState.NotLoaded;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Gets if <see cref="Initialize"/>() was called.
        /// </summary>
        private static bool _isInitialized = false;

        /// <summary>
        /// Maps the names of scenes to their load state
        /// </summary>
        private static Dictionary<string, SceneLoadState> _sceneStates = new Dictionary<string, SceneLoadState>();

        #endregion
    }
}