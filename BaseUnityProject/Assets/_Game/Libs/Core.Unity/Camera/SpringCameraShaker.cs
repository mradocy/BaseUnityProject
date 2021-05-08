using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.Attributes;

namespace Core.Unity.Camera {

    /// <summary>
    /// Calculates a shake over time by calling <see cref="Update(float)"/>.
    /// </summary>
    [System.Serializable]
    public class SpringCameraShaker : ICameraShaker {

        #region Inspector Fields

        [SerializeField, LongLabel]
        private float _xSpringConstant = 3000;
        [SerializeField, LongLabel]
        private float _xDampeningConstant = 4f;

        [SerializeField, LongLabel]
        private float _ySpringConstant = 2400;
        [SerializeField, LongLabel]
        private float _yDampeningConstant = 4f;

        [SerializeField, LongLabel]
        private float _rotSpringConstant = 1000f;
        [SerializeField, LongLabel]
        private float _rotDampeningConstant = 4;

        [SerializeField, LongLabel]
        private float _updatePeriod = 0.02f;

        #endregion

        #region Properties

        public Vector2 Offset { get; private set; }

        public float RotationOffset { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts a shake.
        /// </summary>
        public void Shake(float velocityX, float velocityY) {
            _velocity.Set(velocityX, velocityY);
            _easeInDuration = 0;
        }

        /// <summary>
        /// Starts a shake.
        /// </summary>
        public void Shake(float velocityX, float velocityY, float velocityRot) {
            this.Shake(velocityX, velocityY);
            _rotVelocity = velocityRot;
        }

        /// <summary>
        /// Starts a shake that eases in over the given duration.
        /// </summary>
        public void ShakeEaseIn(float velocityX, float velocityY, float velocityRot, float easeDuration) {
            this.Shake(velocityX, velocityY, velocityRot);
            _easeInTime = 0;
            _easeInDuration = easeDuration;
        }

        /// <summary>
        /// Stops the shaking of this shaker.
        /// </summary>
        public void StopShaking() {
            this.Offset.Set(0, 0);
            _velocity.Set(0, 0);
            this.RotationOffset = 0;
            _rotVelocity = 0;
            _easeInDuration = 0;
        }

        /// <summary>
        /// Performs an update step.
        /// </summary>
        /// <param name="dt">Amount of time passed.</param>
        public void Update(float dt) {
            _offsetTime += dt;
            if (_offsetTime < _updatePeriod)
                return;
            _offsetTime = 0;

            dt = _updatePeriod;
            _easeInTime += dt;

            // spring update
            float accelX = -_offset.x * _xSpringConstant - _velocity.x * _xDampeningConstant;
            float accelY = -_offset.y * _ySpringConstant - _velocity.y * _yDampeningConstant;
            float accelRot = -_rotationOffset * _rotSpringConstant - _rotVelocity * _rotDampeningConstant;
            _velocity.x += accelX * dt;
            _velocity.y += accelY * dt;
            _rotVelocity += accelRot * dt;
            _offset += new Vector2(_velocity.x * dt, _velocity.y * dt);
            _rotationOffset += _rotVelocity * dt;

            // get ease in multiplier if easing in
            float easeInMultiplier = 1;
            if (_easeInDuration > 0 && _easeInTime < _easeInDuration) {
                easeInMultiplier = Easing.QuadInOut(0, 1, _easeInTime, _easeInDuration);
            }

            this.Offset = _offset * easeInMultiplier;
            this.RotationOffset = _rotationOffset * easeInMultiplier;
        }

        #endregion

        #region Private

        private float _offsetTime = 0;
        private Vector2 _offset;
        private float _rotationOffset;
        private Vector2 _velocity = Vector2.zero;
        private float _rotVelocity = 0;
        private float _easeInTime;
        private float _easeInDuration;

        #endregion
    }
}