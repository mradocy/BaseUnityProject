using UnityEditor;
using UnityEngine;

namespace Core.Unity.Attributes {
    /// <summary>
    /// To be overridden by custom header drawers
    /// </summary>
    public abstract class HeaderCustomDrawer : DecoratorDrawer {

        public abstract string Label { get; }

        public virtual Color HeaderColor {
            get { return Color.clear; }
        }

        public override void OnGUI(Rect position) {
            position.y += 8;
            position = EditorGUI.IndentedRect(position);

            GUIStyle guiStyle = EditorStyles.boldLabel;
            Color prevColor = guiStyle.normal.textColor;
            guiStyle.normal.textColor = HeaderColor;

            GUI.Label(position, Label, guiStyle);

            guiStyle.normal.textColor = prevColor;
        }

        public override float GetHeight() { return 24; }
    }
}