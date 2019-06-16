using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Services.Implementations {

    public class CommandLineService : ICommandLineService {
        
        /// <inheritdoc />
        public string[] HelpCommands { get; set; } = new string[] { "-h", "-help", "--help" };

        /// <inheritdoc />
        public void RegisterFlag(string flag, string helpDescription) {
            this.RegisterFlag(new string[] { flag }, helpDescription);
        }

        /// <inheritdoc />
        public void RegisterFlag(string[] flags, string helpDescription) {
            this.RegisterArg(flags, CommandParamType.None, null, false, helpDescription);
        }

        /// <inheritdoc />
        public void RegisterArg(string argumentKey, CommandParamType paramType, string paramExample, bool required, string helpDescription) {
            this.RegisterArg(new string[] { argumentKey }, paramType, paramExample, required, helpDescription);
        }

        /// <inheritdoc />
        public void RegisterArg(string[] argumentKeys, CommandParamType paramType, string paramExample, bool required, string helpDescription) {

            if (_processCalled) {
                ConsoleUtils.WriteError($"Cannot register arg after Process() has been called.");
                return;
            }

            if (argumentKeys == null || argumentKeys.Length < 1) {
                ConsoleUtils.WriteError($"Given {argumentKeys} must contain keys.");
                return;
            }

            foreach (string argumentKey in argumentKeys) {
                if (argumentKey == null || argumentKey.Length < 1 || argumentKey[0] != '-') {
                    ConsoleUtils.WriteError($"First character of argument {argumentKey} must be '-'");
                    return;
                }

                if (GetArgDef(argumentKey) != null) {
                    ConsoleUtils.WriteError($"Argument with key {argumentKey} already registered.");
                    return;
                }

                if (paramType == CommandParamType.None && !string.IsNullOrEmpty(paramExample)) {
                    ConsoleUtils.WriteError($"Arguemnt {argumentKey}'s paramType is {paramType}, which means paramExample must be null.");
                    return;
                }
            }
            
            _argDefs.Add(new ArgDef() {
                Keys = argumentKeys,
                ParamType = paramType,
                ParamExample = paramExample,
                Required = required,
                HelpDescription = helpDescription
            });

        }
        
        /// <inheritdoc />
        public bool Process(string[] args) {

            if (_processCalled) {
                ConsoleUtils.WriteError("CommandLineService.Process() cannot be called more than once.");
                return false;
            }
            
            List<string> errorList = new List<string>();

            // add help ArgDef
            if (this.HelpCommands != null && this.HelpCommands.Length > 0) {
                this.RegisterFlag(this.HelpCommands, "Displays this help page.");
            }

            _processCalled = true;

            if (args == null) {
                ConsoleUtils.WriteError("Given args array cannot be null");
                return false;
            }

            // parsing given args
            for (int i = 0; i < args.Length; i++) {
                string arg = args[i];

                if (arg == null) {
                    errorList.Add("A given arg cannot be null");
                    continue;
                }
                
                ArgDef argDef = this.GetArgDef(arg);
                if (argDef == null) {

                    // TODO: floating argument?  Default input?

                } else {

                    // mark argument as given
                    if (argDef.Given) {
                        errorList.Add($"Argument {arg} cannot be given more than twice.");
                        continue;
                    }
                    argDef.Given = true;

                    // done if no parameter
                    if (argDef.ParamType == CommandParamType.None)
                        continue;

                    // get parameter
                    if (i >= args.Length - 1) {
                        errorList.Add($"Argument {arg} expects a {argDef.ParamType} parameter.");
                        continue;
                    }
                    argDef.ParamStr = args[i + 1];
                    Debug.Assert(argDef != null, "argDef.ParamStr should not be null, how did this happen?");
                    i++;
                    
                    // validate parameter
                    switch (argDef.ParamType) {
                    case CommandParamType.String:
                        // no invalid strings
                        break;
                    case CommandParamType.FilePath:
                        if (!StringUtils.IsValidFileName(argDef.ParamStr)) {
                            errorList.Add($"Argument parameter {argDef.ParamStr} must be a valid file path.");
                        }
                        break;
                    case CommandParamType.ExistingFilePath:
                        if (!File.Exists(argDef.ParamStr)) {
                            errorList.Add($"Argument parameter {argDef.ParamStr} must point to an existing file.");
                        }
                        break;
                    case CommandParamType.Directory:
                        if (!StringUtils.IsValidPath(argDef.ParamStr)) {
                            errorList.Add($"Argument parameter {argDef.ParamStr} must be a valid directory.");
                        }
                        break;
                    case CommandParamType.ExistingDirectory:
                        if (!Directory.Exists(argDef.ParamStr)) {
                            errorList.Add($"Argument parameter {argDef.ParamStr} must point to an existing directory.");
                        }
                        break;
                    case CommandParamType.Int:
                        int _int;
                        if (!int.TryParse(argDef.ParamStr, out _int)) {
                            errorList.Add($"Argument parameter {argDef.ParamStr} must be an integer.");
                        }
                        break;
                    case CommandParamType.Float:
                        float _float;
                        if (!float.TryParse(argDef.ParamStr, out _float)) {
                            errorList.Add($"Argument parameter {argDef.ParamStr} must be a float.");
                        }
                        break;
                    case CommandParamType.Double:
                        double _double;
                        if (!double.TryParse(argDef.ParamStr, out _double))
                        {
                            errorList.Add($"Argument parameter {argDef.ParamStr} must be a double.");
                        }
                        break;
                    default:
                        errorList.Add($"Need to validate parameter type {argDef.ParamType}");
                        break;
                    }
                    
                }
                
            }
            
            // ensure all required args are given
            foreach (ArgDef argDef in _argDefs) {
                if (argDef.Required && !argDef.Given) {
                    errorList.Add($"Argument {argDef.Keys.FirstOrDefault()} must be given.");
                }
            }

            // return without displaying errors if user wanted help
            if (this.GetHelpRequested()) {
                return true;
            }

            // display errors, if any
            if (errorList.Count > 0) {
                foreach (string error in errorList) {
                    ConsoleUtils.WriteError(error);
                }
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public bool GetFlagGiven(string flag) {
            if (this.ProcessNotCalledLogError()) {
                return false;
            }

            ArgDef argDef = this.GetArgDef(flag);
            if (argDef == null) {
                ConsoleUtils.WriteError($"Flag {flag} was not registered.");
                return false;
            }
            return argDef.Given;
        }

        /// <inheritdoc />
        public string GetArgValue(string arg, string defaultValue = null) {
            if (this.ProcessNotCalledLogError()) {
                return defaultValue;
            }

            ArgDef argDef = this.GetArgDef(arg);
            if (argDef == null) {
                ConsoleUtils.WriteError($"Argument {arg} was not registered.");
                return defaultValue;
            }
            if (argDef.ParamType == CommandParamType.None) {
                ConsoleUtils.WriteError($"Argument {arg} was registered as a flag, and does not expect a param value.");
                return defaultValue;
            }

            if (!argDef.Given)
                return defaultValue;
            return argDef.ParamStr;
        }

        /// <inheritdoc />
        public int GetArgValueInt(string arg, int defaultValue = -1) {
            string valStr = this.GetArgValue(arg);
            if (valStr == null)
                return defaultValue;
            int ret;
            if (int.TryParse(valStr, out ret)) {
                return ret;
            }
            return defaultValue;
        }

        /// <inheritdoc />
        public float GetArgValueFloat(string arg, float defaultValue = -1) {
            string valStr = this.GetArgValue(arg);
            if (valStr == null)
                return defaultValue;
            float ret;
            if (float.TryParse(valStr, out ret)) {
                return ret;
            }
            return defaultValue;
        }

        /// <inheritdoc />
        public double GetArgValueDouble(string arg, double defaultValue = -1) {
            string valStr = this.GetArgValue(arg);
            if (valStr == null)
                return defaultValue;
            double ret;
            if (double.TryParse(valStr, out ret)) {
                return ret;
            }
            return defaultValue;
        }

        /// <inheritdoc />
        public bool GetHelpRequested() {
            if (this.HelpCommands == null)
                return false;
            foreach (string helpCommand in this.HelpCommands) {
                if (this.GetFlagGiven(helpCommand))
                    return true;
            }
            return false;
        }
        
        /// <inheritdoc />
        public void DisplayHelpScreen() {
            
            IEnumerable<ArgDef> requiredArgs = _argDefs.Where(a => a.Required);
            IEnumerable<ArgDef> options = _argDefs.Where(a => !a.Required);

            List<string> requiredKeyStrs = new List<string>();
            List<string> requiredDescriptions = new List<string>();
            List<string> requiredUsageStrs = new List<string>();
            List<string> optionsKeyStrs = new List<string>();
            List<string> optionsDescriptions = new List<string>();

            // fill required key strs, descriptions, and usage strs.
            foreach (ArgDef arg in requiredArgs) {
                requiredKeyStrs.Add(arg.GetHelpKeyStr());
                requiredDescriptions.Add(arg.HelpDescription);
                requiredUsageStrs.Add(arg.GetUsageStr());
            }

            // fill optional key strs and descriptions
            foreach (ArgDef arg in options) {
                optionsKeyStrs.Add(arg.GetHelpKeyStr());
                optionsDescriptions.Add(arg.HelpDescription);
            }

            // find width of key strs column
            int keyStrChars = 0;
            foreach (string s in requiredKeyStrs) {
                keyStrChars = Math.Max(keyStrChars, s.Length);
            }
            foreach (string s in optionsKeyStrs) {
                keyStrChars = Math.Max(keyStrChars, s.Length);
            }
            keyStrChars += 2;

            // write description (taken from assembly)
            Assembly assembly = Assembly.GetCallingAssembly();
            string desc = assembly?.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            if (!string.IsNullOrEmpty(desc)) {
                Console.WriteLine(" Description:");
                Console.WriteLine("  " + desc);
                Console.WriteLine();
            }

            // write usage
            Console.WriteLine(" Usage:");
            Console.Write("  " + ConsoleUtils.ExecutableName);
            if (requiredArgs.Any()) {
                Console.Write(" " + string.Join(" ", requiredUsageStrs));
            }
            if (options.Any()) {
                Console.Write(" [options]");
            }
            Console.WriteLine();

            // write required args
            if (requiredKeyStrs.Count > 0) {
                Console.WriteLine();
                Console.WriteLine(" Required:");
                for (int i = 0; i < requiredKeyStrs.Count; i++) {
                    Console.Write("  ");
                    Console.Write(requiredKeyStrs[i] + new string(' ', keyStrChars - requiredKeyStrs[i].Length));
                    if (!string.IsNullOrEmpty(requiredDescriptions[i])) {
                        Console.Write(": " + requiredDescriptions[i]);
                    }
                    Console.WriteLine();
                }
            }

            // write optional args
            if (optionsKeyStrs.Count > 0) {
                Console.WriteLine();
                Console.WriteLine(" Options:");
                for (int i = 0; i < optionsKeyStrs.Count; i++) {
                    Console.Write("  ");
                    Console.Write(optionsKeyStrs[i] + new string(' ', keyStrChars - optionsKeyStrs[i].Length));
                    if (!string.IsNullOrEmpty(optionsDescriptions[i])) {
                        Console.Write(": " + optionsDescriptions[i]);
                    }
                    Console.WriteLine();
                }
            }
            
        }


        #region Private

        private class ArgDef {

            #region Defined when registering arguments

            public string[] Keys;
            public CommandParamType ParamType;
            public string ParamExample;
            public bool Required;
            public string HelpDescription;

            public string GetUsageStr() {
                return this.Keys.FirstOrDefault() + " " + this.ParamExample;
            }

            public string GetHelpKeyStr() {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Join(", ", this.Keys) + " ");
                switch (this.ParamType) {
                case CommandParamType.None:
                    break;
                case CommandParamType.String:
                    sb.Append("<str>");
                    break;
                case CommandParamType.FilePath:
                case CommandParamType.ExistingFilePath:
                    sb.Append("<file>");
                    break;
                case CommandParamType.Directory:
                case CommandParamType.ExistingDirectory:
                    sb.Append("<dir>");
                    break;
                case CommandParamType.Int:
                    sb.Append("<int>");
                    break;
                case CommandParamType.Float:
                    sb.Append("<float>");
                    break;
                case CommandParamType.Double:
                    sb.Append("<double>");
                    break;
                default:
                    sb.Append($"(need to add {this.ParamType})");
                    break;
                }
                return sb.ToString().Trim();
            }

            #endregion

            #region Defined when processing arguments

            public bool Given = false;
            public string ParamStr = null;

            #endregion
            
        }

        /// <summary>
        /// Gets an ArgDef already registered with the given key.  This is case-sensitive.
        /// Returns null if an Arg could not be found.
        /// </summary>
        /// <param name="argKey">The arg key to search.</param>
        private ArgDef GetArgDef(string argKey) {
            return _argDefs.Where(a => a.Keys.Contains(argKey)).FirstOrDefault();
        }

        /// <summary>
        /// Gets if the given argument is requesting a help command.
        /// </summary>
        /// <param name="arg">arg to check.</param>
        private bool IsHelpCommand(string arg) {
            return this.HelpCommands.Contains(arg);
        }

        /// <summary>
        /// If Process() hasn't been called yet, log an error message and return true;
        /// </summary>
        /// <returns></returns>
        private bool ProcessNotCalledLogError() {
            if (!_processCalled) {
                ConsoleUtils.WriteError("Process() must be called first before getting arg data");
                return true;
            }
            return false;
        }
        
        private List<ArgDef> _argDefs = new List<ArgDef>();
        private bool _processCalled = false;

        #endregion

    }

}
