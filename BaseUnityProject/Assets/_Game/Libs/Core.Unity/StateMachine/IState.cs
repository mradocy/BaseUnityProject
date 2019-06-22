using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.StateMachine {

    /// <summary>
    /// Interface to be implemented by <see cref="BaseState{TOwner, TStateMachine, TStateID}"/>.
    /// </summary>
    public interface IState {

        void Initialize(IStateMachine stateMachine);

        void OnBegin();

        void Update();

        void FixedUpdate();

        void OnEnd();
    }
}