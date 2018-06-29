using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Calls dealDamage() to HurtBoxes on touch.  Doesn't do anything if disabled.
/// If gameObject or parent's gameObject has a DealsDamage component, will call that component's dealDamage() instead.
/// Will not collide with HurtBoxes that are disabled.
/// Will not collide with HurtBoxes that have been added to the hurt record.  
///     Every hurtBox is added to the record on collision.
///     HurtBoxes are automatically removed from the record on an OnTriggerExit2D event (which may not be reliable).
///     Can manually clear the hurt record by calling hurtRecordClear().
/// </summary>
public class HitObject : MonoBehaviour {

    #region Inspector Properties

    public float damage = 1;
    public float magnitude = 1;
    [Tooltip("When true, heading of AttackInfo will be set to the angle between the positions of the hitObject and hurtObject.")]
    public bool setHeadingFromPositions = true;
    public float heading = 0;
    [Tooltip("HurtObjects will automatically be removed after being the hurt record for this duration.  Set to 0 for HurtObjects to never be added to the record.")]
    public float hurtRecordAutoClear = 9999;

    #endregion

    #region Properties

    /// <summary>
    /// Neatly sets properties of this HitObject.  heading and setHeadingFromPositions are unchanged.
    /// </summary>
    public void setProps(float damage, float magnitude) {
        this.damage = damage;
        this.magnitude = magnitude;
    }

    /// <summary>
    /// Neatly sets properties of this HitObject.  Since heading is explicitly given, setHeadingFromPositions is also set to false.
    /// </summary>
    public void setProps(float damage, float magnitude, float heading) {
        this.damage = damage;
        this.magnitude = magnitude;
        this.heading = heading;
        setHeadingFromPositions = false;
    }

    /// <summary>
    /// The position of the hitObject.  Can be used to determine the heading of an attack.
    /// </summary>
    public Vector2 position {
        get {
            return transform.position;
        }
    }

    /// <summary>
    /// Reference to the neighboring DealsDamage component, if it exists.
    /// </summary>
    public DealsDamage dealsDamage { get; private set; }

    #endregion

    #region Hurt Records

    /// <summary>
    /// Adds a HurtObject to the record of objects hit by this attack.  Collisions between this HitObject and HurtObjects in the record will not be considered.
    /// </summary>
    /// <param name="hurtObjectID">ID of the HurtObject (given by hurtObject.globalHurtID)</param>
    public void hurtRecordAdd(int hurtObjectID) {
        if (hurtRecordContains(hurtObjectID)) return;
        HurtRecord hr = new HurtRecord();
        hr.hurtObjectGlobalID = hurtObjectID;
        hr.time = Time.time;
        _hurtRecords.Add(hr);
    }
    /// <summary>
    /// Returns if this HitObject's hurt record currently contains the given hurtObject.
    /// </summary>
    /// <param name="hurtObjectID">ID of the HurtObject (given by hurtObject.globalHurtID)</param>
    public bool hurtRecordContains(int hurtObjectID) {
        foreach (HurtRecord hr in _hurtRecords) {
            if (hr.hurtObjectGlobalID == hurtObjectID)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Removes the given hurtObject from the hurt record, if it exists.
    /// </summary>
    /// <param name="hurtObjectID">ID of the HurtObject (given by hurtObject.globalHurtID)</param>
    public void hurtRecordRemove(int hurtObjectID) {
        for (int i = 0; i < _hurtRecords.Count; i++) {
            if (_hurtRecords[i].hurtObjectGlobalID == hurtObjectID) {
                _hurtRecords.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// Removes all hurt records that are older than the given time.
    /// </summary>
    /// <param name="time">Time ago, in seconds.  e.g. '.5' will remove all hurt records that were added more than .5 seconds ago.</param>
    public void hurtRecordRemoveOlderThan(float time) {
        float threshold = Time.time - time;
        for (int i = 0; i < _hurtRecords.Count; i++) {
            if (_hurtRecords[i].time < threshold) {
                _hurtRecords.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Manually clear the hurt record, allowing this hitObject to hit anything again.
    /// </summary>
    public void hurtRecordClear() {
        _hurtRecords.Clear();
    }

    #endregion

    /// <summary>
    /// Hitting a hurtObject.  This is automatically called on Unity trigger collision, but can optionally be called manually.
    /// </summary>
    /// <param name="hurtObject">The hurtObject to hit.</param>
    public void hitHurtObject(HurtObject hurtObject) {
        if (!enabled) return;
        if (hurtObject == null) return;
        if (!hurtObject.enabled) return;

        // don't hit if record contains hurtObject's ID (which means it was hit recently)
        if (hurtRecordContains(hurtObject.globalHurtID)) return;

        // add ID to hurt record
        if (hurtRecordAutoClear > 0) {
            hurtRecordAdd(hurtObject.globalHurtID);
        }

        AttackInfo ai = new AttackInfo();
        ai.hitObject = this;
        ai.hurtObject = hurtObject;
        setAttackInfo(ai);

        if (dealsDamage == null) {
            hurtObject.receiveDamage(ai);
        } else {
            dealsDamage.dealDamage(hurtObject, ai);
        }

    }

    #region Can Be Overridden

    /// <summary>
    /// Called when about to deal damage.  Passes an AttackInfo by reference to be filled with information about the attack (such as damage).
    /// Can be overridden.
    /// </summary>
    /// <param name="ai">Passed in AttackInfo.  The hitObject and hurtObject properties are already set.</param>
    protected virtual void setAttackInfo(AttackInfo ai) {
        ai.damage = damage;
        if (setHeadingFromPositions &&
            ai.hurtObject != null && ai.hitObject != null) {
            ai.heading = M.atan2(ai.hurtObject.position - ai.hitObject.position) * Mathf.Rad2Deg;
        } else {
            ai.heading = heading;
        }
        ai.magnitude = magnitude;
    }

    #endregion

    #region Unity Events

    protected void Awake() {
        // try to find DealsDamage component in gameObject and parent's gameObject.
        dealsDamage = GetComponent<DealsDamage>();
        if (dealsDamage == null) {
            dealsDamage = GetComponentInParent<DealsDamage>();
        }
    }
    
    protected void OnTriggerStay2D(Collider2D c2d) {
        if (c2d == null || c2d.gameObject == null) return;
        HurtObject hurtObject = c2d.gameObject.GetComponent<HurtObject>();
        hitHurtObject(hurtObject);
    }

    protected void OnTriggerExit2D(Collider2D c2d) {
        if (c2d == null || c2d.gameObject == null) return;
        HurtObject hurtObject = c2d.gameObject.GetComponent<HurtObject>();
        if (hurtObject == null) return;

        hurtRecordRemove(hurtObject.globalHurtID);
    }

    protected void OnEnable() {
        hurtRecordClear();
    }

    protected void LateUpdate() {
        // remove hurt records
        hurtRecordRemoveOlderThan(hurtRecordAutoClear);
    }

    #endregion

    #region Private

    List<HurtRecord> _hurtRecords = new List<HurtRecord>();

    struct HurtRecord {
        public int hurtObjectGlobalID;
        public float time;
    }

    #endregion
    
}
