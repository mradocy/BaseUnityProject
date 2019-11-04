using Core.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Core.Unity.Combat {

    public class AttackInfo : IAttackInfo {

        #region Creation

        /// <summary>
        /// Creates a new <see cref="AttackInfo"/>.
        /// </summary>
        /// <param name="data">Data of the attack.</param>
        /// <param name="attacker">The attacker.  Can be null.</param>
        /// <param name="reflectHeading">If the heading should be reflected over the y axis (i.e. if attacker was facing left)</param>
        /// <param name="contactPoint">Point of impact for the attack.</param>
        /// <param name="collision2D">Collision2D object for the attack.  Can be null.</param>
        /// <returns>New attack info.</returns>
        public static AttackInfo CreateNew(IAttackData data, IDealsDamage attacker, bool reflectHeading, Vector2 contactPoint, Collision2D collision2D) {
            if (data == null) {
                throw new System.ArgumentNullException(nameof(data));
            }

            AttackInfo ai = new AttackInfo();
            ai.Data = data;
            ai.Attacker = attacker;

            // copy over properties from data
            ai.AttackingDamage = data.Damage;
            if (reflectHeading) {
                ai.AttackingHeading = MathUtils.Wrap360(180 - data.Heading);
            } else {
                ai.AttackingHeading = data.Heading;
            }
            ai.AttackingHitStopDuration = data.HitStopDuration;
            ai.ContactPoint = contactPoint;
            ai.Collision2D = collision2D;
            ai._flags.Clear();
            data.GetAllFlags(ai._flags);
            ai._attributes.Clear();
            data.GetAllAttributes(ai._attributes);

            return ai;
        }

        /// <summary>
        /// Private Constructor.
        /// </summary>
        private AttackInfo() { }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IAttackData Data { get; private set; }

        /// <inheritdoc />
        public IDealsDamage Attacker { get; private set; }

        /// <inheritdoc />
        public float AttackingDamage { get; set; }

        /// <inheritdoc />
        public float AttackingHeading { get; set; }

        /// <inheritdoc />
        public float AttackingHitStopDuration { get; set; }

        /// <inheritdoc />
        public Vector2 ContactPoint { get; set; }

        /// <inheritdoc />
        public Collision2D Collision2D { get; set; }

        #endregion

        #region Methods

        /// <inheritdoc />
        public bool HasFlag(int flagId) {
            return _flags.Contains(flagId);
        }

        /// <inheritdoc />
        public void SetFlag(int flagId, bool hasFlag) {
            if (hasFlag)
                _flags.Add(flagId);
            else
                _flags.Remove(flagId);
        }

        /// <inheritdoc />
        public float GetAttribute(int attributeId, float defaultValue = 0) {
            float val;
            if (_attributes.TryGetValue(attributeId, out val))
                return val;
            return defaultValue;
        }

        /// <inheritdoc />
        public void SetAttribute(int attributeId, float value) {
            _attributes[attributeId] = value;
        }

        /// <inheritdoc />
        public void RemoveAttribute(int attributeId) {
            _attributes.Remove(attributeId);
        }

        /// <inheritdoc />
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Attacking Damage: {this.AttackingDamage}, ");
            sb.Append($"Attacking Heading: {this.AttackingHeading}, ");
            sb.Append($"Attacking Hit Stop Duration: {this.AttackingHitStopDuration}");

            return sb.ToString();
        }

        #endregion

        #region Private

        private HashSet<int> _flags = new HashSet<int>();
        private Dictionary<int, float> _attributes = new Dictionary<int, float>();

        #endregion
    }
}