using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Rewired;

namespace Core.Unity.RewiredExtensions {

    /// <summary>
    /// Prints the element index for joystick button and axis presses.  Used for debugging
    /// </summary>
    public class DebugLogJoystickElement : MonoBehaviour {

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {
            
            if (RewiredUtils.GetAnyJoystickButtonPressed(out ControllerPollingInfo buttonPollingInfo)) {
                Debug.Log($"Button pressed: elementIdentifierName: {buttonPollingInfo.elementIdentifierName}, elementIndex: {buttonPollingInfo.elementIndex}");
            }

            if (RewiredUtils.GetAnyJoystickAxisPressed(0.5f, out int elementIndex, out bool positive, out string elementName)) {
                Debug.Log($"Axis pressed: elementName: {elementName}, elementIndex: {elementIndex}, positive: {positive}");
            }

        }
    }
}