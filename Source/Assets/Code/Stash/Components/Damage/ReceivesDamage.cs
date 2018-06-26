using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Component for anything with health and can receive damage.
/// To deal damage, call receiveDamage().  Anything can call receiveDamage(), not just the DealsDamage component.
/// </summary>
public class ReceivesDamage : MonoBehaviour {

    /*// Sends messages:
    
    // Will be called when dealDamage is called, just before damage is subtracted from health.
    // Use PreDamage to change some info of the attack.
    void PreReceiveDamage(AttackInfo ai) { }

    // Will be called when dealDamage is called, just after damage is subtracted from health.
    // Use OnDamage to affect the GameObject as a result of taking damage.
    void OnReceiveDamage(AttackInfo ai) { }

    *//////////////////////////////////

    #region Inspector Properties
    
    [Tooltip("Maximum health of this object.  Is also the starting health.")]
    public float maxHealth = 10;
    [Tooltip("How long object is mercy invincible after being dealt damage.  Set to 0 to not use this mechanic.")]
    public float mercyInvincibleDuration = 0;
    [Tooltip("If health should be loaded OnCheckpointLoad().")]
    public bool checkpointLoadHealth = true;

    #endregion

    /// <summary>
    /// Current health.
    /// </summary>
    public float health { get; set; }

    /// <summary>
    /// Object won't receive damage (or receive damage messages) when mercy invincible.
    /// </summary>
    public bool mercyInvincible { get; private set; }

    /// <summary>
    /// How long object has been mercy invincible for.  Mercy invincibility ends after this value surpasses mercyInvincibleDuration.  Can be set directly to prolong or shorten mercy invincibility time.
    /// </summary>
    public float mercyInvincibleTime { get; set; }

    /// <summary>
    /// Receives damage, sending PreReceiveDamage and OnReceiveDamage messages.  The given AttackInfo may change based on how much damage was actually dealt and other conditions.
    /// </summary>
    /// <param name="ai">AttackInfo describing the damage taken.</param>
    public void receiveDamage(AttackInfo ai) {
        
        if (mercyInvincible) {
            ai.damage = 0;
            ai.result = AttackInfo.Result.MERCY_INVINCIBLE;
            return;
        }
        
        SendMessage("PreReceiveDamage", ai, SendMessageOptions.DontRequireReceiver);
        
        if (ai.damage <= 0) {
            return;
        }

        health = Mathf.Max(0, health - ai.damage);

        SendMessage("OnReceiveDamage", ai, SendMessageOptions.DontRequireReceiver);

        if (mercyInvincibleDuration > 0) {
            startMercyInvincibility();
        }

    }

    /// <summary>
    /// Simpler version of receiveDamage(AttackInfo ai).  The AttackInfo is automatically created from the given damage value.
    /// </summary>
    /// <param name="damage">How much damage is dealt.</param>
    public void receiveDamage(float damage) {
        AttackInfo ai = new AttackInfo();
        ai.damage = damage;
        receiveDamage(ai);
    }

    /// <summary>
    /// Manually starts mercy invincibility without taking damage.  This is automatically called when taking damage.
    /// </summary>
    public void startMercyInvincibility() {
        mercyInvincibleTime = 0;
        mercyInvincible = true;
    }

    #region Unity Events

    void Awake() {
        health = maxHealth;
    }

    void Update() {

        if (mercyInvincible) {
            mercyInvincibleTime += Time.deltaTime;
            if (mercyInvincibleTime >= mercyInvincibleDuration) {
                mercyInvincible = false;
            }
        }

    }

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
        mercyInvincible = false;
    }

    #endregion


}
