using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.UI {

    /// <summary>
    /// Enum describing the action for a modal window to take on being closed.
    /// </summary>
    public enum ModalWindowCloseAction {
        /// <summary>
        /// Nothing happens to the gameObject.
        /// </summary>
        None,
        /// <summary>
        /// The window's gameObject is set to be inactive.
        /// </summary>
        SetInactive,
        /// <summary>
        /// The window's gameObject is destroyed.
        /// </summary>
        Destroy,
    }
}