using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Camera {

    /// <summary>
    /// Base class for <see cref="ICameraTargetResolver"/> implementations that are also MonoBehaviours.  These can be serialized and referenced in the editor.
    /// </summary>
    public abstract class CameraTargetResolverMonoBehaviourBase : MonoBehaviour, ICameraTargetResolver {

        /// <inheritdoc />
        public abstract Vector2 CameraTarget { get; }
    }
}