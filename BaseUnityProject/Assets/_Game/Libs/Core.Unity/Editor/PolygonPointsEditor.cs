using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Core.Unity {

    /// <summary>
    /// Editor for the <see cref="PolygonPoints"/> component.
    /// </summary>
    [CustomEditor(typeof(PolygonPoints))]
    public class PolygonPointsEditor : Editor {

        #region Constants

        protected static readonly Color _mainColor = Color.green;
        protected const float _handleSize = .4f;

        #endregion

        /// <summary>
        /// Handles an event in the scene view.
        /// </summary>
        protected virtual void OnSceneGUI() {
            PolygonPoints polygonPoints = this.target as PolygonPoints;
            if (polygonPoints == null)
                return;
            if (polygonPoints.LocalPoints == null || polygonPoints.LocalPoints.Length == 0)
                return;

            Handles.color = _mainColor;

            _polygonPoints.Clear();
            for (int i = 0; i < polygonPoints.LocalPoints.Length; i++) {

                // moving point
                Vector2 currentPosition = polygonPoints.transform.TransformPoint(polygonPoints.LocalPoints[i]);
                EditorGUI.BeginChangeCheck();
                Vector2 newPosition = Handles.FreeMoveHandle(currentPosition, Quaternion.identity, _handleSize, Vector3.zero, Handles.CylinderHandleCap);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(polygonPoints, "Change PolygonPoints Point");
                    polygonPoints.LocalPoints[i] = polygonPoints.transform.InverseTransformPoint(newPosition);
                }

                _polygonPoints.Add(newPosition);
            }

            // draw polygon
            if (_polygonPoints.Count > 1) {
                _polygonPoints.Add(_polygonPoints[0]);
                Handles.DrawPolyLine(_polygonPoints.ToArray());
            }
        }

        protected List<Vector3> _polygonPoints = new List<Vector3>();
    }
}