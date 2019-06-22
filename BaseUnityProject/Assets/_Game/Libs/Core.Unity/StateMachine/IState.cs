using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.StateMachine {

    /// <summary>
    /// Interface to be implemented by <see cref="BaseState{TOwner, TStateMachine, TStateID}"/>.
    /// </summary>
    public interface IState {

        void Initialize(IStateMachine stateMachine);

        /// <summary>
        /// Called immediately after all states in the state machine are registered and the owner is available.
        /// This will only be called once for each state.
        /// </summary>
        void OnRegistered();

        /// <summary>
        /// Called when the state machine has just switched to this state.
        /// </summary>
        void OnBegin();

        void Update();

        void FixedUpdate();

        void OnEnd();
    }
}