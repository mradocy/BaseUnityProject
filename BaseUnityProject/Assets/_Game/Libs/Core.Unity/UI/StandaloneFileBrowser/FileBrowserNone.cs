using System;

namespace Core.Unity.UI.StandaloneFileBrowser {

    /// <summary>
    /// An implementation of <see cref="IFileBrowser"/> that does nothing.
    /// </summary>
    public class FileBrowserNone : IFileBrowser {
        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect) {
            return new string[0];
        }

        public void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb) {
            cb(new string[0]);
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect) {
            return new string[0];
        }

        public void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb) {
            cb(new string[0]);
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions) {
            return "";
        }

        public void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb) {
            cb("");
        }
    }
}