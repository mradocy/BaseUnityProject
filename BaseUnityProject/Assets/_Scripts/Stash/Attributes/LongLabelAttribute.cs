using UnityEngine;

public class LongLabelAttribute : PropertyAttribute {
    
    /// <summary>
    /// Extends the label width.  May not work for some property types.
    /// </summary>
    /// <param name="labelWidth">Width of the label in pixels.</param>
    public LongLabelAttribute(float labelWidth = 200) : base() {
        
        this.labelWidth = labelWidth;
    }

    public readonly float labelWidth;
    
}