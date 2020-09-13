using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Rendering {

    /// <summary>
    /// Extensions for <see cref="Renderer"/>.
    /// </summary>
    public static class RendererExtensions {

        /// <summary>
        /// Ensures that the renderer's main material has a shader with the given name.
        /// </summary>
        /// <param name="renderer">This renderer.</param>
        /// <param name="shaderName">Name of the shader.</param>
        public static Shader EnsureShader(this Renderer renderer, string shaderName) {
            if (renderer == null || renderer.material == null || renderer.material.shader == null)
                return null;
            Shader shader = renderer.material.shader;
            if (string.Compare(shader.name, shaderName, true) != 0) {
                throw new System.Exception($"Renderer's material does not contain shader with the name {shaderName}");
            }
            return shader;
        }

    }
}