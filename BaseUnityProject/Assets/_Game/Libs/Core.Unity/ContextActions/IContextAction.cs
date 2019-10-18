using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.ContextActions {

    /// <summary>
    /// Describes a context-sensitive action to be executed by a <see cref="IContextActionUser"/>.
    /// </summary>
    public interface IContextAction {

        /// <summary>
        /// Gets a value indicating whether the context action can be executed.
        /// </summary>
        /// <param name="user">The user that would execute the action.</param>
        /// <returns>Can execute.</returns>
        bool CanExecute(IContextActionUser user);

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="user">The user executing the action.</param>
        void Execute(IContextActionUser user);

        /// <summary>
        /// Gets the time when this action was last executed.
        /// Is -1 if action has never been executed.
        /// </summary>
        float ExecutedTimestamp { get; }


        /// <summary>
        /// Prompt that would be displayed in a context ui prompt.
        /// </summary>
        string Prompt { get; }

        /// <summary>
        /// The id of the input to be used to execute the action.
        /// </summary>
        int InputId { get; }

        /// <summary>
        /// If the input is for an axis action, this value indicates which direction.  True for positive, false for negative.
        /// </summary>
        bool InputAxisPositiveDirection { get; }

        /// <summary>
        /// If the input can be held, instead of just pressed.
        /// </summary>
        bool CanHold { get; }
    }
}