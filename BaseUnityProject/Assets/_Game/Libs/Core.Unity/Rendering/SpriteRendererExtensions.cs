using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Rendering {

    /// <summary>
    /// Extensions for <see cref="SpriteRenderer"/>.
    /// </summary>
    public static class SpriteRendererExtensions {

        /// <summary>
        /// Updates the color of this sprite renderer to the given alpha value.
        /// </summary>
        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha) {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }

    }
}