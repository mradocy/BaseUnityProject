using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[assembly: System.Reflection.AssemblyVersion("1.0.*")] // using * automatically sets version.Build to number of days after 1/1/2000.  More here: http://luminaryapps.com/blog/showing-the-build-date-in-a-unity-app/
namespace Core.Unity {

    public static class BuildInfo {

        /// <summary>
        /// Returns the version of the game, as a string.
        /// If the RELEASE symbol is not defined, then "DEBUG-" will be prepended to the version name.
        /// </summary>
        /// <remarks>This gets the version from UnityEngine.Application.version, but this value can only be set in Editor scripts.</remarks>
        public static string Version {
            get {
#if RELEASE
                return Application.version;
#else
                return $"DEBUG-{Application.version}";
#endif
            }
        }

        /// <summary>
        /// Gets the date this game was built.
        /// </summary>
        /// <remarks>Uses reflection to look at the generated version in the assembly.</remarks>
        public static System.DateTime BuildDate {
            get {
                if (_buildDate == null) {
                    System.Version assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    System.DateTime startDate = new System.DateTime(2000, 1, 1, 0, 0, 0);
                    System.TimeSpan span = new System.TimeSpan(assemblyVersion.Build, 0, 0, assemblyVersion.Revision * 2);
                    System.DateTime buildDate = startDate.Add(span);
                    _buildDate = buildDate;
                }

                return _buildDate.Value;
            }
        }

        private static System.DateTime? _buildDate = null;
    }
}