using System;
using UnityEngine;

// from https://github.com/johnearnshaw/unity-inspector-help/blob/master/HelpAttribute.cs

namespace Core.Unity.Attributes {

    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class HelpAttribute : PropertyAttribute {

        /// <summary>
        /// Adds a HelpBox to the Unity property inspector above this field.
        /// </summary>
        /// <param name="text">The help text to be displayed in the HelpBox.</param>
        /// <param name="type">The icon to be displayed in the HelpBox.</param>
        public HelpAttribute(string text, HelpMessageType type = HelpMessageType.None) {
            this.Text = text;
            this.Type = type;
        }

        public readonly string Text;
        public readonly HelpMessageType Type;

        /// <summary>
        /// Copy of UnityEditor.MessageType without having to reference UnityEditor in the attribute class
        /// </summary>
        public enum HelpMessageType {
            None,
            Info,
            Warning,
            Error
        }
    }
}