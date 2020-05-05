using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Extension methods for <see cref="RectTransform"/>.
    /// </summary>
    public static class RectTransformExtensions {

        /// <summary>
        /// Sets the x coordinate of this rectTransform's anchored position.
        /// </summary>
        /// <param name="rectTransform">This rectTransform.</param>
        /// <param name="x">x to set.</param>
        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x) {
            Vector2 pos = rectTransform.anchoredPosition;
            pos.x = x;
            rectTransform.anchoredPosition = pos;
        }

    }
}