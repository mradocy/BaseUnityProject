using UnityEngine;
using System.Collections;


namespace TestStateMachine2 {

    #region State ID's

    public enum ID {
        NONE,

        // *** Define state ID's here ***
        TEST_STATE_1,
        TEST_STATE_2,

    }

    #endregion

    public class TestStateMachine2 : MonoBehaviour {

        #region State Declaration and State Inspector Properties

        // *** Declare states here.  Their properties will be exposed to the inspector as long as they're marked as [System.Serializable]. ***
        public TestState1 testState1;
        public TestState2 testState2;

        #endregion

        #region Unity Messages

        private void Awake() {

            // *** register states here ***
            registerState(ID.TEST_STATE_1, testState1);
            registerState(ID.TEST_STATE_2, testState2);

        }

        private void Start() {

            // Can optionally start off with a state here (currently state == null)
            changeState(ID.TEST_STATE_1);

        }

        private void Update() {

            // state update
            if (state != null) {
                state.update();
            }

        }

        private void FixedUpdate() {

            // state update
            if (state != null) {
                state.fixedUpdate();
            }

        }

        private void OnDestroy() {

            // destroying all states
            changeState(ID.NONE);
            foreach (ID stateID in statesDictionary.Keys) {
                statesDictionary[stateID].onDestroy();
            }
            statesDictionary.Clear();

        }

        #endregion

        #region State Functions (do not modify)

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public BaseState state { get; private set; }

        /// <summary>
        /// Gets the ID of the current state.  Is ID.NONE if the current state is null.
        /// </summary>
        public ID stateID {
            get {
                if (state == null) return ID.NONE;
                return state.id;
            }
        }

        /// <summary>
        /// Gets the previous state.
        /// </summary>
        public BaseState previousState { get; private set; }

        /// <summary>
        /// Gets the ID of the previous state.  Is ID.NONE if the current state is null.
        /// </summary>
        public ID previousStateID {
            get {
                if (previousState == null) return ID.NONE;
                return previousState.id;
            }
        }

        /// <summary>
        /// Gets the state with the given ID.  Returns null if the given id is ID.NONE.
        /// </summary>
        public BaseState getState(ID stateID) {
            if (stateID == ID.NONE) return null;

            BaseState ret = null;
            bool found = statesDictionary.TryGetValue(stateID, out ret);
            if (!found) { Debug.LogError("State " + stateID + " has not been registered."); return null; }
            return ret;
        }
        /// <summary>
        /// Gets the instance of the given state class.
        /// </summary>
        public T getState<T>() where T : BaseState {
            foreach (BaseState state in statesDictionary.Values) {
                if (state is T)
                    return state as T;
            }
            Debug.LogError("State has not been registered.");
            return null;
        }

        /// <summary>
        /// Changes state.  Does not do anything if the given state is the same as the current state.
        /// </summary>
        /// <param name="stateID">ID of the state to change to.  Can be ID.NONE for no current state.</param>
        public void changeState(ID stateID) {
            if (this.stateID == stateID) return;
            if (state != null)
                state.onEnd();
            previousState = state;
            state = getState(stateID);
            if (state != null)
                state.onBegin();
        }

        /// <summary>
        /// Changes state.  Does not do anything if the given state is the same as the current state.
        /// </summary>
        /// <param name="state">The state to change to.  Can be null no current state.</param>
        public void changeState(BaseState state) {
            changeState(state == null ? ID.NONE : state.id);
        }

        /// <summary>
        /// Adds a state to the dictionary.  Should be called during Awake().
        /// </summary>
        protected void registerState(ID id, BaseState state) {
            if (id == ID.NONE) { Debug.LogError("Cannot register state with id as ID.NONE"); return; }
            if (statesDictionary.ContainsKey(id)) { Debug.LogError("State ID " + id + " has already been registered"); return; }
            if (state == null) { Debug.LogError("State ID " + id + " cannot be null."); return; }

            state.testStateMachine2 = this;
            state.id = id;

            statesDictionary[id] = state;
        }
        private System.Collections.Generic.Dictionary<ID, BaseState> statesDictionary = new System.Collections.Generic.Dictionary<ID, BaseState>();

        #endregion

    }

    #region Base State

    public class BaseState {

        public ID id {
            get { return _id; }
            set {
                if (_id != ID.NONE) {
                    Debug.LogError("id cannot be set.");
                    return;
                }
                _id = value;
            }
        }
        public TestStateMachine2 testStateMachine2 {
            get { return _testStateMachine2; }
            set {
                if (_testStateMachine2 != null) {
                    Debug.LogError("testStateMachine2 cannot be set.");
                    return;
                }
                _testStateMachine2 = value;
            }
        }

        public virtual void onBegin() { }
        public virtual void update() { }
        public virtual void fixedUpdate() { }
        public virtual void onEnd() { }
        public virtual void onDestroy() { }

        /// <summary>
        /// Gets the state with the given ID.  Returns null if the given id is ID.NONE.
        /// </summary>
        public BaseState getState(ID stateID) {
            return testStateMachine2.getState(stateID);
        }
        /// <summary>
        /// Gets the instance of the given state class.
        /// </summary>
        public T getState<T>() where T : BaseState {
            return testStateMachine2.getState<T>();
        }

        /// <summary>
        /// Changes state.  Does not do anything if the given state is the same as the current state.
        /// </summary>
        /// <param name="stateID">ID of the state to change to.  Can be ID.NONE for no current state.</param>
        public void changeState(ID stateID) {
            testStateMachine2.changeState(stateID);
        }

        public override string ToString() {
            return string.Format("[testStateMachine2={0}, id={1}]", testStateMachine2, id);
        }

        private ID _id = ID.NONE;
        private TestStateMachine2 _testStateMachine2 = null;
    }

    #endregion

}
