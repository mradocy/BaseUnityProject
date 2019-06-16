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
        /// <param name="defaultValue">Value for the property to start with.</param>
        public SaveIntSet(string key, SaveGroup parent, IEnumerable<int> defaultValues) : base(key, parent) {
            if (defaultValues == null) {
                this._defaultValues = new HashSet<int>();
            } else {
                this._defaultValues = new HashSet<int>(defaultValues);
            }
            this._values = new HashSet<int>(this._defaultValues);
        }

        /// <summary>
        /// Resets values to the values provided when the property was registered.
        /// </summary>
        public override void ResetToDefault() {
            this._values.Clear();
            foreach (int val in this._defaultValues) {
                this._values.Add(val);
            }
        }

        /// <summary>
        /// Adds the given int to the set of values.  Returns if the item was added (i.e. not already in the set).
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <returns>Was added</returns>
        public bool Add(int item) {
            return this._values.Add(item);
        }

        /// <summary>
        /// Removes the given int from the set of values.  Returns if an item was removed (i.e. was already in the set).
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Was removed</returns>
        public bool Remove(int item) {
            return this._values.Remove(item);
        }

        /// <summary>
        /// Gets if the given int is currently contained in the set.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Is contained.</returns>
        public bool Contains(int item) {
            return this._values.Contains(item);
        }

        /// <summary>
        /// Clears the set of ints.
        /// </summary>
        public void Clear() {
            this._values.Clear();
        }

        /// <summary>
        /// Gets the number of items in the set.
        /// </summary>
        public int Count {
            get { return this._values.Count; }
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
            this._values.Clear();
            string[] valStrs = valAttr.Value.Split(',');
            foreach (string valStr in valStrs) {
                int i;
                if (int.TryParse(valStr.Trim(), out i)) {
                    this._values.Add(i);
                } else {
                    return LoadStatus.ParseError;
                }
            }

            return LoadStatus.Ok;
        }

        /// <summary>
        /// Create an XmlElement that represents this int property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("IntSet");
            element.SetAttribute("key", this.Key);

            // sorted would be nice
            List<int> valList = new List<int>(this._values);
            valList.Sort();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (int val in valList) {
                sb.Append(val);
                if (i < this._values.Count - 1) {
                    sb.Append(',');
                }
                i++;
            }
            element.SetAttribute("value", sb.ToString());
            return element;
        }

        protected HashSet<int> _values;
        protected HashSet<int> _defaultValues;
    }
}