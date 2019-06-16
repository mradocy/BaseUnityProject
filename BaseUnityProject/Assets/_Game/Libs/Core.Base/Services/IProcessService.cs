using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Services {

    public interface IProcessService {

        /// <summary>
        /// Starts an external process.
        /// Returns the Process object created.
        /// </summary>
        /// <param name="fileName">The application or file to start.</param>
        /// <param name="args">Command line arguments to use when starting the application.</param>
        Process StartProcess(string fileName, string args);

        /// <summary>
        /// Starts an external process, then waits for the process to finish.
        /// Returns the Process object created.
        /// </summary>
        /// <param name="fileName">The application or file to start.</param>
        /// <param name="args">Command line arguments to use when starting the application.</param>
        Process StartProcessAndWaitForExit(string fileName, string args);

        /// <summary>
        /// Opens a file explorer window externally.
        /// </summary>
        /// <param name="path">Path to open in the file explorer.</param>
        void StartFileExplorer(string path);

    }

}
