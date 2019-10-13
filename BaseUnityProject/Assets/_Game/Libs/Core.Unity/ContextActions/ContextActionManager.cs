using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.ContextActions {

    /// <summary>
    /// Static manager to store and get <see cref="IContextAction"/>s.
    /// </summary>
    public static class ContextActionManager {

        /// <summary>
        /// Adds the given action to the manager.  Cannot add an action that has already been added.  Should do this when the object awakes.
        /// </summary>
        /// <param name="contextAction">The action to add.</param>
        public static void AddAction(IContextAction contextAction) {
            if (contextAction == null)
                throw new System.ArgumentNullException(nameof(contextAction));
            if (ContainsAction(contextAction)) {
                Debug.LogError("Cannot add context action because it was already added.");
                return;
            }

            _contextActions.Add(contextAction);
        }

        /// <summary>
        /// Removes the given action from the manager.  Should do this when the object creating the action gets destroyed.
        /// </summary>
        /// <param name="contextAction">The action to remove.</param>
        public static void RemoveAction(IContextAction contextAction) {
            _contextActions.Remove(contextAction);
        }

        /// <summary>
        /// Gets the first context action that the given user can execute.  Returns null if the user can't be executed.
        /// </summary>
        /// <param name="user">The user to execute the action.</param>
        /// <returns>Context action.</returns>
        public static IContextAction GetExecutableAction(IContextActionUser user) {
            foreach (IContextAction action in _contextActions) {
                if (action.CanExecute(user)) {
                    return action;
                }
            }

            return null;
        }

        #region Private

        private static bool ContainsAction(IContextAction contextAction) {
            return _contextActions.Contains(contextAction);
        }

        private static List<IContextAction> _contextActions = new List<IContextAction>();

        #endregion
    }
}