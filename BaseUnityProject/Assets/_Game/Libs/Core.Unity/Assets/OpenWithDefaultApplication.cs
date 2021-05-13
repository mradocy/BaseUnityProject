using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using UnityEditor;
using System.IO;

namespace Core.Unity.Assets {

#if UNITY_EDITOR

    public static class OpenWithDefaultApplication {

        [MenuItem("Assets/Open with Default Application...")]
        static void OpenWithDefaultApplicationMenuItem() {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(Selection.activeObject));
            System.Diagnostics.Process.Start(filePath);
        }

        [MenuItem("Assets/Open with Default Application...", true)]
        static bool ValidateOpenInTextEditor() {
            if (Selection.activeObject == null)
                return false;
            string ext = Path.GetExtension(AssetDatabase.GetAssetPath(Selection.activeObject));
            if (string.IsNullOrEmpty(ext) || string.Equals(ext, ".asset", System.StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

    }

#endif
}