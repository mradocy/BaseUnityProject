using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.StateMachine {

    /// <summary>
    /// Abstract base class of a state.
    /// </summary>
    /// <typeparam name="TOwner">Type of the owner of the state machine.</typeparam>
    /// <typeparam name="TStateMachine">Type of the state machine.</typeparam>
    /// <typeparam name="TStateID">The enum containing the ids representing the states.  Should be based off an int.</typeparam>
    [System.Serializable]
    public abstract class BaseState<TOwner, TStateMachine, TStateID> : IState
        where TOwner : MonoBehaviour
        where TStateID : System.Enum
        where TStateMachine : BaseStateMachine<TOwner, TStateID> {

        #region Properties

        /// <summary>
        /// ID of this state.
        /// </summary>
        public TStateID ID { get { return this._id; } }

        /// <summary>
        /// Reference to the owner of the state machine.
        /// </summary>
        public TOwner Owner { get { return this._owner; } }

        /// <summary>
        /// Reference to the state machine this state belongs to.
        /// </summary>
        public TStateMachine StateMachine { get { return this._stateMachine; } }

        #endregion

        #region Protected Methods to be Overridden

        /// <summary>
        /// Protected constructor.  Should be extended by a parameterless constructor that gives this state's ID.
        /// </summary>
        /// <param name="ID">The id of this state.</param>
        protected BaseState(TStateID ID) {
            this._id = ID;
        }

        protected abstract void OnBegin();
        
        protected abstract void Update();

        protected abstract void FixedUpdate();

        protected abstract void OnEnd();

        #endregion

        #region IState Implementation

        void IState.OnBegin() {
            this.OnBegin();
        }
        void IState.Update() {
            this.Update();
        }

        void IState.FixedUpdate() {
            this.FixedUpdate();
        }

        void IState.OnEnd() {
            this.OnEnd();
        }

        void IState.Initialize(IStateMachine stateMachine) {
            if (this._initialized) {
                throw new System.Exception("State cannot be initialized twice.");
            }
            if (stateMachine == null) {
                throw new System.ArgumentNullException(nameof(stateMachine));
            }
            this._stateMachine = stateMachine as TStateMachine;
            if (this.StateMachine == null) {
                throw new System.ArgumentException($"This state must expect a state machine of type \"{stateMachine.GetType()}\".");
            }
            this._owner = this._stateMachine.Owner;
            if (this._owner == null) {
                throw new System.ArgumentException($"State machine's owner must not be null.");
            }

            this._initialized = true;
        }

        #endregion

        #region Private

        [System.NonSerialized]
        private bool _initialized = false;
        [System.NonSerialized]
        private TStateID _id;
        [System.NonSerialized]
        private TStateMachine _stateMachine;
        [System.NonSerialized]
        private TOwner _owner;

        #endregion
    }
}