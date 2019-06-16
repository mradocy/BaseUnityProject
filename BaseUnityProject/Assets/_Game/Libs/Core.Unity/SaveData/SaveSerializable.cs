using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a typed saved serializable value.
    /// </summary>
    public class SaveSerializable<T> : SaveSerializable
        where T : class, ISerializable {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="objectContainer">Object to be serialized/deserialized.  Cannot be null.</param>
        public SaveSerializable(string key, SaveGroup parent, T objectContainer) : base(key, parent, objectContainer) { }

        /// <summary>
        /// Value of the property.
        /// </summary>
        public T Value {
            get { return this._value; }
            set {
                base.UntypedValue = value;
                this._value = value;
            }
        }

        /// <summary>
        /// ISerializable value of the property.
        /// </summary>
        public override ISerializable UntypedValue {
            get { return base.UntypedValue; }
            set {
                base.UntypedValue = value;
                if (value == null) {
                    this._value = null;
                } else {
                    this._value = value as T;
                    if (this._value == null) {
                        throw new System.ArgumentException($"Value must be of type {typeof(T).Name}");
                    }
                }
            }
        }

        private T _value;
    }

    /// <summary>
    /// Represents a saved serializable value.
    /// </summary>
    public class SaveSerializable : SaveProperty {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="objectContainer">Object to be serialized/deserialized.  Cannot be null.</param>
        public SaveSerializable(string key, SaveGroup parent, ISerializable objectContainer) : base(key, parent) {
            if (objectContainer == null) {
                throw new System.ArgumentNullException(nameof(objectContainer));
            }
            this.UntypedValue = objectContainer;
            this._defaultSerValue = objectContainer.Serialize(false);
        }

        /// <summary>
        /// ISerializable value of the property.
        /// </summary>
        public virtual ISerializable UntypedValue {
            get { return this._untypedValue; }
            set { this._untypedValue = value; }
        }

        /// <summary>
        /// Resets value to the value provided when the property was registered.
        /// </summary>
        public override void ResetToDefault() {
            this.UntypedValue.Deserialize(this._defaultSerValue);
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

            string data = xmlNode.InnerText;
            if (data == null) {
                return LoadStatus.ParseError;
            }

            if (this.UntypedValue.Deserialize(data)) {
                return LoadStatus.Ok;
            }
            return LoadStatus.DeserializationError;
        }

        /// <summary>
        /// Create an XmlElement that represents this string property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("Serializable");
            element.SetAttribute("key", this.Key);
            element.InnerText = this.UntypedValue.Serialize(false);
            return element;
        }

        private ISerializable _untypedValue;
        private string _defaultSerValue;
    }
}