using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackInfo {
    // it's important that this can be passed by reference

    public float damage = 0;
    public float heading = 0;
    public float magnitude = 0;
    public HitObject hitObject = null;
    public HurtObject hurtObject = null;

    public string message = "";

    /// <summary>
    /// Returns if a property with the given name exists.
    /// </summary>
    /// <param name="property">Name of the property.</param>
    public bool propertyExists(string property) {
        if (properties == null) return false;
        return properties.ContainsKey(property);
    }
    /// <summary>
    /// Sets a bool property to be the given value.
    /// </summary>
    /// <param name="property">Name of property to set.</param>
    /// <param name="value">Value to set to the property.</param>
    public void setBool(string property, bool value) {
        if (properties == null)
            properties = new Dictionary<string, string>();
        properties[property] = value ? "true" : "false";
    }
    /// <summary>
    /// Gets a bool property.  Returns defaultValue if property doesn't exist.  Any value stored that isn't "", "0", or "false" is considered true.
    /// </summary>
    /// <param name="property">Name of the property.</param>
    /// <param name="defaultValue">Value to return if property couldn't be obtained.</param>
    public bool getBool(string property, bool defaultValue = false) {
        if (!propertyExists(property)) return defaultValue;
        string valStr = properties[property];
        return !(valStr == "" || valStr == "0" || valStr == "false" || valStr == "FALSE");
    }
    /// <summary>
    /// Sets a float property to be the given value.
    /// </summary>
    /// <param name="property">Name of property to set.</param>
    /// <param name="value">Value to set to the property.</param>
    public void setFloat(string property, float value) {
        if (properties == null)
            properties = new Dictionary<string, string>();
        properties[property] = "" + value;
    }
    /// <summary>
    /// Gets a float property.  Returns defaultValue if property doesn't exist or isn't a float.
    /// </summary>
    /// <param name="property">Name of the property.</param>
    /// <param name="defaultValue">Value to return if property couldn't be obtained.</param>
    public float getFloat(string property, float defaultValue = 0) {
        if (!propertyExists(property)) return defaultValue;
        float ret = defaultValue;
        if (float.TryParse(properties[property], out ret))
            return ret;
        return defaultValue;
    }
    /// <summary>
    /// Sets a string property to be the given value.
    /// </summary>
    /// <param name="property">Name of property to set.</param>
    /// <param name="value">Value to set to the property.</param>
    public void setString(string property, string value) {
        if (properties == null)
            properties = new Dictionary<string, string>();
        properties[property] = value;
    }
    /// <summary>
    /// Gets a string property.  Returns defaultValue if property doesn't exist or isn't a float.
    /// </summary>
    /// <param name="property">Name of the property.</param>
    /// <param name="defaultValue">Value to return if property couldn't be obtained.</param>
    public string getString(string property, string defaultValue = "") {
        if (!propertyExists(property)) return defaultValue;
        return properties[property];
    }

    /// <summary>
    /// ToString() override.
    /// </summary>
    public override string ToString() {
        return "AttackInfo damage: " + damage + " message: " + message;
    }

    /// <summary>
    /// Object that receives the damage can optionally leave a result.
    /// </summary>
    public string result = "";

    /// <summary>
    /// Results that can be returned by various parts of the damage pipeline.
    /// </summary>
    public class Result  {
        public const string NONE = "";
        public const string NO_RECEIVES_DAMAGE = "no receives damage";
        public const string MERCY_INVINCIBLE = "mercy invincible";
        public const string PRE_RECEIVE_DAMAGE_SET_TO_0 = "pre receive damage set to 0";
    }
    
    /// <summary>
    /// If attack's heading is pointed to the right.
    /// </summary>
    public bool toRight {
        get {
            float in360 = M.wrap360(heading);
            return in360 < 90 || in360 > 270;
        }
    }

    /// <summary>
    /// Creates a new AttackInfo with all the same properties as this AttackInfo.
    /// </summary>
    /// <param name="copyReferences">If the properties hitObject and hurtObject should also be copied.  If false, these properties in the clone will be null.</param>
    public AttackInfo clone(bool copyReferences = true) {
        AttackInfo ret = new AttackInfo();

        ret.damage = damage;
        ret.heading = heading;
        ret.magnitude = magnitude;
        if (copyReferences) {
            ret.hitObject = hitObject;
            ret.hurtObject = hurtObject;
        }
        ret.message = message;
        ret.result = result;

        if (properties != null) {
            ret.properties = new Dictionary<string, string>();
            foreach (string key in properties.Keys) {
                ret.properties[key] = properties[key];
            }
        }

        return ret;
    }

    private Dictionary<string, string> properties = null;

}


