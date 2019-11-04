using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Combat {

    /// <summary>
    /// Interface for the "hit stop" effect, where an object briefly freezes as the result of an impactful attack.
    /// <para/>
    /// A hit stop is often the result of an attack, so it's better to apply the hitstop in the methods that handle the attack,
    /// rather than force a new ApplyHitStop() method be implemented.
    /// </summary>
    public interface IHitStopUser {

        /// <summary>
        /// Gets if this object is currently hit stopped.
        /// </summary>
        bool IsHitStopped { get; }
    }
}