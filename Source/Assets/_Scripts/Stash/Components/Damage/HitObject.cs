using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Deals damage to HurtObjects on touch.
/// Doesn't do anything if disabled.
/// Will not deal damage to HurtObjects that are disabled.
/// Will not deal damage to HurtObjects that have been added to the hurt record.  
///     Every hurtBox is added to the record on collision.
///     HurtObjects are automatically removed from the record on an OnTriggerExit2D event (which may not be reliable).
///     Can manually clear the hurt record by calling hurtRecordClear().
///     Hurt records that are older than hurtRecordAutoRemoveDuration will be automatically removed.
///     The hurt record system can be disabled by setting hurtRecordAutoRemoveDuration to 0.
/// setAttackInfo(AttackInfo ai) can be overridden to tweak the damage dealt.
/// </summary>
public class HitObject : MonoBehaviour {

    /*// Sends messages:
    
    // Will be called when dealDamage is called, just before damage is subtracted from health.
    // Use PreReceiveDamage to alter the given AttackInfo, changing some info of the attack.
    // Do not save a reference to the given AttackInfo, as it will be recycled shortly.
    void PreDealDamage(AttackInfo ai) { }

    // Will be called when dealDamage is called, just after damage is subtracted from health.
    // Use OnDealDamage to affect the GameObject as a result of dealing damage.
    // Do not save a reference to the given AttackInfo, as it will be recycled shortly.
    void OnDealDamage(AttackInfo ai) { }

    Message order:
    PreDealDamage -> PreReceiveDamage -> (damage dealt) -> OnReceiveDamage -> OnDealDamage

    *//////////////////////////////////

    #region Inspector Properties
    
    [Tooltip("Damage dealt on hit.")]
    public float damage = 1;

    [Tooltip("How the heading of the AttackInfo is determined.\nMANUAL - Defined by manualHeading.\nPOSITION - Determined by relative positions between hitObject and hurtObject.")]
    public HeadingSetMode headingSetMode = HeadingSetMode.POSITION;
    [Tooltip("The heading of the AttackInfo, if headingSetMode is set to MANUAL.")]
    public float manualHeading = 0;

    [LongLabel]
    [Tooltip("HurtObjects will automatically be removed from the hurt record after this duration.  Set to 0 for HurtObjects to never be added to the record.")]
    public float hurtRecordMaxDuration = 9999;

    [LongLabel]
    [Tooltip("If the PreDealDamage and OnDealDamage messages should be sent upwards.")]
    public bool sendDamageMessagesUpwards = false;

    public enum HeadingSetMode {
        MANUAL,
        POSITION
    }

    #endregion
    
    /// <summary>
    /// The position of the hitObject.  Can be used to determine the heading of an attack.
    /// </summary>
    public virtual Vector2 position {
        get {
            return transform.position;
        }
        set {
            transform.position = value;
        }
    }
    
    #region Hurt Records

    /// <summary>
    /// If the hurt record system is enabled.
    /// When false, hurt records cannot be added.
    /// </summary>
    public bool hurtRecordEnabled {
        get {
            return hurtRecordMaxDuration > 0;
        }
    }

    /// <summary>
    /// Adds a HurtObject to the record of objects hit by this attack.  Collisions between this HitObject and HurtObjects in the record will not be considered.
    /// </summary>
    /// <param name="hurtObjectID">ID of the HurtObject (given by hurtObject.globalHurtID)</param>
    public void hurtRecordAdd(int hurtObjectID) {
        if (!hurtRecordEnabled) return;
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
    /// Deals damage to the given hurtObject.  This is automatically called on Unity trigger collision.
    /// This can also be called manually to deal damage to a HurtObject without having to collide with it first.
    /// </summary>
    /// <param name="hurtObject">The hurtObject to hit.</param>
    public void hitHurtObject(HurtObject hurtObject) {
        if (!isActiveAndEnabled) return;
        if (hurtObject == null) return;
        if (!hurtObject.isActiveAndEnabled) return;

        // don't hit if record contains hurtObject's ID (which means it was hit recently)
        if (hurtRecordContains(hurtObject.globalHurtID)) return;

        // add ID to hurt record
        hurtRecordAdd(hurtObject.globalHurtID);

        // creating AttackInfo, setting with setAttackInfo().
        AttackInfo ai = AttackInfo.createNew();
        ai.hitObject = this;
        ai.hurtObject = hurtObject;
        setAttackInfo(ai);

        // sending PreDealDamage message
        if (sendDamageMessagesUpwards) {
            SendMessageUpwards("PreDealDamage", ai, SendMessageOptions.DontRequireReceiver);
        } else {
            SendMessage("PreDealDamage", ai, SendMessageOptions.DontRequireReceiver);
        }
        
        // dealing damage to hurtObject
        hurtObject.receiveDamage(ai);

        // sending OnDealDamage message
        if (sendDamageMessagesUpwards) {
            SendMessageUpwards("OnDealDamage", ai, SendMessageOptions.DontRequireReceiver);
        } else {
            SendMessage("OnDealDamage", ai, SendMessageOptions.DontRequireReceiver);
        }

        // recycling AttackInfo
        AttackInfo.recycle(ai);
    }
    
    /// <summary>
    /// Called when about to deal damage.  Passes an AttackInfo by reference to be filled with information about the attack (such as damage).
    /// Can be overridden.
    /// </summary>
    /// <param name="ai">Passed in AttackInfo.  The hitObject and hurtObject properties are already set.</param>
    protected virtual void setAttackInfo(AttackInfo ai) {
        ai.damage = damage;

        Vector2 diff = new Vector2();
        switch (headingSetMode) {
        case HeadingSetMode.MANUAL:
            ai.heading = manualHeading;
            break;
        case HeadingSetMode.POSITION:
            diff = ai.hurtObject.position - ai.hitObject.position;
            ai.heading = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            break;
        }
        
    }
    
    #region Unity Events
    
    /// <summary>
    /// Called by Unity every time this object hits a collider.
    /// If the collider hit has a HurtObject as a sibling component, hitHurtObject() is called on it.
    /// </summary>
    protected virtual void OnTriggerStay2D(Collider2D c2d) {
        if (c2d == null || c2d.gameObject == null) return;
        HurtObject hurtObject = c2d.gameObject.GetComponent<HurtObject>();
        hitHurtObject(hurtObject);
    }

    /// <summary>
    /// Called by Unity when this object stops hitting a collider.
    /// If the collider is siblings with a HurtObject, that hurtObject is removed from the hurt record.
    /// </summary>
    /// <param name="c2d"></param>
    protected virtual void OnTriggerExit2D(Collider2D c2d) {
        if (c2d == null || c2d.gameObject == null) return;
        HurtObject hurtObject = c2d.gameObject.GetComponent<HurtObject>();
        if (hurtObject == null) return;

        hurtRecordRemove(hurtObject.globalHurtID);
    }

    protected virtual void OnEnable() {
        hurtRecordClear();
    }

    protected virtual void LateUpdate() {
        // auto remove old hurt records
        hurtRecordRemoveOlderThan(hurtRecordMaxDuration);
    }

    protected virtual void OnDestroy() {
        hurtRecordClear();
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
