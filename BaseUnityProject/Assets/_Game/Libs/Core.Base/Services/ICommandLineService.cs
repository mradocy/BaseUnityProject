using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Services {

    public interface ICommandLineService {

        /// <summary>
        /// Array of commands that represent help (e.g. "-h", "--help")
        /// </summary>
        string[] HelpCommands { get; set; }

        /// <summary>
        /// Register a flag (argument with no parameter) to be given at the command line.
        /// </summary>
        /// <param name="argumentKey">Flag to be given, e.g. "-q".</param>
        /// <param name="helpDescription">Help description to display in the auto-generated help screen.</param>
        void RegisterFlag(string flag, string helpDescription);

        /// <summary>
        /// Register a flag (argument with no parameter) to be given at the command line.
        /// </summary>
        /// <param name="flags">Possible flags to be given, e.g. "-q", "quick".</param>
        /// <param name="helpDescription">Help description to display in the auto-generated help screen.</param>
        void RegisterFlag(string[] flags, string helpDescription);

        /// <summary>
        /// Register an argument to be given at the command line.
        /// </summary>
        /// <param name="argumentKey">Key of the argument, e.g. "-o".</param>
        /// <param name="paramType">Type of the parameter to follow the argument key.</param>
        /// <param name="paramExample">An example of a parameter that can be given after the key.  This should be null if paramType is CommandParamType.None.</param>
        /// <param name="required">If this argument is required.  Arguments that aren't required are considered options.</param>
        /// <param name="helpDescription">Help description to display in the auto-generated help screen.</param>
        void RegisterArg(string argumentKey, CommandParamType paramType, string paramExample, bool required, string helpDescription);

        /// <summary>
        /// Register an argument to be given at the command line.
        /// </summary>
        /// <param name="argumentKeys">Possible keys of the argument, e.g. "-i", "-input"</param>
        /// <param name="paramType">Type of the parameter to follow the argument key.  Can be ParamType.NONE to make the argument key a flag.</param>
        /// <param name="paramExample">An example of a parameter that can be given after the key.  This should be null if paramType is CommandParamType.None.</param>
        /// <param name="required">If this argument is required.  Arguments that aren't required are considered options.</param>
        /// <param name="helpDescription">Help description to display in the auto-generated help screen.</param>
        void RegisterArg(string[] argumentKeys, CommandParamType paramType, string paramExample, bool required, string helpDescription);

        /// <summary>
        /// Processes the given command line arguments.  Returns true if there were no errors.
        /// If there were errors, returns false and prints the errors to the console.
        /// </summary>
        /// <param name="args">Args given in Main</param>
        bool Process(string[] args);

        /// <summary>
        /// Gets if the given flag was supplied when the args were processed.
        /// </summary>
        /// <param name="flag">The argument flag to check.</param>
        bool GetFlagGiven(string flag);

        /// <summary>
        /// Gets the value that immediately follows the given argument.
        /// Returns defaultValue if the specified argument wasn't given.
        /// </summary>
        /// <param name="arg">Name of the argument.</param>
        /// <param name="defaultValue">The value to return if the specified argument wasn't given.  Default is null.</param>
        string GetArgValue(string arg, string defaultValue = null);

        /// <summary>
        /// Gets the value that immediately follows the given argument, cast as an int.
        /// Returns defaultValue if the specified argument wasn't given.
        /// </summary>
        /// <param name="arg">Name of the argument.</param>
        /// <param name="defaultValue">The value to return if the specified argument wasn't given.  Default is -1.</param>
        int GetArgValueInt(string arg, int defaultValue = -1);

        /// <summary>
        /// Gets the value that immediately follows the given argument, cast as a float.
        /// Returns defaultValue if the specified argument wasn't given.
        /// </summary>
        /// <param name="arg">Name of the argument.</param>
        /// <param name="defaultValue">The value to return if the specified argument wasn't given.  Default is -1.</param>
        float GetArgValueFloat(string arg, float defaultValue = -1);

        /// <summary>
        /// Gets the value that immediately follows the given argument, cast as a double.
        /// Returns defaultValue if the specified argument wasn't given.
        /// </summary>
        /// <param name="arg">Name of the argument.</param>
        /// <param name="defaultValue">The value to return if the specified argument wasn't given.  Default is -1.</param>
        double GetArgValueDouble(string arg, double defaultValue = -1);

        /// <summary>
        /// Gets if any of the commands given match commands listed in the HelpCommands property.
        /// </summary>
        bool GetHelpRequested();

        /// <summary>
        /// Displays a help screen in the console.
        /// The content of the help screen is generated from the registered arguments.
        /// </summary>
        void DisplayHelpScreen();

    }

}
