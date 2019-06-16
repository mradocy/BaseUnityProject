using UnityEngine;
using UnityEditor;

namespace Core.Unity.Attributes {
    [CustomPropertyDrawer(typeof(LongLabelAttribute))]
    public class LongLabelDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            EditorGUIUtility.labelWidth = (attribute as LongLabelAttribute).LabelWidth;

            EditorGUI.PropertyField(position, property, label, true);

        }

    }
}