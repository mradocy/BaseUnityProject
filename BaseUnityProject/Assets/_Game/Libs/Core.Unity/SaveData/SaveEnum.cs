using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Represents a saved int value.
    /// </summary>
    public sealed class SaveEnum<TEnum> : SaveEnum where TEnum : System.Enum {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValue">Value for the property to start with.</param>
        public SaveEnum(string key, SaveGroup parent, TEnum defaultValue) : base(key, parent, CastTo<int>.From(defaultValue)) {
            this.Value = defaultValue;
        }

        /// <summary>
        /// Enum value of the property.
        /// </summary>
        public TEnum Value {
            get { return _value; }
            set {
                base.IntValue = CastTo<int>.From(value);
                _value = value;
            }
        }

        protected override int IntValue {
            get { return base.IntValue; }
            set {
                base.IntValue = value;
                _value = (TEnum)(object)value;
            }
        }

        private TEnum _value;
    }

    /// <summary>
    /// The base class for <see cref="SaveEnum{TEnum}"/>.
    /// </summary>
    public class SaveEnum : SaveProperty {

        /// <summary>
        /// Constructor, do not call.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        /// <param name="defaultValue">Value for the property to start with.</param>
        public SaveEnum(string key, SaveGroup parent, int defaultValue) : base(key, parent) {
            _defaultValue = defaultValue;
            this.IntValue = defaultValue;
        }

        /// <summary>
        /// Int value of the property.
        /// </summary>
        protected virtual int IntValue { get; set; }

        /// <summary>
        /// Resets value to the value provided when the property was registered.
        /// </summary>
        public sealed override void ResetToDefault() {
            this.IntValue = _defaultValue;
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

            int i;
            if (int.TryParse(valAttr.Value, out i)) {
                this.IntValue = i;
            } else {
                return LoadStatus.ParseError;
            }

            return LoadStatus.Ok;
        }

        /// <summary>
        /// Caches a copy of the value.  This will be used when creating the save xml.
        /// </summary>
        public sealed override void CacheValue() {
            _cachedValue = this.IntValue;
        }

        /// <summary>
        /// Create an XmlElement that represents this int property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public sealed override XmlElement CreateXML(XmlDocument xmlDoc) {
            XmlElement element = xmlDoc.CreateElement("Enum");
            element.SetAttribute("key", this.Key);
            element.SetAttribute("value", _cachedValue.ToString());
            return element;
        }

        private int _defaultValue;
        private int _cachedValue;
    }
}