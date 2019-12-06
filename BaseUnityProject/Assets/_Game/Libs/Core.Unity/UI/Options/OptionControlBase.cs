using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// Base class for option controls.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class OptionControlBase : MonoBehaviour {

        /// <summary>
        /// Gets or sets if this control is selected.
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// Selects this control.
        /// </summary>
        public void Select() {
            if (this.IsSelected)
                return;

            this.IsSelected = true;
            this.OnSelected();
        }

        /// <summary>
        /// Unselects this control.
        /// </summary>
        public void Unselect() {
            if (!this.IsSelected)
                return;

            this.IsSelected = false;
            this.OnUnselected();
        }

        #region Protected Virtual Methods

        /// <summary>
        /// Is called when this option is selected.  Can be used to bolden the display colors.
        /// </summary>
        protected virtual void OnSelected() { }

        /// <summary>
        /// Is called when this option is unselected.  Can be used to wash out the display colors.
        /// </summary>
        protected virtual void OnUnselected() { }

        #endregion

    }
}