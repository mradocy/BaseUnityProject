using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Services {

    public enum CommandParamType {

        /// <summary>
        /// No parameter is given, the argument just exists by itself.
        /// </summary>
        None,
        /// <summary>
        /// Parameter can be anything.
        /// </summary>
        String,
        /// <summary>
        /// Parameter must be a string representing a file path.
        /// </summary>
        FilePath,
        /// <summary>
        /// Parameter must be a string representing a file path that currently exists.
        /// </summary>
        ExistingFilePath,
        /// <summary>
        /// Parameter must be a string representing a directory.
        /// </summary>
        Directory,
        /// <summary>
        /// Parameter must be a string representing a directory that currently exists.
        /// </summary>
        ExistingDirectory,
        /// <summary>
        /// Parameter must be an integer.
        /// </summary>
        Int,
        /// <summary>
        /// Parameter must be a float.
        /// </summary>
        Float,
        /// <summary>
        /// Parameter must be a double.
        /// </summary>
        Double,

    }

}
