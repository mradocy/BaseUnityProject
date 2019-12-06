using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// Model of a button option (an action that can be executed) that would appear in an options menu.
    /// </summary>
    public interface IButtonOption : IOption {

        /// <summary>
        /// Gets if this button's action can be executed.
        /// </summary>
        bool CanExecute { get; }

        /// <summary>
        /// Executes the button's action.
        /// </summary>
        void Execute();
    }
}