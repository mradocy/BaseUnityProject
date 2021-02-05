using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Extension methods for <see cref="List"/>s.
    /// </summary>
    public static class ListExtensions {

        /// <summary>
        /// Removes the last element from this list and returns it.
        /// </summary>
        /// <param name="list">This list.</param>
        public static T Pop<T>(this List<T> list) {
            if (list.Count <= 0) {
                Debug.LogError("Cannot pop element off list because the list has no elements");
                return default;
            }

            T element = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return element;
        }

        /// <summary>
        /// Resizes this list to contain the given number of elements.
        /// </summary>
        /// <param name="size">New size to give this list.</param>
        /// <param name="defaultValue">If given size is larger than the current count, fill the additional elements with this value.</param>
        public static void Resize<T>(this List<T> list, int size, T defaultValue = default) {
            int count = list.Count;
            if (size < count) {
                list.RemoveRange(size, count - size);
            } else if (size > count) {
                if (size > list.Capacity) {
                    list.Capacity = size; // this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                }
                list.AddRange(System.Linq.Enumerable.Repeat(defaultValue, size - count));
            }
        }

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
            for (int i=0; i < list.Count - 1; i++) {
                int j = Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}