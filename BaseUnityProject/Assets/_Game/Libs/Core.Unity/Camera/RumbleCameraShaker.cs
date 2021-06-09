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
        private float _periodX = 5 / 60f;
        [SerializeField, LongLabel]
        private float _periodY = 3 / 60f;

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
        }

        public void StopShake(float easeDuration) {
            _maxOffset0 = _maxOffset1;
            _maxOffset1.Set(0, 0);
            _easeTime = 0;
            _easeDuration = easeDuration;
        }

        /// <summary>
        /// Performs an update step.
        /// </summary>
        /// <param name="dt">Amount of time passed.</param>
        public void Update(float dt) {
            // get max offset
            _easeTime += dt;
            Vector2 maxOffset;
            if (_easeDuration <= 0) {
                maxOffset = _maxOffset1;
            } else {
                maxOffset = Easing.Linear(_maxOffset0, _maxOffset1, _easeTime, _easeDuration);
            }
            maxOffset.x = Mathf.Abs(maxOffset.x);
            maxOffset.y = Mathf.Abs(maxOffset.y);

            // update offset x
            _xTime += dt;
            if (_xTime >= _periodX) {
                _xTime = 0;

                // update offset target
                _offset0x = _offset1x;
                if (_offset1x < 0) {
                    _offset1x = Random.Range(maxOffset.x / 2, maxOffset.x);
                } else {
                    _offset1x = Random.Range(-maxOffset.x, -maxOffset.x / 2);
                }
            }
            float offsetX = Easing.QuadInOut(_offset0x, _offset0y, _xTime, _periodX);

            // update offset y
            _yTime += dt;
            if (_yTime >= _periodY) {
                _yTime = 0;

                // update offset target
                _offset0y = _offset1y;
                if (_offset1y < 0) {
                    _offset1y = Random.Range(maxOffset.y / 2, maxOffset.y);
                } else {
                    _offset1y = Random.Range(-maxOffset.y, -maxOffset.y / 2);
                }
            }
            float offsetY = Easing.QuadInOut(_offset1x, _offset1y, _yTime, _periodY);

            this.Offset = new Vector2(offsetX, offsetY);
        }

        #endregion

        #region Private

        private Vector2 _maxOffset0;
        private Vector2 _maxOffset1;
        private float _easeTime;
        private float _easeDuration;

        private float _xTime;
        private float _yTime;
        private float _offset0x;
        private float _offset0y;
        private float _offset1x;
        private float _offset1y;

        #endregion
    }
}