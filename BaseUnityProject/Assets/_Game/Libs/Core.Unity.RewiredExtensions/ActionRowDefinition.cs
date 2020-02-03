using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.RewiredExtensions {

    // TODO: Get rid of this

    /// <summary>
    /// Serializable defintion for an action map row that would be seen in a control customization menu.
    /// </summary>
    [System.Serializable]
    public class ActionRowDefinition {

        [Tooltip("Name of the action")]
        public string ActionName = null;

        [Tooltip("Axis direction (only applies to axis actions)")]
        public Pole AxisDirection = Pole.Positive;

        [Tooltip("Prevent Joystick Assign")]
        public bool PreventJoystickAssign = false;

        [Tooltip("Prevent Keyboard Assign")]
        public bool PreventKeyboardAssign = false;
    }
}