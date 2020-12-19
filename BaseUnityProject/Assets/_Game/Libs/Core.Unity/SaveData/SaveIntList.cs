using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved list of ints.  The base class for <see cref="SaveEnumList{TEnum}"/>.
    /// </summary>
    public class SaveIntList : SaveProperty {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValues">Value for the property to start with.</param>
        public SaveIntList(string key, SaveGroup parent, IEnumerable<int> defaultValues) : base(key, parent) {
            if (defaultValues == null) {
                _defaultValues = new List<int>();
            } else {
                _defaultValues = new List<int>(defaultValues);
            }
            _values = new List<int>(_defaultValues);
        }

        /// <summary>
        /// Resets values to the values provided when the property was registered.
        /// </summary>
        public sealed override void ResetToDefault() {
            _values.Clear();
            foreach (int val in _defaultValues) {
                _values.Add(val);
            }
        }

        /// <summary>
        /// Adds the given int to the list of values.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(int item) {
            _values.Add(item);
        }

        /// <summary>
        /// Sets the value at the given index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="item">Value</param>
        public void SetAt(int index, int item) {
            if (index < 0 || index >= _values.Count) {
                Debug.LogError($"Index {index} is invalid");
                return;
            }
            _values[index] = item;
        }

        /// <summary>
        /// Removes the given int from the list of values.  Returns if an item was removed (i.e. was already in the list).
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Was removed</returns>
        public bool Remove(int item) {
            return _values.Remove(item);
        }

        /// <summary>
        /// Removes the item at the given index from the list of values.
        /// </summary>
        /// <param name="index">Index of item to remove.</param>
        public void RemoveAt(int index) {
            _values.RemoveAt(index);
        }

        /// <summary>
        /// Gets the index of an item in the list, or -1 if it doesn't exist.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Index.</returns>
        public int IndexOf(int item) {
            return _values.IndexOf(item);
        }

        /// <summary>
        /// Gets the int value at the given index.  Throws an error if the index is invalid.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Element</returns>
        public int IntAt(int index) {
            return _values[index];
        }

        /// <summary>
        /// Gets all the values of this list copied to an int array.
        /// </summary>
        /// <returns></returns>
        public int[] ToIntArray() {
            return _values.ToArray();
        }

        /// <summary>
        /// Clears the list of ints.
        /// </summary>
        public void Clear() {
            _values.Clear();
        }

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        public int Count {
            get { return _values.Count; }
        }

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
            _values.Clear();
            string str = valAttr.Value;
            if (!string.IsNullOrEmpty(str)) {
                int startIndex = 0;
                while (startIndex < str.Length) {
                    int delimIndex = str.IndexOf(',', startIndex);
                    if (delimIndex == -1) {
                        delimIndex = str.Length;
                    }
                    if (int.TryParse(str.Substring(startIndex, delimIndex - startIndex).Trim(), out int i)) {
                        _values.Add(i);
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
            _cachedValues.Clear();
            _cachedValues.AddRange(_values);
        }

        /// <summary>
        /// Create an XmlElement that represents this int list property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("IntList");
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

        protected List<int> _values;
        protected List<int> _defaultValues;
        protected List<int> _cachedValues = new List<int>();
    }
}