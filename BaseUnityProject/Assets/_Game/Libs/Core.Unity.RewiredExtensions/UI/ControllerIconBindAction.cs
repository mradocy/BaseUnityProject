using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Rewired;

namespace Core.Unity.RewiredExtensions.UI {

    /// <summary>
    /// Binds the icon displayed in a sibling <see cref="IControllerIcon"/> to an action, updated to whatever is the input method currently being used.
    /// </summary>
    [RequireComponent(typeof(IControllerIcon))]
    public class ControllerIconBindAction : MonoBehaviour {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("The id of the Rewired action to bind.")]
        private int _actionId = 0;

        [SerializeField]
        [Tooltip("The direction of the action, if it's an axis action.")]
        private Pole _axisDirection = Pole.Positive;

        [SerializeField]
        [Tooltip("If this bind also sets the icon's IsPressed property.")]
        private bool _setsIsPressed = false;

        #endregion

        #region Properties

        /// <summary>
        /// The id of the Rewired action to bind.
        /// </summary>
        public int ActionId {
            get { return _actionId; }
            set {
                if (_actionId == value)
                    return;

                _actionId = value;
                _inputAction = null;
            }
        }

        /// <summary>
        /// The direction of the action, if it's an axis action.
        /// </summary>
        public Pole AxisDirection {
            get { return _axisDirection; }
            set { _axisDirection = value; }
        }

        #endregion

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

            // get input action
            if (_inputAction == null) {
                _inputAction = RewiredUtils.GetAction(this.ActionId);
            }
            if (_inputAction == null) {
                Debug.LogError($"Rewired action id \"{this.ActionId}\" not recognized.");
                return;
            }

            // update icon
            if (RewiredUtils.IsJoystickLastActiveController) {
                // joystick is current input method
                System.Guid joystickGuid = RewiredUtils.GetPrimaryJoystickGuid().Value;
                ActionElementMap joystickMap = RewiredUtils.GetFirstJoystickActionElementMap(_inputAction, this.AxisDirection);
                JoystickStyleID joystickStyleId = RewiredUtils.GetJoystickStyle(joystickGuid).ID;
                ControllerElementType elementType = joystickMap?.elementType ?? ControllerElementType.Axis;
                int elementIndex = joystickMap?.elementIndex ?? 0;

                _controllerIcon.SetJoystickIcon(joystickStyleId, elementType, elementIndex, this.AxisDirection);
            } else {
                // keyboard is current input method
                ActionElementMap keyboardMap = RewiredUtils.GetFirstKeyboardActionElementMap(_inputAction, this.AxisDirection);
                KeyCode keyCode = keyboardMap?.keyCode ?? KeyCode.None;

                _controllerIcon.SetKeyboardIcon(keyCode);
            }

            if (_setsIsPressed) {
                if (_inputAction.type == InputActionType.Button) {
                    _controllerIcon.IsPressed = RewiredUtils.GetHeld(this.ActionId);
                } else {
                    _controllerIcon.IsPressed = RewiredUtils.GetAxisHeld(this.ActionId, this.AxisDirection == Pole.Positive, 0.5f);
                }
            }
        }

        /// <summary>
        /// Reference to sibling controller icon.
        /// </summary>
        private IControllerIcon _controllerIcon;
        /// <summary>
        /// Cached input action.
        /// </summary>
        private InputAction _inputAction = null;
    }
}