using Core.Unity.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity.Camera {

    [RequireComponent(typeof(UnityEngine.Camera))]
    [DefaultExecutionOrder(999)] // this script should execute late.  NOTE: This attribute works, despite not showing up in the Script Execution Order window
    public class CameraControl : MonoBehaviour {

        #region Static Events

        /// <summary>
        /// Event invoked when the camera's position is updated.
        /// </summary>
        public static event UnityAction CameraUpdate;

        #endregion

        #region Properties

        /// <summary>
        /// Reference to the <see cref="CameraControl"/> of the main camera.
        /// </summary>
        public static CameraControl Main { get; private set; }

        /// <summary>
        /// Reference to the Camera component.
        /// </summary>
        public UnityEngine.Camera Camera { get; private set; }

        /// <summary>
        /// Gets the <see cref="ICameraTargetResolver"/> that resolves the camera's target position.
        /// </summary>
        public ICameraTargetResolver TargetResolver { get; private set; }

        /// <summary>
        /// Gets camera position, ignoring shake offsets.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Gets camera rotation, ignoring shake offsets.
        /// </summary>
        public float Rotation { get; private set; }

        /// <summary>
        /// Gets orthographic size of the camera.  Is half the <see cref="Height"/>.
        /// </summary>
        public float Size {
            get { return this.Camera.orthographicSize; }
            set {
                this.Camera.orthographicSize = Mathf.Max(.5f, value);
            }
        }

        /// <summary>
        /// Gets the width of the camera screen.  Is the <see cref="Height"/> multiplied by the screen ratio.
        /// </summary>
        public float Width {
            get { return this.Height * Screen.width / Screen.height; }
        }

        /// <summary>
        /// Gets the height of the camera screen.  Is twice the <see cref="Size"/>.
        /// </summary>
        public float Height {
            get { return this.Size * 2; }
        }

        /// <summary>
        /// Right bound of the camera screen, ignoring shake offsets.
        /// </summary>
        public float RightBound {
            get {
                return this.Position.x + this.Size * Screen.width / Screen.height;
            }
        }
        /// <summary>
        /// Top bound of the camera screen, ignoring shake offsets.
        /// </summary>
        public float TopBound {
            get {
                return this.Position.y + this.Size;
            }
        }
        /// <summary>
        /// Left bound of the camera screen, ignoring shake offsets.
        /// </summary>
        public float LeftBound {
            get {
                return this.Position.x - this.Size * Screen.width / Screen.height;
            }
        }
        /// <summary>
        /// Bottom bound of the camera screen, ignoring shake offsets.
        /// </summary>
        public float BottomBound {
            get {
                return this.Position.y - this.Size;
            }
        }

        /// <summary>
        /// Gets the limits of how far the bounds of the camera can go.
        /// </summary>
        public CameraLimits CameraLimits { get; private set; } = CameraLimits.NoLimit;

        /// <summary>
        /// Gets the current offset to position from the camera shaking.
        /// </summary>
        public Vector2 ShakeOffset => _shakeOffset * this.Size;

        #endregion

        #region Methods

        /// <summary>
        /// Gets if the given point can be seen on screen, shake offsets.
        /// </summary>
        /// <param name="position">Point to test.</param>
        /// <param name="border">Optional value that expands the border for the test.</param>
        public bool IsPointInBounds(Vector2 position, float border = 0) {
            return this.LeftBound - border <= position.x && position.x <= this.RightBound + border &&
                this.BottomBound - border <= position.y && position.y <= this.TopBound + border;
        }

        /// <summary>
        /// Sets the camera limits to a copy of the given <paramref name="cameraLimits"/>.
        /// Does nothing if the given camera limits is the same as the current camera limits.
        /// </summary>
        /// <param name="cameraLimits">New camera limits.</param>
        public void SetCameraLimits(CameraLimits cameraLimits) {
            if (this.CameraLimits.Equals(cameraLimits)) {
                return;
            }

            // set limits
            this.CameraLimits = cameraLimits;
        }

        /// <summary>
        /// Sets the <see cref="TargetResolver"/>.  This resolver will be prompted for the camera target position in LateUpdate().
        /// Does nothing if the given target resolver is the same as the current target resolver.
        /// </summary>
        /// <param name="targetResolver">New target resolver.  Nothing happens if this is the same as the current <see cref="TargetResolver"/>.</param>
        /// <param name="transitionDuration">Time to transition into the new target resolver.  Set to 0 for no transition period.</param>
        public void SetTargetResolver(ICameraTargetResolver targetResolver, float transitionDuration) {
            if (this.TargetResolver == targetResolver)
                return;

            if (targetResolver == null || transitionDuration <= 0) {
                _prevTargetResolvers.Clear();
            } else {
                _prevTargetResolvers.Add(new PrevTargetResolverDefinition() {
                    TargetResolver = this.TargetResolver,
                    TransitionStartTimestamp = Time.time,
                    TransitionDuration = transitionDuration,
                });
            }

            this.TargetResolver = targetResolver;
        }

        /// <summary>
        /// Same as <see cref="SetTargetResolver(ICameraTargetResolver, float)"/>, but the <see cref="PrevTargetResolver"/> is unchanged.
        /// Ideally would be used to replace the current resolver with one that has the same target.
        /// </summary>
        /// <param name="targetResolver">New target resolver.  Nothing happens if this is the same as the current <see cref="TargetResolver"/>.</param>
        public void SetTargetResolverWithoutChangingPrevResolver(ICameraTargetResolver targetResolver) {
            if (this.TargetResolver == targetResolver)
                return;

            this.TargetResolver = targetResolver;
        }

        /// <summary>
        /// Removes the given target resolver from the internal list of prev target resolvers.
        /// Useful when deleting a target resolver.
        /// </summary>
        public void RemovePrevTargetResolver(ICameraTargetResolver targetResolver) {
            for (int i=0; i < _prevTargetResolvers.Count; i++) {
                if (_prevTargetResolvers[i].TargetResolver == targetResolver) {
                    _prevTargetResolvers.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Adds the given <see cref="ICameraShaker"/> to the list of camera shakers that collectively determine the <see cref="ShakeOffset"/>.
        /// </summary>
        public void AddShaker(ICameraShaker cameraShaker) {
            if (_cameraShakers.Contains(cameraShaker)) {
                Debug.LogError("CameraControl already contains the given camera shaker");
                return;
            }
            _cameraShakers.Add(cameraShaker);
        }

        /// <summary>
        /// Removes the given <see cref="ICameraShaker"/> from the list of camera shakers.
        /// </summary>
        public void RemoveShaker(ICameraShaker cameraShaker) {
            _cameraShakers.Remove(cameraShaker);
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            // singleton
            if (Main != null) {
                Debug.LogError("Cannot use more than one CameraControl");
                Destroy(this.gameObject);
                return;
            }
            Main = this;

            this.Camera = this.EnsureComponent<UnityEngine.Camera>();
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate() {

            // get camera position
            Vector2 pos = this.Position;
            if (this.TargetResolver != null) {
                // set position from target resolver
                pos = this.TargetResolver.CameraTarget;

                // clear outdated prev target resolvers
                for (int i = _prevTargetResolvers.Count - 1; i >= 0; i--) {
                    PrevTargetResolverDefinition prevTargetResolver = _prevTargetResolvers[i];
                    if (Time.time - prevTargetResolver.TransitionStartTimestamp >= prevTargetResolver.TransitionDuration) {
                        // remove all previous resolvers too
                        _prevTargetResolvers.RemoveRange(0, i + 1);
                        break;
                    }
                }

                if (_prevTargetResolvers.Count > 0) {
                    // get prev position by resolving prev target resolvers in order
                    Vector2 prevPos = _prevTargetResolvers[0].TargetResolver.CameraTarget;
                    for (int i = 0; i < _prevTargetResolvers.Count - 1; i++) {
                        prevPos = Easing.QuadInOut(
                            prevPos,
                            _prevTargetResolvers[i + 1].TargetResolver.CameraTarget,
                            Time.time - _prevTargetResolvers[i].TransitionStartTimestamp,
                            _prevTargetResolvers[i].TransitionDuration);
                    }

                    // set position by combining with the current resolver's position
                    pos = Easing.QuadInOut(
                        prevPos,
                        pos,
                        Time.time - _prevTargetResolvers[_prevTargetResolvers.Count - 1].TransitionStartTimestamp,
                        _prevTargetResolvers[_prevTargetResolvers.Count - 1].TransitionDuration);
                }
            }
            
            // keep position in limits
            Vector2 currentLimitPos = this.CameraLimits.ClampPosition(pos, this.Size * Screen.width / Screen.height, this.Size);

            // update position
            this.Position = pos;

            // update shake offset from shakers
            _shakeOffset.Set(0, 0);
            _shakeRotationOffset = 0;
            foreach (ICameraShaker cameraShaker in _cameraShakers) {
                _shakeOffset += cameraShaker.Offset;
                _shakeRotationOffset += cameraShaker.RotationOffset;
            }

            // update transform
            this.transform.position = new Vector3(
                this.Position.x + this.ShakeOffset.x,
                this.Position.y + this.ShakeOffset.y,
                this.transform.position.z);
            this.transform.localRotation = MathUtils.RotToQuat(this.Rotation + _shakeRotationOffset);

            // invoke camera event
            CameraUpdate?.Invoke();
        }

        /// <summary>
        /// Called when this behavior is destroyed.
        /// </summary>
        private void OnDestroy() {

            // set Main to null
            if (Main == this) {
                Main = null;
            }
        }

        #endregion

        #region Private Target Resolver Fields

        private struct PrevTargetResolverDefinition {
            public ICameraTargetResolver TargetResolver;
            public float TransitionStartTimestamp;
            public float TransitionDuration;
        }

        private List<PrevTargetResolverDefinition> _prevTargetResolvers = new List<PrevTargetResolverDefinition>();

        #endregion

        #region Private Shake Fields

        private List<ICameraShaker> _cameraShakers = new List<ICameraShaker>();
        private Vector2 _shakeOffset;
        private float _shakeRotationOffset;

        #endregion
    }
}