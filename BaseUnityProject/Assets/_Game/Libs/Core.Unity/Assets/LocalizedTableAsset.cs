using Core.Unity;
using Core.Unity.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Assets {

    /// <summary>
    /// Extension of <see cref="CSVTableAsset"/> where the first row consists entirely (except the first column) of localization codes.
    /// e.g.
    ///     , en-US, ja
    /// </summary>
    //[CreateAssetMenu(fileName = "New LocalizedTableAsset", menuName = "LocalizedTableAsset", order = 51)]
    public class LocalizedTableAsset : CSVTableAsset {

        /// <summary>
        /// Gets the column that corresponds to the given localization code.
        /// </summary>
        public int GetLocalizationColumn(LocalizationCode localizationCode) {
            if (!this.ParseIfNotParsed())
                return -1;
            for (int col = 1; col < _localizations.Length; col++) {
                if (_localizations[col] == localizationCode) {
                    return col;
                }
            }
            Debug.LogError($"Localization {localizationCode} was not found in this localized table csv");
            return -1;
        }

        protected override bool ParseImpl() {
            CSVParser.ParseCSVArgs args = new CSVParser.ParseCSVArgs() {
                IgnoreEmptyRows = false,
                CellFormatFunc = null
            };
            CSVParser.ParseCSV(_csvTextAsset.text, args, _rows);

            return this.ParseLocalizations();
        }

        protected bool ParseLocalizations() {
            if (_rows.Count < 1) {
                Debug.LogError("Localized table CSV file must have at least 1 row ");
                return false;
            }
            int width = _rows[0].Length;
            if (width < 2) {
                Debug.LogError("The first row of a localized table CSV file must have at least 2 columns");
                return false;
            }
            _localizations = new LocalizationCode[width];
            for (int col = 1; col < width; col++) {
                string str = _rows[0][col];
                LocalizationCode localizationCode = LocalizationSettings.StringToCode(str);
                if (localizationCode == LocalizationCode.None) {
                    Debug.LogError($"\"{str}\" is not a valid localization code.  Localization codes must be defined in the first row, starting with the second column");
                    return false;
                }
                _localizations[col] = localizationCode;
            }

            return true;
        }

        private LocalizationCode[] _localizations = null;
    }
}