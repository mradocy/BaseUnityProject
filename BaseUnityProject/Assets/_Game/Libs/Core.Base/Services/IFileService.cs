using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Services {

    public interface IFileService {

        /// <summary>
        /// Gets all the text stored in the given file.
        /// </summary>
        /// <param name="filePath">Path pointing to the file.</param>
        string ReadFromFile(string filePath);

        /// <summary>
        /// Gets all the text stored in the given file, as lines.
        /// </summary>
        /// <param name="filePath">Path pointing to the file.</param>
        string[] ReadLinesFromFile(string filePath);

        /// <summary>
        /// Writes the given content to the given file.
        /// </summary>
        /// <param name="filePath">Path to the file to write to.</param>
        /// <param name="content">The content to write.</param>
        void WriteToFile(string filePath, string content);

    }

}
