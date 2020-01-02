using Core.Unity;
using Core.Unity.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Combat {

    public class HitBox : MonoBehaviour, IHitStopUser {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("Name of the attack.  Used to identify IAttackData from the parent/sibling IDealsDamage.")]
        private string _attackName = null;

        [SerializeField]
        [Tooltip("If this hit box should start enabled.  Otherwise, it will have to be enabled manually.")]
        private bool _startEnabled = false;

        [SerializeField, LongLabel]
        [Tooltip("How long to keep a record of hitting a hurt box before removing it automatically.")]
        private float _hurtRecordDuration = 999999;

        #endregion

        #region Properties

        /// <summary>
        /// Reference to the <see cref="IDealsDamage"/> component that this hitbox gets info from.
        /// Will be found automatically in Awake() if component is a parent or sibling.
        /// Can be set manually with <see cref="SetDealsDamage(IDealsDamage)"/>.
        /// </summary>
        public IDealsDamage DealsDamage { get; private set; } = null;

        /// <summary>
        /// Name of the attack represented by this hit box.
        /// </summary>
        public string AttackName { get { return _attackName; } }

        /// <inheritdoc />
        public bool IsHitStopped { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Manually sets the <see cref="DealsDamage"/> property.
        /// </summary>
        /// <param name="dealsDamage">The IDealsDamage component to set.</param>
        public void SetDealsDamage(IDealsDamage dealsDamage) {
            this.DealsDamage = dealsDamage;
        }

        /// <summary>
        /// Gets the attack data for this hit box from the referenced <see cref="DealsDamage"/> component.
        /// Will cache the attack data if found.
        /// </summary>
        /// <returns>Attack data.</returns>
        public IAttackData GetAttackData() {
            if (_cachedAttackData != null)
                return _cachedAttackData;
            
            if (this.DealsDamage == null) {
                Debug.LogError("Cannot get attack data, DealsDamage property for this HitBox is null.  Attach a component as a parent or sibling that inherits IDealsDamage, or set the property manually with SetDealsDamage().");
                return null;
            }

            IAttackData data = this.DealsDamage.GetAttackData(_attackName);
            if (data == null) {
                Debug.LogError($"Cannot find attack data with name {_attackName}.");
            }
            _cachedAttackData = data;
            return data;
        }

        /// <summary>
        /// Clears all hurt box records.  This will allow this hit box to hit the same hurt box again without having to re-enable the hit box.
        /// </summary>
        public void ClearHurtBoxRecords() {
            while (_hurtBoxRecords.Count > 0) {
                this.RemoveHurtBoxRecord(_hurtBoxRecords.Count - 1);
            }
        }

        /// <summary>
        /// Enables this hit box (same as setting hitBox.enabled = true).
        /// This method can be called by an animation event.
        /// </summary>
        public void EnableHitBox() {
            this.enabled = true;
        }

        /// <summary>
        /// Disables this hit box (same as setting hitBox.enabled = false).
        /// This method can be called by an animation event.
        /// </summary>
        public void DisableHitBox() {
            this.enabled = false;
        }

        #endregion

        #region Private Unity Methods

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {

            // get reference to IDealsDamage.  Can be null for now
            this.DealsDamage = this.GetComponent<IDealsDamage>();
            if (this.DealsDamage == null) {
                this.DealsDamage = this.GetComponentInParent<IDealsDamage>();
            }

            this.enabled = _startEnabled;
        }

        /// <summary>
        /// Called by Unity every physics step, if the MonoBehaviour is enabled.
        /// </summary>
        private void FixedUpdate() {
            if (!this.IsHitStopped) {
                _fixedTime += Time.fixedDeltaTime;
            }

            this.UpdateHurtBoxRecords();

            // detect hit stop over
            if (this.IsHitStopped) {
                if (Time.fixedTime >= _hitStopEndTime) {
                    this.IsHitStopped = false;
                }
            }
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable() {
            this.ClearHurtBoxRecords();
        }

        /// <summary>
        /// Sent when an incoming collider makes contact with this object's collider.
        /// </summary>
        /// <param name="collision2d">Collision details.</param>
        private void OnCollisionStay2D(Collision2D collision2d) {
            this.OnInteraction2D(collision2d, null);
        }

        /// <summary>
        /// Sent each frame where another object is within a trigger collider attached to this object.
        /// </summary>
        /// <param name="c2d">Collider2D</param>
        private void OnTriggerStay2D(Collider2D c2d) {
            this.OnInteraction2D(null, c2d);
        }

        /// <summary>
        /// Called when this object is destroyed.
        /// </summary>
        private void OnDestroy() {
            this.ClearHurtBoxRecords();
        }

        #endregion

        #region Private

        /// <summary>
        /// Called during a collision or trigger interaction
        /// </summary>
        /// <param name="collision2d">Collision details, if a collision interaction.</param>
        /// <param name="collider2d">The collider hit, if a trigger interaction.</param>
        private void OnInteraction2D(Collision2D collision2d, Collider2D collider2d) {

            if (!this.enabled)
                return;
            if (this.DealsDamage == null || !this.DealsDamage.IsDealsDamageEnabled)
                return;

            // determine collision or trigger interaction and get collider hit
            bool isCollisionInteraction = collision2d != null;
            if (isCollisionInteraction) {
                collider2d = collision2d.collider;

                if (collision2d.contactCount < 1)
                    return;
            }

            HurtBox hurtBox = collider2d?.gameObject?.GetComponent<HurtBox>();
            if (hurtBox?.ReceivesDamage == null)
                return;
            if (!hurtBox.enabled || !hurtBox.ReceivesDamage.IsReceivesDamageEnabled)
                return;

            // ignore if already hit the hurt box
            if (this.GetHurtBoxRecord(hurtBox) != null)
                return;

            // get data for attack
            IAttackData attackData = this.GetAttackData();
            if (attackData == null)
                return;

            // get point of contact
            Vector2 contactPoint;
            if (isCollisionInteraction) {
                ContactPoint2D collisionContactPoint = collision2d.GetContact(0);
                contactPoint = collisionContactPoint.point;
            } else {
                //  (may need a better way to do this for trigger interaction)
                contactPoint = collider2d.ClosestPoint(transform.position);
            }

            // create AttackInfo
            AttackInfo attackInfo = AttackInfo.CreateNew(attackData, this.DealsDamage, this.transform.lossyScale.x < 0, contactPoint, collision2d);

            // attacker processes attack (e.g. does attacking damage calculations)
            if (this.DealsDamage != null) {
                this.DealsDamage.ProcessAttack(attackInfo);
            }

            // deal damage
            IAttackResult result = hurtBox.ReceivesDamage.ReceiveDamage(attackInfo);

            // add hurt box record
            this.AddHurtBoxRecord(hurtBox, _fixedTime + _hurtRecordDuration);

            // notify attacker
            if (this.DealsDamage != null) {
                this.DealsDamage.NotifyAttackResult(attackInfo, result);
            }

            if (result.Hit) {

                // apply hit stop
                if (result.HitStopDuration > 0) {
                    this.IsHitStopped = true;
                    _hitStopEndTime = Mathf.Max(_hitStopEndTime, Time.fixedTime + result.HitStopDuration);
                }
            }
        }

        private IAttackData _cachedAttackData = null;

        /// <summary>
        /// Incremented by <see cref="Time.fixedDeltaTime"/> each FixedUpdate(), except when time stopped.
        /// </summary>
        private float _fixedTime = 0;

        private float _hitStopEndTime = 0;

        #endregion

        #region Private - HurtBoxRecords

        /// <summary>
        /// Record of a <see cref="HurtBox"/> that was hit by this hit box.
        /// </summary>
        private class HurtBoxRecord {
            public int HitBoxId;
            public float TimeHit;
            public float TimeRemove;
        }

        /// <summary>
        /// Gets the <see cref="HurtBoxRecord"/> for the given hurt box.
        /// </summary>
        /// <param name="hurtBox">HurtBox</param>
        /// <returns>HurtBoxRecord</returns>
        private HurtBoxRecord GetHurtBoxRecord(HurtBox hurtBox) {
            int id = hurtBox.UniqueId;
            foreach (HurtBoxRecord record in _hurtBoxRecords) {
                if (record.HitBoxId == id)
                    return record;
            }
            return null;
        }

        /// <summary>
        /// Adds a record for the given hurt box.  Does not check if a record for the hurt box already exists.
        /// </summary>
        /// <param name="hurtBox">Hurt box.</param>
        /// <param name="timeRemove">The time (in <see cref="Time.time"/> to automatically remove the record.</param>
        /// <returns>Record.</returns>
        private HurtBoxRecord AddHurtBoxRecord(HurtBox hurtBox, float timeRemove) {
            HurtBoxRecord record;
            if (_recycledHurtBoxRecords.Count == 0) {
                record = new HurtBoxRecord();
            } else {
                record = _recycledHurtBoxRecords[_recycledHurtBoxRecords.Count - 1];
                _recycledHurtBoxRecords.RemoveAt(_recycledHurtBoxRecords.Count - 1);
            }

            record.HitBoxId = hurtBox.UniqueId;
            record.TimeHit = _fixedTime;
            record.TimeRemove = timeRemove;
            _hurtBoxRecords.Add(record);
            return record;
        }

        /// <summary>
        /// Removes a hurt box record.
        /// </summary>
        /// <param name="recordIndex">The index of the record.  The validity of the index is not checked.</param>
        private void RemoveHurtBoxRecord(int recordIndex) {
            HurtBoxRecord record = _hurtBoxRecords[recordIndex];
            _hurtBoxRecords.RemoveAt(recordIndex);
            _recycledHurtBoxRecords.Add(record);
        }

        /// <summary>
        /// Automatically removes all hurt box records that have stayed their time.
        /// </summary>
        private void UpdateHurtBoxRecords() {
            for (int i = 0; i < _hurtBoxRecords.Count; i++) {
                if (_fixedTime >= _hurtBoxRecords[i].TimeRemove) {
                    this.RemoveHurtBoxRecord(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Records of hurt boxes hit by this hit box.
        /// </summary>
        private List<HurtBoxRecord> _hurtBoxRecords = new List<HurtBoxRecord>();

        /// <summary>
        /// Recycled hurt box records.
        /// </summary>
        private static List<HurtBoxRecord> _recycledHurtBoxRecords = new List<HurtBoxRecord>();

        #endregion
    }
}