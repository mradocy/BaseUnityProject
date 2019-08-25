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
    public class SaveRoot : SaveGroup {

        /// <summary>
        /// Constructor.
        /// </summary>
        public SaveRoot() : base("<Root>", null) { }

        /// <inheritdoc />
        public override SaveRoot Root {
            get { return this; }
        }

        #region File Properties

        /// <summary>
        /// Encoding used for file operations.
        /// </summary>
        public static System.Text.Encoding Encoding {
            get { return System.Text.Encoding.Unicode; }
        }

        /// <summary>
        /// The full path of the directory reserved for save files.  Is contained in Unity's <see cref="Application.persistentDataPath"/>.
        /// </summary>
        public static string SaveDirectory { get { return Path.Combine(Application.persistentDataPath, "SaveData"); } }

        /// <summary>
        /// Gets the full file path for data saved to the <see cref="SaveDirectory"/>.
        /// </summary>
        /// <param name="index">File index</param>
        /// <returns>Path</returns>
        public static string GetSaveDirectoryPath(int index) {
            return Path.Combine(SaveDirectory, $"data{index}.sav");
        }

        #endregion

        #region Loading

        private const string _startLoadMessage = "Started loading save data from \"{0}\".";
        private const string _loadSuccessMessage = "Loaded save data from \"{0}\" successfully.";
        private const string _loadFailMessage = "Error loading save data from \"{0}\": {1}";

        /// <summary>
        /// Gets if the save data has been parsed already.  If so, then no more properties can be registered.
        /// </summary>
        public bool IsParsed { get; private set; }

        /// <summary>
        /// Loads an xml file, the contents of which are then parsed.
        /// Returns the status of the load.
        /// </summary>
        /// <param name="path">Path to the xml file.</param>
        /// <returns>LoadStatus</returns>
        public LoadStatus LoadFromFile(string path) {

            LoadStatus loadStatus = LoadStatus.Ok;

            // load file
            XmlDocument xmlDoc = new XmlDocument();
            try {
                xmlDoc.Load(path);
            } catch (System.IO.FileNotFoundException) {
                loadStatus = LoadStatus.FileNotFound;
            } catch (XmlException) {
                loadStatus = LoadStatus.ParseError;
            } catch (System.Exception) {
                loadStatus = LoadStatus.FileCouldNotBeRead;
            }

            // parse file
            if (loadStatus == LoadStatus.Ok) {
                loadStatus = this.ParseXML(xmlDoc);
            }

            // log
            if (loadStatus == LoadStatus.Ok) {
                Debug.Log(string.Format(_loadSuccessMessage, path));
            } else {
                Debug.LogError(string.Format(_loadFailMessage, path, loadStatus));
            }

            return loadStatus;
        }

        /// <summary>
        /// Loads a file from <see cref="SaveDirectory"/>.
        /// </summary>
        /// <param name="index">File index</param>
        /// <returns>Load status</returns>
        public LoadStatus LoadFromSaveDirectory(int index) {
            return this.LoadFromFile(GetSaveDirectoryPath(index));
        }

        /// <summary>
        /// Parses an <see cref="XmlDocument"/> object.
        /// Returns the status of the load.
        /// </summary>
        /// <param name="xmlDoc">The document to parse.</param>
        /// <returns>LoadStatus</returns>
        public LoadStatus ParseXML(XmlDocument xmlDoc) {
            if (xmlDoc == null) {
                return LoadStatus.ParseError;
            }

            // top node should always be <Root>
            XmlNode rootNode = xmlDoc.FirstChild;
            if (rootNode?.Name != "Root") {
                return LoadStatus.ParseError;
            }

            // clear existing data
            this.ResetToDefault();

            // parse node
            LoadStatus status = this.ParseXML(rootNode);

            this.IsParsed = true;
            return status;
        }

        /// <summary>
        /// Parses a string containing xml data.
        /// Returns the status of the load.
        /// </summary>
        /// <param name="xmlString">String to parse.</param>
        /// <returns>LoadStatus</returns>
        public LoadStatus ParseXML(string xmlString) {
            XmlDocument xmlDoc = new XmlDocument();
            try {
                xmlDoc.LoadXml(xmlString);
            } catch (XmlException) {
                return LoadStatus.ParseError;
            }

            return this.ParseXML(xmlDoc);
        }

        #endregion

        #region Saving

        private const string _startSaveMessage = "Started saving to \"{0}\".";
        private const string _saveSuccessMessage = "Saved data to \"{0}\" successfully.";
        private const string _saveFailMessage = "Error saving data to \"{0}\": {1}";

        /// <summary>
        /// If data is currently being saved.
        /// </summary>
        public bool IsSaving { get; private set; }

        /// <summary>
        /// Saves the data to a string.
        /// Returns null if there was a problem.
        /// </summary>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <returns>string</returns>
        public string SaveToString(bool prettyPrint) {

            // create xml document, starting with root node
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = this.CreateXML(xmlDoc, true);
            xmlDoc.AppendChild(root);

            // write to string
            try {
                XmlWriterSettings writerSettings = new XmlWriterSettings() {
                    OmitXmlDeclaration = true,
                    Encoding = Encoding,
                    Indent = prettyPrint,
                };
                using (System.IO.StringWriter stringWriter = new System.IO.StringWriter())
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

            if (saveStatus == SaveStatus.Ok) {
                try {
                    File.WriteAllText(path, str, Encoding);
                } catch (System.Exception) {
                    saveStatus = SaveStatus.IOError;
                }
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

            return this.SaveToFile(GetSaveDirectoryPath(index), prettyPrint);
        }

        /// <summary>
        /// Returns a coroutine <see cref="IEnumerator"/> that will save data to the given <paramref name="path"/> in another thread,
        /// then call <paramref name="callback"/> in the main Unity thread on completion, even if there was an error.
        /// Usage: this.StartCoroutine(saveRoot.SaveToFileCoroutine)
        /// </summary>
        /// <param name="path">Path to save the data to.</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <param name="callback">Callback function to call once the save is complete.  The given <see cref="SaveStatus"/> param is the status of the save.</param>
        /// <returns>Coroutine IEnumerator.</returns>
        public IEnumerator SaveToFileCoroutine(string path, bool prettyPrint, UnityAction<SaveStatus> callback) {

            if (callback == null) {
                throw new System.ArgumentNullException(nameof(callback));
            }
            if (this.IsSaving) {
                Debug.LogError(string.Format(_saveFailMessage, path, SaveStatus.AlreadySaving));
                callback(SaveStatus.AlreadySaving);
                yield break;
            }
            this.IsSaving = true;

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
        /// Returns a coroutine <see cref="IEnumerator"/> that will save data to the data to <see cref="SaveDirectory"/> in another thread,
        /// then call <paramref name="callback"/> in the main Unity thread on completion, even if there was an error.
        /// Usage: this.StartCoroutine(saveRoot.SaveToPersistentDataCoroutine)
        /// </summary>
        /// <param name="index">File index</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <param name="callback">Callback function to call once the save is complete.  The given <see cref="SaveStatus"/> param is the status of the save.</param>
        /// <returns>Coroutine IEnumerator.</returns>
        public IEnumerator SaveToSaveDirectoryCoroutine(int index, bool prettyPrint, UnityAction<SaveStatus> callback) {
            if (!Directory.Exists(SaveDirectory)) {
                Directory.CreateDirectory(SaveDirectory);
            }

            return this.SaveToFileCoroutine(GetSaveDirectoryPath(index), prettyPrint, callback);
        }

        #endregion

        #region Saving - Private

        /// <summary>
        /// Async method for saving to a file.
        /// </summary>
        /// <param name="path">Path to save the data to.</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <returns>The status of the save.</returns>
        private async Task<SaveStatus> SaveToFileAsync(string path, bool prettyPrint) {

            string str = this.SaveToString(prettyPrint);
            if (str == null) {
                return SaveStatus.StringError;
            }

            // so function won't finish almost immediately
            System.Threading.Thread.Sleep(1000);

            try {
                using (StreamWriter outputFile = new StreamWriter(path, false, Encoding)) {
                    await outputFile.WriteAsync(str);
                }
            } catch (System.Exception) {
                return SaveStatus.IOError;
            }

            return SaveStatus.Ok;
        }

        #endregion
    }
}