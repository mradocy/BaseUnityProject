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
        /// Constructor.
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