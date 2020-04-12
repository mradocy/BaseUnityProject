using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using System.IO;

namespace Core.Unity.BuildAutomation {

    /// <summary>
    /// Uses 7-Zip (external application) to compress a directory into a zip file.
    /// </summary>
    public static class ZipManager {

        #region Constants

        /// <summary>
        /// How long to run the zip process before killing it.
        /// </summary>
        private static readonly System.TimeSpan _zipProcessTimeout = System.TimeSpan.FromSeconds(10);

        #endregion

        /// <summary>
        /// Uses 7-Zip to compress a source directory into a zip file.
        /// </summary>
        /// <param name="sourceDirectory">The directory to compress.</param>
        /// <param name="destinationFile">The destination file path (should end in a .zip extension)</param>
        public static void ZipDirectory(string sourceDirectory, string destinationFile) {
            if (!Directory.Exists(sourceDirectory)) {
                Debug.LogError($"Given source directory \"{sourceDirectory}\" does not exist");
                return;
            }
            if (Path.GetExtension(destinationFile) != ".zip") {
                Debug.LogError($"Destination file \"{destinationFile}\" is invalid, must have a .zip extension.");
                return;
            }

            // run zip process
            string zipTempFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
            System.Diagnostics.Process zipProcess = new System.Diagnostics.Process();
            zipProcess.StartInfo = new System.Diagnostics.ProcessStartInfo() {
                FileName = "7z.exe",
                Arguments = $"a \"{zipTempFilePath}\" \"{sourceDirectory}\"",
            };
            try {
                zipProcess.Start();
            } catch (System.ComponentModel.Win32Exception ex) {
                Debug.LogError($"Error running 7-zip process: \"{ex.Message.Trim()}\".  Is 7-zip installed, and is 7z.exe accessible in the system PATH?");
                return;
            }

            // wait for process to complete
            if (!zipProcess.WaitForExit((int)_zipProcessTimeout.TotalMilliseconds)) {
                Debug.LogError("7-zip process is taking too long.  Killing process.");
                zipProcess.Kill();
                return;
            }
            int exitCode = zipProcess.ExitCode;
            if (exitCode != 0) {
                Debug.LogError($"7-zip process failed.  Exit code: {exitCode}");
                return;
            }

            // move zipped file to destination
            try {
                File.Delete(destinationFile);
                File.Move(zipTempFilePath, destinationFile);
            } catch (System.Exception ex) {
                Debug.LogError($"Exception occurred moving zip file \"{zipTempFilePath}\" to destination \"{destinationFile}\": {ex.Message}");
                return;
            }

            Debug.Log($"Successfully zipped \"{sourceDirectory}\" to \"{destinationFile}\"");
        }
    }
}