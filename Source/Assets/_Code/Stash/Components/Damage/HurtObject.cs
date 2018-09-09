using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Connects to the parent's ReceivesDamage.
/// </summary>
public class HurtObject : MonoBehaviour {

    /// <summary>
    /// The position of the hurtObject.  Can be used to determine the heading of an attack.
    /// </summary>
    public virtual Vector2 position {
        get {
            return transform.position;
        }
    }
    
    /// <summary>
    /// Calls dealDamage() of parent ReceivesDamage.  Doesn't do anything if disabled.
    /// </summary>
    /// <param name="ai">AttackInfo describing the attack.</param>
    public void receiveDamage(AttackInfo ai) {
        if (!isActiveAndEnabled) {
            ai.damage = 0;
            return;
        }

        ai.hurtObject = this;

        if (receivesDamage == null) {
            ai.damage = 0;
        } else {
            setAttackInfo(ai);
            receivesDamage.receiveDamage(ai);
        }

    }

    /// <summary>
    /// ID assigned to a HurtObject.  No HurtObject has the same globalHurtID.
    /// </summary>
    public int globalHurtID { get; private set; }

    #region Can Be Overridden

    /// <summary>
    /// Called when about to receive damage.  Passes an AttackInfo by reference to be filled with information about the attack (such as damage).
    /// Can be overridden.
    /// </summary>
    /// <param name="ai">Passed in AttackInfo.  The hitObject and hurtObject properties are already set.</param>
    protected virtual void setAttackInfo(AttackInfo ai) {
        
    }

    #endregion

    void Awake() {
        receivesDamage = GetComponent<ReceivesDamage>();
        if (receivesDamage == null) {
            receivesDamage = GetComponentInParent<ReceivesDamage>();
        }
        if (receivesDamage == null) {
            Debug.LogError("Niether HurtObject's gameObject nor its parent have a ReceivesDamage component.");
        }
        
        // assign unique ID
        globalHurtID = globalIDCounter;
        globalIDCounter++;

        // add this to static list of all hurt objects
        allHurtObjects.Add(globalHurtID, this);

    }

    void Update() {

    }

    void OnDestroy() {
        // remove from static list of all hurt objects
        allHurtObjects.Remove(globalHurtID);
    }

    public ReceivesDamage receivesDamage { get; private set; }

    /// <summary>
    /// Gets a hurtObject from its globalHurtID.  Returns null if the hurtObject doesn't exist (e.g. it was destroyed).
    /// </summary>
    /// <param name="globalID">The hurtObject's globalHurtID.</param>
    public static HurtObject getHurtObject(int globalHurtID) {
        if (!allHurtObjects.ContainsKey(globalHurtID)) return null;
        return allHurtObjects[globalHurtID];
    }

    private static Dictionary<int, HurtObject> allHurtObjects = new Dictionary<int, HurtObject>();


    private static int globalIDCounter = 0;

}
