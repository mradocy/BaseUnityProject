using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Base class for save properties.
    /// </summary>
    public abstract class SaveProperty {

        /// <summary>
        /// Key of this property.  Is unique within a <see cref="SaveGroup"/>.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// The parent of this property.  Will be null only if this property is a <see cref="SaveRoot"/>.
        /// </summary>
        public SaveGroup Parent { get; private set; }

        /// <summary>
        /// Gets the root of this property.
        /// </summary>
        public virtual SaveRoot Root {
            get {
                if (this._root == null) {
                    this._root = this.Parent?.Root;
                }
                return this._root;
            }
        }

        /// <summary>
        /// Resets value to the value provided when the property was registered.
        /// </summary>
        public abstract void ResetToDefault();

        /// <summary>
        /// Create an XmlElement that represents this string property.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public abstract XmlElement CreateXML(XmlDocument xmlDoc);

        /// <summary>
        /// Parses the given XmlNode.
        /// Returns the status of the load.
        /// </summary>
        /// <param name="xmlNode">Node to parse.</param>
        /// <returns>LoadStatus</returns>
        public abstract LoadStatus ParseXML(XmlNode xmlNode);

        /// <summary>
        /// Protected constructor.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        protected SaveProperty(string key, SaveGroup parent) {
            this.Key = key;
            this.Parent = parent;
        }

        /// <summary>
        /// Cached root.
        /// </summary>
        private SaveRoot _root;
    }
}