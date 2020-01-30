using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {
    /// <summary>
    /// Static class for managing assets found in the Resources.
    /// </summary>
    public static class ResourcesManager {

        #region Getting Loaded Assets

        /// <summary>
        /// Gets the text asset with the given name, as given when loading the asset or defined in the manifest file.
        /// </summary>
        /// <param name="name">Asset name.</param>
        /// <returns>text asset</returns>
        public static TextAsset GetTextAsset(string name) {
            Source<TextAsset> assetSource;
            if (_textAssets.TryGetValue(DicKey(name), out assetSource)) {
                return assetSource.Asset;
            }
            return null;
        }

        #endregion

        #region Loading Assets

        /// <summary>
        /// Loads assets defined in the manifest file pointed to by the given path in Resources.
        /// </summary>
        /// <param name="manifestFile">Path to the manifest file in the Resources directory.</param>
        public static void LoadManifest(string manifestFile) {
            TextAsset manifestAsset = null;
            try {
                manifestAsset = Resources.Load<TextAsset>(PathConvert(manifestFile));
            } catch (System.Exception e) {
                Debug.LogError($"Manifest file \"{PathConvert(manifestFile)}\" could not be loaded.  Error: {e.Message}");
                return;
            }
            if (manifestAsset == null) {
                Debug.LogError($"There was a problem loading manifest file \"{PathConvert(manifestFile)}\"");
                return;
            }

            LoadManifest(manifestAsset);
        }

        /// <summary>
        /// Loads assets from a TextAsset containing a .json to be parsed into a <see cref="ResourcesManifest"/> object.
        /// </summary>
        /// <param name="manifestAsset">Manifest TextAsset.</param>
        public static void LoadManifest(TextAsset manifestAsset) {
            if (manifestAsset == null) {
                throw new System.ArgumentNullException(nameof(manifestAsset));
            }

            ResourcesManifest manifest = JsonUtility.FromJson<ResourcesManifest>(manifestAsset.text);
            if (manifest == null) {
                throw new System.ArgumentException($"Could not parse the TextAsset \"{manifestAsset}\"");
            }

            LoadManifest(manifest);
        }

        /// <summary>
        /// Loads assets from a <see cref="ResourcesManifest"/> object.
        /// </summary>
        /// <param name="manifest">Manifest object.</param>
        public static void LoadManifest(ResourcesManifest manifest) {

            LocalizationCode localizationCode = LocalizationSettings.Localization;

            // parse text assets
            if (manifest.TextAssets != null) {
                foreach (ResourcesManifest.LocalizedElement manifestElement in manifest.TextAssets) {
                    LoadTextAsset(manifestElement.Name, manifestElement.Paths.GetPath(localizationCode));
                }
            }

            // TODO: Other types of assets

        }

        /// <summary>
        /// Loads a text asset from Resources.
        /// </summary>
        /// <param name="name">Name to attribute to the asset.  Name must not match the name of a text asset already loaded.</param>
        /// <param name="path">Path to the asset, relative to Resources.</param>
        public static void LoadTextAsset(string name, string path) {
            // ensure name is new
            if (string.IsNullOrEmpty(name)) {
                Debug.LogError($"Name must be defined when loading text asset.");
                return;
            }
            if (GetTextAsset(name) != null) {
                Debug.LogError($"Text asset with name \"{name}\" already added.");
                return;
            }

            // load asset
            if (string.IsNullOrEmpty(path)) {
                Debug.LogError($"Path for text asset \"{name}\" must be defined.");
                return;
            }
            Source<TextAsset> source = new Source<TextAsset>() {
                Path = path
            };
            source.Reload();
            if (source.Asset == null) {
                return;
            }

            // add to dictionary
            _textAssets[DicKey(name)] = source;
        }

        /// <summary>
        /// Reloads all the currently loaded assets.  Useful if the localization changes.
        /// </summary>
        public static void ReloadAllAssets() {

            foreach (Source<TextAsset> textSource in _textAssets.Values) {
                textSource.Reload();
            }

        }

        #endregion

        #region Private

        /// <summary>
        /// Gets the key for the assets dictionary from the given string.
        /// </summary>
        /// <param name="str">str</param>
        /// <returns>key</returns>
        private static string DicKey(string str) {
            return str.ToLower();
        }

        /// <summary>
        /// Converts the given raw path into a path that would be used by Resources.Load().
        /// </summary>
        /// <param name="path">raw path</param>
        /// <returns>path</returns>
        private static string PathConvert(string path) {
            return System.IO.Path.ChangeExtension(path, null).Replace("$(Localization)", LocalizationSettings.CodeToString(LocalizationSettings.Localization));
        }

        private class Source<T> where T : Object {
            /// <summary>
            /// Unformatted Resources path to the asset.
            /// </summary>
            public string Path = null;
            /// <summary>
            /// The asset loaded.
            /// </summary>
            public T Asset = null;

            /// <summary>
            /// Reloads the asset pointed to by the path.
            /// </summary>
            public void Reload() {
                this.Asset = null;
                T asset;
                try {
                    asset = Resources.Load<T>(PathConvert(this.Path));
                } catch (System.Exception e) {
                    Debug.LogError($"{typeof(T).Name} asset with path \"{PathConvert(this.Path)}\" could not be loaded.  Error: {e.Message}");
                    return;
                }
                if (asset == null) {
                    Debug.LogError($"There was a problem loading {typeof(T).Name} asset \"{PathConvert(this.Path)}\"");
                    return;
                }

                this.Asset = asset;
            }
        }

        private static Dictionary<string, Source<TextAsset>> _textAssets = new Dictionary<string, Source<TextAsset>>();

        #endregion

    }
}