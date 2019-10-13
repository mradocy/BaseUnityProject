using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rewired;
using Core.Unity.RewiredExtensions.JoystickStyles;

namespace Core.Unity.RewiredExtensions {

    /// <summary>
    /// Collection of static utility functions for interacting with Rewired.
    /// </summary>
    public static class RewiredUtils {

        #region Initialization

        /// <summary>
        /// Initializes joystick styles, if it has not been done already.
        /// </summary>
        private static void InitializeJoystickStyles() {
            if (_genericJoystickStyle != null)
                return;

            _genericJoystickStyle = new GenericJoystickStyle();
            _joystickStyles = new List<IJoystickStyle>() {
                _genericJoystickStyle,
                new XBox360JoystickStyle(),
            };
        }

        #endregion

        /// <summary>
        /// Gets the main <see cref="Rewired.Player"/>.
        /// </summary>
        public static Rewired.Player Player {
            get {
                if (_player == null) {
                    _player = ReInput.players.GetPlayer(0);
                }
                return _player;
            }
        }

        /// <summary>
        /// Gets the axis value of an axis action.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <returns>Axis value.</returns>
        public static float GetAxis(int actionId) {
            return Player.GetAxis(actionId);
        }

        /// <summary>
        /// Gets if the given button action was pressed this frame.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <returns>Pressed</returns>
        public static bool GetPressed(int actionId) {
            return Player.GetButtonDown(actionId);
        }

        /// <summary>
        /// Gets if the given button action is currently being held.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <returns>Held</returns>
        public static bool GetHeld(int actionId) {
            return Player.GetButton(actionId);
        }

        /// <summary>
        /// Gets if the given button action was released this frame.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <returns>Released</returns>
        public static bool GetReleased(int actionId) {
            return Player.GetButtonUp(actionId);
        }

        /// <summary>
        /// Gets keyboard map.
        /// </summary>
        /// <returns>Keyboard map.</returns>
        public static KeyboardMap GetKeyboardMap() {
            if (_keyboardMap == null) {
                _keyboardMap = new KeyboardMap();
            }

            return _keyboardMap;
        }

        /// <summary>
        /// Gets a joystick style from its ID.  Returns the generic style if not found.
        /// </summary>
        /// <param name="joystickStyleID">ID of the joystick style.</param>
        /// <returns>Joystick style</returns>
        public static IJoystickStyle GetJoystickStyle(JoystickStyleID joystickStyleID) {
            InitializeJoystickStyles();

            IJoystickStyle joystickStyle = _joystickStyles.FirstOrDefault(js => js.ID == joystickStyleID);
            if (joystickStyle == null) {
                Debug.LogError($"Joystick style with ID {joystickStyleID} could not be found.");
                return _genericJoystickStyle;
            }

            return joystickStyle;
        }

        /// <summary>
        /// Gets a joystick style from its Rewired hardware guid.  Returns the generic style if not found.
        /// </summary>
        /// <param name="rewiredHardwareGuid">Guid corresponding to the controller's hardware, as specified by Rewired.</param>
        /// <returns>Joystick style</returns>
        public static IJoystickStyle GetJoystickStyle(System.Guid rewiredHardwareGuid) {
            InitializeJoystickStyles();

            IJoystickStyle joystickStyle = _joystickStyles.FirstOrDefault(js => js.RewiredHardwareGuid == rewiredHardwareGuid);
            if (joystickStyle == null) {
                Debug.LogWarning($"Could not find joystick with rewired hardware guid {rewiredHardwareGuid}.");
                return _genericJoystickStyle;
            }

            return joystickStyle;
        }

        /// <summary>
        /// Gets the input action from the given action ID.
        /// </summary>
        /// <param name="actionID">Action ID</param>
        /// <returns>InputAction</returns>
        public static InputAction GetAction(int actionID) {
            return ReInput.mapping.GetAction(actionID);
        }

        /// <summary>
        /// Gets the input action from the given action name.
        /// </summary>
        /// <param name="actionName">Action ID</param>
        /// <returns>InputAction</returns>
        public static InputAction GetAction(string actionName) {
            return ReInput.mapping.GetAction(actionName);
        }

        /// <summary>
        /// Gets the id of the input action with the given name.
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <returns>Id</returns>
        public static int GetActionId(string actionName) {
            return ReInput.mapping.GetActionId(actionName);
        }

        /// <summary>
        /// Gets the first keyboard <see cref="ActionElementMap"/> for the given <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The input action.</param>
        /// <param name="axisDirection">The direction of the action, if it's an axis action.</param>
        /// <returns>keyboard ActionElementMap</returns>
        public static ActionElementMap GetFirstKeyboardActionElementMap(InputAction action, Pole axisDirection) {
            ActionElementMap keyboardMap = null;
            if (action.type == InputActionType.Button) {
                keyboardMap = Player.controllers.maps.GetFirstButtonMapWithAction(ControllerType.Keyboard, action.id, true);
            } else if (action.type == InputActionType.Axis) {
                List<ActionElementMap> maps = new List<ActionElementMap>();
                Player.controllers.maps.GetButtonMapsWithAction(ControllerType.Keyboard, action.id, true, maps);
                foreach (ActionElementMap map in maps) {

                    // TODO: Test invert?

                    if (map.axisContribution == axisDirection) {
                        keyboardMap = map;
                        break;
                    }
                }
            }

            return keyboardMap;
        }

        /// <summary>
        /// Gets a list of keyboard <see cref="ActionElementMap"/>s for the given <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The input action.</param>
        /// <param name="axisDirection">The direction of the action, if it's an axis action.</param>
        /// <returns>List of ActionElementMaps</returns>
        public static IList<ActionElementMap> GetKeyboardActionElementMaps(InputAction action, Pole axisDirection) {
            IEnumerable<ActionElementMap> keyboardMaps = Player.controllers.maps.ElementMapsWithAction(ControllerType.Keyboard, action.id, true);
            if (action.type == InputActionType.Axis) {
                // Each controller mapper row represents a button action, or an axis action in a specific direction.
                // So if a keyboard map represents the id of an axis action but not the direction specified in this row, it should not be included
                keyboardMaps = keyboardMaps.Where(km => km.elementType == ControllerElementType.Axis || km.axisContribution == axisDirection);
            }

            // sort to put axis maps before button maps
            return keyboardMaps.OrderBy(km => km.elementIdentifierId).ToList();
        }

        /// <summary>
        /// Gets the first joystick <see cref="ActionElementMap"/> for the given <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The input action.</param>
        /// <param name="axisDirection">The direction of the action, if it's an axis action.</param>
        /// <returns>joystick ActionElementMap</returns>
        public static ActionElementMap GetFirstJoystickActionElementMap(InputAction action, Pole axisDirection) {

            // TODO: better way to get first

            return GetJoystickActionElementMaps(action, axisDirection).FirstOrDefault();
        }

        /// <summary>
        /// Gets list of joystick <see cref="ActionElementMap"/>s for the given <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The input action.</param>
        /// <param name="axisDirection">The direction of the action, if it's an axis action.</param>
        /// <returns>List of ActionElementMaps</returns>
        public static IList<ActionElementMap> GetJoystickActionElementMaps(InputAction action, Pole axisDirection) {

            IEnumerable<ActionElementMap> joystickMaps = Player.controllers.maps.ElementMapsWithAction(ControllerType.Joystick, action.id, true);
            if (action.type == InputActionType.Axis) {
                // Each controller mapper row represents a button action, or an axis action in a specific direction.
                // So if a joystick map represents the id of an axis action but not the direction specified in this row, it should not be included
                joystickMaps = joystickMaps.Where(jm => jm.elementType == ControllerElementType.Axis || jm.axisContribution == axisDirection);
            }

            // sort to put axis maps before button maps
            return joystickMaps.OrderBy(jm => jm.elementIdentifierId).ToList();
        }

        /// <summary>
        /// Gets if a joystick is currently connected.
        /// </summary>
        public static bool IsJoystickConnected {
            get { return Player.controllers.joystickCount > 0; }
        }

        /// <summary>
        /// Gets the Rewired hardware guid of the first listed connected joystick.  Returns null if no joysticks are connected.
        /// </summary>
        /// <returns>Guid?</returns>
        public static System.Guid? GetFirstJoystickGuid() {
            return Player.controllers.Joysticks.FirstOrDefault()?.hardwareTypeGuid;
        }


        #region Private

        /// <summary>
        /// Cache of <see cref="Player"/>.
        /// </summary>
        private static Rewired.Player _player = null;

        /// <summary>
        /// Keyboard map.
        /// </summary>
        private static KeyboardMap _keyboardMap = null;

        /// <summary>
        /// Joystick style for the generic controller.
        /// </summary>
        private static IJoystickStyle _genericJoystickStyle = null;
        /// <summary>
        /// List of joystick styles.
        /// </summary>
        private static List<IJoystickStyle> _joystickStyles = null;

        #endregion
    }
}