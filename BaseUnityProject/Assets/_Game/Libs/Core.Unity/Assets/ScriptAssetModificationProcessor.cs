using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;
using System.Text;

namespace Core.Unity.Assets {

    public class ScriptAssetModificationProcessor : UnityEditor.AssetModificationProcessor {

        /// <summary>
        /// Names of folders to be considered the "root" where the namespace will start from.
        /// </summary>
        public static readonly string[] RootDirectories = { "Assets", "Scripts", "Libs" };

        /// <summary>
        /// Names of folders to skip over when constructing the namespace name.
        /// </summary>
        public static readonly string[] IgnoredDirectories = { "Editor" };

        /// <summary>
        /// Default namespace to give the script file if the resolver can't figure it out.
        /// </summary>
        public const string DefaultNamespace = "Default";

        /// <summary>
        /// Attempts to create a namespace for a .cs file based on its file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CreateNamespace(string filePath) {
            // get directory as segments
            string directory = Path.GetDirectoryName(filePath);
            List<string> namespaceSegments = directory.Split(Path.DirectorySeparatorChar).ToList();

            // remove "Editor" and other ignored directories
            namespaceSegments.RemoveAll(s => IgnoredDirectories.Any(id => string.Equals(s, id, System.StringComparison.OrdinalIgnoreCase)));

            // find root index
            int rootIndex = -1;
            for (int i=0; i < RootDirectories.Length; i++) {
                string rootDirectory = RootDirectories[i];
                for (int j=0; j < namespaceSegments.Count; j++) {
                    if (string.Equals(rootDirectory, namespaceSegments[j], System.StringComparison.OrdinalIgnoreCase)) {
                        rootIndex = Mathf.Max(rootIndex, j);
                        break;
                    }
                }
            }

            // construct namespace from directories
            StringBuilder sb = new StringBuilder();
            for (int i = rootIndex + 1; i < namespaceSegments.Count; i++) {
                sb.Append(Capitalize(namespaceSegments[i]));
                if (i + 1 < namespaceSegments.Count) {
                    sb.Append('.');
                }
            }
            string namesp;
            if (string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace)) {
                if (sb.Length < 1) {
                    namesp = DefaultNamespace;
                } else {
                    namesp = sb.ToString();
                }
            } else {
                // using editor root namespace (NOT TESTED)
                if (sb.Length < 1) {
                    namesp = EditorSettings.projectGenerationRootNamespace;
                } else {
                    namesp = EditorSettings.projectGenerationRootNamespace + "." + sb.ToString();
                }
            }

            return namesp;
        }

        public static void OnWillCreateAsset(string metaFilePath) {
            string fileName = Path.GetFileNameWithoutExtension(metaFilePath);

            // only modify .cs files
            if (!fileName.EndsWith(".cs"))
                return;

            // get namespace
            string namesp = CreateNamespace(metaFilePath);

            // edit file
            string filePath = Path.Combine(Path.GetDirectoryName(metaFilePath), fileName);
            string content = File.ReadAllText(filePath);
            string newContent = content.Replace("#NAMESPACE#", namesp);
            if (content != newContent) {
                File.WriteAllText(filePath, newContent);
                AssetDatabase.Refresh();
            }
        }

        private static string Capitalize(string s) {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }
            if (s.Length == 1) {
                return s.ToUpper();
            }

            return s.Substring(0, 1).ToUpper() + s.Substring(1);
        }

    }
}