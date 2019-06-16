using UnityEngine;

namespace Core.Unity.Attributes {
    /// <summary>
    /// Extends the label width.  May not work for some property types.
    /// </summary>
    /// <param name="labelWidth">Width of the label in pixels.</param>
    public class LongLabelAttribute : PropertyAttribute {

        public LongLabelAttribute(float labelWidth = 200) : base() {

            this.LabelWidth = labelWidth;
        }

        public readonly float LabelWidth;

    }
}