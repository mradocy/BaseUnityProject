﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.StateMachine {

    /// <summary>
    /// Abstract base class of a state machine.
    /// </summary>
    /// <typeparam name="TOwner">Type of the <see cref="MonoBehaviour"/> owner of this state machine.</typeparam>
    /// <typeparam name="TStateId">The enum containing the ids representing the states.  Should be based off an int.</typeparam>
    [System.Serializable]
    public abstract class StateMachineBase<TOwner, TStateId> : IStateMachine
        where TOwner : MonoBehaviour
        where TStateId : System.Enum {

        #region Properties

        /// <summary>
        /// The <see cref="MonoBehaviour"/> that owns this state machine.
        /// </summary>
        public TOwner Owner { get { return _owner; } }

        /// <summary>
        /// The id of the current state.
        /// </summary>
        public TStateId CurrentState { get { return _currentState; } }

        /// <summary>
        /// The <see cref="IState"/> object of the current state.
        /// </summary>
        public IState CurrentStateObject { get { return _currentStateObject; } }

        /// <summary>
        /// Gets the id of the null state (equivalent to 0)
        /// </summary>
        public TStateId NullStateId { get { return default; } }

        #endregion

        #region Methods to be called by the Owner

        /// <summary>
        /// To be called by the owner in the Awake() method.  Sets this state machine's owner and registers the states.
        /// </summary>
        /// <param name="owner">Reference to the owner of this state machine.</param>
        /// <param name="initialState">The state to start with.</param>
        public void Awake(TOwner owner, TStateId initialState) {
            if (_awakeCalled) {
                Debug.LogError($"Cannot call {nameof(this.Awake)}() more than once.");
                return;
            }
            _awakeCalled = true;

            _owner = owner;
            this.RegisterStates();
            foreach (IState state in _states.Values) {
                state.OnRegistered();
            }
            this.ChangeState(initialState);
        }

        /// <summary>
        /// To be called by the owner in the Update() method.  Calls Update() of the current state.
        /// </summary>
        public void Update() {
            _currentStateObject?.Update();
        }

        /// <summary>
        /// To be called by the owner in the FixedUpdate() method.  Calls FixedUpdate() of the current state.
        /// </summary>
        public void FixedUpdate() {
            _currentStateObject?.FixedUpdate();
        }

        /// <summary>
        /// To be called by the owner in the LateUpdate() method.  Calls LateUpdate() of the current state.
        /// </summary>
        public void LateUpdate() {
            _currentStateObject?.LateUpdate();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the state with the given ID.
        /// The state ID with integer value 0 will return null.
        /// </summary>
        /// <param name="stateID">State ID</param>
        /// <returns>state</returns>
        public IState GetStateObject(TStateId stateID) {
            if (stateID.Equals(this.NullStateId)) {
                return null;
            }
            IState state;
            if (_states.TryGetValue(stateID, out state)) {
                return state;
            }

            if (!_awakeCalled) {
                Debug.LogError($"State machine cannot be used until {nameof(this.Awake)}() has been called.");
                return null;
            }
            Debug.LogError($"State \"{stateID}\" could not be found.  Has it been registered?");
            return null;
        }

        /// <summary>
        /// Changes state.  Nothing happens if the given state is equal to the current state.
        /// <para/>
        /// Returns if the state has changed.
        /// <see cref="IState.OnEnd()"/> of the current state is called and <see cref="IState.OnBegin"/> of the new state is called.
        /// </summary>
        /// <param name="state">The id of the state to change to.</param>
        /// <returns>If the state has changed.</returns>
        public bool ChangeState(TStateId state) {
            if (this.CurrentState.Equals(state)) {
                return false;
            }
            _currentStateObject?.OnEnd();
            _currentState = state;
            _currentStateObject = this.GetStateObject(this.CurrentState);
            _currentStateObject?.OnBegin();
            return true;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Registers the states of this state machine.  To be overridden.
        /// </summary>
        protected abstract void RegisterStates();

        /// <summary>
        /// To be called by a derived class's implementation of <see cref="RegisterStates()"/>.
        /// </summary>
        /// <param name="state">The state to register</param>
        protected void Register(IState state) {
            if (state == null) {
                throw new System.ArgumentNullException(nameof(state));
            }
            if (state.IdInt == 0) {
                throw new System.ArgumentException($"Cannot register state because its integer id value is 0.  This is reserved for the null state.", nameof(state));
            }

            TStateId id = (TStateId)(object)state.IdInt;
            if (_states.ContainsKey(id)) {
                throw new System.ArgumentException($"State with id \"{id}\" has already been registered.", nameof(state));
            }

            state.Initialize(this);

            _states.Add(id, state);
        }

        /// <summary>
        /// To be called by a derived class's implementation of <see cref="RegisterStates()"/>.  Registers several states.
        /// </summary>
        /// <param name="states">The states to register</param>
        protected void Register(params IState[] states) {
            foreach (IState state in states) {
                this.Register(state);
            }
        }

        #endregion

        #region Private

        [System.NonSerialized]
        private TOwner _owner = null;

        [System.NonSerialized]
        private TStateId _currentState;

        [System.NonSerialized]
        private IState _currentStateObject;

        [System.NonSerialized]
        private Dictionary<TStateId, IState> _states = new Dictionary<TStateId, IState>();

        [System.NonSerialized]
        private bool _awakeCalled = false;

        #endregion
    }
}