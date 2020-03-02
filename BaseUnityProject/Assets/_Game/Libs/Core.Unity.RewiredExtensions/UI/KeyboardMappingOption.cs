using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.UI.Options;
using Rewired;

namespace Core.Unity.RewiredExtensions.UI {

    public class KeyboardMappingOption : IButtonOption {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="axisDirection">Axis direction (only applies to axis actions).</param>
        /// <param name="displayName">The display name of the action.</param>
        /// <param name="canKeyboardMappingChange">If the keyboard mapping for this action can be changed by the user.  Should be false for UI actions.</param>
        /// <param name="defaultKeyCode">The default keyboard key code for this action.</param>
        public KeyboardMappingOption(
            int actionId,
            Pole axisDirection,
            string displayName,
            bool canKeyboardMappingChange,
            KeyCode defaultKeyCode) {

            this.ActionId = actionId;
            this.AxisDirection = axisDirection;
            this.DisplayName = displayName;
            _canKeyboardMappingChange = canKeyboardMappingChange;
            this.DefaultKeyCode = defaultKeyCode;
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
        /// The default keyboard key code for this action.
        /// </summary>
        public KeyCode DefaultKeyCode { get; }

        /// <inheritdoc />
        public bool CanExecute {
            get { return _canKeyboardMappingChange; }
        }

        /// <inheritdoc />
        public void Execute() {
            _executeAction.Invoke(this);
        }

        /// <summary>
        /// Sets the method to be invoked when this option is executed.
        /// </summary>
        /// <param name="executeAction">Action to execute.</param>
        public void SetExecuteAction(System.Action<KeyboardMappingOption> executeAction) {
            _executeAction = executeAction;
        }

        private readonly bool _canKeyboardMappingChange = true;
        private System.Action<KeyboardMappingOption> _executeAction = null;
    }
}