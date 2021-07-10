using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unity.UI {

    /// <summary>
    /// Extensions for <see cref="Graphic"/>.
    /// </summary>
    public static class GraphicExtensions {

        /// <summary>
        /// Ensures that the graphic's main material has a shader with the given name.
        /// </summary>
        /// <param name="graphic">This graphic.</param>
        /// <param name="shaderName">Name of the shader.</param>
        public static Shader EnsureShader(this Graphic graphic, string shaderName) {
            Shader shader = graphic?.material?.shader;
            if (string.Compare(shader?.name, shaderName, true) != 0) {
                throw new System.Exception($"Graphic's material does not contain shader with the name {shaderName}");
            }
            return shader;
        }

        /// <summary>
        /// Updates the color of this graphic to the given alpha value.
        /// </summary>
        public static void SetAlpha(this Graphic graphic, float alpha) {
            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }

    }
}