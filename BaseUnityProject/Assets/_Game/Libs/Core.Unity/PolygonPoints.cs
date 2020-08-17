using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// A set of points that can be modified in the editor.
    /// </summary>
    public class PolygonPoints : MonoBehaviour {

        #region Inspector Fields

        [SerializeField]
        public Vector2[] LocalPoints = new Vector2[] { new Vector2(-2, -2), new Vector2(0, 2), new Vector2(2, -2) };

        #endregion

        public int PointsCount => LocalPoints == null ? 0 : LocalPoints.Length;

        /// <summary>
        /// Gets the point at the given index in the local space.  No bounds checking is done.
        /// </summary>
        public Vector2 GetLocalPoint(int index) { return LocalPoints[index]; }

        /// <summary>
        /// Gets the point at the given index in the world space.  No bounds checking is done.
        /// </summary>
        public Vector2 GetPoint(int index) {
            return this.transform.TransformPoint(this.GetLocalPoint(index));
        }

    }
}