using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Extension methods for <see cref="List"/>s.
    /// </summary>
    public static class ListExtensions {

        /// <summary>
        /// Destroys every <see cref="GameObject"/> in the list, then clears the list.
        /// </summary>
        /// <param name="list">This list.</param>
        public static void ClearAndDestroy(this List<GameObject> list) {
            foreach (GameObject go in list) {
                Object.Destroy(go);
            }
            list.Clear();
        }

        /// <summary>
        /// Destroys the gameObject of every <see cref="Component"/> in the list, then clears the list.
        /// </summary>
        /// <param name="list">This list.</param>
        public static void ClearAndDestroy<T>(this List<T> list)
            where T : Component {
            foreach (T comp in list) {
                Object.Destroy(comp.gameObject);
            }
            list.Clear();
        }

        /// <summary>
        /// Randomizes this list in place.
        /// </summary>
        /// <param name="list">This list.</param>
        /// <remarks>https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm</remarks>
        public static void Randomize<T>(this List<T> list) {
            for (int i=0; i < list.Count - 2; i++) {
                int j = Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}