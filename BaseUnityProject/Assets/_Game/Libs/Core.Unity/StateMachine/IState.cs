using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.StateMachine {

    /// <summary>
    /// Interface to be implemented by <see cref="StateBase{TOwner, TStateMachine, TStateId}"/>.
    /// </summary>
    public interface IState {

        /// <summary>
        /// Gets the id of the state as an int.
        /// </summary>
        int IdInt { get; }

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

        void LateUpdate();

        void OnEnd();
    }
}