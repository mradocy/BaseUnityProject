using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved bool value.
    /// </summary>
    public sealed class SaveBool : SaveProperty {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValue">Value for the property to start with.</param>
        public SaveBool(string key, SaveGroup parent, bool defaultValue) : base(key, parent) {
            _defaultValue = defaultValue;
            this.Value = defaultValue;
        }

        /// <summary>
        /// Bool value of the property.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Resets value to the value provided when the property was registered.
        /// </summary>
        public override void ResetToDefault() {
            this.Value = _defaultValue;
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

            string strVal = valAttr.Value;
            if (strVal == "1" || strVal.ToLower() == "true") {
                this.Value = true;
            } else if (strVal == "0" || strVal.ToLower() == "false") {
                this.Value = false;
            } else {
                return LoadStatus.ParseError;
            }
            
            return LoadStatus.Ok;
        }

        /// <summary>
        /// Caches a copy of the value.  This will be used when creating the save xml.
        /// </summary>
        public sealed override void CacheValue() {
            _cachedValue = this.Value;
        }

        /// <summary>
        /// Create an XmlElement that represents this bool property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("Bool");
            element.SetAttribute("key", this.Key);
            element.SetAttribute("value", _cachedValue ? "True" : "False");
            return element;
        }

        private bool _defaultValue;
        private bool _cachedValue;
    }
}