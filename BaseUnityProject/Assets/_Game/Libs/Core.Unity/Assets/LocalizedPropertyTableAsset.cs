using Core.Unity;
using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Core.Unity.Assets {

    /// <summary>
    /// A ScriptableObject that wraps around a csv text file intended for getting values associated with property keys.
    /// The first column are for property keys, the other columns are for values separated by localization.
    /// The first row is a header specifying the localization codes for each column.
    /// </summary>
    [CreateAssetMenu(fileName = "New LocalizedPropertyTableAsset", menuName = "Localization/LocalizedPropertyTableAsset", order = 51)]
    public class LocalizedPropertyTableAsset : LocalizedTableAsset {

        /// <summary>
        /// Gets the value for the given property key with the current localization.
        /// Null is returned only if the key or localization couldn't be found.
        /// </summary>
        public string GetValue(string propertyKey) {
            return this.GetValue(propertyKey, LocalizationSettings.Localization);
        }

        /// <summary>
        /// Gets the value for the given property key and localization.  If the <paramref name="propertyKey"/> can't be found, an error is displayed.
        /// This can be used by the editor without parsing the text asset.
        /// </summary>
        public string GetValue(string propertyKey, LocalizationCode localizationCode) {
            if (this.TryGetValue(propertyKey, localizationCode, out string value)) {
                return value;
            } else {
                if (!_propertyToRowMap.ContainsKey(propertyKey)) {
                    Debug.LogError($"Could not find property key \"{propertyKey}\" in table");
                }
                return null;
            }
        }

        /// <summary>
        /// Attempts to get the value for the given property key with the current localization.  No error is displayed if the <paramref name="propertyKey"/> couldn't be found.
        /// Null is returned only if the key or localization couldn't be found.
        /// This can be used by the editor without parsing the text asset.
        /// </summary>
        public bool TryGetValue(string propertyKey, out string value) {
            return this.TryGetValue(propertyKey, LocalizationSettings.Localization, out value);
        }

        /// <summary>
        /// Attempts to get the value for the given property key and localization.  No error is displayed if the <paramref name="propertyKey"/> couldn't be found.
        /// Null is returned only if the key or localization couldn't be found.
        /// This can be used by the editor without parsing the text asset.
        /// </summary>
        public bool TryGetValue(string propertyKey, LocalizationCode localizationCode, out string value) {
            value = null;
            if (Application.isPlaying) {
                // in game.  Parse once, then quickly get the values
                if (!this.ParseIfNotParsed())
                    return false;

                // get column from localization
                int col = this.GetLocalizationColumn(localizationCode);
                if (col <= 0)
                    return false;

                if (_propertyToRowMap.TryGetValue(propertyKey, out int row)) {
                    string[] rowValues = _rows[row];
                    if (col >= rowValues.Length) {
                        value = string.Empty;
                        return true;
                    }
                    value = rowValues[col];
                    return true;
                } else {
                    return false;
                }

            } else {
                // in editor.  Attempt to get value without caching the properties
                value = this.GetValueFromTextAsset(propertyKey, localizationCode);
                return value != null;
            }
        }

        /// <summary>
        /// Function used to format the content of a cell while parsing the csv file.
        /// </summary>
        protected static string CellFormat(string str) {
            // trim comment
            int commentIndex = str.IndexOf("//");
            if (commentIndex != -1) {
                str = str.Substring(0, commentIndex);
            }

            // unescape
            try {
                str = System.Text.RegularExpressions.Regex.Unescape(str);
            } catch (System.Exception ex) {
                Debug.LogError($"Regex failed to unescape \"{str}\" and this throws an error for some reason: {ex.Message}");
            }

            return str;
        }

        protected override bool ParseImpl() {
            CSVParser.ParseCSVArgs args = new CSVParser.ParseCSVArgs() {
                IgnoreEmptyRows = true,
                CellFormatFunc = CellFormat
            };
            CSVParser.ParseCSV(_csvTextAsset.text, args, _rows);

            if (!this.ParseLocalizations())
                return false;

            // create property dictionary
            _propertyToRowMap = new Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase);
            for (int row = 1; row < _rows.Count; row++) {
                string key = _rows[row][0];
                if (string.IsNullOrEmpty(key)) {
                    Debug.LogError("Cannot have empty key in table");
                    return false;
                }
                if (_propertyToRowMap.ContainsKey(key)) {
                    Debug.LogError($"Already have key \"{key}\" in table");
                    return false;
                }
                _propertyToRowMap.Add(key, row);
            }

            return true;
        }

        /// <summary>
        /// Get property from the text asset directly, without caching the properties.
        /// Used in the editor.  Errors are not logged.
        /// </summary>
        private string GetValueFromTextAsset(string propertyKey, LocalizationCode localizationCode) {
            if (_csvTextAsset == null) {
                return null;
            }

            string[] stringArray = new string[CSVParser.MaxColumns];
            int col = -1;
            bool firstRow = true;
            using (StringReader stringReader = new StringReader(_csvTextAsset.text)) {
                while (true) {
                    // read line
                    string line = stringReader.ReadLine();
                    if (line == null)
                        break;

                    // read each column of a row
                    int columnCount = CSVParser.SplitCSVRow(line, stringArray);

                    if (firstRow) {
                        // first row is for localizations.  Get the column for the given localization code
                        for (int i = 1; i < columnCount; i++) {
                            string str = CellFormat(stringArray[i]);
                            LocalizationCode lc = LocalizationSettings.StringToCode(str);
                            if (lc == localizationCode) {
                                col = i;
                                break;
                            }
                        }
                        if (col < 0) {
                            // the given localization couldn't be found
                            return null;
                        }
                        firstRow = false;
                    } else {
                        // row is a match if the first column matches the property key
                        if (string.Equals(propertyKey, CellFormat(stringArray[0]), System.StringComparison.OrdinalIgnoreCase)) {
                            return CellFormat(stringArray[col]);
                        }
                    }
                }
            }

            return null;
        }

        private Dictionary<string, int> _propertyToRowMap = null;
    }
}