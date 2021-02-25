using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity {

    /// <summary>
    /// Utilities class for System related tasks.
    /// </summary>
    public static class SystemUtils {

        /// <summary>
        /// Gets if the given argument was provided in the command line args.
        /// </summary>
        /// <param name="arg">Argument (e.g. "-quick")</param>
        public static bool IsCommandLineArgProvided(string arg) {
            CacheCommandLineArgs();
            foreach (string a in _commandLineArgs) {
                if (string.Equals(a, arg, System.StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to get the value of a command line argument.
        /// e.g. if run like: game.exe -arg "value", then value (without quotes) would be returned.
        /// </summary>
        /// <param name="arg">Argument (e.g. "-out")</param>
        /// <param name="value">Out value to contain the argument provided to the right of <paramref name="arg"/></param>
        /// <returns>If the argument was provided.</returns>
        public static bool TryGetCommandLineArgValue(string arg, out string value) {
            CacheCommandLineArgs();
            for (int i=0; i < _commandLineArgs.Length; i++) {
                string a = _commandLineArgs[i];
                if (string.Equals(a, arg, System.StringComparison.OrdinalIgnoreCase)) {
                    if (i + 1 >= _commandLineArgs.Length) {
                        value = null;
                        return false;
                    }

                    value = _commandLineArgs[i + 1];
                    return true;
                }
            }

            value = null;
            return false;
        }

        private static void CacheCommandLineArgs() {
            if (_commandLineArgs != null)
                return;

            _commandLineArgs = System.Environment.GetCommandLineArgs();
        }

        private static string[] _commandLineArgs = null;
    }
}