using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity.Combat {

    public interface IReceivesDamage {

        /// <summary>
        /// Gets if the IReceivesDamage is enabled.  If not, attack events won't be triggered and the object won't take damage.
        /// IReceivesDamage can be disabled for an invincibility effect, or if the object has already died and shouldn't be hit again.
        /// </summary>
        bool IsReceivesDamageEnabled { get; }

        /// <summary>
        /// Receive an attack, then returns the result of the attack.
        /// </summary>
        /// <param name="attackInfo">Info of the attack.  This will be recycled, so don't save a reference to it.</param>
        /// <returns>Result</returns>
        AttackResult ReceiveDamage(AttackInfo attackInfo);
    }
}