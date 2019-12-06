using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// A <see cref="IButtonOption"/> where its name and methods can be specified in the constructor.
    /// </summary>
    public class RoutedButtonOption : IButtonOption {

        /// <summary>
        /// Constructor.  This option can always be executed.
        /// </summary>
        /// <param name="displayName">Display name.</param>
        /// <param name="executeAction">Action to execute when <see cref="Execute"/> is called.</param>
        public RoutedButtonOption(string displayName, System.Action executeAction) {
            this.DisplayName = displayName;
            _executeAction = executeAction;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="displayName">Display name.</param>
        /// <param name="canExecuteFunc">Function to call to see if the button can be executed.</param>
        /// <param name="executeAction">Action to execute when <see cref="Execute"/> is called.</param>
        public RoutedButtonOption(string displayName, System.Func<bool> canExecuteFunc, System.Action executeAction) {
            this.DisplayName = displayName;
            _canExecuteFunc = canExecuteFunc;
            _executeAction = executeAction;
        }

        public string DisplayName { get; private set; }

        public bool CanExecute {
            get {
                if (_canExecuteFunc == null) {
                    return true;
                }

                return _canExecuteFunc.Invoke();
            }
        }

        public void Execute() {
            _executeAction?.Invoke();
        }

        private System.Func<bool> _canExecuteFunc = null;
        private System.Action _executeAction = null;
    }
}