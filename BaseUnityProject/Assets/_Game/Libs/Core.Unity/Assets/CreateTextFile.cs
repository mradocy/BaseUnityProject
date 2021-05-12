using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Core.Unity.Assets {

#if UNITY_EDITOR

    public static class CreateTextFile {

        public static void CreateFile(string fileName) {
            // get file path
            string path;
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
            Encoding encoding = new UTF8Encoding(false);
            File.WriteAllText(Application.dataPath.Replace("Assets", newFile), string.Empty, encoding);

            // import to Unity
            AssetDatabase.ImportAsset(newFile);
        }

        /// <summary>
        /// A menu item that creates an empty .txt file.
        /// </summary>
        [MenuItem("Assets/Create/File/.txt", priority = 50)]
        public static void Create() {
            CreateFile("New Text File.txt");
        }
    }

#endif

}