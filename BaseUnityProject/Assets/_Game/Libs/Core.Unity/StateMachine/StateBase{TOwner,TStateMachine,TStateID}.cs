using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.StateMachine {

    /// <summary>
    /// Abstract base class of a state.
    /// </summary>
    /// <typeparam name="TOwner">Type of the owner of the state machine.</typeparam>
    /// <typeparam name="TStateMachine">Type of the state machine.</typeparam>
    /// <typeparam name="TStateId">The enum containing the ids representing the states.  Should be based off an int.</typeparam>
    [System.Serializable]
    public abstract class StateBase<TOwner, TStateMachine, TStateId> : IState
        where TOwner : MonoBehaviour
        where TStateId : System.Enum
        where TStateMachine : StateMachineBase<TOwner, TStateId> {

        #region Properties

        /// <summary>
        /// Gets the id of this state.
        /// </summary>
        public abstract TStateId Id { get; }

        /// <summary>
        /// Reference to the owner of the state machine.
        /// </summary>
        public TOwner Owner { get { return _owner; } }

        /// <summary>
        /// Reference to the state machine this state belongs to.
        /// </summary>
        public TStateMachine StateMachine { get { return _stateMachine; } }

        /// <summary>
        /// Gets if this state is the current state in the state machine.
        /// </summary>
        public bool IsCurrentState { get { return _stateMachine.CurrentStateObject == this; } }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Called immediately after all states in the state machine are registered and the owner is available.
        /// This will only be called once for each state.
        /// </summary>
        protected virtual void OnRegistered() { }

        /// <summary>
        /// Called when the state machine switches to this state.  This will only be called when changing to a different state.
        /// </summary>
        protected virtual void OnBegin() { }
        
        /// <summary>
        /// Called by the <see cref="StateMachine"/> during Update() method when this is the current state.
        /// </summary>
        protected virtual void Update() { }

        /// <summary>
        /// Called by the <see cref="StateMachine"/> during FixedUpdate() method when this is the current state.
        /// </summary>
        protected virtual void FixedUpdate() { }

        /// <summary>
        /// Called by the <see cref="StateMachine"/> during LateUpdate() method when this is the current state.
        /// </summary>
        protected virtual void LateUpdate() { }

        /// <summary>
        /// Called when the state machine switches away from this state to a different state.
        /// </summary>
        protected virtual void OnEnd() { }

        #endregion

        #region IState Implementation

        void IState.OnRegistered() {
            this.OnRegistered();
        }

        void IState.OnBegin() {
            this.OnBegin();
        }
        void IState.Update() {
            this.Update();
        }

        void IState.FixedUpdate() {
            this.FixedUpdate();
        }

        void IState.LateUpdate() {
            this.LateUpdate();
        }

        void IState.OnEnd() {
            this.OnEnd();
        }

        void IState.Initialize(IStateMachine stateMachine) {
            if (_initialized) {
                throw new System.Exception("State cannot be initialized twice.");
            }
            if (stateMachine == null) {
                throw new System.ArgumentNullException(nameof(stateMachine));
            }
            _stateMachine = stateMachine as TStateMachine;
            if (this.StateMachine == null) {
                throw new System.ArgumentException($"This state must expect a state machine of type \"{stateMachine.GetType()}\".");
            }
            _owner = _stateMachine.Owner;
            if (_owner == null) {
                throw new System.ArgumentException($"State machine's owner must not be null.");
            }

            _initialized = true;
        }

        #endregion

        #region Private

        [System.NonSerialized]
        private bool _initialized = false;
        [System.NonSerialized]
        private TStateMachine _stateMachine;
        [System.NonSerialized]
        private TOwner _owner;

        #endregion
    }
}