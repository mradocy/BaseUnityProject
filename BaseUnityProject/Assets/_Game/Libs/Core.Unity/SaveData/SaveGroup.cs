using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Core.Unity.SaveData {

    /// <summary>
    /// Save property representing a group of properties.
    /// </summary>
    public class SaveGroup : SaveProperty {

        #region Registering Properties

        /// <summary>
        /// Registers a <see cref="SaveGroup"/>.
        /// </summary>
        /// <param name="key">Key of the group.</param>
        /// <returns>Created save group.</returns>
        public SaveGroup RegisterGroup(string key) {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveGroup group = new SaveGroup(key, this);
            _properties[key] = group;
            _propSearch[key] = false;
            return group;
        }

        /// <summary>
        /// Registers a <see cref="SaveString"/>.
        /// </summary>
        /// <param name="key">Key of the string property.</param>
        /// <param name="defaultValue">Default value to give the property if data isn't found for it.</param>
        /// <returns>Created string property.</returns>
        public SaveString RegisterString(string key, string defaultValue = null) {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveString str = new SaveString(key, this, defaultValue);
            _properties[key] = str;
            _propSearch[key] = false;
            return str;
        }

        /// <summary>
        /// Registers a <see cref="SaveFloat"/>.
        /// </summary>
        /// <param name="key">Key of the float property.</param>
        /// <param name="defaultValue">Default value to give the property if data isn't found for it.</param>
        /// <returns>Created float property.</returns>
        public SaveFloat RegisterFloat(string key, float defaultValue = 0) {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveFloat f = new SaveFloat(key, this, defaultValue);
            _properties[key] = f;
            _propSearch[key] = false;
            return f;
        }

        /// <summary>
        /// Registers a <see cref="SaveInt"/>.
        /// </summary>
        /// <param name="key">Key of the int property.</param>
        /// <param name="defaultValue">Default value to give the property if data isn't found for it.</param>
        /// <returns>Created int property.</returns>
        public SaveInt RegisterInt(string key, int defaultValue = 0) {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveInt i = new SaveInt(key, this, defaultValue);
            _properties[key] = i;
            _propSearch[key] = false;
            return i;
        }

        /// <summary>
        /// Registers a <see cref="SaveEnum{TEnum}"/>.
        /// </summary>
        /// <param name="key">Key of the enum property.</param>
        /// <param name="defaultValue">Default value to give the property if data isn't found for it.</param>
        /// <returns>Created enum property.</returns>
        public SaveEnum<TEnum> RegisterEnum<TEnum>(string key, TEnum defaultValue = default) where TEnum : System.Enum {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveEnum<TEnum> e = new SaveEnum<TEnum>(key, this, defaultValue);
            _properties[key] = e;
            _propSearch[key] = false;
            return e;
        }

        /// <summary>
        /// Registers a <see cref="SaveBool"/>.
        /// </summary>
        /// <param name="key">Key of the bool property.</param>
        /// <param name="defaultValue">Default value to give the property if data isn't found for it.</param>
        /// <returns>Created bool property.</returns>
        public SaveBool RegisterBool(string key, bool defaultValue = false) {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveBool b = new SaveBool(key, this, defaultValue);
            _properties[key] = b;
            _propSearch[key] = false;
            return b;
        }

        /// <summary>
        /// Registers a <see cref="SaveIntList"/>.
        /// </summary>
        /// <param name="key">Key of the int list property.</param>
        /// <param name="defaultValues">Default values to give the property if data isn't found for it.  Passing in null will make the default values an empty list.</param>
        /// <returns>Created int list property.</returns>
        public SaveIntList RegisterIntList(string key, IEnumerable<int> defaultValues = null) {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveIntList intList = new SaveIntList(key, this, defaultValues);
            _properties[key] = intList;
            _propSearch[key] = false;
            return intList;
        }

        /// <summary>
        /// Registers a <see cref="SaveIntSet"/>.
        /// </summary>
        /// <param name="key">Key of the int set property.</param>
        /// <param name="defaultValues">Default values to give the property if data isn't found for it.  Passing in null will make the default values an empty set.</param>
        /// <returns>Created int set property.</returns>
        public SaveIntSet RegisterIntSet(string key, IEnumerable<int> defaultValues = null) {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveIntSet intSet = new SaveIntSet(key, this, defaultValues);
            _properties[key] = intSet;
            _propSearch[key] = false;
            return intSet;
        }

        /// <summary>
        /// Registers a <see cref="SaveIntBitArray"/>.
        /// </summary>
        /// <param name="key">Key of the int bit array property.</param>
        /// <param name="max">Max value that can be stored in this array.  Range is [0, max].  This value cannot be higher than <see cref="SaveIntBitArray.GlobalMax"/>.</param>
        /// <param name="defaultValues">Default values to give the property if data isn't found for it.  Passing in null will make the default values empty.</param>
        /// <returns>Created int bit array property.</returns>
        public SaveIntBitArray RegisterIntBitArray(string key, int max, IEnumerable<int> defaultValues = null) {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveIntBitArray intBitArray = new SaveIntBitArray(key, max, this, defaultValues);
            _properties[key] = intBitArray;
            _propSearch[key] = false;
            return intBitArray;
        }

        /// <summary>
        /// Registers a <see cref="SaveEnumList{TEnum}"/>.
        /// </summary>
        /// <param name="key">Key of the enum list property.</param>
        /// <param name="defaultValues">Default values to give the property if data isn't found for it.  Passing in null will make the default values an empty list.</param>
        /// <returns>Created enum list property.</returns>
        public SaveEnumList<TEnum> RegisterEnumList<TEnum>(string key, IEnumerable<TEnum> defaultValues = null)
            where TEnum : System.Enum {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveEnumList<TEnum> el = new SaveEnumList<TEnum>(key, this, defaultValues);
            _properties[key] = el;
            _propSearch[key] = false;
            return el;
        }

        /// <summary>
        /// Registers a <see cref="SaveEnumSet{TEnum}"/>.
        /// </summary>
        /// <param name="key">Key of the enum set property.</param>
        /// <param name="defaultValues">Default values to give the property if data isn't found for it.  Passing in null will make the default values an empty set.</param>
        /// <returns>Created enum set property.</returns>
        public SaveEnumSet<TEnum> RegisterEnumSet<TEnum>(string key, IEnumerable<TEnum> defaultValues = null)
            where TEnum : System.Enum {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveEnumSet<TEnum> es = new SaveEnumSet<TEnum>(key, this, defaultValues);
            _properties[key] = es;
            _propSearch[key] = false;
            return es;
        }

        /// <summary>
        /// Registers a <see cref="SaveEnumBitArray{TEnum}"/>.
        /// </summary>
        /// <param name="key">Key of the int bit array property.</param>
        /// <param name="max">Max enum value that can be stored in this array.  Range is [0, max].  This value cannot be higher than <see cref="SaveIntBitArray.GlobalMax"/>.</param>
        /// <param name="defaultValues">Default values to give the property if data isn't found for it.  Passing in null will make the default values empty.</param>
        /// <returns>Created enum bit array property.</returns>
        public SaveEnumBitArray<TEnum> RegisterEnumBitArray<TEnum>(string key, TEnum max, IEnumerable<TEnum> defaultValues = null)
            where TEnum : System.Enum {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveEnumBitArray<TEnum> enumBitArray = new SaveEnumBitArray<TEnum>(key, max, this, defaultValues);
            _properties[key] = enumBitArray;
            _propSearch[key] = false;
            return enumBitArray;
        }

        /// <summary>
        /// Registers a <see cref="SaveSerializable"/>.
        /// </summary>
        /// <typeparam name="T">Type of the value of the object container</typeparam>
        /// <param name="key">Key of the serializable property.</param>
        /// <param name="objectContainer">Object to be serialized/deserialized (e.g. new T()).  Cannot be null.</param>
        /// <returns>Created serializable property.</returns>
        public SaveSerializable<T> RegisterSerializable<T>(string key, T objectContainer)
            where T : class, ISerializable {
            if (this.RegisterErrorCheck(key))
                return null;

            SaveSerializable<T> serializable = new SaveSerializable<T>(key, this, objectContainer);
            _properties[key] = serializable;
            _propSearch[key] = false;
            return serializable;
        }

        #endregion

        #region Getting Properties

        /// <summary>
        /// Gets <see cref="SaveGroup"/> by key.  Returns null if no save group with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Save group.</returns>
        public SaveGroup GetGroup(string key) {
            return this.GetProperty<SaveGroup>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveString"/> by key.  Returns null if no string property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>String property.</returns>
        public SaveString GetString(string key) {
            return this.GetProperty<SaveString>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveFloat"/> by key.  Returns null if no float property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Float property.</returns>
        public SaveFloat GetFloat(string key) {
            return this.GetProperty<SaveFloat>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveInt"/> by key.  Returns null if no int property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Int property.</returns>
        public SaveInt GetInt(string key) {
            return this.GetProperty<SaveInt>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveEnum{TEnum}"/> by key.  Returns null if no enum property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Enum property.</returns>
        public SaveEnum<TEnum> GetEnum<TEnum>(string key) where TEnum : System.Enum {
            return this.GetProperty<SaveEnum<TEnum>>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveBool"/> by key.  Returns null if no bool property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Bool property.</returns>
        public SaveBool GetBool(string key) {
            return this.GetProperty<SaveBool>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveIntList"/> by key.  Returns null if no int list property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Int list property.</returns>
        public SaveIntList GetIntList(string key) {
            return this.GetProperty<SaveIntList>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveIntSet"/> by key.  Returns null if no int set property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Int set property.</returns>
        public SaveIntSet GetIntSet(string key) {
            return this.GetProperty<SaveIntSet>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveIntBitArray"/> by key.  Returns null if no int bit array property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Int bit array property.</returns>
        public SaveIntBitArray GetIntBitArray(string key) {
            return this.GetProperty<SaveIntBitArray>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveEnumList{TEnum}"/> by key.  Returns null if no enum list property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Enum list property.</returns>
        public SaveEnumList<TEnum> GetEnumList<TEnum>(string key)
            where TEnum : System.Enum {
            return this.GetProperty<SaveEnumList<TEnum>>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveEnumSet{TEnum}"/> by key.  Returns null if no enum set property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Enum set property.</returns>
        public SaveEnumSet<TEnum> GetEnumSet<TEnum>(string key)
            where TEnum : System.Enum {
            return this.GetProperty<SaveEnumSet<TEnum>>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveEnumBitArray{TEnum}"/> by key.  Returns null if no enum bit array property with the given key has been registered.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Enum bit array property.</returns>
        public SaveEnumBitArray<TEnum> GetEnumBitArray<TEnum>(string key)
            where TEnum : System.Enum {
            return this.GetProperty<SaveEnumBitArray<TEnum>>(key);
        }

        /// <summary>
        /// Gets <see cref="SaveSerializable"/> by key.  Returns null if no serializable property with the given key has been registered.
        /// </summary>
        /// <typeparam name="T">Type of the value of the serializable.</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Serializable property.</returns>
        public SaveSerializable<T> GetSerializable<T>(string key)
            where T : class, ISerializable {
            return this.GetProperty<SaveSerializable<T>>(key);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the values of all this group's properties.
        /// </summary>
        public override void ResetToDefault() {
            foreach (SaveProperty prop in _properties.Values) {
                prop.ResetToDefault();
            }
        }

        /// <summary>
        /// Create an XmlElement that represents this save group.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <returns>XmlElement</returns>
        public override XmlElement CreateXML(XmlDocument xmlDoc) {
            return this.CreateXML(xmlDoc, false);
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

            // reset property search
            foreach (string key in _propSearch.Keys.ToList()) {
                _propSearch[key] = false;
            }

            // parse nodes
            LoadStatus status = LoadStatus.Ok;
            foreach (XmlNode childNode in xmlNode.ChildNodes) {
                LoadStatus nodeStatus = LoadStatus.Ok;

                // get key
                string key = childNode.Attributes?["key"]?.Value;
                if (key == null) {
                    return LoadStatus.ParseError;
                }

                // get property from name and key
                string name = childNode.Name;
                bool nameFound = true;
                SaveProperty property = null;
                if (name == "Group") {
                    property = this.GetGroup(key);
                } else if (name == "String") {
                    property = this.GetString(key);
                } else if (name == "Serializable") {
                    property = this.GetProperty<SaveSerializable>(key);
                } else if (name == "Float") {
                    property = this.GetFloat(key);
                } else if (name == "Int") {
                    property = this.GetInt(key);
                } else if (name == "Enum") {
                    property = this.GetProperty<SaveEnum>(key);
                } else if (name == "Bool") {
                    property = this.GetBool(key);
                } else if (name == "IntList" || name == "EnumList") {
                    property = this.GetProperty<SaveIntList>(key); // SaveEnumList<T> extends SaveIntList
                } else if (name == "IntSet" || name == "EnumSet") {
                    property = this.GetProperty<SaveIntSet>(key); // SaveEnumSet<T> extends SaveIntSet
                } else if (name == "IntBitArray" || name == "EnumBitArray") {
                    property = this.GetProperty<SaveIntBitArray>(key); // SaveEnumBitArray<T> extends SaveIntBitArray
                } else {
                    nameFound = false;
                    Debug.Log($"TODO: parse node with name \"{name}\"");
                }

                // have property parse node
                if (nameFound) {
                    if (property == null) {
                        Debug.LogWarning($"{name} property with key \"{key}\" (parent: \"{this.Key}\") is not registered; data lost.");
                    } else {
                        nodeStatus = property.ParseXML(childNode);
                        _propSearch[key] = true;
                    }
                }
                if (nodeStatus != LoadStatus.Ok) {
                    status = LoadStatus.ParseError;
                }
            }

            // check property search
            foreach (string key in _propSearch.Keys) {
                if (!_propSearch[key]) {
                    Debug.LogWarning($"Parsed data did not have an entry for registered property \"{key}\" (parent: \"{this.Key}\")");
                }
            }

            return status;
        }

        #endregion

        #region Protected and Private

        /// <summary>
        /// Protected constructor.  All save properties, except for the root, have to be registered.
        /// </summary>
        /// <param name="key">Key to identify the group.</param>
        /// <param name="parent">Parent <see cref="SaveGroup"/></param>
        protected SaveGroup(string key, SaveGroup parent) : base(key, parent) { }

        /// <summary>
        /// Create an XmlElement that represents this save group.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument to use to create the element.</param>
        /// <param name="isRoot">If the element being created is the root.</param>
        /// <returns>XmlElement</returns>
        protected XmlElement CreateXML(XmlDocument xmlDoc, bool isRoot) {
            // create element for this group
            XmlElement element;
            if (isRoot) {
                element = xmlDoc.CreateElement("Root");
            } else {
                element = xmlDoc.CreateElement("Group");
                element.SetAttribute("key", this.Key);
            }

            // append child elements
            XmlElement childElement;
            foreach (SaveProperty prop in _properties.Values) {
                childElement = prop.CreateXML(xmlDoc);
                element.AppendChild(childElement);
            }

            return element;
        }

        /// <summary>
        /// Logs an error if there would be a problem registering a property with the given key.
        /// Returns if there was an error.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>If error occurred.</returns>
        protected bool RegisterErrorCheck(string key) {
            if (this.Root.IsParsed) {
                Debug.LogError("Save data has already been parsed, no more properties can be registered.");
                return true;
            }
            if (this.GetProperty<SaveProperty>(key) != null) {
                Debug.LogError($"Already registered property with key \"{key}\" (parent: \"{this.Key}\")");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a property by key, or null if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type to cast the property to.</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Property</returns>
        protected T GetProperty<T>(string key) where T : SaveProperty {
            SaveProperty prop;
            if (_properties.TryGetValue(key, out prop)) {
                return prop as T;
            }
            return null;
        }

        /// <summary>
        /// Dictionary of all the save properties in this group.
        /// </summary>
        private Dictionary<string, SaveProperty> _properties = new Dictionary<string, SaveProperty>();
        /// <summary>
        /// Mapping of save property keys to bools.  Used for parsing.
        /// </summary>
        private Dictionary<string, bool> _propSearch = new Dictionary<string, bool>();

        #endregion
    }
}