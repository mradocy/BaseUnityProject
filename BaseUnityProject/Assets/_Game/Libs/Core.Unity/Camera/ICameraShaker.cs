using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Camera {

    public interface ICameraShaker {
        /// <summary>
        /// Gets the current shake magnitude.
        /// </summary>
        Vector2 Offset { get; }
        /// <summary>
        /// Gets the current shake rotation magnitude.
        /// </summary>
        float RotationOffset { get; }
    }
}