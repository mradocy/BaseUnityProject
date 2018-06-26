using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;

/*


SaveManager.save(); // save to file
SaveManager.save(int fileIndex); // save to a file with the given file index
SaveManager.load(); // load the file last saved
SaveManager.load(int fileIndex); // load the file with the specified file index
SaveManager.createNew();




    */



public class SaveManager {

    /// <summary>
    /// Initializes SaveManager so it can be used.  Provides default values.
    /// Returns the saveData provided as type T.
    /// </summary>
    /// <typeparam name="T">Type of the saveData, extending BaseSaveData.</typeparam>
    /// <param name="saveData">The instance of type T which will store the data.</param>
    public static T initialize<T>(T saveData) where T : BaseSaveData {
        return initialize(saveData, Application.persistentDataPath, "data", ".sav");
    }

    /// <summary>
    /// Initializes SaveManager so it can be used.
    /// Returns the saveData provided as type T.
    /// </summary>
    /// <typeparam name="T">Type of the saveData, extending BaseSaveData.</typeparam>
    /// <param name="saveData">The instance of type T which will store the data.</param>
    /// <param name="fileDirectory">The directory storing the save files.</param>
    /// <param name="fileBaseName">The base name of the save files.</param>
    /// <param name="fileExtension">The extension the save files will have (e.g. ".sav")</param>
    public static T initialize<T>(T saveData, string fileDirectory, string fileBaseName, string fileExtension) where T : BaseSaveData {
        if (initialized) {
            Debug.LogError("Cannot initialize SaveManager once already initialized.");
            return null;
        }
        baseSaveData = saveData;
        SaveManager.fileDirectory = fileDirectory;
        SaveManager.fileBaseName = fileBaseName;
        SaveManager.fileExtension = fileExtension;
        fileIndex = 0;
        initialized = true;
        return saveData;
    }

    /// <summary>
    /// If the SaveManager was initialized.
    /// </summary>
    public static bool initialized { get; private set; }

    /// <summary>
    /// Getter to the save data as base type BaseSaveData.
    /// </summary>
    public static BaseSaveData baseSaveData { get; private set; }

    /// <summary>
    /// Gets the save data.  To be accessed/modified as type T (i.e. the type used in initialize()).
    /// </summary>
    public static T data<T>() where T : BaseSaveData {
        return baseSaveData as T;
    }
    
    #region Saving and Creating

    /// <summary>
    /// Function signature for the callback when saving.
    /// </summary>
    /// <param name="status">Status of the save.</param>
    public delegate void SaveCallback(SaveStatus status);

    public enum SaveStatus {
        /// <summary>
        /// File saved with no problems.
        /// </summary>
        OK,
        /// <summary>
        /// Cannot save because SaveManager is already saving a file.
        /// </summary>
        ALREADY_SAVING,
        /// <summary>
        /// Had a problem writing to a file.
        /// </summary>
        PROBLEM_WRITING_TO_FILE,
    }

    /// <summary>
    /// If a save is currently being written to a file.  Cannot call save() while this is true.
    /// </summary>
    public static bool currentlySaving { get; private set; }
    
    /// <summary>
    /// Saves the current data to a file with the current fileIndex.
    /// </summary>
    /// <param name="callback">Function to be called once the save operation is completed.</param>
    public static void save(SaveCallback callback) {

        save(fileIndex, callback);

    }
    
    /// <summary>
    /// Saves the current data to a file with the given fileIndex, then switches to that fileIndex.
    /// </summary>
    /// <param name="fileIndex">File to save the content to.</param>
    /// <param name="callback">Function to be called once the save operation is completed.</param>
    public static void save(int fileIndex, SaveCallback callback) {

        if (!initialized) {
            logNotInitializedError();
            return;
        }

        if (fileIndex < 0) {
            logError("Invalid file index " + fileIndex);
            return;
        }

        // don't save if already saving
        if (currentlySaving || debugForceSaveStatusEquals(SaveStatus.ALREADY_SAVING)) {
            callback(SaveStatus.ALREADY_SAVING);
            return;
        }
        currentlySaving = true;
        
        saveFuncFilePath = getFilePath(fileIndex);
        saveFuncFileIndex = fileIndex;
        saveFuncContent = baseSaveData.saveToString();
        saveFuncCallback = callback;
        
        System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(saveFunc);
        System.Threading.Thread newThread = new System.Threading.Thread(threadDelegate);
        newThread.Start();
    }

    /// <summary>
    /// Creates a new save with a unique file index and saves it to a file.  You are then editing this new save file.
    /// </summary>
    public static void createNew(SaveCallback callback) {

        if (!initialized) {
            logNotInitializedError();
            return;
        }

        // determine unique fileIndex
        int[] takenIndices = getAvailableSaveFiles();
        int index = 0;
        while (Array.IndexOf(takenIndices, index) != -1) {
            index++;
        }

        // create new 
        baseSaveData.clearDefault();
        fileIndex = index;

        save(fileIndex, callback);
    }
    
    /// <summary>
    /// Used for debug purposes.  Can force the callback upon saving to have an error message.
    /// Set to SaveStatus.OK to disable this feature.
    /// Feature is automatically disabled if BuildSettings.debug is false.
    /// </summary>
    public static SaveStatus debugForceSaveStatus = SaveStatus.OK;

    #endregion
    
    #region Loading

    /// <summary>
    /// Function signature for the callback when loading.
    /// </summary>
    /// <param name="status">Status of the save.</param>
    public delegate void LoadCallback(LoadStatus status);

    public enum LoadStatus {
        /// <summary>
        /// File loaded with no problems.
        /// </summary>
        OK,
        /// <summary>
        /// Cannot load because SaveManager is already loading a file.
        /// </summary>
        ALREADY_LOADING,
        /// <summary>
        /// Cannot load because file doesn't exist.
        /// </summary>
        FILE_DOES_NOT_EXIST,
        /// <summary>
        /// Had a problem reading from a file.
        /// </summary>
        PROBLEM_READING_FROM_FILE,
        /// <summary>
        /// The data had a problem parsing the file's content.
        /// </summary>
        PROBLEM_PARSING_CONTENT,
    }

    /// <summary>
    /// If a save is currently being written to a file.  Cannot call save() while this is true.
    /// </summary>
    public static bool currentlyLoading { get; private set; }

    /// <summary>
    /// If there's currently at least 1 save file available.
    /// </summary>
    public static bool isSaveFileCreated() {
        return getAvailableSaveFiles().Length > 0;
    }
    /// <summary>
    /// If there exists a save file at the specified file index.
    /// </summary>
    public static bool isSaveFileCreated(int fileIndex) {
        return File.Exists(getFilePath(fileIndex));
    }

    /// <summary>
    /// Loads the current data to a file with the current fileIndex.
    /// </summary>
    /// <param name="callback">Function to be called once the load operation is completed.</param>
    public static void load(LoadCallback callback) {

        load(fileIndex, callback);

    }

    /// <summary>
    /// Loads content from a file with the given fileIndex into the current data, then switches to that fileIndex.
    /// </summary>
    /// <param name="fileIndex">File to load the content from.</param>
    /// <param name="callback">Function to be called once the load operation is completed.</param>
    public static void load(int fileIndex, LoadCallback callback) {

        if (!initialized) {
            logNotInitializedError();
            return;
        }

        if (fileIndex < 0) {
            logError("Invalid file index " + fileIndex);
            return;
        }

        // don't load if already loading
        if (currentlyLoading || debugForceLoadStatusEquals(LoadStatus.ALREADY_LOADING)) {
            callback(LoadStatus.ALREADY_LOADING);
            return;
        }

        loadFuncFilePath = getFilePath(fileIndex);
        loadFuncFileIndex = fileIndex;
        loadFuncCallback = callback;
        
        if (!isSaveFileCreated(fileIndex) || debugForceLoadStatusEquals(LoadStatus.FILE_DOES_NOT_EXIST)) {
            callback(LoadStatus.FILE_DOES_NOT_EXIST);
            return;
        }

        currentlyLoading = true;

        // not doing threading for loading, too risky
        loadFunc();

    }

    /// <summary>
    /// Loads the file that was saved last.
    /// </summary>
    /// <param name="callback">Function to be called once the load operation is completed.</param>
    public static void loadLatest(LoadCallback callback) {

        load(getSaveFileLastSaved(), callback);

    }

    /// <summary>
    /// Used for debug purposes.  Can force the callback upon saving to have an error message.
    /// Set to LoadStatus.OK to disable this feature.
    /// Feature is automatically disabled if BuildSettings.debug is false.
    /// </summary>
    public static LoadStatus debugForceLoadStatus = LoadStatus.OK;

    #endregion
    
    #region Deleting

    /// <summary>
    /// Function signature for the callback when deleting a file.
    /// </summary>
    /// <param name="status">Status of the deletion.</param>
    public delegate void DeleteCallback(DeleteStatus status);

    public enum DeleteStatus {
        /// <summary>
        /// File deleted with no problems.
        /// </summary>
        OK,
        /// <summary>
        /// Cannot delete because SaveManager is already deleting a file.
        /// </summary>
        ALREADY_DELETING,
        /// <summary>
        /// Had a problem deleting a file.
        /// </summary>
        PROBLEM_DELETING_FILE,
    }

    /// <summary>
    /// If currently deleting a file.  Cannot call delete() while this is true.
    /// </summary>
    public static bool currentlyDeleting { get; private set; }

    /// <summary>
    /// Deletes the file with the given fileIndex.
    /// </summary>
    /// <param name="fileIndex">File to be deleted.</param>
    /// <param name="callback">Function to be called once the delete operation is completed.</param>
    public static void delete(int fileIndex, DeleteCallback callback) {
        
        if (!initialized) {
            logNotInitializedError();
            return;
        }

        if (fileIndex < 0) {
            logError("Invalid file index " + fileIndex);
            return;
        }

        // don't delete if already deleting
        if (currentlyDeleting || debugForceDeleteStatusEquals(DeleteStatus.ALREADY_DELETING)) {
            callback(DeleteStatus.ALREADY_DELETING);
            return;
        }
        currentlyDeleting = true;

        deleteFuncFilePaths.Clear();
        deleteFuncFilePaths.Add(getFilePath(fileIndex));
        deleteFuncCallback = callback;

        System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(deleteFunc);
        System.Threading.Thread newThread = new System.Threading.Thread(threadDelegate);
        newThread.Start();
    }

    /// <summary>
    /// Deletes all save files.
    /// </summary>
    /// <param name="callback">Function to be called once the delete operation is completed.</param>
    public static void deleteAll(DeleteCallback callback) {

        if (!initialized) {
            logNotInitializedError();
            return;
        }
        
        // don't delete if already deleting
        if (currentlyDeleting || debugForceDeleteStatusEquals(DeleteStatus.ALREADY_DELETING)) {
            callback(DeleteStatus.ALREADY_DELETING);
            return;
        }
        currentlyDeleting = true;

        deleteFuncFilePaths.Clear();
        int[] indeces = getAvailableSaveFiles();
        foreach (int index in indeces) {
            deleteFuncFilePaths.Add(getFilePath(index));
        }
        deleteFuncCallback = callback;

        System.Threading.ThreadStart threadDelegate = new System.Threading.ThreadStart(deleteFunc);
        System.Threading.Thread newThread = new System.Threading.Thread(threadDelegate);
        newThread.Start();

    }

    /// <summary>
    /// Used for debug purposes.  Can force the callback upon deleting to have an error message.
    /// Set to DeleteStatus.OK to disable this feature.
    /// Feature is automatically disabled if BuildSettings.debug is false.
    /// </summary>
    public static DeleteStatus debugForceDeleteStatus = DeleteStatus.OK;

    #endregion

    #region File Info

    /// <summary>
    /// Gets the index of the save file currently loaded.
    /// </summary>
    public static int fileIndex { get; private set; }

    /// <summary>
    /// Gets a list of indeces of save files in the fileDirectory.
    /// </summary>
    public static int[] getAvailableSaveFiles() {
        string[] paths = Directory.GetFiles(fileDirectory, "*" + fileExtension);
        List<int> ret = new List<int>();
        foreach (string path in paths) {
            int index = getFileIndexFromPath(path);
            if (index == -1)
                continue;
            ret.Add(index);
        }
        return ret.ToArray();
    }

    /// <summary>
    /// Gets the date the given save file was last saved (written to).
    /// </summary>
    /// <param name="fileIndex">Index of the save file to check.</param>
    public static DateTime getSaveDate(int fileIndex) {
        string saveFileName = getFilePath(fileIndex);
        if (!File.Exists(saveFileName)) {
            return new DateTime();
        }
        return File.GetLastWriteTime(saveFileName);
    }

    /// <summary>
    /// Gets the fileIndex of the save file (from the fileDirectory) that was last modified, or -1 if no file exists.
    /// </summary>
    public static int getSaveFileLastSaved() {
        int[] fileIndices = getAvailableSaveFiles();
        if (fileIndices.Length == 0)
            return -1;
        int ret = 0;
        DateTime lastModifiedTime = getSaveDate(ret);
        foreach (int fileIndex in fileIndices) {
            DateTime dt = getSaveDate(fileIndex);
            if (dt > lastModifiedTime) {
                ret = fileIndex;
                lastModifiedTime = dt;
            }
        }
        return ret;
    }

    /// <summary>
    /// Directory containing the save files.
    /// </summary>
    public static string fileDirectory { get; private set; }
    /// <summary>
    /// The file name (trimmed of the index) without the extension.
    /// </summary>
    public static string fileBaseName { get; private set; }
    /// <summary>
    /// The extension of the file.  Should contain a period as the first character.
    /// </summary>
    public static string fileExtension { get; private set; }

    /// <summary>
    /// Returns what the file path would be for a file with the given fileIndex.  Essentially combines fileIndex with SaveBox's fileDirectory, fileBaseName, fileExtension.
    /// </summary>
    /// <param name="fileIndex"></param>
    public static string getFilePath(int fileIndex) {
        return Path.Combine(fileDirectory, fileBaseName + fileIndex + fileExtension);
    }

    #endregion

    #region Private

    private static void logError(string message) {
        Debug.LogError(message);
    }

    private static void logNotInitializedError() {
        logError("Cannot use SaveManager until SaveManager.initialize() is called.");
    }

    /// <summary>
    /// Return the file index of a given file path.  Returns -1 if file is invalid or file index could not be found.
    /// </summary>
    /// <param name="filePath"></param>
    private static int getFileIndexFromPath(string filePath) {
        string preStr = Path.Combine(fileDirectory, fileBaseName);
        int index = filePath.IndexOf(preStr);
        if (index != 0)
            return -1;
        index = preStr.Length;
        int index2 = filePath.IndexOf(fileExtension, index);
        if (index2 == -1)
            return -1;
        string fileIndexStr = filePath.Substring(index, index2 - index);
        int fileIndex = -1;
        bool success = int.TryParse(fileIndexStr, out fileIndex);
        if (!success)
            return -1;
        return fileIndex;
    }

    /// <summary>
    /// Save function called in another thread
    /// </summary>
    private static void saveFunc() {
        
        SaveStatus retStatus = SaveStatus.OK;

        try {

            if (debugForceSaveStatusEquals(SaveStatus.PROBLEM_WRITING_TO_FILE)) {
                System.Threading.Thread.Sleep(1000);
                throw new IOException();
            }

            StringBuilder fullContent = addHeader(saveFuncContent);
            
            File.WriteAllText(saveFuncFilePath, fullContent.ToString());
            retStatus = SaveStatus.OK;

        } catch {

            retStatus = SaveStatus.PROBLEM_WRITING_TO_FILE;

        } finally {

            fileIndex = saveFuncFileIndex;
            saveFuncCallback(retStatus);
            currentlySaving = false;

        }

    }
    private static string saveFuncFilePath = "";
    private static int saveFuncFileIndex = -1;
    private static string saveFuncContent = "";
    private static SaveCallback saveFuncCallback = null;
    private static bool debugForceSaveStatusEquals(SaveStatus saveStatus) {
        if (!UDeb.debug) return false;
        return saveStatus == debugForceSaveStatus;
    }

    /// <summary>
    /// Delete function called in another thread
    /// </summary>
    private static void deleteFunc() {
        
        DeleteStatus retStatus = DeleteStatus.OK;

        try {

            if (debugForceDeleteStatusEquals(DeleteStatus.PROBLEM_DELETING_FILE)) {
                System.Threading.Thread.Sleep(1000);
                throw new IOException();
            }

            foreach (string filePath in deleteFuncFilePaths) {
                File.Delete(filePath);
            }
            retStatus = DeleteStatus.OK;

        } catch {

            retStatus = DeleteStatus.PROBLEM_DELETING_FILE;

        } finally {

            deleteFuncCallback(retStatus);
            currentlyDeleting = false;

        }

    }
    private static List<string> deleteFuncFilePaths = new List<string>();
    private static DeleteCallback deleteFuncCallback = null;
    private static bool debugForceDeleteStatusEquals(DeleteStatus deleteStatus) {
        if (!UDeb.debug) return false;
        return deleteStatus == debugForceDeleteStatus;
    }

    /// <summary>
    /// Load function.  Not doing threading for loading, too risky.
    /// </summary>
    private static void loadFunc() {

        LoadStatus retStatus = LoadStatus.OK;
        bool contentLoaded = false;
        
        try {

            if (debugForceLoadStatusEquals(LoadStatus.PROBLEM_READING_FROM_FILE)) {
                System.Threading.Thread.Sleep(1000);
                throw new IOException();
            }

            loadFuncContent = File.ReadAllText(loadFuncFilePath);

            contentLoaded = true;
            retStatus = LoadStatus.OK;

        } catch {

            retStatus = LoadStatus.PROBLEM_READING_FROM_FILE;

        } finally {

            fileIndex = loadFuncFileIndex;

            if (contentLoaded) {

                // parse content
                bool success = false;
                string trimmedContent = trimHeader(loadFuncContent, out success);
                if (!success || debugForceLoadStatusEquals(LoadStatus.PROBLEM_PARSING_CONTENT)) {
                    retStatus = LoadStatus.PROBLEM_PARSING_CONTENT;
                }
                baseSaveData.loadFromString(trimmedContent);
            }

            loadFuncCallback(retStatus);
            currentlyLoading = false;

        }

    }
    private static string loadFuncFilePath = "";
    private static int loadFuncFileIndex = -1;
    private static string loadFuncContent = "";
    private static LoadCallback loadFuncCallback = null;
    private static bool debugForceLoadStatusEquals(LoadStatus loadStatus) {
        if (!UDeb.debug) return false;
        return loadStatus == debugForceLoadStatus;
    }

    private static StringBuilder addHeader(string content) {
        StringBuilder sb = new StringBuilder();

        // checksum
        int checksum = 0;
        for (int i=0; i < content.Length; i++) {
            checksum += content[i];
        }
        sb.Append(checksum);

        // close header
        sb.Append("|");

        // append content
        sb.Append(content);

        return sb;
    }

    private static string trimHeader(string str, out bool success) {

        success = false;

        // get header
        int index = str.IndexOf("|");
        if (index == -1) {
            return "";
        }
        int contentIndex = index + 1;

        // get checksum
        int checksum = 0;
        if (!int.TryParse(str.Substring(0, index), out checksum)) {
            return "";
        }

        // validate checksum
        int c = 0;
        for (int i=contentIndex; i < str.Length; i++) {
            c += str[i];
        }
        if (c != checksum) {
            success = false;
            return "";
        }

        success = true;
        return str.Substring(contentIndex);

    }

    #endregion

}
