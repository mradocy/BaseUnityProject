using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Extension methods for <see cref="GameObject"/>s.
    /// </summary>
    public static class GameObjectExtensions {

        /// <summary>
        /// Gets the component of the given type, but throws an error if the component could not be found.
        /// </summary>
        /// <typeparam name="T">Type of component to search.</typeparam>
        /// <param name="component">This component to search</param>
        /// <returns>Component</returns>
        public static T EnsureComponent<T>(this GameObject gameObject) where T : Component {
            T comp = gameObject.GetComponent<T>();
            if (comp == null) {
                throw new System.Exception($"Component {typeof(T)} not found in game object {gameObject}");
            }
            return comp;
        }

    }

}