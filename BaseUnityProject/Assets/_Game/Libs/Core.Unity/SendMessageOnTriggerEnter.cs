using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.Attributes;

namespace Core.Unity {

    /// <summary>
    /// Sends a message to a GameObject when an object enters the attached trigger.
    /// </summary>
    public class SendMessageOnTriggerEnter : MonoBehaviour {

        #region Inspector Fields

        [SerializeField]
        private GameObject _gameObject = null;

        [SerializeField]
        private string _message = null;

        [SerializeField]
        private string _param = null;

        [SerializeField, LongLabel]
        private bool _onlyIActivatesGameTriggers = true;

        [SerializeField, LongLabel]
        private bool _onlyOnce = true;

        #endregion

        /// <summary>
        /// Gets if the message was sent at some point.
        /// </summary>
        public bool WasTriggered { get; private set; }

        /// <summary>
        /// Sent when another object enters a trigger collider attached to this object.
        /// </summary>
        /// <param name="c2d">The collider that entered the trigger.</param>
        private void OnTriggerEnter2D(Collider2D c2d) {
            if (_gameObject == null)
                return;
            if (_onlyIActivatesGameTriggers && c2d.gameObject.GetComponent<IActivatesGameTriggers>() == null)
                return;
            if (_onlyOnce && this.WasTriggered)
                return;

            this.WasTriggered = true;

            if (string.IsNullOrEmpty(_param)) {
                _gameObject.SendMessage(_message, SendMessageOptions.RequireReceiver);
            } else {
                _gameObject.SendMessage(_message, _param, SendMessageOptions.RequireReceiver);
            }
        }
    }
}