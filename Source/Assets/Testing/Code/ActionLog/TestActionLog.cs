using UnityEngine;
using System.Collections;
using System.IO;

public class TestActionLog : MonoBehaviour {
    
    void Start() {
        
    }

    void Awake() {
        
	}
    
    void Update() {

        if (Input.GetKeyDown(KeyCode.Home)) {
            ActionLog.startLog();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ActionLog.logEntry("1 pressed");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ActionLog.logEntry("2 pressed");
        }

        if (Input.GetKeyDown(KeyCode.End)) {
            ActionLog.endLog();
        }


    }

}
