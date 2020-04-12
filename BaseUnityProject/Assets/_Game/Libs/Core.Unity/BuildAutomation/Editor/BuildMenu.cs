using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using UnityEditor;
using System.Linq;
using UnityEditor.Build.Reporting;

namespace Core.Unity.BuildAutomation {

    /// <summary>
    /// Static class providing options for building the game.
    /// </summary>
    /// <remarks>
    /// BuildPlayer: https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
    /// </remarks>
    public static class BuildMenu {

        #region Constants and Properties

        /// <summary>
        /// The name of the directory for debug builds.
        /// </summary>
        public const string _debugDirectoryName = "Debug";

        /// <summary>
        /// The name of the directory for release builds.
        /// </summary>
        public const string _releaseDirectoryName = "Release";

        /// <summary>
        /// The name of the directory for windows64 builds.
        /// </summary>
        public const string _windows64DirectoryName = "Windows";

        /// <summary>
        /// Gets the top directory containing the builds.
        /// </summary>
        public static string BuildsDirectory {
            get { return System.IO.Path.GetFullPath($"{Application.dataPath}/../../Builds").Replace('\\', '/'); }
        }

        /// <summary>
        /// Gets all the supported build targets.
        /// </summary>
        public static BuildTarget[] SupportedBuildTargets => new BuildTarget[] { BuildTarget.StandaloneWindows64 };

        /// <summary>
        /// Gets all the supported build target groups.
        /// </summary>
        public static BuildTargetGroup[] SupportedBuildTargetGroups => new BuildTargetGroup[]{ BuildTargetGroup.Standalone };

        #endregion

        #region Menu Item Methods

        /// <summary>
        /// Sets the Debug configuration (removes RELEASE from the scripting define symbols) for all supported build target groups.
        /// </summary>
        [MenuItem("Build/Set Debug Configuration", false, 0)]
        public static void SetDebugConfiguration() {
            foreach (BuildTargetGroup buildTargetGroup in SupportedBuildTargetGroups) {
                SetIncludedInScriptingDefineSymbols("RELEASE", false, buildTargetGroup);
            }
        }

        [MenuItem("Build/Set Debug Configuration", true)]
        public static bool SetDebugConfigurationValidate() {
            return GetIncludedInScriptingDefineSymbols("RELEASE", BuildTargetGroup.Standalone);
        }

        /// <summary>
        /// Sets the Release configuration (adds RELEASE to the scripting define symbols) for all supported build target groups.
        /// </summary>
        [MenuItem("Build/Set Release Configuration", false, 1)]
        public static void SetReleaseConfiguration() {
            foreach (BuildTargetGroup buildTargetGroup in SupportedBuildTargetGroups) {
                SetIncludedInScriptingDefineSymbols("RELEASE", true, buildTargetGroup);
            }
        }

        [MenuItem("Build/Set Release Configuration", true)]
        public static bool SetReleaseConfigurationValidate() {
            return !GetIncludedInScriptingDefineSymbols("RELEASE", BuildTargetGroup.Standalone);
        }

        [MenuItem("Build/Build Debug - Windows64", false, 20)]
        public static void BuildDebugWindows64() {
            Build(false, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Build/Build Debug - Windows64", true)]
        public static bool BuildDebugWindows64Validate() {
            return !GetIncludedInScriptingDefineSymbols("RELEASE", BuildTargetGroup.Standalone);
        }

        [MenuItem("Build/Build Release - Windows64", false, 21)]
        public static void BuildReleaseWindows64() {
            Build(true, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Build/Build Release - Windows64", true)]
        public static bool BuildReleaseWindows64Validate() {
            return GetIncludedInScriptingDefineSymbols("RELEASE", BuildTargetGroup.Standalone);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets scenes selected to be part of the build (selected in the Build Settings window).
        /// </summary>
        /// <returns>Scenes</returns>
        private static string[] GetScenes() {
            return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        }

        /// <summary>
        /// Gets whether the given symbol is included in the define symbols for the given build target group.
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="buildTargetGroup">Build target group</param>
        /// <returns>Is included</returns>
        private static bool GetIncludedInScriptingDefineSymbols(string symbol, BuildTargetGroup buildTargetGroup) {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').Contains(symbol);
        }

        /// <summary>
        /// Sets whether or not the given symbol is included in the define symbols for the given build target group.
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="included">Is included or not.</param>
        /// <param name="buildTargetGroup">Build target group.</param>
        private static void SetIncludedInScriptingDefineSymbols(string symbol, bool included, BuildTargetGroup buildTargetGroup) {
            List<string> symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').Distinct().ToList();
            if (included) {
                if (!symbols.Contains(symbol)) {
                    symbols.Insert(0, symbol);
                }
            } else {
                symbols.Remove(symbol);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", symbols));
        }

        /// <summary>
        /// Prints the result of a build.
        /// </summary>
        /// <param name="buildReport">Report to print.</param>
        private static void PrintBuildReport(BuildReport buildReport) {
            BuildSummary summary = buildReport.summary;
            switch (summary.result) {
            case BuildResult.Unknown:
                Debug.LogError("The outcome of this build is unknown.");
                break;
            case BuildResult.Succeeded:
                Debug.Log($"Build succeeded!  Output to {summary.outputPath}");
                break;
            case BuildResult.Failed:
                Debug.LogError("Build failed.");
                break;
            case BuildResult.Cancelled:
                Debug.Log("Build was cancelled by the user.");
                break;
            }
        }

        /// <summary>
        /// Gets the name of the directory for the given configuration.
        /// </summary>
        /// <param name="releaseConfig">True for release configuration, false for debug configuration.</param>
        /// <returns>Directory name.</returns>
        private static string GetConfigDirectoryName(bool releaseConfig) {
            if (releaseConfig) {
                return _releaseDirectoryName;
            } else {
                return _debugDirectoryName;
            }
        }

        /// <summary>
        /// Gets the name of the directory for the given build target.  Returns null if the build target doesn't yet have a directory name set.
        /// </summary>
        /// <param name="buildTarget">Build target.</param>
        /// <returns></returns>
        private static string GetBuildTargetDirectoryName(BuildTarget buildTarget) {
            switch (buildTarget) {
            case BuildTarget.StandaloneWindows64:
                return _windows64DirectoryName;
            }

            return null;
        }

        /// <summary>
        /// Builds the game with the given configuration and build target.
        /// </summary>
        /// <param name="releaseConfig">True for release configuration, false for debug configuration.</param>
        /// <param name="buildTarget">The build target.</param>
        private static void Build(bool releaseConfig, BuildTarget buildTarget) {
            Debug.Log($"Build Start - Release: {releaseConfig}, Build Target: {buildTarget}");

            if (!SupportedBuildTargets.Contains(buildTarget)) {
                Debug.LogError($"BuildTarget {buildTarget} is not supported.");
                return;
            }

            string configDirectoryName = GetConfigDirectoryName(releaseConfig);
            string buildTargetDirectoryName = GetBuildTargetDirectoryName(buildTarget);
            if (string.IsNullOrEmpty(buildTargetDirectoryName)) {
                Debug.LogError($"BuildTarget {buildTarget} does not have a directory defined.");
                return;
            }

            string buildDirectory = $"{BuildsDirectory}/{configDirectoryName}/{buildTargetDirectoryName}";
            string exeDirectory = $"{buildDirectory}/{Application.productName}";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes();
            buildPlayerOptions.locationPathName = $"{exeDirectory}/{Application.productName}.exe";
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            PrintBuildReport(report);
            if (report.summary.result != BuildResult.Succeeded) {
                return;
            }

            // create readme file
            ReadmeManager.CreateReadmeFileForBuild(exeDirectory);

            // zip build
            string zipFileName;
            if (releaseConfig) {
                zipFileName = $"{Application.productName}-{buildTargetDirectoryName}-{Application.version}.zip";
            } else {
                zipFileName = $"{Application.productName}-DEBUG-{buildTargetDirectoryName}-{Application.version}.zip";
            }
            string zipFilePath = $"{buildDirectory}/{zipFileName}";
            ZipManager.ZipDirectory(exeDirectory, zipFilePath);

            // open build directory in explorer
            System.Diagnostics.Process.Start(buildDirectory);
        }

        #endregion

        // building from command line:
        // https://docs.unity3d.com/Manual/CommandLineArguments.html
        // "C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\UserName\Documents\MyProject" -executeMethod MyEditorScript.PerformBuild
    }
}