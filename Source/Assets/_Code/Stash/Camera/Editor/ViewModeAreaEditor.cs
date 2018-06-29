using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ViewModeArea))]
public class ViewModeAreaEditor : Editor {

    ViewModeArea viewModeArea;

    protected void OnEnable() {
        viewModeArea = (ViewModeArea)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

    }

    /// <summary>
    /// Recersively finds a "good enough" value for the global scale of the given transform.
    /// ViewModeArea shouldn't be scaled anyway.  This probably isn't needed.
    /// </summary>
    protected float calculateGlobalScale(Transform transform) {
        float scale = (Mathf.Abs(transform.localScale.x) + Mathf.Abs(transform.localScale.y)) / 2;
        if (transform.parent != null) {
            scale *= calculateGlobalScale(transform.parent);
        }
        return scale;
    }

    protected void OnSceneGUI() {
        
        float lossyScale = calculateGlobalScale(viewModeArea.transform);
        if (lossyScale > .0001f) {
            
            if (viewModeArea.showBoundsHandles) {

                Vector2 minPos = viewModeArea.boundsCenter - new Vector2(viewModeArea.boundsWidth / 2, viewModeArea.boundsHeight / 2);
                Vector2 maxPos = viewModeArea.boundsCenter + new Vector2(viewModeArea.boundsWidth / 2, viewModeArea.boundsHeight / 2);

                EditorGUI.BeginChangeCheck();
                Vector3 newOffset = Handles.PositionHandle(viewModeArea.transform.TransformPoint(minPos), Quaternion.identity);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(target, "Change Target Position");

                    minPos = viewModeArea.transform.InverseTransformPoint(newOffset);
                    viewModeArea.boundsCenter = (minPos + maxPos) / 2;
                    viewModeArea.boundsWidth = Mathf.Abs(maxPos.x - minPos.x);
                    viewModeArea.boundsHeight = Mathf.Abs(maxPos.y - minPos.y);

                    minPos = viewModeArea.boundsCenter - new Vector2(viewModeArea.boundsWidth / 2, viewModeArea.boundsHeight / 2);
                    maxPos = viewModeArea.boundsCenter + new Vector2(viewModeArea.boundsWidth / 2, viewModeArea.boundsHeight / 2);
                }

                EditorGUI.BeginChangeCheck();
                newOffset = Handles.PositionHandle(viewModeArea.transform.TransformPoint(maxPos), Quaternion.identity);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(target, "Change Target Position");

                    maxPos = viewModeArea.transform.InverseTransformPoint(newOffset);
                    viewModeArea.boundsCenter = (minPos + maxPos) / 2;
                    viewModeArea.boundsWidth = Mathf.Abs(maxPos.x - minPos.x);
                    viewModeArea.boundsHeight = Mathf.Abs(maxPos.y - minPos.y);
                }

            }
            

        }


    }


}
