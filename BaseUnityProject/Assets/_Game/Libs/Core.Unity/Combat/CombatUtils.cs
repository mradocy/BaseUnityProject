using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Combat {

    /// <summary>
    /// Static combat utils class.
    /// </summary>
    public static class CombatUtils {

        /// <summary>
        /// Executes an attack against a receiver.
        /// </summary>
        /// <param name="attackData">Raw data of the attack.</param>
        /// <param name="receiver">The receiver of the attack.  If <see cref="IReceivesDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        public static AttackResult ExecuteAttack(IAttackData attackData, IReceivesDamage receiver) {
            return ExecuteAttack(attackData, attackData.Heading, null, receiver, Vector2.zero, null, null, null);
        }

        /// <summary>
        /// Executes an attack against a receiver.
        /// </summary>
        /// <param name="attackData">Raw data of the attack.</param>
        /// <param name="attacker">The attacker.  Can be null.  If <see cref="IDealsDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        /// <param name="receiver">The receiver of the attack.  If <see cref="IReceivesDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        /// <param name="contactPoint">The point of impact for the attack.</param>
        /// <param name="collision2D">The Collision2D object involved in the attack.  Can be null.</param>
        /// <param name="hitBox">The <see cref="HitBox"/> involved in the attack.  Can be null.  If disabled, then the attack will fail immediately.</param>
        /// <param name="hurtBox">The <see cref="HurtBox"/> involved in the attack.  Can be null.  If disabled, then the attack will fail immedaitely.</param>
        public static AttackResult ExecuteAttack(IAttackData attackData, IDealsDamage attacker, IReceivesDamage receiver, Vector2 contactPoint, Collision2D collision2D, HitBox hitBox, HurtBox hurtBox) {
            return ExecuteAttack(attackData, attackData.Heading, attacker, receiver, contactPoint, collision2D, hitBox, hurtBox);
        }

        /// <summary>
        /// Executes an attack against a receiver.
        /// </summary>
        /// <param name="attackData">Raw data of the attack.</param>
        /// <param name="attacker">The attacker.  Can be null.  If <see cref="IDealsDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        /// <param name="receiver">The receiver of the attack.  If <see cref="IReceivesDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        /// <param name="contactPoint">The point of impact for the attack.</param>
        /// <param name="hurtBox">The <see cref="HurtBox"/> involved in the attack.  Can be null.  If disabled, then the attack will fail immedaitely.</param>
        public static AttackResult ExecuteAttack(IAttackData attackData, IDealsDamage attacker, IReceivesDamage receiver, Vector2 contactPoint, HurtBox hurtBox) {
            return ExecuteAttack(attackData, attackData.Heading, attacker, receiver, contactPoint, null, null, hurtBox);
        }

        /// <summary>
        /// Executes an attack against a receiver.
        /// </summary>
        /// <param name="attackData">Raw data of the attack.</param>
        /// <param name="heading">Overrides the attack's heading.</param>
        /// <param name="attacker">The attacker.  Can be null.  If <see cref="IDealsDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        /// <param name="receiver">The receiver of the attack.  If <see cref="IReceivesDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        /// <param name="contactPoint">The point of impact for the attack.</param>
        /// <param name="hurtBox">The <see cref="HurtBox"/> involved in the attack.  Can be null.  If disabled, then the attack will fail immedaitely.</param>
        public static AttackResult ExecuteAttack(IAttackData attackData, float heading, IDealsDamage attacker, IReceivesDamage receiver, Vector2 contactPoint, HurtBox hurtBox) {
            return ExecuteAttack(attackData, heading, attacker, receiver, contactPoint, null, null, hurtBox);
        }

        /// <summary>
        /// Executes an attack against a receiver.
        /// </summary>
        /// <param name="attackData">Raw data of the attack.</param>
        /// <param name="heading">Overrides the attack's heading.</param>
        /// <param name="attacker">The attacker.  Can be null.  If <see cref="IDealsDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        /// <param name="receiver">The receiver of the attack.  If <see cref="IReceivesDamage.IsDealsDamageEnabled"/> is false, then attack will fail immediately.</param>
        /// <param name="contactPoint">The point of impact for the attack.</param>
        /// <param name="collision2D">The Collision2D object involved in the attack.  Can be null.</param>
        /// <param name="hitBox">The <see cref="HitBox"/> involved in the attack.  Can be null.  If disabled, then the attack will fail immediately.</param>
        /// <param name="hurtBox">The <see cref="HurtBox"/> involved in the attack.  Can be null.  If disabled, then the attack will fail immedaitely.</param>
        public static AttackResult ExecuteAttack(IAttackData attackData, float heading, IDealsDamage attacker, IReceivesDamage receiver, Vector2 contactPoint, Collision2D collision2D, HitBox hitBox, HurtBox hurtBox) {
            // null checks
            if (attackData == null) {
                Debug.LogError($"Param {nameof(attackData)} cannot be null.");
                return AttackResult.Failed;
            }
            if (receiver == null) {
                Debug.LogError($"Param {nameof(receiver)} cannot be null.");
                return AttackResult.Failed;
            }

            // enabled checks
            if (attacker != null && !attacker.IsDealsDamageEnabled) {
                return AttackResult.Failed;
            }
            if (!receiver.IsReceivesDamageEnabled) {
                return AttackResult.Failed;
            }
            if (hitBox != null && !hitBox.enabled) {
                return AttackResult.Failed;
            }
            if (hurtBox != null && !hurtBox.enabled) {
                return AttackResult.Failed;
            }

            // get attack info
            AttackInfo attackInfo;
            if (_recycledAttackInfos.Count > 0) {
                attackInfo = _recycledAttackInfos.Pop();
            } else {
                attackInfo = new AttackInfo();
            }

            // copy data over
            attackInfo.Initialize(attackData, heading, attacker, receiver, contactPoint, collision2D, hitBox, hurtBox);

            // have attacker processes the attack (e.g. does attacking damage calculations)
            if (attackInfo.Attacker != null) {
                attackInfo.Attacker.ProcessAttack(attackInfo);
            }

            // deal damage
            AttackResult result = hurtBox.ReceivesDamage.ReceiveDamage(attackInfo);

            // notify attacker of result
            if (attackInfo.Attacker != null) {
                attackInfo.Attacker.NotifyAttackResult(attackInfo, result);
            }

            // recycle attack info
            _recycledAttackInfos.Push(attackInfo);

            return result;
        }

        private static Stack<AttackInfo> _recycledAttackInfos = new Stack<AttackInfo>();
    }
}