using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using UnityEngine.Events;

namespace Core.Unity {

    /// <summary>
    /// Represents a GameObject that instantiates itself and doesn't get destroyed.
    /// </summary>
    public static class PersistantGameObject {

        #region Events

        /// <summary>
        /// Event invoked during the Update() step in the default time.
        /// </summary>
        public static event UnityAction UpdateEvent;

        /// <summary>
        /// Event invoked during the FixedUpdate() step in the default time.
        /// </summary>
        public static event UnityAction FixedUpdateEvent;

        /// <summary>
        /// Event invoked during the OnGUI() step in the default time.
        /// </summary>
        public static event UnityAction OnGUIEvent;

        #endregion

        #region Methods

        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Registers event function to be called when <see cref="PersistantGameObject"/> initializes.  Cleanup is done automatically.
        /// If <see cref="PersistantGameObject"/> has already initialized, the function is called immediately.
        /// </summary>
        /// <param name="callback">Function to call on initialize.</param>
        public static void CallWhenInitialized(UnityAction callback) {
            if (callback == null) {
                throw new System.ArgumentNullException();
            }
            if (IsInitialized) {
                callback();
                return;
            }

            _callbacks.Add(callback);
        }

        public static T AddComponent<T>() where T : Component {
            if (!IsInitialized) {
                Debug.LogError("Cannot add component until initialized");
                return null;
            }
            return _gameObject.AddComponent<T>();
        }

        public static void AddComponentWhenInitialized<T>() where T : Component {
            CallWhenInitialized(() => AddComponent<T>());
        }

        #endregion

        #region Setup to Receive Unity messages

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            Initialize();
        }

        private static void Initialize() {
            IsInitialized = true;

            _gameObject = new GameObject("PersistantGameObject");
            Object.DontDestroyOnLoad(_gameObject);
            _mb = _gameObject.AddComponent<MB>();

            // call callback functions
            foreach (UnityAction callback in _callbacks) {
                callback();
            }
            _callbacks.Clear();
        }

        
        private class MB : MonoBehaviour {
            private void Update() {
                UpdateEvent?.Invoke();
            }
            private void FixedUpdate() {
                FixedUpdateEvent?.Invoke();
            }
            private void OnGUI() {
                OnGUIEvent?.Invoke();
            }
            private void OnDestroy() {
                if (_mb == this)
                    _mb = null;
            }
        }

        #endregion

        #region Private

        private static GameObject _gameObject = null;
        private static MB _mb = null;
        private static List<UnityAction> _callbacks = new List<UnityAction>();

        #endregion
    }
}