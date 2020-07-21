using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Camera {

    /// <summary>
    /// The simplest implementatin of <see cref="ICameraTargetResolver"/>.
    /// </summary>
    public class SimpleCameraTargetResolver : ICameraTargetResolver {

        public SimpleCameraTargetResolver() { }

        public SimpleCameraTargetResolver(Vector2 target) {
            this.CameraTarget = target;
        }

        /// <inheritdoc />
        public Vector2 CameraTarget { get; set; }

    }
}