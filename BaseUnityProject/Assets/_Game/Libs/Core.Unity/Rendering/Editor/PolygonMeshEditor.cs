using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using UnityEditor;

namespace Core.Unity.Rendering {

    /// <summary>
    /// Editor for the <see cref="PolygonMesh"/> component.
    /// </summary>
    [CustomEditor(typeof(PolygonMesh))]
    public class PolygonMeshEditor : Editor {

        #region Constants

        private static readonly Color _mainColor = Color.green;
        private const float _handleSize = .4f;

        #endregion

        /// <summary>
        /// Handles an event in the scene view.
        /// </summary>
        protected virtual void OnSceneGUI() {
            PolygonMesh polygonMesh = this.target as PolygonMesh;
            if (polygonMesh == null)
                return;
            if (polygonMesh._localPoints == null || polygonMesh._localPoints.Length == 0)
                return;

            Handles.color = _mainColor;

            _polygonPoints.Clear();
            for (int i=0; i < polygonMesh._localPoints.Length; i++) {

                // moving point
                Vector2 currentPosition = polygonMesh.transform.TransformPoint(polygonMesh._localPoints[i]);
                EditorGUI.BeginChangeCheck();
                Vector2 newPosition = Handles.FreeMoveHandle(currentPosition, Quaternion.identity, _handleSize, Vector3.zero, Handles.CylinderHandleCap);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(polygonMesh, "Change PolygonMesh Point");
                    polygonMesh._localPoints[i] = polygonMesh.transform.InverseTransformPoint(newPosition);
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