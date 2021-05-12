using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity.SaveData {

    /// <summary>
    /// The root of all save groups.
    /// </summary>
    public sealed class SaveRoot : SaveGroup {

        /// <summary>
        /// Constructor providing default values.
        /// </summary>
        public SaveRoot() : this("data{0}.sav", System.Guid.Empty) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">The formattable name of a save file, including the extension (e.g. "data{0}.sav").  Can include "{0}" to represent an index number.</param>
        /// <param name="compatibilityId">The <see cref="CompatibilityId"/> expected from the save data.</param>
        public SaveRoot(string fileName, System.Guid compatibilityId) : base("<Root>", null) {
            if (string.IsNullOrEmpty(fileName))
                throw new System.ArgumentNullException(nameof(fileName));
            _saveFileName = fileName;

            this.CompatibilityId = compatibilityId;
        }

        public override SaveRoot Root => this;

        #region File Properties

        /// <summary>
        /// Encoding used for file operations.
        /// </summary>
        public static readonly System.Text.Encoding Encoding = new System.Text.UTF8Encoding(false);

        /// <summary>
        /// The full path of the directory reserved for save files.  Is contained in Unity's <see cref="Application.persistentDataPath"/>.
        /// </summary>
        public static string SaveDirectory => Path.Combine(Application.persistentDataPath, "SaveData");

        /// <summary>
        /// Gets the file name for a save file at the given index (includes extension, does not include path)
        /// </summary>
        /// <param name="index">File index</param>
        public string GetSaveFileName(int index) {
            return string.Format(_saveFileName, index);
        }

        /// <summary>
        /// Gets the full file path for data saved to the <see cref="SaveDirectory"/>.
        /// </summary>
        /// <param name="index">File index</param>
        public string GetSavePath(int index) {
            return Path.Combine(SaveDirectory, this.GetSaveFileName(index));
        }

        /// <summary>
        /// Gets the full file path for the backup of a given file.
        /// </summary>
        /// <param name="savePath">Original save path.</param>
        public static string GetBackupPath(string savePath) {
            int extIndex = savePath.LastIndexOf('.');
            if (extIndex == -1) {
                return savePath + "_backup";
            }
            return $"{savePath.Substring(0, extIndex)}_backup{savePath.Substring(extIndex)}";
        }

        private string _saveFileName;

        #endregion

        #region Loading

        private const string _startLoadFileMessage = "Started loading save data from \"{0}\".";
        private const string _loadFileSuccessMessage = "Loaded save data from \"{0}\" successfully.";
        private const string _loadFileFailMessage = "Error loading save data from \"{0}\": {1}";
        private const string _loadStringSuccessMessage = "Loaded save data from a string successfully.";
        private const string _loadStringFailMessage = "Error loading save data from a string: {0}";

        /// <summary>
        /// Event that's invoked immediately before loading save data from a string or file.
        /// This is done before verification, so it's possible the save data isn't loaded after this event is invoked.
        /// </summary>
        public event UnityAction<SaveRoot> PreLoad;

        /// <summary>
        /// Event that's invoked immediately after xml is sucessfully loaded and parsed.
        /// </summary>
        public event UnityAction<SaveRoot> Loaded;

        /// <summary>
        /// Gets the compatibility id expected from the save data.  If this does not match the save data's compatibility id, the data is considered incompatible and cannot be used.
        /// </summary>
        public System.Guid CompatibilityId { get; }

        /// <summary>
        /// Gets if the save data has been parsed already.  If so, then no more properties can be registered.
        /// This becomes true when data gets loaded.
        /// </summary>
        public bool IsParsed { get; private set; }

        /// <summary>
        /// Gets if save data was successfully loaded from or saved to a file at least once.
        /// </summary>
        public bool IsDataCached => _cachedFileString != null;

        // TODO: Does this need to be public?  Should loading from an index in the save directory be the only option?

        /// <summary>
        /// Loads an xml file, the contents of which are then parsed.
        /// Returns the status of the load.
        /// </summary>
        /// <param name="path">Path to the xml file.</param>
        /// <returns>LoadStatus</returns>
        public LoadStatus LoadFromFile(string path) {
            LoadStatus loadStatus = LoadStatus.Ok;

            // invoke preload
            this.PreLoad?.Invoke(this);

            // load file
            XmlDocument xmlDoc = new XmlDocument();
            try {
                using (XmlReader reader = XmlReader.Create(path, _readerSettings)) {
                    xmlDoc.Load(reader);
                }
            } catch (FileNotFoundException) {
                loadStatus = LoadStatus.FileNotFound;
            } catch (XmlException) {
                loadStatus = LoadStatus.ParseError;
            } catch (System.Exception) {
                loadStatus = LoadStatus.FileCouldNotBeRead;
            }

            // parse file
            if (loadStatus == LoadStatus.Ok) {

                loadStatus = this.ParseXML(xmlDoc);

                if (loadStatus == LoadStatus.Ok) {
                    // set cached file string after successful load
                    string str = WriteXmlToString(xmlDoc, false);
                    if (str == null) {
                        loadStatus = LoadStatus.ParseError;
                    } else {
                        _cachedFileString = str;
                    }
                }
            }

            // log
            if (loadStatus == LoadStatus.Ok) {
                Debug.Log(string.Format(_loadFileSuccessMessage, path));
            } else {
                Debug.LogError(string.Format(_loadFileFailMessage, path, loadStatus));
            }

            return loadStatus;
        }

        /// <summary>
        /// Loads a file from <see cref="SaveDirectory"/>.
        /// </summary>
        /// <param name="index">File index</param>
        /// <returns>Load status</returns>
        public LoadStatus LoadFromSaveDirectory(int index) {
            return this.LoadFromFile(this.GetSavePath(index));
        }

        /// <summary>
        /// Loads the data from a string.
        /// Returns the status of the load.
        /// </summary>
        /// <param name="str">String to load.</param>
        /// <returns>LoadStatus</returns>
        public LoadStatus LoadFromString(string str) {
            LoadStatus loadStatus = LoadStatus.Ok;

            // invoke preload
            this.PreLoad?.Invoke(this);

            // load from string
            XmlDocument xmlDoc = new XmlDocument();
            using (StringReader stringReader = new StringReader(str))
            using (XmlReader xmlReader = XmlReader.Create(stringReader, _readerSettings)) {
                try {
                    xmlDoc.Load(xmlReader);
                } catch (XmlException) {
                    loadStatus = LoadStatus.ParseError;
                } catch (System.Exception) {
                    loadStatus = LoadStatus.FileCouldNotBeRead;
                }
            }

            // parse xml
            if (loadStatus == LoadStatus.Ok) {
                loadStatus = this.ParseXML(xmlDoc);
            }

            // log
            if (loadStatus == LoadStatus.Ok) {
                Debug.Log(string.Format(_loadStringSuccessMessage));
            } else {
                Debug.LogError(string.Format(_loadStringFailMessage, loadStatus));
            }

            return loadStatus;
        }

        /// <summary>
        /// Loads data from cached save data that was last loaded from or saved to a file (this cached data only changes if the load/save operation was a success).
        /// This is essentially the same as reloading a save file, without needing to read from the actual file.
        /// Ensure that <see cref="IsDataCached"/> is true before calling.
        /// </summary>
        public LoadStatus LoadFromCachedData() {
            if (!this.IsDataCached) {
                return LoadStatus.CacheDoesNotExist;
            }

            return this.LoadFromString(_cachedFileString);
        }

        /// <summary>
        /// Loads from the data properties' default values.  The same as calling <see cref="SaveGroup.ResetToDefault"/>, but treats the action as a load.
        /// This means that the <see cref="PreLoad"/> and <see cref="Loaded"/> events are invoked.
        /// </summary>
        public LoadStatus LoadFromDefault() {
            this.PreLoad?.Invoke(this);

            this.ResetToDefault();
            this.IsParsed = true;

            this.Loaded?.Invoke(this);

            return LoadStatus.Ok;
        }

        /// <summary>
        /// Gets if a file in the <see cref="SaveDirectory"/> exists at the given index.
        /// </summary>
        /// <param name="index">File index</param>
        /// <returns>File exists</returns>
        public bool SaveFileExists(int index) {
            return File.Exists(this.GetSavePath(index));
        }

        #endregion

        #region Loading - Private

        /// <summary>
        /// Parses an <see cref="XmlDocument"/> object.
        /// Returns the status of the load.
        /// </summary>
        /// <param name="xmlDoc">The document to parse.</param>
        /// <returns>LoadStatus</returns>
        private LoadStatus ParseXML(XmlDocument xmlDoc) {
            if (xmlDoc == null) {
                return LoadStatus.ParseError;
            }

            // top node should always be <Root>
            XmlNode rootNode = xmlDoc.FirstChild;
            if (rootNode?.Name != "Root") {
                return LoadStatus.ParseError;
            }

            // verify compatibility
            XmlAttribute compatAttr = rootNode.Attributes?["compatibilityId"];
            System.Guid compatId;
            if (!System.Guid.TryParse(compatAttr?.Value, out compatId)) {
                compatId = System.Guid.Empty;
            }
            if (compatId != this.CompatibilityId) {
                return LoadStatus.CompatibilityError;
            }

            // clear existing data
            this.ResetToDefault();

            // parse node
            LoadStatus status = this.ParseXML(rootNode);

            this.IsParsed = true;

            if (status == LoadStatus.Ok) {
                this.Loaded?.Invoke(this);
            }

            return status;
        }

        /// <summary>
        /// The settings for xml readers.
        /// </summary>
        private readonly XmlReaderSettings _readerSettings = new XmlReaderSettings() {
            IgnoreComments = true
        };

        #endregion

        #region Saving

        private const string _startSaveMessage = "Started saving to \"{0}\".";
        private const string _saveSuccessMessage = "Saved data to \"{0}\" successfully.";
        private const string _saveFailMessage = "Error saving data to \"{0}\": {1}";

        /// <summary>
        /// Event invoked immediately before save root is saved to a string.
        /// </summary>
        public event UnityAction<SaveRoot> PreSave;

        /// <summary>
        /// If data is currently being saved.
        /// </summary>
        public bool IsSaving { get; private set; }

        /// <summary>
        /// Gets or sets the warning comment to place at the top of the .xml file.
        /// Default is null (no comment will be added).
        /// This does NOT get overridden when loading a save file.
        /// </summary>
        public string WarningComment { get; set; }

        /// <summary>
        /// Saves the data to a string.
        /// Returns null if there was a problem.
        /// </summary>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <returns>string</returns>
        public string SaveToString(bool prettyPrint) {
            this.PrepareForSaveToString();
            return this.SaveToStringAfterPrepared(prettyPrint);
        }

        /// <summary>
        /// Saves the data to the specified file.
        /// Returns the status of the save.
        /// </summary>
        /// <param name="path">Path to save the data to.</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <returns>Save status.</returns>
        public SaveStatus SaveToFile(string path, bool prettyPrint) {
            SaveStatus saveStatus = SaveStatus.Ok;

            string str = this.SaveToString(prettyPrint);
            if (str == null) {
                saveStatus = SaveStatus.StringError;
            }

            // save to temporary file first
            string tempPath = null;
            if (saveStatus == SaveStatus.Ok) {
                try {
                    tempPath = Path.GetTempFileName();
                } catch (IOException) {
                    saveStatus = SaveStatus.IOError;
                }
                if (tempPath != null) {
                    try {
                        File.WriteAllText(tempPath, str, Encoding);
                    } catch (System.Exception) {
                        saveStatus = SaveStatus.IOError;
                    }
                }
            }

            // override previous file if successful
            if (saveStatus == SaveStatus.Ok) {
                string backupPath = GetBackupPath(path);
                try {
                    if (File.Exists(path)) {
                        File.Replace(tempPath, path, backupPath);
                    } else {
                        File.Copy(tempPath, path);
                    }
                } catch (System.Exception) {
                    saveStatus = SaveStatus.IOError;
                }
            }

            // set cached file string after successful save
            if (saveStatus == SaveStatus.Ok) {
                _cachedFileString = str;
            }

            if (saveStatus == SaveStatus.Ok) {
                Debug.Log(string.Format(_saveSuccessMessage, path));
            } else {
                Debug.LogError(string.Format(_saveFailMessage, path, saveStatus));
            }

            return saveStatus;
        }

        /// <summary>
        /// Saves the data to <see cref="SaveDirectory"/>.
        /// </summary>
        /// <param name="index">File index</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <returns>Save status</returns>
        public SaveStatus SaveToSaveDirectory(int index, bool prettyPrint) {
            if (!Directory.Exists(SaveDirectory)) {
                Directory.CreateDirectory(SaveDirectory);
            }

            return this.SaveToFile(this.GetSavePath(index), prettyPrint);
        }

        /// <summary>
        /// Calls StartCoroutine() on the <paramref name="coroutineStarter"/>, which will save data to the given <paramref name="path"/> in another thread,
        /// then call <paramref name="callback"/> in the main Unity thread on completion, even if there was an error.
        /// Save data is cached immediately when this method is called.  Writing to the file from the cached data is done asyncronously.
        /// </summary>
        /// <param name="coroutineStarter">The MonoBehaviour that will start the coroutine.</param>
        /// <param name="path">Path to save the data to.</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <param name="callback">Callback function to call once the save is complete.  The given <see cref="SaveStatus"/> param is the status of the save.</param>
        public void SaveToFileCoroutine(MonoBehaviour coroutineStarter, string path, bool prettyPrint, UnityAction<SaveStatus> callback) {
            if (coroutineStarter == null) {
                throw new System.ArgumentNullException(nameof(coroutineStarter));
            }
            if (callback == null) {
                throw new System.ArgumentNullException(nameof(callback));
            }

            if (this.IsSaving) {
                Debug.LogError(string.Format(_saveFailMessage, path, SaveStatus.AlreadySaving));
                callback(SaveStatus.AlreadySaving);
                return;
            }
            this.IsSaving = true;

            // get ready for save in main thread
            this.PrepareForSaveToString();

            // tell starter to start the coroutine
            coroutineStarter.StartCoroutine(this.SaveToFileCoroutineAfterPrepared(path, prettyPrint, callback));
        }

        /// <summary>
        /// Calls StartCoroutine() on the <paramref name="coroutineStarter"/>, which will save data to the data to <see cref="SaveDirectory"/> in another thread,
        /// then call <paramref name="callback"/> in the main Unity thread on completion, even if there was an error.
        /// A string version of the data is saved immediately when this method is called.  Writing the string to the file is done asyncronously.
        /// </summary>
        /// <param name="coroutineStarter">The MonoBehaviour that will start the coroutine.</param>
        /// <param name="index">File index</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <param name="callback">Callback function to call once the save is complete.  The given <see cref="SaveStatus"/> param is the status of the save.</param>
        public void SaveToSaveDirectoryCoroutine(MonoBehaviour coroutineStarter, int index, bool prettyPrint, UnityAction<SaveStatus> callback) {
            if (!Directory.Exists(SaveDirectory)) {
                Directory.CreateDirectory(SaveDirectory);
            }

            this.SaveToFileCoroutine(coroutineStarter, this.GetSavePath(index), prettyPrint, callback);
        }

        #endregion

        #region Saving - Private

        /// <summary>
        /// Takes necessary steps before saving to a string.
        /// Includes invoking <see cref="PreSave"/> and caching values.
        /// </summary>
        private void PrepareForSaveToString() {

            this.PreSave?.Invoke(this);

            // cache values, preparing for writing xml files
            this.CacheValue();
        }

        /// <summary>
        /// Saves the data to a string.  Make sure <see cref="PrepareForSaveToString"/> was called first!
        /// Can be done asyncronously.
        /// Returns null if there was a problem.
        /// </summary>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <returns>string</returns>
        private string SaveToStringAfterPrepared(bool prettyPrint) {
            // create xml document, starting with root node
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = this.CreateXML(xmlDoc, true);
            xmlDoc.AppendChild(root);

            // create warning comment at top
            if (!string.IsNullOrEmpty(this.WarningComment)) {
                XmlComment warningComment = xmlDoc.CreateComment(this.WarningComment);
                xmlDoc.InsertBefore(warningComment, root);
            }

            // write to string
            return WriteXmlToString(xmlDoc, prettyPrint);
        }

        /// <summary>
        /// Saves to a file asyncronously, wrapped in a coroutine.  Make sure <see cref="PrepareForSaveToString"/> was called first!
        /// </summary>
        /// <param name="dataString">String data to save</param>
        /// <param name="path">Path to save the data to.</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <param name="callback">Callback when the save operation completes.</param>
        /// <returns>The status of the save.</returns>
        private IEnumerator SaveToFileCoroutineAfterPrepared(string path, bool prettyPrint, UnityAction<SaveStatus> callback) {
            yield return null;

            // run save to file asyncronously
            SaveStatus? saveStatus = null;
            Task.Run(async () => {
                try {
                    saveStatus = await this.SaveToFileAsync(path, prettyPrint);
                } catch {
                    saveStatus = SaveStatus.IOError;
                }
            });

            // wait until save is complete.
            while (saveStatus == null) {
                yield return null;
            }

            // save complete
            this.IsSaving = false;
            if (saveStatus == SaveStatus.Ok) {
                Debug.Log(string.Format(_saveSuccessMessage, path));
            } else {
                Debug.LogError(string.Format(_saveFailMessage, path, saveStatus));
            }
            callback(saveStatus.Value);
            yield return null;
        }

        /// <summary>
        /// Async method for saving to a file.
        /// </summary>
        /// <param name="dataString">String data to save</param>
        /// <param name="path">Path to save the data to.</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <returns>The status of the save.</returns>
        private async Task<SaveStatus> SaveToFileAsync(string path, bool prettyPrint) {
            //// uncomment for function to not finish almost immediately
            //System.Threading.Thread.Sleep(1000);

            // get string to save
            string str = this.SaveToStringAfterPrepared(prettyPrint);

            // save to temporary file first
            string tempPath;
            try {
                tempPath = Path.GetTempFileName();
            } catch (IOException) {
                return SaveStatus.IOError;
            }
            try {
                using (StreamWriter outputFile = new StreamWriter(tempPath, false, Encoding)) {
                    await outputFile.WriteAsync(str);
                }
            } catch (System.Exception) {
                return SaveStatus.IOError;
            }

            // override previous file if successful
            string backupPath = GetBackupPath(path);
            try {
                File.Replace(tempPath, path, backupPath);
            } catch (System.Exception) {
                return SaveStatus.IOError;
            }

            // set cached file string after successful save
            _cachedFileString = str;

            return SaveStatus.Ok;
        }

        /// <summary>
        /// Uses an xml writer to convert the given xml document into a string.  Returns null if something went wrong.
        /// </summary>
        /// <param name="xmlDoc">The xml document</param>
        /// <param name="prettyPrint">If pretty printing should be enabled.</param>
        /// <returns>string</returns>
        private static string WriteXmlToString(XmlDocument xmlDoc, bool prettyPrint) {
            try {
                XmlWriterSettings writerSettings = new XmlWriterSettings() {
                    OmitXmlDeclaration = true,
                    Encoding = Encoding,
                    Indent = prettyPrint,
                };
                using (StringWriter stringWriter = new StringWriter())
                using (XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter, writerSettings)) {
                    xmlDoc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    return stringWriter.GetStringBuilder().ToString();
                }
            } catch (System.Exception) {
                return null;
            }
        }

        /// <summary>
        /// string representation of the save data that was last loaded from or saved to a file.
        /// </summary>
        private string _cachedFileString = null;

        #endregion
    }
}