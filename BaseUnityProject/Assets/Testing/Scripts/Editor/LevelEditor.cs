using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

// building a custom inspector: https://unity3d.com/learn/tutorials/topics/interface-essentials/building-custom-inspector?playlist=17117

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor {

    public static Color GIZMO_NON_SELECTED_COLOR = new Color(0, .2f, .2f);
    public static Color GIZMO_SELECTED_COLOR = new Color(0, .7f, .7f);
    
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void drawGizmos(Level level, GizmoType gizmoType) {
        
        Color color = Color.black;
        if ((gizmoType & GizmoType.Selected) != 0){
            color = GIZMO_SELECTED_COLOR;
        } else {
            color = GIZMO_NON_SELECTED_COLOR;
        }
        Vector3 origin = level.transform.position;
        float w = level.width;
        float h = level.height;

        Gizmos.color = color;

        // draw rectangle
        Gizmos.DrawLine(origin, origin + new Vector3(0, h, 0));
        Gizmos.DrawLine(origin + new Vector3(0, h, 0), origin + new Vector3(w, h, 0));
        Gizmos.DrawLine(origin + new Vector3(w, h, 0), origin + new Vector3(w, 0, 0));
        Gizmos.DrawLine(origin + new Vector3(w, 0, 0), origin);

    }

    /// <summary>
    /// Gets the target as a Level
    /// </summary>
    public Level level {
        get {
            if (_level == null)
                _level = target as Level;
            return _level;
        }
    }

    public override void OnInspectorGUI() {

        //DrawDefaultInspector();

        //Vector2 dim = EditorGUILayout.Vector2Field("Dimensions", new Vector2(level.width, level.height));
        //level.width = Mathf.Max(1, Mathf.FloorToInt(dim.x));
        //level.height = Mathf.Max(1, Mathf.FloorToInt(dim.y));

        EditorGUILayout.LabelField("Size and Position (X: " + level.x + " Y: " + level.y + " W: " + level.width + " H: " + level.height + ")", EditorStyles.boldLabel);

        level.width = Mathf.Max(1, EditorGUILayout.IntField(new GUIContent("Width", "Tooltip: The width of the level"), level.width));
        level.height = Mathf.Max(1, EditorGUILayout.IntField(new GUIContent("Height", "Tooltip: The height of the level"), level.height));



        if (GUILayout.Button("Snap Position")) { // this shouldn't be needed
            level.transform.localPosition = new Vector3(Mathf.Round(level.transform.localPosition.x), Mathf.Round(level.transform.localPosition.y), level.transform.localPosition.z);
        }
        


        //EditorGUILayout.LabelField("Dimensions", "X: " + level.x + " Y: " + level.y + " W: " + level.width + " H: " + level.height);


        EditorGUILayout.HelpBox("This is a help box", MessageType.Info);

        

        // redraws scene (e.g. gizmos)
        SceneView.RepaintAll();

    }
    


    

    void printAdjLevelsLabel(string labelPrefix, List<Level> adjLevels) {
        StringBuilder sb = new StringBuilder();
        foreach (Level adjLevel in adjLevels) {
            if (sb.Length != 0) sb.Append(", ");
            sb.Append(adjLevel.name);
        }
        EditorGUILayout.LabelField(labelPrefix + (sb.Length == 0 ? "(none)" : sb.ToString()));
    }


    void OnSceneGUI() {

        Level level = target as Level;

        // lock scale, rotation
        level.transform.localScale = Vector3.one;
        level.transform.rotation = Quaternion.identity;
        

    }

    private Level _level = null;

}
