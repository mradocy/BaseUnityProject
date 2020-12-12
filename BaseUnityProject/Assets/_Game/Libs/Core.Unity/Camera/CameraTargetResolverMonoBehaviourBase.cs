using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Camera {

    /// <summary>
    /// Base class for <see cref="ICameraTargetResolver"/> implementations that are also MonoBehaviours.  These can be serialized and referenced in the editor.
    /// </summary>
    public abstract class CameraTargetResolverMonoBehaviourBase : MonoBehaviour, ICameraTargetResolver {

        public abstract Vector2 CameraTarget { get; }

        protected virtual void OnDerivedDestroy() { }

        protected void OnDestroy() {
            this.OnDerivedDestroy();

            if (CameraControl.Main != null) {
                if (ReferenceEquals(CameraControl.Main.TargetResolver, this)) {
                    Debug.LogWarning($"Removing this {this.GetType()} from the camera control target resolver because it's being destroyed.");
                    CameraControl.Main.SetTargetResolver(null, 0);
                }

                CameraControl.Main.RemovePrevTargetResolver(this);
            }
        }
    }
}