using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved set of enums.  Values are represented as ints internally.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    public class SaveEnumSet<TEnum> : SaveIntSet where TEnum : System.Enum {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValue">Value for the property to start with.</param>
        public SaveEnumSet(string key, SaveGroup parent, IEnumerable<TEnum> defaultValues) : base(key, parent, null) {
            if (defaultValues != null) {
                foreach (TEnum enumVal in defaultValues) {
                    int val = System.Convert.ToInt32(enumVal);
                    this._defaultValues.Add(val);
                    this._values.Add(val);
                }
            }
        }

        /// <summary>
        /// Adds the given item to the set of values.  Returns if the item was added (i.e. not already in the set).
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <returns>Was added</returns>
        public bool Add(TEnum item) {
            return this._values.Add(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Removes the given item from the set of values.  Returns if an item was removed (i.e. was already in the set).
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Was removed</returns>
        public bool Remove(TEnum item) {
            return this._values.Remove(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Gets if the given item is currently contained in the set.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>Is contained.</returns>
        public bool Contains(TEnum item) {
            return this._values.Contains(System.Convert.ToInt32(item));
        }

        /// <summary>
        /// Create an XmlElement that represents this int property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("EnumSet");
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
    }
}