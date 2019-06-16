using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base.Services.Implementations {

    /// <summary>
    /// Implementation of IFileService
    /// </summary>
    public class FileService : IFileService {

        /// <inheritdoc />
        public string ReadFromFile(string filePath) {
            return File.ReadAllText(filePath);
        }

        /// <inheritdoc />
        public string[] ReadLinesFromFile(string filePath) {
            return File.ReadAllLines(filePath);
        }

        /// <inheritdoc />
        public void WriteToFile(string filePath, string content) {
            File.WriteAllText(filePath, content);
        }

    }

}
