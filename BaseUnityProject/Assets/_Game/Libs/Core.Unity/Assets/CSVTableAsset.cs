using Core.Unity;
using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Assets {

    /// <summary>
    /// A ScriptableObject that wraps around a csv text file.
    /// </summary>
    //[CreateAssetMenu(fileName = "New CSVTableAsset", menuName = "CSVTableAsset", order = 51)]
    public class CSVTableAsset : ScriptableObject {

        #region Inspector Fields

        [SerializeField]
        [Tooltip("The csv text file this asset wraps around.")]
        protected TextAsset _csvTextAsset = null;

        #endregion

        /// <summary>
        /// Gets the number of columns in the csv table.
        /// </summary>
        public int TableWidth {
            get {
                this.ParseIfNotParsed();
                return _tableWidth;
            }
        }

        /// <summary>
        /// Gets the number of rows in the csv table.
        /// </summary>
        public int TableHeight {
            get {
                if (!this.ParseIfNotParsed())
                    return -1;
                return _rows.Count;
            }
        }

        /// <summary>
        /// Gets the parsed content at the given cell.
        /// </summary>
        public string GetContent(int rowIndex, int columnIndex) {
            if (!this.ParseIfNotParsed())
                return null;

            if (rowIndex < 0 || rowIndex >= this.TableHeight ||
                columnIndex < 0 || columnIndex >= this.TableWidth) {
                Debug.LogError($"Dimensions {rowIndex},{columnIndex} are invalid");
                return null;
            }

            string[] row = _rows[rowIndex];
            if (columnIndex < row.Length) {
                return row[columnIndex];
            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets if the csv asset was parsed.
        /// </summary>
        protected bool IsParsed => _isParsed;

        /// <summary>
        /// Parse implementation.  Can be overridden.
        /// </summary>
        /// <returns>If parsing was a success.</returns>
        protected virtual bool ParseImpl() {
            CSVParser.ParseCSVArgs args = new CSVParser.ParseCSVArgs() {
                IgnoreEmptyRows = false,
                CellFormatFunc = null
            };
            CSVParser.ParseCSV(_csvTextAsset.text, args, _rows);
            return true;
        }

        /// <summary>
        /// Parses the csv asset if not done so yet.  Returns if the parsing was successful.
        /// </summary>
        protected bool ParseIfNotParsed() {
            if (this.IsParsed)
                return true;
            return this.Parse();
        }

        /// <summary>
        /// Parses the csv asset.  Returns if the parsing was successful.
        /// </summary>
        private bool Parse() {
            if (!Application.isPlaying) {
                Debug.LogError("CSV text asset can only be parsed while the game is playing.");
                return false;
            }
            if (_csvTextAsset == null) {
                Debug.LogError("CSV text asset not provided");
                return false;
            }

            if (!this.ParseImpl())
                return false;

            int maxWidth = 0;
            for (int i = 0; i < _rows.Count; i++) {
                maxWidth = Mathf.Max(maxWidth, _rows[i].Length);
            }
            _tableWidth = maxWidth;

            _isParsed = true;
            return true;
        }

        private bool _isParsed = false;
        protected List<string[]> _rows = new List<string[]>();
        private int _tableWidth = -1;
    }
}