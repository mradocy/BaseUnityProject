using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Core.Unity.SaveData {
    /// <summary>
    /// Describes the status of a save data load.
    /// </summary>
    public enum LoadStatus {
        [Description("File loaded with no problems")]
        Ok = 0,

        //[Description("Cannot load because SaveManager is already loading a file.")]
        //AlreadyLoading = 1,

        [Description("The file attempting to be loaded could not be found.")]
        FileNotFound = 2,

        [Description("The file exists, but could not be read.")]
        FileCouldNotBeRead = 3,

        [Description("There was an issue parsing the data.")]
        ParseError = 4,

        [Description("There was an issue deserializing a serializable part of the data.")]
        DeserializationError = 5,

        [Description("The save data is incompatible with what the game expects.")]
        CompatibilityError = 6,
    }

    public static class LoadStatusExtensions {

        /// <summary>
        /// Gets the description of the <see cref="LoadStatus"/>, as given by its <see cref="DescriptionAttribute"/> attribute.
        /// </summary>
        /// <param name="loadStatus">this loadStatus</param>
        /// <returns>Description</returns>
        public static string GetDescription(this LoadStatus loadStatus) {
            System.Type type = loadStatus.GetType();
            string name = System.Enum.GetName(type, loadStatus);
            if (name != null) {
                System.Reflection.FieldInfo field = type.GetField(name);
                if (field != null) {
                    DescriptionAttribute attr = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null) {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}