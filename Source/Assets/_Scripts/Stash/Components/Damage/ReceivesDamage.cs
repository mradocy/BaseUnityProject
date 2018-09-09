using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Component for anything with health and can receive damage.
/// To deal damage, call receiveDamage().  Anything can call receiveDamage(), not just the DealsDamage component.
/// receiveDamage() does nothing if this component isn't active and enabled.
/// </summary>
public class ReceivesDamage : MonoBehaviour {

    /*// Sends messages:
    
    // Will be called when receiveDamage is called, just before damage is subtracted from health.
    // Use PreReceiveDamage to alter the given AttackInfo, changing some info of the attack.
    // Do not save a reference to the given AttackInfo, as it will be recycled shortly.
    void PreReceiveDamage(AttackInfo ai) { }

    // Will be called when receiveDamage is called, just after damage is subtracted from health.
    // Use OnReceiveDamage to affect the GameObject as a result of taking damage.
    // Do not save a reference to the given AttackInfo, as it will be recycled shortly.
    void OnReceiveDamage(AttackInfo ai) { }

    *//////////////////////////////////

    #region Inspector Properties

    [Tooltip("Maximum health of this object.  Is also the starting health.")]
    public float maxHealth = 10;
    [Tooltip("If health should be loaded OnCheckpointLoad().")]
    public bool checkpointLoadHealth = true;

    #endregion

    /// <summary>
    /// Current health.
    /// Can be set manually to avoid sending messages.
    /// </summary>
    public float health { get; set; }
    
    /// <summary>
    /// Receives damage, sending PreReceiveDamage and OnReceiveDamage messages.  The given AttackInfo may change based on how much damage was actually dealt and other conditions.
    /// </summary>
    /// <param name="ai">AttackInfo describing the damage taken.</param>
    public void receiveDamage(AttackInfo ai) {

        if (!isActiveAndEnabled) return;
        
        SendMessage("PreReceiveDamage", ai, SendMessageOptions.DontRequireReceiver);
        
        health = Mathf.Max(0, health - ai.damage);

        SendMessage("OnReceiveDamage", ai, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Simpler version of receiveDamage(AttackInfo ai).  The AttackInfo is automatically created from the given damage value.
    /// </summary>
    /// <param name="damage">How much damage is dealt.</param>
    public void receiveDamage(float damage) {
        AttackInfo ai = AttackInfo.createNew();
        ai.damage = damage;
        receiveDamage(ai);
        AttackInfo.recycle(ai);
    }
    
    #region Unity Events

    void Awake() {
        health = maxHealth;
    }

    void Update() { }

    #endregion

    #region Checkpoint saving/reloading

    float cpHealth = 0;
    float cpMaxHealth = 0;
    void OnCheckpointSave() {
        cpHealth = health;
        cpMaxHealth = maxHealth;
    }
    
    void OnCheckpointLoad() {
        if (checkpointLoadHealth) {
            health = cpHealth;
        }
        maxHealth = cpMaxHealth;
    }

    #endregion


}
