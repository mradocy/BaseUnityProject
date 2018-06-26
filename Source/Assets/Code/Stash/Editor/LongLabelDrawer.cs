using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LongLabelAttribute))]
public class LongLabelDrawer : PropertyDrawer {
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        
        EditorGUIUtility.labelWidth = (attribute as LongLabelAttribute).labelWidth;

        EditorGUI.PropertyField(position, property, label, true);
        
    }

}