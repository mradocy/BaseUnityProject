using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.UI.Options;
using Rewired;

namespace Core.Unity.RewiredExtensions.UI {

    public class JoystickMappingOption : IButtonOption {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="axisDirection">Axis direction (only applies to axis actions).</param>
        /// <param name="displayName">The display name of the action.</param>
        /// <param name="canJoystickMappingChange">If the joystick mapping for this action can be changed by the user.  Should be false for UI actions.</param>
        /// <param name="isAlt">If this mapping is the alternate joystick mapping for this action.</param>
        /// <param name="defaultElementType">The default controller element type for this action.</param>
        /// <param name="defaultElementIndex">Index of the default element for this action.</param>
        public JoystickMappingOption(
            int actionId,
            Pole axisDirection,
            string displayName,
            bool canJoystickMappingChange,
            bool isAlt,
            ControllerElementType defaultElementType,
            int defaultElementIndex) {

            this.ActionId = actionId;
            this.AxisDirection = axisDirection;
            this.DisplayName = displayName;
            _canJoystickMappingChange = canJoystickMappingChange;
            this.IsAlt = isAlt;
            this.DefaultElementType = defaultElementType;
            this.DefaultElementIndex = defaultElementIndex;
        }

        /// <summary>
        /// Id of the action.
        /// </summary>
        public int ActionId { get; }

        /// <summary>
        /// Axis direction (only applies to axis actions).
        /// </summary>
        public Pole AxisDirection { get; }

        /// <inheritdoc />
        public string DisplayName { get; }

        /// <summary>
        /// Gets a value indicating whether this mapping is the alternate joystick mapping for this action.
        /// </summary>
        public bool IsAlt { get; }

        /// <summary>
        /// The default controller element type for this action.
        /// </summary>
        public ControllerElementType DefaultElementType { get; }

        /// <summary>
        /// The default controller element type for this action.
        /// </summary>
        public int DefaultElementIndex { get; }

        /// <inheritdoc />
        public bool CanExecute {
            get { return _canJoystickMappingChange; }
        }

        /// <inheritdoc />
        public void Execute() {
            _executeAction.Invoke(this);
        }

        /// <summary>
        /// Sets the method to be invoked when this option is executed.
        /// </summary>
        /// <param name="executeAction">Action to execute.</param>
        public void SetExecuteAction(System.Action<JoystickMappingOption> executeAction) {
            _executeAction = executeAction;
        }

        private readonly bool _canJoystickMappingChange = true;
        private System.Action<JoystickMappingOption> _executeAction = null;
    }
}