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
                new DS4JoystickStyle(),
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
        /// Gets if the given axis was "pressed" this frame.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">If the axis was in the positive direction (false for negative direction).</param>
        /// <param name="axisThreshold">Axis value that would consider a value "pressed" (e.g. 0.5)</param>
        /// <returns>Pressed</returns>
        public static bool GetAxisPressed(int actionId, bool positive, float axisThreshold) {
            if (positive) {
                axisThreshold = Mathf.Abs(axisThreshold);
                return Player.GetAxis(actionId) >= axisThreshold && Player.GetAxisPrev(actionId) < axisThreshold;
            } else {
                axisThreshold = -Mathf.Abs(axisThreshold);
                return Player.GetAxis(actionId) <= axisThreshold && Player.GetAxisPrev(actionId) > axisThreshold;
            }
        }

        /// <summary>
        /// Gets if the given axis was "held" this frame.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">If the axis was in the positive direction (false for negative direction).</param>
        /// <param name="axisThreshold">Axis value that would consider a value "held" (e.g. 0.5)</param>
        /// <returns>Pressed</returns>
        public static bool GetAxisHeld(int actionId, bool positive, float axisThreshold) {
            if (positive) {
                axisThreshold = Mathf.Abs(axisThreshold);
                return Player.GetAxis(actionId) >= axisThreshold;
            } else {
                axisThreshold = -Mathf.Abs(axisThreshold);
                return Player.GetAxis(actionId) <= axisThreshold;
            }
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
        /// Gets if any keyboard button has been pressed this frame.  If so, the key pressed is outed as <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key pressed.</param>
        /// <returns>Any key pressed.</returns>
        public static bool GetAnyKeyboardButtonPressed(out KeyCode key) {
            key = KeyCode.None;
            IEnumerable<ControllerPollingInfo> pollingInfos = Player.controllers.polling.PollAllControllersOfTypeForAllButtonsDown(ControllerType.Keyboard);
            if (!pollingInfos.Any()) {
                return false;
            }

            ControllerPollingInfo pollingInfo = pollingInfos.First();
            key = pollingInfo.keyboardKey;
            return true;
        }

        /// <summary>
        /// Gets if any joystick button has been pressed this frame.  If so, the element index of the button pressed is outed as <paramref name="elementIndex"/>.
        /// </summary>
        /// <param name="elementIndex">The element index of the button pressed.</param>
        /// <returns>Any joystick button pressed.</returns>
        public static bool GetAnyJoystickButtonPressed(out int elementIndex) {
            if (GetAnyJoystickButtonPressed(out ControllerPollingInfo pollingInfo)) {
                elementIndex = pollingInfo.elementIndex;
                return true;
            }

            elementIndex = 0;
            return false;
        }

        /// <summary>
        /// Gets if any joystick button has been pressed this frame.  If so, the polling info of the button pressed is outed as <paramref name="pollingInfo"/>.
        /// </summary>
        /// <returns>Any joystick button pressed.</returns>
        public static bool GetAnyJoystickButtonPressed(out ControllerPollingInfo pollingInfo) {
            IEnumerable<ControllerPollingInfo> pollingInfos = Player.controllers.polling.PollAllControllersOfTypeForAllButtonsDown(ControllerType.Joystick);
            if (!pollingInfos.Any()) {
                pollingInfo = new ControllerPollingInfo();
                return false;
            }

            pollingInfo = pollingInfos.First();
            return true;
        }

        /// <summary>
        /// Gets if any joystick axis was "pressed" this frame.  If so, the element index of the axis pressed is outed as <paramref name="elementIndex"/> and the direction is outed as <paramref name="positive"/>.
        /// </summary>
        /// <param name="axisThreshold">Axis value that would consider a value "pressed" (e.g. 0.5)</param>
        /// <param name="elementIndex">The element index of the axis pressed.</param>
        /// <param name="positive">If the axis was in the positive direction (false for negative direction).</param>
        /// <returns>Any joystick axis "pressed".</returns>
        /// <remarks>Stupidly there's no way to detected if an axis is "pressed" with rewired controller polling.</remarks>
        public static bool GetAnyJoystickAxisPressed(float axisThreshold, out int elementIndex, out bool positive, out string elementName) {
            axisThreshold = Mathf.Abs(axisThreshold);

            foreach (Joystick joystick in Player.controllers.Joysticks) {
                foreach (ControllerElementIdentifier axisIdentifier in joystick.AxisElementIdentifiers) {
                    int axisIdentifierId = axisIdentifier.id;
                    float prevAxis = joystick.GetAxisPrevById(axisIdentifierId);
                    float axis = joystick.GetAxisById(axisIdentifierId);
                    if (axis >= axisThreshold && prevAxis < axisThreshold) {
                        positive = true;
                        elementIndex = axisIdentifierId;
                        elementName = axisIdentifier.positiveName;
                        return true;
                    }
                    if (axis <= -axisThreshold && prevAxis > -axisThreshold) {
                        positive = false;
                        elementIndex = axisIdentifierId;
                        elementName = axisIdentifier.negativeName;
                        return true;
                    }
                }
            }

            elementIndex = 0;
            positive = false;
            elementName = null;
            return false;
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
        /// Gets a <see cref="IJoystickStyle"/> from its ID.  Returns the generic style if not found.
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
        /// Gets a <see cref="IJoystickStyle"/> from its Rewired hardware guid.  Returns the generic style if not found.
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
        /// Gets a <see cref="IJoystickStyle"/> from a rewired <see cref="Joystick"/>.
        /// </summary>
        public static IJoystickStyle GetJoystickStyle(Joystick joystick) {
            if (joystick == null)
                return null;

            return GetJoystickStyle(joystick.hardwareTypeGuid);
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
        /// Gets the first keyboard <see cref="ActionElementMap"/> for the given <paramref name="action"/>, or null if no map exists.
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

            return keyboardMaps.ToList();
        }

        /// <summary>
        /// Gets the first joystick <see cref="ActionElementMap"/> for the given <paramref name="action"/>, or null if no map exists.
        /// </summary>
        /// <param name="action">The input action.</param>
        /// <param name="axisDirection">The direction of the action, if it's an axis action.</param>
        /// <returns>joystick ActionElementMap</returns>
        public static ActionElementMap GetFirstJoystickActionElementMap(InputAction action, Pole axisDirection) {
            IList<ActionElementMap> maps = GetJoystickActionElementMaps(action, axisDirection);

            // simple cases
            if (maps.Count == 0)
                return null;
            if (maps.Count == 1)
                return maps[0];

            // should only be at most 2 maps per action-direction
            if (maps.Count > 2) {
                Debug.LogWarning($"More then 2 maps for action {action.descriptiveName}, {axisDirection}");
            }

            // for consistency, list buttons before axes
            ActionElementMap firstButtonMap = maps.FirstOrDefault(m => m.elementType == ControllerElementType.Button);
            if (firstButtonMap != null)
                return firstButtonMap;

            return maps.First();
        }

        /// <summary>
        /// Gets the second joystick <see cref="ActionElementMap"/> for the given <paramref name="action"/>, or null if an alt map doesn't exist.
        /// </summary>
        /// <param name="action">The input action.</param>
        /// <param name="axisDirection">The direction of the action, if it's an axis action.</param>
        /// <returns>joystick ActionElementMap</returns>
        public static ActionElementMap GetAltJoystickActionElementMap(InputAction action, Pole axisDirection) {
            IList<ActionElementMap> maps = GetJoystickActionElementMaps(action, axisDirection);

            // simple case
            if (maps.Count <= 1)
                return null;

            // should only be at most 2 maps per action-direction
            if (maps.Count > 2) {
                Debug.LogWarning($"More then 2 maps for action {action.descriptiveName}, {axisDirection}");
            }

            // consider first map
            if (maps[0] == GetFirstJoystickActionElementMap(action, axisDirection)) {
                return maps[1];
            } else {
                return maps[0];
            }
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
                joystickMaps = joystickMaps.Where(jm =>
                    jm.axisContribution == axisDirection ||
                    (jm.elementType == ControllerElementType.Axis && (jm.axisRange == AxisRange.Full || jm.axisContribution == axisDirection)));
            }

            return joystickMaps.ToList();
        }

        /// <summary>
        /// Loads the maps defined in the Rewired Editor and assigned to this player for the keyboard. All existing maps will be cleared and replaced with the default maps. 
        /// </summary>
        public static void LoadDefaultKeyboardMaps() {
            Player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
        }

        /// <summary>
        /// Loads the maps defined in the Rewired Editor and assigned to this player for the joysticks. All existing maps will be cleared and replaced with the default maps. 
        /// </summary>
        public static void LoadDefaultJoystickMaps() {
            Player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
        }

        /// <summary>
        /// Gets if a joystick is currently connected.
        /// </summary>
        public static bool IsJoystickConnected {
            get { return Player.controllers.joystickCount > 0; }
        }

        /// <summary>
        /// Gets if the last active controller is the joystick (a joystick must be connected).  Otherwise it's the keyboard.
        /// </summary>
        public static bool IsJoystickLastActiveController {
            get {
                if (!IsJoystickConnected)
                    return false;
                return Player.controllers.GetLastActiveController() is Joystick;
            }
        }

        /// <summary>
        /// Gets the last active joystick.
        /// Returns null if no joysticks are connected.
        /// </summary>
        public static Joystick GetPrimaryJoystick() {
            Joystick joystick = null;
            foreach (Joystick j in Player.controllers.Joysticks) {
                if (joystick == null || j.GetLastTimeActive() > joystick.GetLastTimeActive())
                    joystick = j;
            }
            return joystick;
        }

        /// <summary>
        /// Gets the Rewired hardware guid of the <see cref="GetPrimaryJoystick"/>.  Returns null if no joysticks are connected.
        /// </summary>
        /// <returns>Guid?</returns>
        public static System.Guid? GetPrimaryJoystickGuid() {
            return GetPrimaryJoystick()?.hardwareTypeGuid;
        }

        /// <summary>
        /// Gets the joystick style of the <see cref="GetPrimaryJoystick"/>.  Returns null if no joysticks are connected.
        /// </summary>
        /// <returns>JoystickStyleID?</returns>
        public static IJoystickStyle GetPrimaryJoystickStyle() {
            System.Guid? guid = GetPrimaryJoystickGuid();
            if (guid == null) {
                return null;
            } else {
                return GetJoystickStyle(guid.Value);
            }
        }

        /// <summary>
        /// Creates a <see cref="InputMapper.Context"/> for rebinding a keyboard assignment to the given input action.
        /// <para/>
        /// To be used when starting the input mapper like so: _inputMapper.Start(_inputMapperContext);
        /// </summary>
        /// <param name="inputActionId">Id of the input action to rebind the keyboard assignment for.</param>
        /// <param name="axisDirection">Axis direction for the action</param>
        /// <returns>Input mapper context.</returns>
        public static InputMapper.Context CreateKeyboardInputMapperContext(int inputActionId, Pole axisDirection) {
            InputAction inputAction = GetAction(inputActionId);

            // TODO: can use this for joystick mapping?
            //RewiredUtils.Player.controllers.GetLastActiveController().id
            ControllerMap controllerMap = Player.controllers.maps.GetMapsInCategory(ControllerType.Keyboard, 0, inputAction.categoryId).FirstOrDefault();

            ActionElementMap existingKeyboardMap = GetFirstKeyboardActionElementMap(inputAction, axisDirection);

            return new InputMapper.Context() {
                actionId = inputActionId,
                controllerMap = controllerMap,
                actionRange = axisDirection == Pole.Negative ? AxisRange.Negative : AxisRange.Positive,
                actionElementMapToReplace = existingKeyboardMap
            };
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