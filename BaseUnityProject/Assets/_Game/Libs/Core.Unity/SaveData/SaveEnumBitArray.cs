using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved bit array of enums.  Values are represented as ints internally.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    public sealed class SaveEnumBitArray<TEnum> : SaveIntBitArray where TEnum : System.Enum {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="max">Max value of the enum that can be stored in this array.  Range is [0, max]</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValues">Value for the property to start with.</param>
        public SaveEnumBitArray(string key, TEnum max, SaveGroup parent, IEnumerable<TEnum> defaultValues) : base(key, System.Convert.ToInt32(max), parent, null) {
            if (defaultValues != null) {
                // set default values
                foreach (TEnum enumVal in defaultValues) {
                    this.Add(enumVal);
                }
                for (int i = 0; i < _bits.Length; i++) {
                    _defaultBits[i] = _bits[i];
                }
            }
        }

        /// <summary>
        /// Adds the given enum to the bit array of values.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(TEnum item) {
            this.Add(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Removes the given enum from the bit array.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public void Remove(TEnum item) {
            this.Remove(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Gets if the given enum is currently contained in the bit array.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Is contained.</returns>
        public bool Contains(TEnum item) {
            return this.Contains(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Create an XmlElement that represents this enum bit array.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("EnumBitArray");
            element.SetAttribute("key", this.Key);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _cachedBits.Length; i++) {
                byte b = _cachedBits[i];
                for (int j = 0; j < 8; j++) {
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
    }
}