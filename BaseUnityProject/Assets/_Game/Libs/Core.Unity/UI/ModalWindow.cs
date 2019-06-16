﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity.UI {
    /// <summary>
    /// - ModalWindow is a base class to be extended by UI components.
    /// - ModalWindows are disabled by default.  Only by launching a ModalWindow will it be enabled.
    /// - All functionality of the extend class must be disabled if the component is disabled.
    /// - Only one ModalWindow can be enabled at a time.  If a new ModalWindow is launched, the current ModalWindow (if it exists) will be disabled.
    /// - Once launched, a ModalWindow must eventually be closed by calling Close().  This will enable the previous ModalWindow (if it exists).
    /// </summary>
    public class ModalWindow : MonoBehaviour {

        #region Public Static

        /// <summary>
        /// The <see cref="ModalWindow"/> that's currently enabled.
        /// Only one <see cref="ModalWindow"/> can be enabled at a time.
        /// </summary>
        public static ModalWindow CurrentWindow {
            get {
                if (_windowStack.Count == 0)
                    return null;
                return _windowStack[_windowStack.Count - 1];
            }
        }

        /// <summary>
        /// Launches the given <see cref="ModalWindow"/>.
        /// </summary>
        /// <param name="window">The window to launch.  Must currently not be launched.</param>
        /// <param name="closeCallbackFunction">Function to call when the launched window closes.</param>
        public static void Launch(ModalWindow window, UnityAction<ModalWindowCallbackArgs> closeCallbackFunction) {
            // error checking
            if (window == null) {
                throw new System.ArgumentNullException(nameof(window));
            }
            window.VerifyEnabled();
            if (window.enabled) {
                Debug.LogError("The modal window being launched must not already be enabled.");
                return;
            }
            if (window.IsLaunched) {
                Debug.LogError("The window being launched must not already be launched.");
                return;
            }
            if (_windowStack.Contains(window)) {
                Debug.LogError("The modal window attempting to be launched is already in the window stack.");
                return;
            }

            // disable current modal window
            if (CurrentWindow != null) {
                CurrentWindow.enabled = false;
            }

            // launch created window
            window.IsLaunched = true;
            window._closeCallbackFunction = closeCallbackFunction;
            _windowStack.Add(window);
            window.enabled = true;
            window.OnLaunch();
        }

        /// <summary>
        /// Instantiates a new GameObject that contains a <see cref="ModalWindow"/> component,
        /// then launches it.
        /// </summary>
        /// <typeparam name="T">Type of the GameObject's component (must extend <see cref="ModalWindow"/>)</typeparam>
        /// <param name="prefab">The original GameObject to instantiate from.</param>
        /// <param name="transformParent">The Transform to set as the new GameObject's parent.</param>
        /// <param name="closeCallbackFunction">Function to call when the launched window closes.</param>
        /// <returns>Created window.</returns>
        public static T CreateAndLaunch<T>(GameObject prefab, Transform transformParent, UnityAction<ModalWindowCallbackArgs> closeCallbackFunction) where T : ModalWindow {
            if (prefab == null) {
                throw new System.ArgumentNullException(nameof(prefab));
            }

            // create modal window
            GameObject childGO = GameObject.Instantiate(prefab, transformParent);
            T window = childGO.GetComponent<T>();
            if (window == null) {
                throw new System.ArgumentException($"Prefab {prefab} must contain a component of the given type {typeof(T).Name}.");
            }

            // launch window
            Launch(window, closeCallbackFunction);

            return window;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets if this <see cref="ModalWindow"/> was launched.
        /// </summary>
        public bool IsLaunched { get; private set; }

        /// <summary>
        /// Gets if this window should be destroyed when it closes.  The default is true.
        /// Not destroting the window on close may be useful for recycling. 
        /// </summary>
        public bool DestroyOnClose { get; set; } = true;

        #endregion

        #region Methods

        /// <summary>
        /// Closes the window.  Can only be called if this is the <see cref="CurrentWindow"/>.
        /// </summary>
        /// <param name="closeArgs"></param>
        public void Close(ModalWindowCallbackArgs closeArgs) {
            this.VerifyEnabled();
            if (CurrentWindow != this) {
                Debug.LogError("This modal window must be the current window to be closed.");
                return;
            }
            this.enabled = false;
            this.IsLaunched = false;

            // pop off stack
            _windowStack.RemoveAt(_windowStack.Count - 1);
            if (CurrentWindow != null) {
                CurrentWindow.enabled = true;
            }

            // call callback function
            this._closeCallbackFunction?.Invoke(closeArgs);

            // destroy
            if (this.DestroyOnClose) {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Closes the window, with <see cref="ModalWindowCallbackArgs"/> created from the given result.
        /// </summary>
        /// <param name="result">Result of closing the window.</param>
        public void Close(ModalWindowResult result) {
            ModalWindowCallbackArgs closeArgs = new ModalWindowCallbackArgs() {
                Result = result
            };

            this.Close(closeArgs);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Function called when this window gets launched.  Meant to be overridden.
        /// </summary>
        protected virtual void OnLaunch() { }

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake() {
            // Modal windows are disabled at first.  They become enabled when launched.
            this.enabled = false;
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update() {
            this.VerifyEnabled();
        }

        /// <summary>
        /// Called by Unity when the script instance is being destroyed.
        /// </summary>
        protected virtual void OnDestroy() {
            if (this.IsLaunched) {
                Debug.LogWarning("A modal window should be closed before being destroyed.");
            }
            _windowStack.Remove(this); // failsafe
        }

        /// <summary>
        /// Verifies that this <see cref="ModalWindow"/> is enabled iff it's the <see cref="CurrentWindow"/>.
        /// Returns false if verify failed.
        /// </summary>
        /// <returns>False if verify failed, true otherwise.</returns>
        protected bool VerifyEnabled() {
            if (CurrentWindow == this && !this.enabled) {
                Debug.LogError("This modal window is the current window, so it should be enabled");
                this.enabled = true;
                return false;
            }
            if (CurrentWindow != this && this.enabled) {
                Debug.LogError("This modal window is not the current window, so it should not be enabled");
                this.enabled = false;
                return false;
            }
            return true;
        }

        #endregion

        #region Private

        private static List<ModalWindow> _windowStack = new List<ModalWindow>();

        private UnityAction<ModalWindowCallbackArgs> _closeCallbackFunction = null;

        #endregion
    }
}