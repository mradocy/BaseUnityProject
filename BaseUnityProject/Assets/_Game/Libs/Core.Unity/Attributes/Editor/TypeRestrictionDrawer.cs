using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using UnityEditor;

namespace Core.Unity.Attributes {

    [CustomPropertyDrawer(typeof(TypeRestrictionAttribute))]
    public class TypeRestrictionPropertyDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // validate inputs - could draw a 'fail' message if not validated correctly
            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return;

            TypeRestrictionAttribute attribute = this.attribute as TypeRestrictionAttribute;
            if (attribute == null)
                return;

            // draw
            EditorGUI.BeginChangeCheck();
            UnityEngine.Object obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(UnityEngine.Object), attribute.AllowSceneObjects);
            if (EditorGUI.EndChangeCheck()) {
                if (obj == null) {
                    property.objectReferenceValue = null;
                } else {
                    System.Type type = obj.GetType();
                    if (!attribute.Type.IsAssignableFrom(type)) {
                        GameObject go = obj as GameObject;
                        Component comp = obj as Component;
                        if (go != null) {
                            obj = go.GetComponent(attribute.Type);
                        } else if (comp != null) {
                            obj = comp.GetComponent(attribute.Type);
                        } else {
                            obj = null;
                        }
                    }

                    property.objectReferenceValue = obj;
                }
            }
        }

    }
}