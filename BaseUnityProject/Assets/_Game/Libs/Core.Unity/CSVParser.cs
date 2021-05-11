using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using System.Text;
using System.Globalization;
using System.IO;

namespace Core.Unity {

    public static class CSVParser {

        /// <summary>
        /// The max number of columns this parser will handle.
        /// </summary>
        public const int MaxColumns = 20;

        /// <summary>
        /// Defines additional arguments for the parser.
        /// </summary>
        public struct ParseCSVArgs {
            /// <summary>
            /// If true, then rows will be ignored if all the cells in a row are empty.
            /// </summary>
            public bool IgnoreEmptyRows { get; set; }
            /// <summary>
            /// An optional function that format the string of a cell before being added to a row.
            /// </summary>
            public System.Func<string, string> CellFormatFunc { get; set; }
        }

        /// <summary>
        /// Parses the given content of a csv file into rows.
        /// </summary>
        /// <param name="csvText">The content of a csv file, all in one string</param>
        /// <param name="args">Additional args for the parser.</param>
        /// <param name="rows">This list will be filled with the rows of the csv file.</param>
        /// <returns>If parsing was a success</returns>
        public static void ParseCSV(string csvText, ParseCSVArgs args, List<string[]> rows) {
            using (StringReader sr = new StringReader(csvText)) {
                ParseCSV(sr, args, rows);
            }
        }

        /// <summary>
        /// Parses csv lines from a given text reader into rows.
        /// </summary>
        /// <param name="textReader">The text reader to read.  Does not handle disposal, opening/closing, etc.</param>
        /// <param name="args">Additional args for the parser.</param>
        /// <param name="rows">This list will be filled with the rows of the csv file.</param>
        public static void ParseCSV(TextReader textReader, ParseCSVArgs args, List<string[]> rows) {
            rows.Clear();
            string[] stringArray = new string[MaxColumns];
            while (true) {
                // read line
                string line = textReader.ReadLine();
                if (line == null)
                    break;

                // read each column of a row
                int columnCount = SplitCSVRow(line, stringArray);

                // parse columns into row
                string[] row = new string[columnCount];
                bool allCellsEmpty = true;
                for (int i = 0; i < columnCount; i++) {
                    string cell = stringArray[i];
                    if (cell == null)
                        continue;

                    // format
                    if (args.CellFormatFunc != null) {
                        cell = args.CellFormatFunc(cell);
                    }

                    if (cell != string.Empty) {
                        allCellsEmpty = false;
                    }

                    // add to row
                    row[i] = cell;
                }

                // ignore if all cells are empty
                if (!args.IgnoreEmptyRows || !allCellsEmpty) {
                    rows.Add(row);
                }
            }
        }

        /// <summary>
        /// Assuming the given string is a line representing a csv row, parses the line and separates it into its comma separated values.
        /// </summary>
        /// <param name="line">This csv line to parse.</param>
        /// <returns>The elements of the csv row.</returns>
        public static string[] SplitCSVRow(string line) {
            if (line == null)
                return null;

            StringBuilder elementBuilder = new StringBuilder();
            List<string> elementsList = new List<string>();
            int index;
            bool inQuotes = false;

            // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.stringinfo?view=net-5.0
            // Use the enumerator returned from GetTextElementEnumeratormethod to examine each real character.
            TextElementEnumerator charEnum = StringInfo.GetTextElementEnumerator(line);
            while (charEnum.MoveNext()) {
                index = charEnum.ElementIndex;
                char c = line[index];
                if (inQuotes) {
                    // currently in quotes, commas don't count
                    if (c == '\"') {
                        // hit quote
                        if (index + 1 < line.Length && line[index + 1] == '\"') {
                            // double quotes while in quotes, just add one quote
                            elementBuilder.Append('\"');
                            charEnum.MoveNext();
                        } else {
                            // single quote, end quoted expression
                            inQuotes = false;
                        }
                    } else {
                        // add character
                        elementBuilder.Append(charEnum.GetTextElement());
                    }
                } else {
                    // outside of quotes
                    if (c == '\"') {
                        // hit quote, enter quotes
                        inQuotes = true;
                    } else if (c == ',') {
                        // hit comma, make new element
                        elementsList.Add(elementBuilder.ToString());
                        elementBuilder.Clear();
                    } else {
                        // add character
                        elementBuilder.Append(charEnum.GetTextElement());
                    }
                }
            }

            // add remaining part of line
            elementsList.Add(elementBuilder.ToString());

            return elementsList.ToArray();
        }

        /// <summary>
        /// Assuming the given string is a line representing a csv row, parses the line and separates it into its comma separated values.
        /// </summary>
        /// <param name="line">This csv line to parse.</param>
        /// <param name="elements">Given array to place the elements of the csv line.</param>
        /// <returns>Number of elements.</returns>
        public static int SplitCSVRow(string line, string[] elements) {
            if (line == null)
                return -1;

            StringBuilder elementBuilder = new StringBuilder();
            int elementCount = 0;
            int index;
            bool inQuotes = false;

            // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.stringinfo?view=net-5.0
            // Use the enumerator returned from GetTextElementEnumeratormethod to examine each real character.
            TextElementEnumerator charEnum = StringInfo.GetTextElementEnumerator(line);
            while (charEnum.MoveNext()) {
                index = charEnum.ElementIndex;
                char c = line[index];
                if (inQuotes) {
                    // currently in quotes, commas don't count
                    if (c == '\"') {
                        // hit quote
                        if (index + 1 < line.Length && line[index + 1] == '\"') {
                            // double quotes while in quotes, just add one quote
                            elementBuilder.Append('\"');
                            charEnum.MoveNext();
                        } else {
                            // single quote, end quoted expression
                            inQuotes = false;
                        }
                    } else {
                        // add character
                        elementBuilder.Append(charEnum.GetTextElement());
                    }
                } else {
                    // outside of quotes
                    if (c == '\"') {
                        // hit quote, enter quotes
                        inQuotes = true;
                    } else if (c == ',') {
                        // hit comma, make new element
                        if (elementCount >= elements.Length) {
                            Debug.LogError($"Given elements array with length {elements.Length} is too small to contain the elements of csv line {line}");
                            return elementCount;
                        }
                        elements[elementCount] = elementBuilder.ToString();
                        elementBuilder.Clear();
                        elementCount++;
                    } else {
                        // add character
                        elementBuilder.Append(charEnum.GetTextElement());
                    }
                }
            }

            // add remaining part of line
            if (elementCount >= elements.Length) {
                Debug.LogError($"Given elements array with length {elements.Length} is too small to contain the elements of csv line {line}");
                return elementCount;
            }
            elements[elementCount] = elementBuilder.ToString();
            elementCount++;

            return elementCount;
        }

    }
}