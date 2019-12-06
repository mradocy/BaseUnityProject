using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// Base class for a control representing a <see cref="IButtonOption"/>.
    /// </summary>
    public abstract class ButtonOptionControlBase : OptionControlBase {

        /// <summary>
        /// Gets the <see cref="IButtonOption"/> this control is bound to.
        /// </summary>
        public IButtonOption ButtonOption { get; private set; }

        /// <summary>
        /// Sets the <see cref="IButtonOption"/> this control is bound to.
        /// </summary>
        /// <param name="buttonOption"></param>
        public void SetButtonOption(IButtonOption buttonOption) {
            this.ButtonOption = buttonOption;
        }

        #region Abstract Methods

        /// <summary>
        /// Executes the action represented by the button.
        /// </summary>
        /// <remarks>Different from executing the button model directly, e.g. may play an animation first.</remarks>
        public abstract void Execute();

        #endregion
    }
}