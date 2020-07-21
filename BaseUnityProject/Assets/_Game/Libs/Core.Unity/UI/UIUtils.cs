using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI {
    /// <summary>
    /// Static class for UI utility functions.
    /// </summary>
    public static class UIUtils {

        /// <summary>
        /// Gets the local position of a UI object that would be at the same position as the given world position.
        /// The result would be used like so: this.rectTransform.localPosition = result;
        /// </summary>
        /// <param name="worldPosition">The world position to consider.  e.g. player.transform.position</param>
        /// <param name="parentRectTransform">The <see cref="RectTransform"/> of the parent of the UI object to position.</param>
        /// <returns>Local position.</returns>
        public static Vector2 WorldToUILocalPosition(Vector3 worldPosition, RectTransform parentRectTransform) {
            return WorldToUILocalPosition(worldPosition, parentRectTransform, UnityEngine.Camera.main);
        }

        /// <summary>
        /// Gets the local position of a UI object that would be at the same position as the given world position.
        /// The result would be used like so: this.rectTransform.localPosition = result;
        /// </summary>
        /// <param name="worldPosition">The world position to consider.  e.g. player.transform.position</param>
        /// <param name="parentRectTransform">The <see cref="RectTransform"/> of the parent of the UI object to position.</param>
        /// <param name="worldCamera">The world camera to use.</param>
        /// <returns>Local position.</returns>
        public static Vector2 WorldToUILocalPosition(Vector3 worldPosition, RectTransform parentRectTransform, UnityEngine.Camera worldCamera) {
            if (parentRectTransform == null) {
                throw new System.ArgumentNullException(nameof(parentRectTransform));
            }
            if (worldCamera == null) {
                throw new System.ArgumentNullException(nameof(worldCamera));
            }

            Canvas canvas = parentRectTransform.GetComponentInParent<Canvas>();
            if (canvas == null) {
                throw new System.ArgumentException("Given RectTransform is not contained in a Canvas, somehow", nameof(parentRectTransform));
            }

            Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPosition);
            Vector2 ret;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPos, canvas.worldCamera, out ret);
            return ret;
        }

    }
}