using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.TextMeshPro {

    /// <summary>
    /// Describes changes made to the vertices of a character of a text mesh pro text.
    /// </summary>
    public class VertexChanges {
        public float OffsetX;
        public float OffsetY;
        public float Rotation;

        public void Reset() {
            OffsetX = 0;
            OffsetY = 0;
            Rotation = 0;
        }
    }
}