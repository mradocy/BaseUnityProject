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
        }

        public void StopShaking() {
            this.Offset.Set(0, 0);
            _velocity.Set(0, 0);
            this.RotationOffset = 0;
            _rotVelocity = 0;
        }

        /// <summary>
        /// Performs an update step.
        /// </summary>
        /// <param name="dt">Amount of time passed.</param>
        public void Update(float dt) {
            float accelX = -this.Offset.x * _xSpringConstant - _velocity.x * _xDampeningConstant;
            float accelY = -this.Offset.y * _ySpringConstant - _velocity.y * _yDampeningConstant;
            float accelRot = -this.RotationOffset * _rotSpringConstant - _rotVelocity * _rotDampeningConstant;
            _velocity.x += accelX * dt;
            _velocity.y += accelY * dt;
            _rotVelocity += accelRot * dt;
            this.Offset += new Vector2(_velocity.x * dt, _velocity.y * dt);
            this.RotationOffset += _rotVelocity * dt;
        }

        #endregion

        #region Private

        private Vector2 _velocity = Vector2.zero;
        private float _rotVelocity = 0;

        #endregion
    }
}