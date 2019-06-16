using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Core.Unity.SaveData {
    /// <summary>
    /// Describes the status when saving data.
    /// </summary>
    public enum SaveStatus {
        [Description("File saved with no problems.")]
        Ok = 0,

        [Description("Data was currently being saved.")]
        AlreadySaving = 1,

        [Description("Error occurred when saving data to a string.")]
        StringError = 2,

        [Description("Error occurred when saving the data to a file.")]
        IOError = 3,
    }

    public static class SaveStatusExtensions {

        /// <summary>
        /// Gets the description of the <see cref="SaveStatus"/>, as given by its <see cref="DescriptionAttribute"/> attribute.
        /// </summary>
        /// <param name="saveStatus">this saveStatus</param>
        /// <returns>Description</returns>
        public static string GetDescription(this SaveStatus saveStatus) {
            System.Type type = saveStatus.GetType();
            string name = System.Enum.GetName(type, saveStatus);
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