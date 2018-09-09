using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackInfo {
    // it's important that this can be passed by reference

    public float damage = 0;
    /// <summary>
    /// Direction of the attack, in degrees.
    /// </summary>
    public float heading = 0;
    public HitObject hitObject = null;
    public HurtObject hurtObject = null;

    public string message = "";
    
    /// <summary>
    /// ToString() override.
    /// </summary>
    public override string ToString() {
        return "AttackInfo damage: " + damage + " message: " + message;
    }
    
    /// <summary>
    /// If attack's heading is pointed to the right.
    /// </summary>
    public bool toRight {
        get {
            float in360 = M.wrap360(heading);
            return in360 < 90 || in360 > 270;
        }
    }


    /// <summary>
    /// Resets all values of this AttackInfo.
    /// </summary>
    public void resetValues() {
        damage = 0;
        heading = 0;
        hitObject = null;
        hurtObject = null;
        message = "";
    }


    /// <summary>
    /// Private constructor, forces createNew() to be used instead.
    /// </summary>
    private AttackInfo() { }

    /// <summary>
    /// Creates a new AttackInfo, using a recycled AttackInfo if available.
    /// </summary>
    /// <returns></returns>
    public static AttackInfo createNew() {
        AttackInfo ai = null;
        if (recycledAttackInfos.Count > 0) {
            ai = recycledAttackInfos.Pop();
            ai.resetValues();
            ai.recycled = false;
        } else {
            ai = new AttackInfo();
        }
        return ai;
    }
    /// <summary>
    /// Recycles an AttackInfo to be reused later.  Does nothing if the given AttackInfo is currently recycled.
    /// </summary>
    /// <param name="ai"></param>
    public static void recycle(AttackInfo ai) {
        if (ai.recycled) return;
        ai.recycled = true;
        recycledAttackInfos.Push(ai);
    }
    /// <summary>
    /// Clears all recycled AttackInfos.
    /// </summary>
    public static void clearAllRecycled() {
        recycledAttackInfos.Clear();
    }
    private static Stack<AttackInfo> recycledAttackInfos = new Stack<AttackInfo>();
    private bool recycled = false;
    
}


