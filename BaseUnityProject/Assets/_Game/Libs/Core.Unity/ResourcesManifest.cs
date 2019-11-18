using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {
    /// <summary>
    /// Describes how a resources manifest file should be structured.
    /// </summary>
    [System.Serializable]
    public class ResourcesManifest {

        #region Fields

        public LocalizedElement[] TextAssets;

        #endregion

        #region Classes

        [System.Serializable]
        public class LocalizedElement {
            public string Name;
            public LocalizedPaths Paths;
        }

        [System.Serializable]
        public class LocalizedPaths {
            public string en_US;
            public string ja;

            public string GetPath(LocalizationCode localizationCode) {
                switch (localizationCode) {
                case LocalizationCode.en_US:
                    return this.en_US;
                case LocalizationCode.ja:
                    return this.ja;
                }

                throw new System.Exception($"Localization code {localizationCode} not implemented yet.");
            }
        }

        #endregion
    }
}