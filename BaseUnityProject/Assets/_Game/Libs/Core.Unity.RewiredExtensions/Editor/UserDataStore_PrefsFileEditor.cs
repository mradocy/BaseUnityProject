// File copied from UserDataStore_PlayerPrefsInspector, with PlayerPrefs replaced with ControlsPrefs.

// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Core.Unity.RewiredExtensions.Editor {

    using UnityEngine;
    using UnityEditor;
    using Rewired;
    using Rewired.Data;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [CustomEditor(typeof(UserDataStore_PrefsFile))]
    public sealed class UserDataStore_PrefsFileInspector : UnityEditor.Editor {

        private bool showDebugOptions;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            showDebugOptions = EditorGUILayout.Foldout(showDebugOptions, new GUIContent("Debug Options"));

            if (showDebugOptions) {
                GUILayout.Space(15);
                if (GUILayout.Button(new GUIContent("Clear All ControlsPrefs Data", "This will clear all controls prefs data! Use this with caution!"))) {
                    if (EditorUtility.DisplayDialog("Clear All ControlsPrefs Data", $"WARNING: This will delete the custom controls data stored in \"{ControlsPrefs.GetFilePath()}\". Are you sure?", "DELETE", "Cancel")) {
                        ControlsPrefs.DeleteAll();
                    }
                }
                GUILayout.Space(15);
            }
        }


    }
}