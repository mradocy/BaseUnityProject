using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity.Combat {

    /// <summary>
    /// Interface for objects that launch attacks and deal damage.
    /// </summary>
    public interface IDealsDamage {

        /// <summary>
        /// Gets if the IDealsDamage is enabled.  If not, hitboxes won't attempt to make attacks.
        /// IDealsDamage can be disabled when the object dies, to ensure it can't deal any more damage to the player.
        /// </summary>
        bool IsDealsDamageEnabled { get; }

        /// <summary>
        /// Gets an <see cref="IAttackData"/> by name, or returns null if not defined.
        /// </summary>
        /// <param name="name">Name identifying the attack data.</param>
        /// <returns>IAttackData.</returns>
        IAttackData GetAttackData(string name);

        /// <summary>
        /// Process a given attack.  This is called before damage is dealt to the opponent.
        /// This can be used to perform calculations from the attacker's side, apply attributes, etc.
        /// Attacking damage, flags, attributes have already been copied over from the attack data.
        /// </summary>
        /// <param name="attackInfo">Attack info to modify.</param>
        void ProcessAttack(AttackInfo attackInfo);

        /// <summary>
        /// Get notified of the result of an attack.
        /// </summary>
        /// <param name="attackInfo">Info of the attack.  This will be recycled, so don't save a reference to it.</param>
        /// <param name="result">Result of the attack.</param>
        void NotifyAttackResult(AttackInfo attackInfo, AttackResult result);
    }
}