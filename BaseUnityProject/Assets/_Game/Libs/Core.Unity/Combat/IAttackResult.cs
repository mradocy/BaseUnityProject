using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Combat {

    /// <summary>
    /// Interface detailing the result of an attack.
    /// </summary>
    public interface IAttackResult {

        /// <summary>
        /// Gets if the attack hit.
        /// </summary>
        bool Hit { get; }

        /// <summary>
        /// The damage actually dealt.  This will be affected by the opponent's defense or other factors.
        /// </summary>
        int Damage { get; }

        /// <summary>
        /// Gets how much hit stop should be applied as the result of this attack.  0 means no hit stop.
        /// </summary>
        float HitStopDuration { get; }
    }
}