using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base {

    public static class ConsoleUtils {

        /// <summary>
        /// Gets the name of this executable.
        /// Name is obtained with Environment.GetCommandLineArgs()[0].
        /// </summary>
        public static string ExecutableName {
            get {
                return Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            }
        }

        /// <summary>
        /// Writes a line signifying the start of a program.  The name used is the ExecutableName.
        /// Looks like --- ExecName Start ---
        /// </summary>
        public static void WriteProgramStart() {
            WriteProgramStart(ExecutableName);
        }
        /// <summary>
        /// Writes a line signifying the start of a program.
        /// Looks like --- ExecName Start ---
        /// </summary>
        /// <param name="applicationName">The name of the application to write.</param>
        public static void WriteProgramStart(string applicationName) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"===== {applicationName} Start =====");
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a line signifying the end of a program.  The name used is the ExecutableName.
        /// Looks like --- ExecName End ---
        /// </summary>
        public static void WriteProgramEnd() {
            WriteProgramEnd(ExecutableName);
        }
        /// <summary>
        /// Writes a line signifying the end of a program.
        /// Looks like --- ExecName End ---
        /// </summary>
        /// <param name="applicationName">The name of the application to write.</param>
        public static void WriteProgramEnd(string applicationName) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"===== {applicationName} End =====");
            Console.ResetColor();
        }

        /// <summary>
        /// Writes an error to the console in a scary red color.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteError(object message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[X] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        /// <summary>
        /// Pauses the console (Console.Read) if the console window will be destroyed at the end of program execution.
        /// </summary>
        public static void PauseIfConsoleWillBeDestroyed() {
            if (ConsoleWillBeDestroyed()) {
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Gets if the console window will be destroyed at the end of program execution.
        /// </summary>
        public static bool ConsoleWillBeDestroyed() {
            uint[] processList = new uint[1];
            uint processCount = GetConsoleProcessList(processList, 1);

            return processCount == 1;
        }

        /// <summary>
        /// C code that retrieves a list of the processes attached to the current console.
        /// </summary>
        /// <param name="processList"></param>
        /// <param name="processCount"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetConsoleProcessList(uint[] processList, uint processCount);

    }

}
