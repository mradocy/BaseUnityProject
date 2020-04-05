using Core.Unity.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Combat {

    /// <summary>
    /// Details the result of an attack.
    /// </summary>
    public struct AttackResult {

        /// <summary>
        /// An attack that failed.
        /// </summary>
        public static readonly AttackResult Failed = new AttackResult(false);

        #region Constructors

        /// <summary>
        /// Creates a default <see cref="AttackResult"/> where the attack was a success and dealt the amount of damage and hitstop specified in the <paramref name="attackInfo"/>.
        /// </summary>
        /// <param name="attackInfo">Info of the attack.</param>
        public AttackResult(AttackInfo attackInfo) {
            Success = true;
            Damage = attackInfo.Damage;
            HitStopDuration = attackInfo.HitStopDuration;
        }

        /// <summary>
        /// Creates a default <see cref="AttackResult"/> with the given <paramref name="success"/> value.
        /// </summary>
        /// <param name="success">If the attack hit.</param>
        public AttackResult(bool success) {
            Success = success;
            Damage = 0;
            HitStopDuration = 0;
        }

        #endregion

        /// <summary>
        /// If the attack successfully hit the receiver.
        /// Note that it's possible for an attack to be a success and deal 0 damage.
        /// </summary>
        public bool Success;

        /// <summary>
        /// The damage actually dealt.  This will be affected by the opponent's defense or other factors.
        /// </summary>
        public int Damage;

        /// <summary>
        /// How much hit stop should be applied as the result of this attack.  0 means no hit stop.
        /// </summary>
        public float HitStopDuration;
    }
}