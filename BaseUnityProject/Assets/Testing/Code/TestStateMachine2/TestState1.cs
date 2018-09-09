using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestStateMachine2 {

    [System.Serializable]
    public class TestState1 : BaseState {

        public float prop1;
        public int prop2;
        [LongLabel]
        public KeyCode toState2Key;

        public override void onBegin() {
            Debug.Log("begin state 1");
        }

        public override void update() {
            
            if (UDeb.keyPressed(toState2Key)) {
                changeState(testStateMachine2.testState2.id);
            }

        }

        public override void fixedUpdate() {
            
        }

        public override void onEnd() {
            Debug.Log("The end");
        }

        public override void onDestroy() {
            Debug.Log("Destroyed");
        }

    }

}