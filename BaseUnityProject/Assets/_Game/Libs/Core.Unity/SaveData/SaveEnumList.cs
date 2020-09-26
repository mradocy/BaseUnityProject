using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved list of enums.  Values are represented as ints internally.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    public sealed class SaveEnumList<TEnum> : SaveIntList where TEnum : System.Enum {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValues">Value for the property to start with.</param>
        public SaveEnumList(string key, SaveGroup parent, IEnumerable<TEnum> defaultValues) : base(key, parent, null) {
            if (defaultValues != null) {
                foreach (TEnum enumVal in defaultValues) {
                    int val = System.Convert.ToInt32(enumVal);
                    _defaultValues.Add(val);
                    _values.Add(val);
                }
            }
        }

        /// <summary>
        /// Adds the given item to the list of values.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(TEnum item) {
            _values.Add(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Removes the given item from the set of values.  Returns if an item was removed (i.e. was already in the set).
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Was removed</returns>
        public bool Remove(TEnum item) {
            return _values.Remove(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Gets the index of the item in the list, or -1 if the item isn't included.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Index.</returns>
        public int IndexOf(TEnum item) {
            return _values.IndexOf(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Gets the enum value at the given index.  Throws an error if the index is invalid.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Element</returns>
        public TEnum At(int index) {
            return (TEnum)(object)_values[index];
        }

        /// <summary>
        /// Gets all the values of this list copied to an array.
        /// </summary>
        /// <returns></returns>
        public TEnum[] ToArray() {
            TEnum[] arr = new TEnum[_values.Count];
            for (int i=0; i < _values.Count; i++) {
                arr[i] = (TEnum)(object)_values[i];
            }
            return arr;
        }

        /// <summary>
        /// Create an XmlElement that represents this enum list property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("EnumList");
            element.SetAttribute("key", this.Key);

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (int val in _cachedValues) {
                sb.Append(val);
                if (i < _cachedValues.Count - 1) {
                    sb.Append(',');
                }
                i++;
            }
            element.SetAttribute("value", sb.ToString());
            return element;
        }
    }
}