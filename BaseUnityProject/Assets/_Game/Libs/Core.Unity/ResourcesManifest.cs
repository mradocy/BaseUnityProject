using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {
    /// <summary>
    /// Describes how a resources manifest file should be structured.
    /// </summary>
    [System.Serializable]
    public class ResourcesManifest {

        [System.Serializable]
        public class Element {
            public string Name;
            public string Path;
        }

        public Element[] TextAssets;
    }
}