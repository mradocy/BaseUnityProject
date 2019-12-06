using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.UI {

    /// <summary>
    /// Interface for getting user input for the UI.
    /// </summary>
    public interface IUIInput {

        /// <summary>
        /// Gets if the start button was pressed.
        /// </summary>
        bool IsStartPressed { get; }

        /// <summary>
        /// Gets if the ui up button was pressed.
        /// </summary>
        bool IsUpPressed { get; }

        /// <summary>
        /// Gets if the ui up button is currently held.
        /// </summary>
        bool IsUpHeld { get; }

        /// <summary>
        /// Gets if the ui down button was pressed.
        /// </summary>
        bool IsDownPressed { get; }

        /// <summary>
        /// Gets if the ui down button is currently held.
        /// </summary>
        bool IsDownHeld { get; }

        /// <summary>
        /// Gets if the ui left button was pressed.
        /// </summary>
        bool IsLeftPressed { get; }

        /// <summary>
        /// Gets if the ui left button is currently held.
        /// </summary>
        bool IsLeftHeld { get; }

        /// <summary>
        /// Gets if the ui right button was pressed.
        /// </summary>
        bool IsRightPressed { get; }

        /// <summary>
        /// Gets if the ui right button is currently held.
        /// </summary>
        bool IsRightHeld { get; }

        /// <summary>
        /// Gets if the submit button was pressed.
        /// </summary>
        bool IsSubmitPressed { get; }

        /// <summary>
        /// Gets if the cancel button was pressed.
        /// </summary>
        bool IsCancelPressed { get; }
    }
}