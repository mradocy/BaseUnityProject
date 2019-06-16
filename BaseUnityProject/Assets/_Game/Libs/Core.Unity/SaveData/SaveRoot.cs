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
        /// The complete file path for data saved to Unity's persistent data path.
        /// </summary>
        /// <param name="index">File index</param>
        /// <returns>Path</returns>
        public static string PersistentDataPath(int index) {
            return Path.Combine(Application.persistentDataPath, $"data{index}.sav");
        }

        #endregion

        #region Loading

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

            // load file
            XmlDocument xmlDoc = new XmlDocument();
            try {
                xmlDoc.Load(path);
            } catch (System.IO.FileNotFoundException) {
                return LoadStatus.FileNotFound;
            } catch (XmlException) {
                return LoadStatus.ParseError;
            } catch (System.Exception) {
                return LoadStatus.FileCouldNotBeRead;
            }
            
            return this.ParseXML(xmlDoc);
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

            string str = this.SaveToString(prettyPrint);
            if (str == null) {
                return SaveStatus.StringError;
            }

            try {
                File.WriteAllText(path, str, Encoding);
            } catch (System.Exception) {
                return SaveStatus.IOError;
            }

            return SaveStatus.Ok;
        }

        /// <summary>
        /// Saves the data to Unity's persistent data directory.
        /// </summary>
        /// <param name="index">File index</param>
        /// <param name="prettyPrint">If the output string should be formatted.</param>
        /// <returns>Save status</returns>
        public SaveStatus SaveToPersistentData(int index, bool prettyPrint) {
            return this.SaveToFile(PersistentDataPath(index), prettyPrint);
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
            callback(saveStatus.Value);
            yield return null;
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