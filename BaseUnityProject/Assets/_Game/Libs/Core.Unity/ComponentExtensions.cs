using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Extension methods for <see cref="Component"/>s.
    /// </summary>
    public static class ComponentExtensions {

        /// <summary>
        /// Gets the component of the given type, but logs an error if the component could not be found.
        /// </summary>
        /// <typeparam name="T">Type of component to search.</typeparam>
        /// <param name="component">This component to search</param>
        /// <returns>Component</returns>
        public static T EnsureComponent<T>(this Component component) {
            T comp = component.GetComponent<T>();
            if (comp == null) {
                Debug.LogError($"Component of type {typeof(T)} not found in component {component}");
            }
            return comp;
        }

        /// <summary>
        /// Searches parents for the component of the given type, but logs an error if the component could not be found.
        /// </summary>
        /// <typeparam name="T">Type of component to search.</typeparam>
        /// <param name="component">This component to search</param>
        /// <returns>Component</returns>
        public static T EnsureComponentInParent<T>(this Component component) {
            T comp = component.GetComponentInParent<T>();
            if (comp == null) {
                Debug.LogError($"Component of type {typeof(T)} not found in parents of component {component}");
            }
            return comp;
        }

        /// <summary>
        /// Gets if this component belongs to a gameObject that currently belongs to the active scene.
        /// </summary>
        /// <param name="component">This component</param>
        /// <returns>In active scene</returns>
        public static bool IsInActiveScene(this Component component) {
            return component.gameObject.scene.buildIndex == UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }
    }
}