using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using System.IO;
using UnityEditor;

namespace Core.Unity.BuildAutomation {

    /// <summary>
    /// Handles the creation of the game's README file.
    /// </summary>
    public static class ReadmeManager {

        #region Constants and Properties

        /// <summary>
        /// Gets the filename for the readme that will be created in the build directory.
        /// </summary>
        public const string _readmeFilename = "README.txt";

        /// <summary>
        /// Gets the path to the README template file.
        /// </summary>
        public static string ReadmeTemplatePath {
            get { return Path.GetFullPath($"{Application.dataPath}/README_template.txt").Replace('\\', '/'); }
        }

        #endregion

        /// <summary>
        /// Creates a readme file and places it in the given directory.
        /// A warning is logged if the readme template file doesn't exist.
        /// </summary>
        /// <param name="buildDirectory">Directory to place the readme file.</param>
        public static void CreateReadmeFileForBuild(string buildDirectory) {
            if (!File.Exists(ReadmeTemplatePath)) {
                Debug.LogWarning($"Readme could not be added to the build!  Readme template {ReadmeTemplatePath} does not exist.");
                return;
            }

            if (!Directory.Exists(buildDirectory)) {
                Debug.LogWarning($"Readme could not be added to the build!  Given build directory {buildDirectory} does not exist.");
                return;
            }

            string readmeText;
            try {
                readmeText = File.ReadAllText(ReadmeTemplatePath);
            } catch (System.Exception e) {
                Debug.LogError($"Could not read readme template file.  Exception: {e.Message}");
                return;
            }

            // make replacements in readme text
            readmeText = readmeText.Replace("$(version)", BuildInfo.Version);
            readmeText = readmeText.Replace("$(build_date)", System.DateTime.Now.ToShortDateString());

            // create readme file in directory
            string filePath = Path.Combine(buildDirectory, _readmeFilename).Replace('\\', '/');
            try {
                File.WriteAllText(filePath, readmeText);
            } catch (System.Exception ex) {
                Debug.LogError($"Could not create readme file.  Exception: {ex.Message}");
                return;
            }

            Debug.Log($"Successfully created readme at {filePath}");
        }
    }
}