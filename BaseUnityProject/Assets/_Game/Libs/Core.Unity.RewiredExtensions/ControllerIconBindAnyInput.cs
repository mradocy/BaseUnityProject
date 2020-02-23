using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace Core.Unity.RewiredExtensions {

    /// <summary>
    /// Binds the icon displayed in a sibling <see cref="IControllerIcon"/> to whatever input the user last submitted.
    /// </summary>
    [RequireComponent(typeof(IControllerIcon))]
    public class ControllerIconBindAnyInput : MonoBehaviour {

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            _controllerIcon = this.GetComponent<IControllerIcon>();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {
            KeyCode key;
            if (RewiredUtils.GetAnyKeyboardButtonPressed(out key)) {
                _controllerIcon.SetKeyboardIcon(key);
            }
            int elementIndex;
            if (RewiredUtils.GetAnyJoystickButtonPressed(out elementIndex)) {
                _controllerIcon.SetJoystickIcon(RewiredUtils.GetFirstJoystickStyle().ID, ControllerElementType.Button, elementIndex, Pole.Positive);
            }
            int axisElementIndex;
            bool positive;
            if (RewiredUtils.GetAnyJoystickAxisPressed(0.5f, out axisElementIndex, out positive)) {
                _controllerIcon.SetJoystickIcon(RewiredUtils.GetFirstJoystickStyle().ID, ControllerElementType.Axis, axisElementIndex, positive ? Pole.Positive : Pole.Negative);
            }
        }

        /// <summary>
        /// Reference to sibling controller icon.
        /// </summary>
        private IControllerIcon _controllerIcon;
    }
}