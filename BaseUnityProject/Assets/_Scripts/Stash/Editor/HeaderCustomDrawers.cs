using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// To be overridden by custom header drawers
/// </summary>
public class HeaderCustomDrawer : DecoratorDrawer {
    
    public virtual string label {
        get {
            return "";
        }
    }

    public virtual Color headerColor {
        get {
            return Color.clear;
        }
    }

    public override void OnGUI(Rect position) {
        position.y += 8;
        position = EditorGUI.IndentedRect(position);

        GUIStyle guiStyle = EditorStyles.boldLabel;
        Color prevColor = guiStyle.normal.textColor;
        guiStyle.normal.textColor = headerColor;

        GUI.Label(position, label, guiStyle);

        guiStyle.normal.textColor = prevColor;
    }

    public override float GetHeight() {
        return 24;
    }

}

[CustomPropertyDrawer(typeof(HeaderPrefabsAttribute))]
public class HeaderPrefabsDrawer : HeaderCustomDrawer {

    public override string label {
        get {
            return "Prefabs";
        }
    }

    public override Color headerColor {
        get {
            return new Color(0, .23f, .23f);
        }
    }

}

[CustomPropertyDrawer(typeof(HeaderChildrenAttribute))]
public class HeaderChildrenDrawer : HeaderCustomDrawer {

    public override string label {
        get {
            return "Children";
        }
    }

    public override Color headerColor {
        get {
            return new Color(.23f, .23f, 0);
        }
    }

}

[CustomPropertyDrawer(typeof(HeaderSceneAttribute))]
public class HeaderSceneDrawer : HeaderCustomDrawer {

    public override string label {
        get {
            return "Scene";
        }
    }

    public override Color headerColor {
        get {
            return new Color(.23f, 0, .23f);
        }
    }

}

[CustomPropertyDrawer(typeof(HeaderAssetsAttribute))]
public class HeaderAssetsDrawer : HeaderCustomDrawer {

    public override string label {
        get {
            return "Assets";
        }
    }

    public override Color headerColor {
        get {
            return new Color(0, 0, .23f);
        }
    }

}