using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.Unity.Assets {
    public static class CreateTextFile {

        /// <summary>
        /// A menu item that creates an empty text file.
        /// </summary>
        [MenuItem("Assets/Create/Text File", priority = 50)]
        public static void Create() {

            // get file name
            string fileName = "New Text File.txt";
            string path = null;
            if (Selection.activeObject == null) {
                path = "Assets";
            } else {
                path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (Path.HasExtension(path)) {
                    path = Path.GetDirectoryName(path);
                }
            }
            string newFile = AssetDatabase.GenerateUniqueAssetPath(path + "/" + fileName);

            // create file (and close stream)
            File.WriteAllText(Application.dataPath.Replace("Assets", newFile), "");

            // import to Unity
            AssetDatabase.ImportAsset(newFile);

        }

    }
}