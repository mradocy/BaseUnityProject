using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved set of ints.  Provides faster access than <see cref="SaveIntSet"/>, but with a limited size.
    /// The base class for <see cref="SaveEnumBitArray{TEnum}"/>.
    /// </summary>
    public class SaveIntBitArray : SaveProperty {

        /// <summary>
        /// Global max for any <see cref="SaveIntBitArray"/>.
        /// </summary>
        public const uint GlobalMax = 255;

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="max">Max value that can be stored in this array.  Range is [0, max]</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValues">Value for the property to start with.</param>
        public SaveIntBitArray(string key, int max, SaveGroup parent, IEnumerable<int> defaultValues) : base(key, parent) {
            if (max < 0 || max > GlobalMax) {
                throw new System.ArgumentException($"Bit array max {max} must be non-negative and lower than GlobalMax {GlobalMax}.", nameof(max));
            }

            // initialize arrays
            this.Max = max;
            _bits = new byte[max / 8 + 1];
            _defaultBits = new byte[_bits.Length];
            _cachedBits = new byte[_bits.Length];

            // set default values
            if (defaultValues != null) {
                foreach (int val in defaultValues) {
                    this.Add(val);
                }
                for (int i = 0; i < _bits.Length; i++) {
                    _defaultBits[i] = _bits[i];
                }
            }
        }

        /// <summary>
        /// Resets values to the values provided when the property was registered.
        /// </summary>
        public sealed override void ResetToDefault() {
            for (int i=0; i < _bits.Length; i++) {
                _bits[i] = _defaultBits[i];
            }
        }

        /// <summary>
        /// Adds the given int to the bit array of values.  Returns if the array changed.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public bool Add(int item) {
            if (item < 0 || item > this.Max) {
                throw new System.ArgumentException($"Value {item} cannot be added to this bit array.  Range is [0, {this.Max}]", nameof(item));
            }
            if (this.Contains(item))
                return false;

            _bits[item / 8] |= (byte)(1 << (item % 8));
            return true;
        }

        /// <summary>
        /// Removes the given int from the bit array.  Returns if the array changed.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public bool Remove(int item) {
            if (item < 0 || item > this.Max)
                return false;
            if (!this.Contains(item))
                return false;

            _bits[item / 8] &= (byte)~(1 << (item % 8));
            return true;
        }

        /// <summary>
        /// Gets if the given int is currently contained in the bit array.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Is contained.</returns>
        public bool Contains(int item) {
            if (item < 0 || item > this.Max)
                return false;

            return (_bits[item / 8] & (byte)(1 << (item % 8))) != 0;
        }

        /// <summary>
        /// Clears the bit array.
        /// </summary>
        public void Clear() {
            for (int i=0; i < _bits.Length; i++) {
                _bits[i] = 0;
            }
        }

        /// <summary>
        /// Max value that can be added to this bit array.  Min is 0.
        /// </summary>
        public int Max { get; }

        /// <summary>
        /// Parses the given XmlNode.
        /// Returns the status of the load.
        /// </summary>
        /// <param name="xmlNode">Node to parse.</param>
        /// <returns>LoadStatus</returns>
        public sealed override LoadStatus ParseXML(XmlNode xmlNode) {
            if (xmlNode == null) {
                throw new System.ArgumentNullException();
            }

            if (xmlNode.HasChildNodes) {
                return LoadStatus.ParseError;
            }

            XmlAttribute valAttr = xmlNode.Attributes?["value"];
            if (valAttr == null) {
                return LoadStatus.ParseError;
            }

            // parse values
            this.Clear();
            string str = valAttr.Value;
            if (!string.IsNullOrEmpty(str)) {
                int startIndex = 0;
                while (startIndex < str.Length) {
                    int delimIndex = str.IndexOf(',', startIndex);
                    if (delimIndex == -1) {
                        delimIndex = str.Length;
                    }
                    if (int.TryParse(str.Substring(startIndex, delimIndex - startIndex).Trim(), out int i)) {
                        this.Add(i);
                    } else {
                        return LoadStatus.ParseError;
                    }
                    startIndex = delimIndex + 1;
                }
            }

            return LoadStatus.Ok;
        }

        /// <summary>
        /// Caches a copy of the value.  This will be used when creating the save xml.
        /// </summary>
        public sealed override void CacheValue() {
            for (int i=0; i < _bits.Length; i++) {
                _cachedBits[i] = _bits[i];
            }
        }

        /// <summary>
        /// Create an XmlElement that represents this int set property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("IntBitArray");
            element.SetAttribute("key", this.Key);

            StringBuilder sb = new StringBuilder();
            for (int i=0; i < _cachedBits.Length; i++) {
                byte b = _cachedBits[i];
                for (int j=0; j < 8; j++) {
                    if ((b & (1 << j)) != 0) {
                        sb.Append(i * 8 + j);
                        sb.Append(',');
                    }
                }
            }
            if (sb.Length > 0) {
                // remove trailing comma
                sb.Remove(sb.Length - 1, 1);
            }
            element.SetAttribute("value", sb.ToString());
            return element;
        }

        protected byte[] _bits;
        protected byte[] _defaultBits;
        protected byte[] _cachedBits;
    }
}