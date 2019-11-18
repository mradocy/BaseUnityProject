using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity.Combat {

    public interface IReceivesDamage {

        /// <summary>
        /// Event triggered when this object receives damage.
        /// Damage has already been dealt, altering the values passed in will not change anything.
        /// </summary>
        event UnityAction<IAttackInfo, IAttackResult> DamageResult;

        /// <summary>
        /// Gets if the IReceivesDamage is enabled.  If not, attack events won't be triggered and the object won't take damage.
        /// IReceivesDamage can be disabled for an invincibility effect, or if the object has already died and shouldn't be hit again.
        /// </summary>
        bool IsReceivesDamageEnabled { get; }

        /// <summary>
        /// The maximum health of the object.
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// The amount of health the object currently has.
        /// </summary>
        float Health { get; }

        /// <summary>
        /// Receive an attack, then returns the result of the attack.
        /// </summary>
        /// <param name="attackInfo">Info of the attack.</param>
        /// <returns>Result</returns>
        IAttackResult ReceiveDamage(IAttackInfo attackInfo);
    }
}