using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.RewiredExtensions;
using Rewired;

namespace Core.Unity.RewiredExtensions {

    /// <summary>
    /// Abstract monobehavior that detects when a controller is connected or disconnected.
    /// Will be automatically set to not be destroyed on load.
    /// </summary>
    public abstract class ControllerListenerBase : MonoBehaviour {

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static ControllerListenerBase Instance { get; private set; }

        /// <summary>
        /// Called when a controller is connected.
        /// </summary>
        /// <param name="args">Args</param>
        protected abstract void OnControllerConnected(ControllerStatusChangedEventArgs args);

        /// <summary>
        /// Called when a controller is disconnected.
        /// </summary>
        /// <param name="args">Args</param>
        protected abstract void OnControllerDisconnected(ControllerStatusChangedEventArgs args);

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected void Awake() {
            if (Instance != null) {
                Debug.LogError("There can only be one ControllerListener");
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            ReInput.ControllerConnectedEvent += this.OnControllerConnected;
            ReInput.ControllerDisconnectedEvent += this.OnControllerDisconnected;
        }

        /// <summary>
        /// Called by Unity when this object is destroyed.
        /// </summary>
        protected void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }

            ReInput.ControllerConnectedEvent -= this.OnControllerConnected;
            ReInput.ControllerDisconnectedEvent -= this.OnControllerDisconnected;
        }
    }
}