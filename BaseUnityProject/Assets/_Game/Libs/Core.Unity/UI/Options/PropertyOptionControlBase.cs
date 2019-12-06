using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// Base class for a control representing a <see cref="IPropertyOption"/>.
    /// </summary>
    public abstract class PropertyOptionControlBase : OptionControlBase {

        /// <summary>
        /// Gets the <see cref="IPropertyOption"/> this control is bound to.
        /// </summary>
        public IPropertyOption PropertyOption { get; private set; }

        /// <summary>
        /// Sets the <see cref="IPropertyOption"/> this control is bound to.
        /// </summary>
        /// <param name="propertyOption"></param>
        public void SetPropertyOption(IPropertyOption propertyOption) {
            this.PropertyOption = propertyOption;
        }

        #region Abstract Methods

        /// <summary>
        /// Increments the property.
        /// </summary>
        /// <remarks>Different from incrementing the property model directly, e.g. may play an animation first.</remarks>
        public abstract void Increment();

        /// <summary>
        /// Decrements the property.
        /// </summary>
        /// <remarks>Different from decrementing the property model directly, e.g. may play an animation first.</remarks>
        public abstract void Decrement();

        #endregion
    }
}