using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Combat {

    /// <summary>
    /// Used by <see cref="HitBox"/> to automatically set the heading of an attack.
    /// </summary>
    public enum HeadingSetMode {
        /// <summary>
        /// Uses the heading from the attack data.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Flips the attack heading if the receiver's hurtBox's position.x < contactPoint.x
        /// </summary>
        FlipPosition = 1,
        /// <summary>
        /// Flips the attack data heading if the hitBox's lossyScale.x < 0.
        /// </summary>
        FlipLossyScale = 2,
        
    }
}