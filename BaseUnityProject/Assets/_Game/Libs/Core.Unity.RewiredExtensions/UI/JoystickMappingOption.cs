using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.UI.Options;
using Rewired;
using UnityEngine.Events;

namespace Core.Unity.RewiredExtensions.UI {

    public class JoystickMappingOption : IButtonOption {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="displayName">The display name of the action.</param>
        /// <param name="controllerElementType">If the mapping is for a button or an axis.</param>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="actionPositiveDirection">Axis direction (only applies to button mappings for axis actions).</param>
        /// <param name="isAlt">If this mapping is the alternate joystick mapping for this action.</param>
        /// <param name="canJoystickMappingChange">If the joystick mapping for this action can be changed by the user.  Should be false for UI actions.</param>
        public JoystickMappingOption(
            string displayName,
            int actionId,
            bool actionPositiveDirection,
            bool isAlt,
            bool canJoystickMappingChange) {

            this.DisplayName = displayName;
            this.ActionId = actionId;
            this.ActionPositiveDirection = actionPositiveDirection;
            this.IsAlt = isAlt;
            _canJoystickMappingChange = canJoystickMappingChange;
        }

        public string DisplayName { get; }

        /// <summary>
        /// Id of the action.
        /// </summary>
        public int ActionId { get; }

        /// <summary>
        /// If the action is in the positive direction.  Only affects button mappings for axis actions.
        /// </summary>
        public bool ActionPositiveDirection { get; }

        /// <summary>
        /// Gets a value indicating whether this mapping is the alternate joystick mapping for this action.
        /// </summary>
        public bool IsAlt { get; }

        public bool CanExecute => _canJoystickMappingChange;

        public void Execute() {
            _executeAction.Invoke(this);
        }

        /// <summary>
        /// Sets the method to be invoked when this option is executed.
        /// </summary>
        /// <param name="executeAction">Action to execute.</param>
        public void SetExecuteAction(UnityAction<JoystickMappingOption> executeAction) {
            _executeAction = executeAction;
        }

        private readonly bool _canJoystickMappingChange;
        private UnityAction<JoystickMappingOption> _executeAction = null;
    }
}