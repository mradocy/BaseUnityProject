using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class InitializationSettings {

    /// <summary>
    /// If loadFromFile() was called yet.  If it hasn't, then none of the properties will be loaded.
    /// </summary>
    public static bool fileLoaded { get; private set; }

    #region Default Settings

    /// <summary>
    /// Clears the current settings, then loads the default initialization settings.
    /// </summary>
    public static void loadDefaultSettings() {
        properties.Clear();
        fullscreen = false;
        //sfxVolume = 1;
        sfxVolume = 0;
        musicVolume = 1;
        localization = "en-us";
    }

    #endregion

    #region Common Settings (call loadFromFile() first)

    /// <summary>
    /// If the game should start in fullscreen.
    /// </summary>
    public static bool fullscreen {
        get {
            return getBool("fullscreen", false);
        }
        set {
            setBool("fullscreen", value);
        }
    }
    /// <summary>
    /// The master sfx volume level.
    /// </summary>
    public static float sfxVolume {
        get {
            return getFloat("sfx_volume", 1);
        }
        set {
            setFloat("sfx_volume", value);
        }
    }
    /// <summary>
    /// The master music volume level.
    /// </summary>
    public static float musicVolume {
        get {
            return getFloat("music_volume", 1);
        }
        set {
            setFloat("music_volume", value);
        }
    }

    /// <summary>
    /// Gets the language code used.
    /// </summary>
    public static string localization {
        get {
            return getString("localization", "en-us");
        }
        set {
            setString("localization", value);
        }
    }
    
    #endregion

    #region Custom Settings

    /// <summary>
    /// Gets a property.  If the property doesn't exist, reterns the given defaultValue.
    /// </summary>
    public static string getString(string key, string defaultValue = "") {
        if (!properties.ContainsKey(key)) return defaultValue;
        return properties[key];
    }
    /// <summary>
    /// Gets a property as a boolean.  If the property doesn't exist, reterns the given defaultValue.
    /// </summary>
    public static bool getBool(string key, bool defaultValue = false) {
        if (!properties.ContainsKey(key)) return defaultValue;
        string str = properties[key];
        return !(str == "" || str == "0" || str.ToLower() == "false");
    }
    /// <summary>
    /// Gets a property as an int.  If the property doesn't exist, reterns the given defaultValue.
    /// </summary>
    public static int getInt(string key, int defaultValue = 0) {
        if (!properties.ContainsKey(key)) return defaultValue;
        string str = properties[key];
        int ret = defaultValue;
        if (int.TryParse(str, out ret))
            return ret;
        return defaultValue;
    }
    /// <summary>
    /// Gets a property as a float.  If the property doesn't exist, reterns the given defaultValue.
    /// </summary>
    public static float getFloat(string key, float defaultValue = 0) {
        if (!properties.ContainsKey(key)) return defaultValue;
        string str = properties[key];
        float ret = defaultValue;
        if (float.TryParse(str, out ret))
            return ret;
        return defaultValue;
    }
    /// <summary>
    /// Sets a property.
    /// </summary>
    public static void setString(string key, string value) {
        properties[key] = value;
    }
    /// <summary>
    /// Sets a bool property.
    /// </summary>
    public static void setBool(string key, bool value) {
        properties[key] = value ? "1" : "0";
    }
    /// <summary>
    /// Sets an int property.
    /// </summary>
    public static void setInt(string key, int value) {
        properties[key] = "" + value;
    }
    /// <summary>
    /// Sets a float property.
    /// </summary>
    public static void setFloat(string key, float value) {
        properties[key] = "" + value;
    }

    #endregion

    #region File Info

    /// <summary>
    /// Directory containing the initialization file.
    /// </summary>
    public static string fileDirectory {
        get {
            return Application.dataPath;
        }
    }
    /// <summary>
    /// The file name.
    /// </summary>
    public static string fileName = "settings.ini";
    /// <summary>
    /// The full file path, gotten from combining fileDirectory with fileName.  On windows this is in the [Game Name]_Data folder.
    /// </summary>
    public static string filePath {
        get {
            return Path.Combine(fileDirectory, fileName);
        }
    }

    #endregion

    #region Saving/Loading

    /// <summary>
    /// Loads initialization settings from the filePath.  If the file doesn't exist, creates a new one with the default settings.
    /// </summary>
    public static void loadFromFile() {

        loadDefaultSettings();
        if (File.Exists(filePath)) {
            string[] lines = File.ReadAllLines(filePath);

            // parsing lines
            properties.Clear();
            for (int i=0; i<lines.Length; i++) {
                string line = lines[i].Trim();
                if (line == "") continue;

                int index = line.IndexOf(':');
                if (index == -1) {
                    Debug.LogError("Could not parse initialization line " + line + ", no ':'");
                } else {
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();
                    properties[key] = value;
                }
            }

            fileLoaded = true;

        } else {
            Debug.Log("Initialization file " + filePath + " does not exist.  Creating new file.");
            saveToFile();
        }

    }

    /// <summary>
    /// Saves initialization settings to the filePath.
    /// </summary>
    public static void saveToFile() {

        StringBuilder sb = new StringBuilder();
        foreach (string key in properties.Keys) {
            sb.AppendLine(key + ": " + properties[key]);
        }

        try {
            File.WriteAllText(filePath, sb.ToString());
        } catch (System.Exception e) {
            Debug.LogError("Could not save initialization settings to " + filePath + ".  Exception: " + e.Message);
        }

    }

    #endregion

    #region Private

    private static Dictionary<string, string> properties = new Dictionary<string, string>();

    #endregion

}
