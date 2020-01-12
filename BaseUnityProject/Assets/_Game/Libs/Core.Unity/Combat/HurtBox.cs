using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Combat {

    public class HurtBox : MonoBehaviour {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("Can be null.  If assigned a value, the unique id of this hurtbox will match this group's unique id.")]
        private HurtBoxGroup _hurtBoxGroup = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an id that uniquely defines this hurt box instance.  e.g. the result of this.GetInstanceID().
        /// Hitboxes will not hit hurtboxes with the same <see cref="UniqueId"/> until reset.
        /// </summary>
        public int UniqueId {
            get {
                if (_uniqueId == null)
                    this.SetDefaultUniqueId();
                
                return _uniqueId.Value;
            }
        }

        /// <summary>
        /// Reference to the <see cref="IReceivesDamage"/> component that this hurtbox sends info to.
        /// Will be found automatically in Awake() if component is a parent or sibling.
        /// Can be set manually with <see cref="SetDealsDamage(IDealsDamage)"/>.
        /// </summary>
        public IReceivesDamage ReceivesDamage { get; private set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// Manually sets the <see cref="ReceivesDamage"/> property.
        /// </summary>
        /// <param name="receivesDamage">The IReceivesDamage component to set.</param>
        public void SetReceivesDamage(IReceivesDamage receivesDamage) {
            this.ReceivesDamage = receivesDamage;
        }

        /// <summary>
        /// Manually sets the <see cref="UniqueId"/> property.
        /// For niche (hacky) uses.
        /// </summary>
        /// <param name="id">Unique id</param>
        public void SetUniqueId(int id) {
            _uniqueId = id;
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            // set unique id
            if (_uniqueId == null) {
                this.SetDefaultUniqueId();
            }
            
            // get reference to IReceivesDamage.  Can be null for now
            this.ReceivesDamage = this.GetComponent<IReceivesDamage>();
            if (this.ReceivesDamage == null) {
                this.ReceivesDamage = this.GetComponentInParent<IReceivesDamage>();
            }
        }

        #endregion

        /// <summary>
        /// Sets the unique id to its default value.
        /// </summary>
        private void SetDefaultUniqueId() {
            if (_hurtBoxGroup == null) {
                _uniqueId = this.GetInstanceID();
            } else {
                _uniqueId = _hurtBoxGroup.UniqueId;
            }
        }

        /// <summary>
        /// Field for <see cref="UniqueId"/>.  If null, it will be set by <see cref="SetDefaultUniqueId"/>.
        /// </summary>
        private int? _uniqueId = null;
    }
}