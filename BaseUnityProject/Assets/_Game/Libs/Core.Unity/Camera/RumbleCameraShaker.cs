using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.Attributes;

namespace Core.Unity.Camera {

    [System.Serializable]
    public class RumbleCameraShaker : ICameraShaker {

        #region Inspector Fields

        [SerializeField, LongLabel]
        private float _updatePeriod = 1 / 60f;

        #endregion

        #region Properties

        public Vector2 Offset { get; private set; }

        public float RotationOffset => 0;

        #endregion

        #region Methods

        public void StartShake(float maxOffsetX, float maxOffsetY, float easeDuration) {
            _maxOffset0 = _maxOffset1;
            _maxOffset1.Set(maxOffsetX, maxOffsetY);
            _easeTime = 0;
            _easeDuration = easeDuration;
            this.UpdateOffset();
        }

        public void StopShake(float easeDuration) {
            _maxOffset0 = _maxOffset1;
            _maxOffset1.Set(0, 0);
            _easeTime = 0;
            _easeDuration = easeDuration;
            this.UpdateOffset();
        }

        /// <summary>
        /// Performs an update step.
        /// </summary>
        /// <param name="dt">Amount of time passed.</param>
        public void Update(float dt) {
            _easeTime += dt;
            _offsetTime += dt;
            if (_offsetTime >= _updatePeriod) {
                _offsetTime = 0;
                this.UpdateOffset();
            }
        }

        #endregion

        #region Private

        private void UpdateOffset() {
            Vector2 maxOffset;
            if (_easeDuration <= 0) {
                maxOffset = _maxOffset1;
            } else {
                maxOffset = Easing.Linear(_maxOffset0, _maxOffset1, _easeTime, _easeDuration);
            }
            maxOffset.x = Mathf.Abs(maxOffset.x);
            maxOffset.y = Mathf.Abs(maxOffset.y);

            if (maxOffset.x == 0 && maxOffset.y == 0) {
                this.Offset = Vector2.zero;
            } else {
                this.Offset = new Vector2(Random.Range(-maxOffset.x, maxOffset.x), Random.Range(-maxOffset.y, maxOffset.y));
            }
        }

        private float _offsetTime;
        private Vector2 _maxOffset0;
        private Vector2 _maxOffset1;
        private float _easeTime;
        private float _easeDuration;

        #endregion
    }
}