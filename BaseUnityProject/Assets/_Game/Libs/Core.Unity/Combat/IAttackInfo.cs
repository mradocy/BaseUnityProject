using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Combat {

    /// <summary>
    /// Interface detailing info of an attack.
    /// </summary>
    public interface IAttackInfo {

        /// <summary>
        /// Gets the original raw data for this attack, before it was modified by the attacker.
        /// </summary>
        IAttackData Data { get; }

        /// <summary>
        /// The attacker.  This can be null.
        /// </summary>
        IDealsDamage Attacker { get; }
        
        /// <summary>
        /// Gets the attack's damage.
        /// This is the result of all the calculations done on the attacker's side, without knowing anything about the opponent.
        /// E.g. the attacker's level, attack stat, move's power would be considered here.
        /// </summary>
        float AttackingDamage { get; set; }

        /// <summary>
        /// The direction of the attack in degrees, in [0, 360)
        /// </summary>
        float AttackingHeading { get; set; }

        /// <summary>
        /// Gets how long the hit stop should be as a result of the attack hitting.
        /// Set to 0 for no hit stop.
        /// </summary>
        float AttackingHitStopDuration { get; set; }

        /// <summary>
        /// Point of impact for the attack.
        /// </summary>
        Vector2 ContactPoint { get; }

        /// <summary>
        /// The <see cref="Collision2D"/> object associated with the attack.  Can be null (e.g. if the attack was the result of a trigger interaction).
        /// </summary>
        Collision2D Collision2D { get; }

        /// <summary>
        /// The <see cref="HurtBox"/> of the defender that got hit by the attack.  Can be null.
        /// </summary>
        HurtBox HurtBox { get; }

        /// <summary>
        /// Gets if this attack contains the flag with the given id.
        /// </summary>
        /// <param name="flagId">Id of the flag.</param>
        /// <returns>Has flag.</returns>
        bool HasFlag(int flagId);

        /// <summary>
        /// Sets if the given flag is contained in this data.
        /// </summary>
        /// <param name="flagId">Id of the flag.</param>
        void SetFlag(int flagId, bool hasFlag);

        /// <summary>
        /// Gets the value of the attribute with the given id.
        /// </summary>
        /// <param name="attributeId">Id of the attribute.</param>
        /// <param name="defaultValue">Value to return if the attack data does not contain the attribute.</param>
        /// <returns>Attribute value.</returns>
        float GetAttribute(int attributeId, float defaultValue = 0);

        /// <summary>
        /// Sets the value of the attribute with the given id.
        /// </summary>
        /// <param name="attributeId">Id of the attribute.</param>
        /// <param name="value">Value to set.</param>
        void SetAttribute(int attributeId, float value);

        /// <summary>
        /// Removes the given attribute from the attack data.
        /// </summary>
        /// <param name="attributeId">Id of the attribute.</param>
        void RemoveAttribute(int attributeId);
    }
}