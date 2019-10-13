using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.RewiredExtensions {

    /// <summary>
    /// Input method enum.
    /// </summary>
    public enum InputMethod {
        /// <summary>
        /// Value matches Rewired.ControllerType.Keyboard
        /// </summary>
        Keyboard = 0,

        /// <summary>
        /// Value matches Rewired.ControllerType.Joystick
        /// </summary>
        Joystick = 2,
    }
}