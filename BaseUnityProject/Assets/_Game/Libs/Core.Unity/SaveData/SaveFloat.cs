using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved float value.
    /// </summary>
    public class SaveFloat : SaveProperty {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValue">Value for the property to start with.</param>
        public SaveFloat(string key, SaveGroup parent, float defaultValue) : base(key, parent) {
            this._defaultValue = defaultValue;
            this.Value = defaultValue;
        }

        /// <summary>
        /// Float value of the property.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Resets value to the value provided when the property was registered.
        /// </summary>
        public override void ResetToDefault() {
            this.Value = this._defaultValue;
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

            float f;
            if (float.TryParse(valAttr.Value, out f)) {
                this.Value = f;
            } else {
                return LoadStatus.ParseError;
            }

            return LoadStatus.Ok;
        }

        /// <summary>
        /// Create an XmlElement that represents this float property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("Float");
            element.SetAttribute("key", this.Key);
            element.SetAttribute("value", $"{this.Value}");
            return element;
        }

        private float _defaultValue;
    }
}