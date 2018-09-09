using UnityEngine;
using System.Collections;

public class SendMessagesToTransform : MonoBehaviour {

    public bool onTriggerEnter2D = true;
    public bool onTriggerStay2D = true;
    public bool onTriggerExit2D = true;

    [Tooltip("Set by Prefab/Scene")]
    public Transform targetTransform = null;

    void OnTriggerEnter2D(Collider2D c2d) {
        if (!enabled) return;
        if (!onTriggerEnter2D) return;

        targetTransform.SendMessage("OnTriggerEnter2D", c2d, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerStay2D(Collider2D c2d) {
        if (!enabled) return;
        if (!onTriggerStay2D) return;

        targetTransform.SendMessage("OnTriggerStay2D", c2d, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit2D(Collider2D c2d) {
        if (!enabled) return;
        if (!onTriggerExit2D) return;

        targetTransform.SendMessage("OnTriggerExit2D", c2d, SendMessageOptions.DontRequireReceiver);
    }

}
