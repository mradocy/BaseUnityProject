using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Unity.Assets {

#if UNITY_EDITOR

    public static class CreateCSVFile {

        /// <summary>
        /// A menu item that creates an empty .csv file.
        /// </summary>
        [MenuItem("Assets/Create/File/.csv", priority = 51)]
        public static void Create() {
            CreateTextFile.CreateFile("New CSV File.csv");
        }
    }

#endif

}