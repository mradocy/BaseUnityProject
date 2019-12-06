using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// Model of a property option (a value that can be increased or decreased) that would appear in an options menu.
    /// </summary>
    public interface IPropertyOption : IOption {

        /// <summary>
        /// Gets the value to be displayed in the option control.
        /// </summary>
        string DisplayValue { get; }

        /// <summary>
        /// Gets if this property should allowed to be changed by holding left/right, instead of having to press the buttton.
        /// </summary>
        bool CanHoldChange { get; }

        /// <summary>
        /// Gets if this property can currently be incremented.
        /// </summary>
        bool CanIncrement { get; }

        /// <summary>
        /// Increments the value.
        /// </summary>
        void Increment();

        /// <summary>
        /// Gets if this property can currently be decremented.
        /// </summary>
        bool CanDecrement { get; }

        /// <summary>
        /// Decrements the value.
        /// </summary>
        void Decrement();
    }
}