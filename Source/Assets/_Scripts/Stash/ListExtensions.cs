using UnityEngine;
using System.Collections.Generic;

public static class ListExtensions {

    /// <summary>
    /// Shuffles the list in place.
    /// </summary>
    public static void Shuffle<T>(this List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n+1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Resizes the list.  If the list increases in size, the new elements will be null or whatever the default value of T is.
    /// </summary>
    /// <param name="size">The new Count of the list.</param>
    public static void Resize<T>(this List<T> list, int size) {
        list.Resize(size, default(T));
    }

    /// <summary>
    /// Resizes the list.
    /// </summary>
    /// <param name="size">The new Count of the list.</param>
    /// <param name="element">When increasing the size of the list, this value will be used for the new elements.</param>
    public static void Resize<T>(this List<T> list, int size, T element) {
        int count = list.Count;

        if (size < count) {
            list.RemoveRange(size, count - size);
        } else if (size > count) {
            if (size > list.Capacity) // Optimization
                list.Capacity = size;

            list.AddRange(System.Linq.Enumerable.Repeat(element, size - count));
        }
    }

    /// <summary>
    /// Returns a string of all the contained elements.
    /// </summary>
    public static string ToStringElements<T>(this List<T> list) {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("[");
        for (int i=0; i < list.Count; i++) {
            if (list[i] == null) {
                sb.Append("null");
            } else {
                sb.Append(list[i].ToString());
            }
            if (i < list.Count - 1) {
                sb.Append(", ");
            }
        }
        sb.Append("]");
        return sb.ToString();
    }

    /// <summary>
    /// Destroys every GameObject (or gameObject attached to MonoBehaviour) in this list.
    /// </summary>
    public static void DestroyAndClear<T>(this List<T> list) {
        foreach (T item in list) {
            if (item is GameObject) {
                Object.Destroy(item as GameObject);
            } else if (item is MonoBehaviour) {
                Object.Destroy((item as MonoBehaviour).gameObject);
            }
        }
        list.Clear();
    }

}
