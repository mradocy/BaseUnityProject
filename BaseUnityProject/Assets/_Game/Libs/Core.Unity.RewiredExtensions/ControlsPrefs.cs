using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Core.Unity.RewiredExtensions {

    /// <summary>
    /// A replacement of PlayerPrefs used for storing custom rewired controls to a file by <see cref="UserDataStore_PrefsFile"/>.
    /// </summary>
    public static class ControlsPrefs {

        /// <summary>
        /// Gets the file path for the rewired controls file.
        /// </summary>
        /// <returns></returns>
        public static string GetFilePath() {
            return Path.Combine(Application.persistentDataPath, "rewired_controls.xml");
        }

        public static bool HasKey(string key) {
            if (!_isLoaded) {
                Load();
            }

            return _prefs.ContainsKey(key);
        }

        public static string GetString(string key) {
            if (!_isLoaded) {
                Load();
            }

            string val;
            if (_prefs.TryGetValue(key, out val)) {
                return val;
            }

            return null;
        }

        public static void SetString(string key, string value) {
            if (!_isLoaded) {
                Load();
            }

            _prefs[key] = value;
        }

        public static void DeleteAll() {
            if (!_isLoaded) {
                Load();
            }

            _prefs.Clear();
            Save();
        }

        public static void Save() {
            if (!_isLoaded) {
                Load();
            }

            // convert to string
            StringBuilder sb = new StringBuilder();
            foreach (string key in _prefs.Keys) {
                sb.AppendLine(key + ":" + _prefs[key]);
            }

            // save to file
            try {
                File.WriteAllText(GetFilePath(), sb.ToString());
            } catch (System.Exception ex) {
                Debug.LogError($"Error saving rewired controls prefs: {ex.Message}");
            }
        }

        private static void Load() {
            if (_isLoaded)
                return;

            string filePath = GetFilePath();
            if (!File.Exists(filePath)) {
                _prefs.Clear();
                _isLoaded = true;
                return;
            }

            string[] lines;
            try {
                lines = File.ReadAllLines(filePath);
            } catch (System.Exception ex) {
                Debug.LogError($"Error loading rewired controls prefs: {ex.Message}");
                return;
            }

            _prefs.Clear();
            foreach (string line in lines) {
                if (string.IsNullOrEmpty(line))
                    continue;

                int colonIndex = line.IndexOf(':');
                if (colonIndex == -1)
                    continue;
                string key = line.Substring(0, colonIndex);
                string val = line.Substring(colonIndex + 1);
                _prefs[key] = val;
            }
            _isLoaded = true;
        }

        private static Dictionary<string, string> _prefs = new Dictionary<string, string>();
        private static bool _isLoaded = false;

    }
}