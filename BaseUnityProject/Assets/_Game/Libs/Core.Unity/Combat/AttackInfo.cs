using Core.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Core.Unity.Combat {

    /// <summary>
    /// Details information about an attack.
    /// </summary>
    public class AttackInfo {

        #region Properties

        /// <summary>
        /// Gets the original raw data for this attack, before it was modified by the attacker.
        /// </summary>
        public IAttackData AttackData;

        /// <summary>
        /// Gets the attack's damage.
        /// This is the result of all the calculations done on the attacker's side, without knowing anything about the opponent.
        /// E.g. the attacker's level, attack stat, move's power would be considered here.
        /// </summary>
        public int Damage;

        /// <summary>
        /// The direction of the attack in degrees, in [0, 360)
        /// </summary>
        public float Heading;

        /// <summary>
        /// Gets if the heading of this attack info points to the right.
        /// </summary>
        public bool IsToRight {
            get {
                float heading = MathUtils.Wrap360(Heading);
                return heading < 90 || heading >= 270;
            }
        }

        /// <summary>
        /// Gets how long the hit stop should be as a result of the attack hitting.
        /// Set to 0 for no hit stop.
        /// </summary>
        public float HitStopDuration;

        /// <summary>
        /// The attacker.  This can be null.
        /// </summary>
        public IDealsDamage Attacker;

        /// <summary>
        /// The receiver of the attack.
        /// </summary>
        public IReceivesDamage Receiver;

        /// <summary>
        /// Point of impact for the attack.
        /// </summary>
        public Vector2 ContactPoint;

        /// <summary>
        /// The Collision2D object associated with the attack.  Can be null (e.g. if the attack was the result of a trigger interaction).
        /// </summary>
        public Collision2D Collision2D;

        /// <summary>
        /// The attacker's hit box involved in the attack.  Can be null.
        /// </summary>
        public HitBox HitBox;

        /// <summary>
        /// The receiver's hurt box involved in the attack.  Can be null.
        /// </summary>
        public HurtBox HurtBox;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this attack info with the given data.
        /// </summary>
        /// <param name="attackData">Raw data of the attack</param>
        /// <param name="heading">Overrides the attack's heading.</param>
        /// <param name="attacker">The attacker.  Can be null.</param>
        /// <param name="receiver">The receiver of the attack.</param>
        /// <param name="contactPoint">The point of impact for the attack.</param>
        /// <param name="collision2D">The Collision2D object involved in the attack.  Can be null.</param>
        /// <param name="hitBox">The <see cref="HitBox"/> involved in the attack.  Can be null.</param>
        /// <param name="hurtBox">The <see cref="HurtBox"/> involved in the attack.  Can be null.</param>
        public void Initialize(IAttackData attackData, float heading, IDealsDamage attacker, IReceivesDamage receiver, Vector2 contactPoint, Collision2D collision2D, HitBox hitBox, HurtBox hurtBox) {
            this.AttackData = attackData;
            this.Damage = attackData.Damage;
            this.Heading = heading;
            this.HitStopDuration = attackData.HitStopDuration;
            _flags.Clear();
            attackData.GetAllFlags(_flags);
            _attributes.Clear();
            attackData.GetAllAttributes(_attributes);
            this.Attacker = attacker;
            this.Receiver = receiver;
            this.ContactPoint = contactPoint;
            this.Collision2D = collision2D;
            this.HitBox = hitBox;
            this.HurtBox = hurtBox;
        }

        /// <summary>
        /// Gets if this attack contains the flag with the given id.
        /// </summary>
        /// <param name="flagId">Id of the flag.</param>
        /// <returns>Has flag.</returns>
        public bool HasFlag(int flagId) {
            return _flags.Contains(flagId);
        }

        /// <summary>
        /// Sets if the given flag is contained in this data.
        /// </summary>
        /// <param name="flagId">Id of the flag.</param>
        public void SetFlag(int flagId, bool hasFlag) {
            if (hasFlag)
                _flags.Add(flagId);
            else
                _flags.Remove(flagId);
        }

        /// <summary>
        /// Gets the value of the attribute with the given id.
        /// </summary>
        /// <param name="attributeId">Id of the attribute.</param>
        /// <param name="defaultValue">Value to return if the attack data does not contain the attribute.</param>
        /// <returns>Attribute value.</returns>
        public float GetAttribute(int attributeId, float defaultValue = 0) {
            float val;
            if (_attributes.TryGetValue(attributeId, out val))
                return val;
            return defaultValue;
        }

        /// <summary>
        /// Tries to get the value of the attribute with the given id.
        /// </summary>
        /// <param name="attributeId">Id of the attribute.</param>
        /// <param name="value">Out param to be set to the value of the attribute.</param>
        /// <returns>If the attribute was found.</returns>
        public bool TryGetAttribute(int attributeId, out float value) {
            return _attributes.TryGetValue(attributeId, out value);
        }

        /// <summary>
        /// Sets the value of the attribute with the given id.
        /// </summary>
        /// <param name="attributeId">Id of the attribute.</param>
        /// <param name="value">Value to set.</param>
        public void SetAttribute(int attributeId, float value) {
            _attributes[attributeId] = value;
        }

        /// <summary>
        /// Removes the given attribute from the attack data.
        /// </summary>
        /// <param name="attributeId">Id of the attribute.</param>
        public void RemoveAttribute(int attributeId) {
            _attributes.Remove(attributeId);
        }

        /// <summary>
        /// Shallow copies all values from this attack info to the given attack info.
        /// </summary>
        /// <param name="attackInfo"></param>
        public void CopyTo(AttackInfo attackInfo) {
            attackInfo.AttackData = this.AttackData;
            attackInfo.Damage = this.Damage;
            attackInfo.Heading = this.Heading;
            attackInfo.HitStopDuration = this.HitStopDuration;
            attackInfo.Attacker = this.Attacker;
            attackInfo.Receiver = this.Receiver;
            attackInfo.ContactPoint = this.ContactPoint;
            attackInfo.Collision2D = this.Collision2D;
            attackInfo.HitBox = this.HitBox;
            attackInfo.HurtBox = this.HurtBox;
            attackInfo._flags.Clear();
            foreach (int f in _flags) {
                attackInfo._flags.Add(f);
            }
            attackInfo._attributes.Clear();
            foreach (KeyValuePair<int, float> a in _attributes) {
                attackInfo._attributes.Add(a.Key, a.Value);
            }
        }

        #endregion

        #region Private

        private HashSet<int> _flags = new HashSet<int>();
        private Dictionary<int, float> _attributes = new Dictionary<int, float>();

        #endregion
    }
}