using UnityEngine;
using System.Collections;

public class TestShake : MonoBehaviour {
    
    void Awake() {
        
	}
    
    void Update() {

        if (UDeb.num1Pressed) {
            CameraControl.instance.shake(new Vector2(.02f, .1f), new Vector2(13f, 10f), 1);
        }
        if (UDeb.num2Pressed) {
            CameraControl.instance.shakeStop();
        }

    }

}
