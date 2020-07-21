using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Camera {

    public interface ICameraTargetResolver {

        /// <summary>
        /// Gets the position for the camera's center to target.
        /// </summary>
        Vector2 CameraTarget { get; }

    }
}