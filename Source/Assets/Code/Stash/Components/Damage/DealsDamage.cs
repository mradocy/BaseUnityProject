using UnityEngine;
using System.Collections;

public class DealsDamage : MonoBehaviour {

    /*// Sends messages:
    
    void PreDealDamage(AttackInfo ai) { }
    
    void OnDealDamage(AttackInfo ai) { }

    *//////////////////////////////////
    
    public void dealDamage(ReceivesDamage receivesDamage, AttackInfo ai) {

        SendMessage("PreDealDamage", ai, SendMessageOptions.DontRequireReceiver);

        receivesDamage.receiveDamage(ai);

        SendMessage("OnDealDamage", ai, SendMessageOptions.DontRequireReceiver);

    }

    public void dealDamage(HurtObject hurtObject, AttackInfo ai) {

        SendMessage("PreDealDamage", ai, SendMessageOptions.DontRequireReceiver);

        hurtObject.receiveDamage(ai);

        SendMessage("OnDealDamage", ai, SendMessageOptions.DontRequireReceiver);

    }

}
