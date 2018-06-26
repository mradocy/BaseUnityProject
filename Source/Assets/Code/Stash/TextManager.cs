using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class TextManager {

    #region Getting Text

    /// <summary>
    /// Gets text from a file already loaded and stored in the dictionary.
    /// </summary>
    /// <param name="file">path/name of the file.  Use '/' to separate directories, don't include the file extension.</param>
    public static string getFile(string file) {
        string f = file.Replace('\\', '/');
        if (!files.ContainsKey(f)) {
            logError("File " + f + " not found or hasn't been loaded yet.");
            return "";
        }
        return files[f];
    }

    /// <summary>
    /// Gets a value of a given key from a file already loaded and stored in the dictionary.
    /// </summary>
    /// <param name="file">path/name of the file.  Use '/' to separate directories, don't include the file extension.</param>
    /// <param name="key">key of the property.</param>
    public static string getProperty(string file, string key) {
        string f = file.Replace('\\', '/');
        if (!properties.ContainsKey(f)) {
            logError("Properties file " + f + " not found or hasn't been loaded yet.");
            return "";
        }
        Dictionary<string, string> dFile = properties[f];
        if (!dFile.ContainsKey(key)) {
            logError("Properties file " + f + " does not contain the key " + key + ".");
            return "";
        }
        return dFile[key];
    }
    
    #endregion

    #region Setting Up

    /// <summary>
    /// Sets up the TextManager, loading all files and properties.  Assumes localization has already been set.
    /// Returns false if something went wrong.
    /// </summary>
    public static bool setUp() {
        if (!verifyPaths()) {
            return false;
        }
        clearAll();
        if (!loadAllFiles()) {
            return false;
        }
        if (!loadAllProperties()) {
            return false;
        }
        setUpCalled = true;
        return true;
    }

    /// <summary>
    /// If setUp() was already called and completed successfully.
    /// </summary>
    public static bool setUpCalled { get; private set; }

    #endregion

    #region File Paths

    /// <summary>
    /// Extenstion used for all files being loaded.
    /// </summary>
    public static string fileExtension = ".txt";
    /// <summary>
    /// Gets path to the text directory.
    /// </summary>
    public static string textPath {
        get {
            return Path.Combine(Application.streamingAssetsPath, "text");
        }
    }
    /// <summary>
    /// The localization code being used.
    /// </summary>
    public static string localizationCode {
        get {
            return InitializationSettings.localization;
        }
    }
    /// <summary>
    /// The directory of the localization being used.
    /// </summary>
    public static string localizationPath {
        get {
            return Path.Combine(textPath, localizationCode);
        }
    }
    /// <summary>
    /// Directory where files are being stored in the current localization.
    /// </summary>
    public static string filesPath {
        get {
            return Path.Combine(localizationPath, "files");
        }
    }
    /// <summary>
    /// Directory where properties are being stored in the current localization.
    /// </summary>
    public static string propertiesPath {
        get {
            return Path.Combine(localizationPath, "properties");
        }
    }
    /// <summary>
    /// Checks if textPath, localizationPath, filesPath, propertiesPath exist.  Returns true if they all exist, false otherwise.  Also logs an error if a path does not exist.
    /// </summary>
    /// <returns></returns>
    public static bool verifyPaths() {
        if (!Directory.Exists(textPath)) {
            logError("textPath " + textPath + " does not exist.");
            return false;
        }
        if (!Directory.Exists(localizationPath)) {
            logError("localizationPath " + localizationPath + " does not exist.");
            return false;
        }
        if (!Directory.Exists(filesPath)) {
            logError("filesPath " + filesPath + " does not exist.");
            return false;
        }
        if (!Directory.Exists(propertiesPath)) {
            logError("propertiesPath " + propertiesPath + " does not exist.");
            return false;
        }
        return true;
    }

    #endregion

    #region Loading Files

    /// <summary>
    /// Loads all .txt files in the files folder for the current localization.  Returns false if there was a problem.
    /// </summary>
    public static bool loadAllFiles() {
        string[] files;
        try {
            files = Directory.GetFiles(filesPath, "*" + fileExtension, SearchOption.AllDirectories);
        } catch (Exception e) {
            logError("Could not load " + filesPath + " files directory.  Exception: " + e.Message);
            return false;
        }
        if (files == null) {
            return true;
        }
        for (int i=0; i<files.Length; i++) {
            if (!loadFile(files[i])) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Loads all .txt files in the properties folder for the current localization.  Returns false if there was a problem.
    /// </summary>
    public static bool loadAllProperties() {
        string[] files;
        try {
            files = Directory.GetFiles(propertiesPath, "*" + fileExtension, SearchOption.AllDirectories);
        } catch (Exception e) {
            logError("Could not load " + propertiesPath + " properties directory.  Exception: " + e.Message);
            return false;
        }
        if (files == null) {
            return true;
        }
        for (int i = 0; i < files.Length; i++) {
            if (!loadProperties(files[i])) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Loads a file.  filesPath is automatically added before fileName when loading.
    /// Doesn't do anything if the file was already loaded (returns true).
    /// Returns true if file loaded sucessfully, false if there was a problem.
    /// </summary>
    /// <param name="fileName">name of the file to load.</param>
    public static bool loadFile(string fileName) {
        if (files.ContainsKey(fileName)) {
            logWarning("File " + fileName + " already loaded");
            return true;
        }
        string path = Path.Combine(filesPath, fileName);
        if (!File.Exists(path)) {
            logError("File " + path + " does not exist.");
        }

        string simpleName = path.Replace(filesPath + Path.DirectorySeparatorChar, "");
        simpleName = simpleName.Substring(0, simpleName.LastIndexOf('.'));
        simpleName = simpleName.Replace('\\', '/');

        try {
            files[simpleName] = File.ReadAllText(path);
        } catch (Exception e) {
            logError("Error reading from file " + path + ", exception: " + e.Message);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Loads a file containing properties.  propertiesPath is automatically added before fileName when loading.
    /// Doesn't do anything if the properties file was already loaded (returns true).
    /// Returns true if properties file loaded sucessfully, false if there was a problem.
    /// </summary>
    /// <param name="fileName">name of the properties file to load.</param>
    public static bool loadProperties(string fileName) {
        if (properties.ContainsKey(fileName)) {
            logWarning("Properties file " + fileName + " already loaded");
            return true;
        }
        string path = Path.Combine(propertiesPath, fileName);
        if (!File.Exists(path)) {
            logError("File " + path + " does not exist.");
        }
        string[] lines;
        try {
            lines = File.ReadAllLines(path);
        } catch (Exception e) {
            logError("Error reading from properties file " + path + ", exception: " + e.Message);
            return false;
        }
        
        string simpleName = path.Replace(propertiesPath + Path.DirectorySeparatorChar, "");
        simpleName = simpleName.Substring(0, simpleName.LastIndexOf('.'));
        simpleName = simpleName.Replace('\\', '/');

        if (!properties.ContainsKey(simpleName)) {
            properties[simpleName] = new Dictionary<string, string>();
        }
        Dictionary<string, string> propDic = properties[simpleName];
        if (lines == null) {
            return true;
        }
        for (int i=0; i<lines.Length; i++) {
            string line = lines[i].Trim();
            if (line == "") continue;
            int index = line.IndexOf(':');
            if (index == -1) {
                logError("Line in properties file " + path + " could not be parsed, ':' character is needed.");
                return false;
            }
            string key = line.Substring(0, index).Trim();
            string value = line.Substring(index + 1).Trim();
            
            if (propDic.ContainsKey(key)) {
                logWarning("Properties " + simpleName + " already contains key " + key + ".");
            }
            propDic[key] = value;
        }
        return true;
    }

    #endregion

    #region Private
    
    private static void clearAll() {
        files.Clear();
        foreach (string key in properties.Keys) {
            if (properties[key] == null)
                continue;
            properties[key].Clear();
        }
        properties.Clear();
    }
    
    private static void logWarning(string message) {
        Debug.LogWarning(message);
    }
    private static void logError(string message) {
        Debug.LogError(message);
    }

    private static Dictionary<string, string> files = new Dictionary<string, string>();

    private static Dictionary<string, Dictionary<string, string>> properties = new Dictionary<string, Dictionary<string, string>>();

    #endregion


}
