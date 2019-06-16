using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base {

    public static class AssemblyUtils {

        /// <summary>
        /// Gets the name of the calling assembly.
        /// </summary>
        public static string GetName() {
            Assembly assembly = Assembly.GetCallingAssembly();
            return assembly?.GetName()?.Name;
        }

        /// <summary>
        /// Gets the description of the calling assembly.
        /// Returns null if description is not defined.
        /// </summary>
        public static string GetDescription() {
            Assembly assembly = Assembly.GetCallingAssembly();
            return assembly?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        }

        /// <summary>
        /// Gets the version of the calling assembly.
        /// </summary>
        public static Version GetVersion() {
            Assembly assembly = Assembly.GetCallingAssembly();
            return assembly?.GetName()?.Version;
        }

        /// <summary>
        /// Gets the build date of the calling assembly, based off of its version.
        /// For this to work, the Build property of the version must be "*",  e.g. [assembly: AssemblyVersion("1.0.*")]
        /// </summary>
        public static DateTime GetBuildDateFromVersion() {
            Assembly assembly = Assembly.GetCallingAssembly();
            Version version = assembly?.GetName()?.Version;
            if (version == null) {
                Debug.Fail($"To use {nameof(GetBuildDateFromVersion)}(), AssemblyVersion must be defined in the calling assembly.");
                return new DateTime(0);
            }
            int build = version.Build;
            if (build < 5000) {
                Debug.Fail($"To use {nameof(GetBuildDateFromVersion)}(), the Build property of the AssemblyVersion must be set to \"*\".");
                return new DateTime(0);
            }
            DateTime startDate = new DateTime(2000, 1, 1, 0, 0, 0);
            TimeSpan span = new TimeSpan(build, 0, 0, 0);
            DateTime buildDate = startDate.Add(span);
            return buildDate;
        }

    }

}
