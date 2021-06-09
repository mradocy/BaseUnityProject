using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using UnityEditor;
using System.IO;

namespace Core.Unity.Assets {

#if UNITY_EDITOR

    public static class DuplicateFile {

        [MenuItem("Assets/Duplicate File", priority = 62)]
        static void DuplcateFile() {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string currentDirectory = Directory.GetCurrentDirectory();
            string assetFullPath = Path.Combine(currentDirectory, assetPath);
            string newAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            string newAssetFullPath = Path.Combine(currentDirectory, newAssetPath);

            // create file
            File.Copy(assetFullPath, newAssetFullPath);

            // import to Unity
            AssetDatabase.ImportAsset(newAssetPath);
        }

        [MenuItem("Assets/Open with Default Application...", true)]
        static bool ValidateDuplicateFile() {
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