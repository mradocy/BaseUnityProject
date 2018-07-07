using System.Collections;
using UnityEngine;

namespace TestStateMachine2 {

    [System.Serializable]
    public class TestState2 : BaseState {

        public KeyCode toState1Key;

        public override void onBegin() {
            Debug.Log("began state 2");
        }

        public override void update() {

            if (UDeb.keyPressed(toState1Key)) {
                changeState(ID.TEST_STATE_1);
            }

        }

        public override void fixedUpdate() { }

        public override void onEnd() { }

    }

}
