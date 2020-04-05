using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Rendering {

    /// <summary>
    /// Enum similar to <see cref="UnityEngine.Rendering.ColorWriteMask"/>, only allowing options to write either no or all color channels.
    /// Used in the SpritesMask shader.
    /// </summary>
    public enum ColorWriteMaskToggle {
        None = 0,
        All = UnityEngine.Rendering.ColorWriteMask.All
    }
}