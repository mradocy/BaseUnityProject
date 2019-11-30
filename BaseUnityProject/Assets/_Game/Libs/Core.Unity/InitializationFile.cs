using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Represents an .ini file to be loaded on initialization.
    /// </summary>
    public class InitializationFile {

        /// <summary>
        /// Constructor.
        /// </summary>
        public InitializationFile() {
            if (_instance != null) {
                Debug.LogError("An initialization file has already been created");
                return;
            }
            _instance = this;
        }

        #region Properties

        /// <summary>
        /// Gets if a property by the given name exists.
        /// </summary>
        /// <param name="key">Name of the property.</param>
        /// <returns>Exists</returns>
        public bool HasProperty(string key) {
            string dicKey = key.ToLower();
            return _properties.ContainsKey(dicKey);
        }

        /// <summary>
        /// Gets a property as a string.  If the property doesn't exist, it's created with the given defaultValue.
        /// </summary>
        public string GetString(string key, string defaultValue = null) {
            if (string.IsNullOrEmpty(key)) {
                return defaultValue;
            }
            string dicKey = key.ToLower();
            string val;
            if (_properties.TryGetValue(dicKey, out val)) {
                return val;
            } else {
                _properties[dicKey] = defaultValue;
            }

            return defaultValue;
        }
        /// <summary>
        /// Gets a property as a boolean.  If the property doesn't exist, it's created with the given defaultValue.
        /// </summary>
        public bool GetBool(string key, bool defaultValue = false) {
            string valStr = this.GetString(key, defaultValue ? "true" : "false");
            if (valStr == null)
                return defaultValue;
            return !(string.IsNullOrEmpty(valStr) || valStr == "0" || valStr.ToLower() == "false");
        }
        /// <summary>
        /// Gets a property as an int.  If the property doesn't exist, it's created with the given defaultValue.
        /// </summary>
        public int GetInt(string key, int defaultValue = 0) {
            string valStr = this.GetString(key, $"{defaultValue}");
            if (valStr == null)
                return defaultValue;
            int ret;
            if (int.TryParse(valStr, out ret))
                return ret;
            return defaultValue;
        }
        /// <summary>
        /// Gets a property as a float.  If the property doesn't exist, it's created with the given defaultValue.
        /// </summary>
        public float GetFloat(string key, float defaultValue = 0) {
            string valStr = this.GetString(key, $"{defaultValue}");
            if (valStr == null)
                return defaultValue;
            float ret;
            if (float.TryParse(valStr, out ret))
                return ret;
            return defaultValue;
        }

        /// <summary>
        /// Sets a property.
        /// </summary>
        public void SetString(string key, string value) {
            if (string.IsNullOrEmpty(key))
                return;
            _properties[key.ToLower()] = value;
        }
        /// <summary>
        /// Sets a bool property.
        /// </summary>
        public void SetBool(string key, bool value) {
            this.SetString(key, value ? "1" : "0");
        }
        /// <summary>
        /// Sets an int property.
        /// </summary>
        public void SetInt(string key, int value) {
            this.SetString(key, "" + value);
        }
        /// <summary>
        /// Sets a float property.
        /// </summary>
        public void SetFloat(string key, float value) {
            this.SetString(key, "" + value);
        }

        #endregion

        #region File I/O

        /// <summary>
        /// Directory containing the initialization file.
        /// </summary>
        public string FileDirectory {
            get {
                return Application.dataPath;
            }
        }
        /// <summary>
        /// The file name.
        /// </summary>
        public const string FileName = "settings.ini";
        /// <summary>
        /// The full file path, gotten from combining fileDirectory with fileName.  On windows this is in the [Game Name]_Data folder.
        /// </summary>
        public string FilePath {
            get {
                return Path.Combine(FileDirectory, FileName);
            }
        }

        /// <summary>
        /// Saves properties to disk.  This is a syncronous action.
        /// </summary>
        public void Save() {

            StringBuilder sb = new StringBuilder();
            foreach (string key in _properties.Keys) {
                sb.AppendLine($"{key}: {_properties[key]}");
            }

            try {
                File.WriteAllText(FilePath, sb.ToString());
            } catch (System.Exception e) {
                Debug.LogError($"Could not save initialization settings to \"{FilePath}\".  Exception: {e.Message}");
            }

        }

        /// <summary>
        /// Loads properties from disk.  This is a syncronous action.
        /// </summary>
        public void Load() {

            if (!File.Exists(FilePath)) {
                Debug.Log($"INI file \"{FilePath}\" does not exist, creating new file.");
                Save();
                return;
            }

            string[] lines;
            try {
                lines = File.ReadAllLines(FilePath);
            } catch (System.Exception e) {
                Debug.LogError($"Error reading INI file \"{FilePath}\": {e.Message}");
                Save();
                return;
            }

            // parsing lines
            _properties.Clear();
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();
                if (line == "")
                    continue;
                if (line.StartsWith("//"))
                    continue;

                int index = line.IndexOf(':');
                if (index == -1) {
                    Debug.LogError($"Could not parse initialization line {line}, no ':'");
                } else {
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();
                    _properties[key] = value;
                }
            }

        }

        #endregion

        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private static InitializationFile _instance = null;
    }
}