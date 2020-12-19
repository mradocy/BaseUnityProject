using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved set of ints.  The base class for <see cref="SaveEnumSet{TEnum}"/>.
    /// </summary>
    public class SaveIntSet : SaveProperty {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValues">Value for the property to start with.</param>
        public SaveIntSet(string key, SaveGroup parent, IEnumerable<int> defaultValues) : base(key, parent) {
            if (defaultValues == null) {
                _defaultValues = new HashSet<int>();
            } else {
                _defaultValues = new HashSet<int>(defaultValues);
            }
            _values = new HashSet<int>(_defaultValues);
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
        /// Adds the given int to the set of values.  Returns if the item was added (i.e. not already in the set).
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <returns>Was added</returns>
        public bool Add(int item) {
            return _values.Add(item);
        }

        /// <summary>
        /// Removes the given int from the set of values.  Returns if an item was removed (i.e. was already in the set).
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Was removed</returns>
        public bool Remove(int item) {
            return _values.Remove(item);
        }

        /// <summary>
        /// Gets if the given int is currently contained in the set.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Is contained.</returns>
        public bool Contains(int item) {
            return _values.Contains(item);
        }

        /// <summary>
        /// Clears the set of ints.
        /// </summary>
        public void Clear() {
            _values.Clear();
        }

        /// <summary>
        /// Gets the number of items in the set.
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
        public override LoadStatus ParseXML(XmlNode xmlNode) {
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
            foreach (int val in _values) {
                _cachedValues.Add(val);
            }
        }

        /// <summary>
        /// Create an XmlElement that represents this int set property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("IntSet");
            element.SetAttribute("key", this.Key);

            // sorted would be nice
            List<int> valList = new List<int>(_cachedValues);
            valList.Sort();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (int val in valList) {
                sb.Append(val);
                if (i < _cachedValues.Count - 1) {
                    sb.Append(',');
                }
                i++;
            }
            element.SetAttribute("value", sb.ToString());
            return element;
        }

        protected HashSet<int> _values;
        protected HashSet<int> _defaultValues;
        protected HashSet<int> _cachedValues = new HashSet<int>();
    }
}