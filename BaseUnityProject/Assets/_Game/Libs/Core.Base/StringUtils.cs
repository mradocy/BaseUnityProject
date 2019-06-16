using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Base {

    public static class StringUtils {

        /// <summary>
        /// Returns if the given string is a valid path.
        /// This only compares the string's content with invalid characters.  This function may fail with certain edge cases.
        /// </summary>
        /// <param name="str">The string to check.</param>
        public static bool IsValidPath(string str) {
            if (string.IsNullOrWhiteSpace(str)) return false;
            return str.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }

        /// <summary>
        /// Returns if the given string is a valid file name.
        /// This only compares the string's content with invalid characters.  This function may fail with certain edge cases.
        /// </summary>
        /// <param name="str">The string to check.</param>
        public static bool IsValidFileName(string str) {
            if (string.IsNullOrWhiteSpace(str)) return false;
            return str.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }

        /// <summary>
        /// Converts the given text to base64 using UTF8 encoding.
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Converted text</returns>
        public static string ToBase64(string text) {
            return ToBase64(text, Encoding.UTF8);
        }

        /// <summary>
        /// Converts the given text to base64.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Converted text</returns>
        public static string ToBase64(string text, Encoding encoding) {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }

            byte[] textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        /// <summary>
        /// Attempts to parse the given base64 text using UTF8 encoding.  Returns if the conversion was successful.
        /// </summary>
        /// <param name="base64Text">Base64 text to convert.</param>
        /// <param name="decodedText">Out param to put the decoded text.</param>
        /// <returns>Success</returns>
        public static bool TryParseBase64(string base64Text, out string decodedText) {
            return TryParseBase64(base64Text, Encoding.UTF8, out decodedText);
        }

        /// <summary>
        /// Attempts to parse the given base64 text.  Returns if the conversion was successful.
        /// </summary>
        /// <param name="base64Text">Base64 text to convert.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="decodedText">Out param to put the decoded text.</param>
        /// <returns>Success</returns>
        public static bool TryParseBase64(string base64Text, Encoding encoding, out string decodedText) {
            if (string.IsNullOrEmpty(base64Text)) {
                decodedText = base64Text;
                return false;
            }

            try {
                byte[] textAsBytes = Convert.FromBase64String(base64Text);
                decodedText = encoding.GetString(textAsBytes);
                return true;
            } catch (Exception) {
                decodedText = null;
                return false;
            }
        }

    }

}
