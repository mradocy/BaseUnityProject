using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.TextMeshPro {

    /// <summary>
    /// Describes changes made to the colors of a character of a text mesh pro text.
    /// </summary>
    public class ColorChanges {
        public float Alpha = 1;

        public void Reset() {
            Alpha = 1;
        }
    }
}