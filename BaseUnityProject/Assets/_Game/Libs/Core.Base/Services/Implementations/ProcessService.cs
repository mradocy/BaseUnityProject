using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Services.Implementations {

    public class ProcessService : IProcessService {

        /// <summary>
        /// Starts an external process.
        /// Returns the Process object created.
        /// </summary>
        /// <param name="fileName">The application or file to start.</param>
        /// <param name="args">Command line arguments to use when starting the application.</param>
        public Process StartProcess(string fileName, string args) {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.Arguments = args;

            Process process = new Process() {
                StartInfo = startInfo
            };

            process.Start();
            return process;
        }

        /// <summary>
        /// Starts an external process, then waits for the process to finish.
        /// Returns the Process object created.
        /// </summary>
        /// <param name="fileName">The application or file to start.</param>
        /// <param name="args">Command line arguments to use when starting the application.</param>
        public Process StartProcessAndWaitForExit(string fileName, string args) {

            Process process = this.StartProcess(fileName, args);
            process.WaitForExit();
            return process;
        }

        /// <summary>
        /// Opens a file explorer window externally.
        /// </summary>
        /// <param name="path">Path to open in the file explorer.</param>
        public void StartFileExplorer(string path) {

            this.StartProcess("explorer.exe", path);
        }

    }

}
