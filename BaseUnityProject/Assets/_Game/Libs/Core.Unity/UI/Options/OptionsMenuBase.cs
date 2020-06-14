using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// Abstract base class for an options menu.
    /// Does not reference controls.
    /// </summary>
    public abstract class OptionsMenuBase : MonoBehaviour {

        #region Public Properties

        /// <summary>
        /// Gets the index of the option that is currently selected.  -1 means no option is selected.
        /// </summary>
        public int SelectedIndex { get; private set; } = -1;

        /// <summary>
        /// Gets the timestamp (in <see cref="Time.unscaledTime"/>) when the selected option changed.
        /// </summary>
        public float SelectedOptionChangeTime { get; private set; } = 0;

        /// <summary>
        /// Gets the number of options.
        /// </summary>
        public int OptionsCount => _options.Count;

        #endregion

        #region Public Methods

        /// <summary>
        /// Selects the option control at the given index.  Does nothing if the index is the same as the current index.
        /// </summary>
        /// <param name="optionIndex">Index of the option to select.  Input -1 to select no option.</param>
        /// <param name="animate">If the option selection should be animated.  False may be useful for setting the first option selected.</param>
        public void SelectOption(int optionIndex, bool animate) {
            if (optionIndex < -1 || optionIndex >= this.OptionsCount) {
                Debug.LogError($"Option index {optionIndex} is invalid.  There are only {this.OptionsCount} options.");
                return;
            }

            if (optionIndex == this.SelectedIndex)
                return;

            int prevIndex = this.SelectedIndex;
            this.SelectedOptionChangeTime = Time.unscaledTime;
            this.SelectedIndex = optionIndex;

            this.OnSelectionChanged(prevIndex, this.SelectedIndex, animate);
        }

        /// <summary>
        /// Gets the <see cref="IOption"/> at the given index.
        /// </summary>
        /// <param name="optionIndex">Option index</param>
        /// <returns>Option</returns>
        public IOption GetOption(int optionIndex) {
            if (optionIndex < 0 || optionIndex >= this.OptionsCount)
                return null;

            return _options[optionIndex];
        }

        /// <summary>
        /// Gets the option at the given index cast as a <see cref="IButtonOption"/>.
        /// </summary>
        /// <param name="optionIndex">Option index</param>
        /// <returns>Button option</returns>
        public IButtonOption GetButtonOption(int optionIndex) {
            return this.GetOption(optionIndex) as IButtonOption;
        }

        /// <summary>
        /// Gets the option at the given index cast as a <see cref="IPropertyOption"/>.
        /// </summary>
        /// <param name="optionIndex">Option index</param>
        /// <returns>Property option</returns>
        public IPropertyOption GetPropertyOption(int optionIndex) {
            return this.GetOption(optionIndex) as IPropertyOption;
        }

        #endregion

        #region Protected Abstract Properties

        /// <summary>
        /// Gets a reference to a ui input handler.
        /// </summary>
        protected abstract IUIInput UIInput { get; }

        /// <summary>
        /// Gets the duration to hold left or right before increments or decrements will happen while the button is being held, rather than pressed.
        /// </summary>
        /// <remarks>0.5f is a good value to use.</remarks>
        protected abstract float HoldChangeWaitDuration { get; }

        /// <summary>
        /// Gets the amount of time in-between changes to a property value while hold changing.
        /// </summary>
        /// <remarks>0.1f is a good value to use.</remarks>
        protected abstract float HoldChangePeriod { get; }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="prevIndex">The previous <see cref="SelectedIndex"/>.</param>
        /// <param name="currentIndex">The current <see cref="SelectedIndex"/>.  -1 for no option selected.</param>
        /// <param name="animate">If the selection change should be animated.  False may be useful for setting the first option selected.</param>
        protected virtual void OnSelectionChanged(int prevIndex, int currentIndex, bool animate) { }

        /// <summary>
        /// Called when the value of a <see cref="IPropertyOption"/> is decremented or incremented.
        /// </summary>
        /// <param name="optionIndex">The index of the property option changed.</param>
        /// <param name="incremented">True if the value was incremented, false if the value was decremented.</param>
        protected virtual void OnPropertyValueChanged(int optionIndex, bool incremented) { }

        /// <summary>
        /// Called when the cancel button is pressed.
        /// </summary>
        protected virtual void OnCancelPressed() { }

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// Called by Unity when this object is destroyed.
        /// </summary>
        protected virtual void OnDerivedDestroy() { }

        #endregion

        #region Protected Helper Methods

        /// <summary>
        /// Sets the options of this menu.
        /// A shallow copy of the provided list is made.
        /// </summary>
        /// <param name="options">Options</param>
        protected virtual void SetOptions(IList<IOption> options) {
            _options.Clear();
            foreach (IOption option in options) {
                _options.Add(option);
            }

            // need to re-select option
            this.SelectedIndex = -1;
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected void Awake() {
            _parentModalWindow = this.GetComponentInParent<ModalWindow>();

            this.OnAwake();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected void Update() {
            // disable update if parent modal window is disabled
            if (_parentModalWindow != null && !_parentModalWindow.enabled)
                return;

            float timestamp = Time.unscaledTime;
            if (this.UIInput.IsLeftPressed) {
                _leftPressedTime = timestamp;
            }
            if (this.UIInput.IsRightPressed) {
                _rightPressedTime = timestamp;
            }

            if (_options.Count > 0) {
                if (this.UIInput.IsSubmitPressed) {
                    // execute the selected button option
                    IButtonOption buttonOption = this.GetButtonOption(this.SelectedIndex);
                    if (buttonOption != null && buttonOption.CanExecute) {
                        buttonOption.Execute();
                    }
                } else if (this.UIInput.IsCancelPressed) {
                    // call derived cancel method
                    this.OnCancelPressed();
                } else if (this.UIInput.IsDownPressed) {
                    // increment selected index
                    if (this.SelectedIndex < 0 || this.SelectedIndex >= this.OptionsCount) {
                        // failsafe
                        this.SelectOption(0, false);
                    } else if (this.SelectedIndex >= this.OptionsCount - 1) {
                        // wrap around
                        this.SelectOption(0, true);
                    } else {
                        this.SelectOption(this.SelectedIndex + 1, true);
                    }
                } else if (this.UIInput.IsUpPressed) {
                    // decrement selected index
                    if (this.SelectedIndex < 0 || this.SelectedIndex >= this.OptionsCount) {
                        // failsafe
                        this.SelectOption(0, false);
                    } else if (this.SelectedIndex == 0) {
                        this.SelectOption(this.OptionsCount - 1, true);
                    } else {
                        this.SelectOption(this.SelectedIndex - 1, true);
                    }
                } else if (this.UIInput.IsLeftHeld) {
                    // decrement the selected property option
                    IPropertyOption propertyOption = this.GetPropertyOption(this.SelectedIndex);
                    if (propertyOption != null && propertyOption.CanDecrement) {
                        bool decrement = false;
                        if (this.UIInput.IsLeftPressed) {
                            // pressing the left button to decrement
                            decrement = true;
                        } else if (propertyOption.CanHoldChange) {
                            // holding the left button to decrement
                            if (timestamp > _leftPressedTime + this.HoldChangeWaitDuration &&
                                timestamp > _decrementTime + this.HoldChangePeriod) {
                                decrement = true;
                            }
                        }

                        if (decrement) {
                            propertyOption.Decrement();
                            _decrementTime = timestamp;
                            this.OnPropertyValueChanged(this.SelectedIndex, false);
                        }
                    }
                } else if (this.UIInput.IsRightHeld) {
                    // increment the selected property option
                    IPropertyOption propertyOption = this.GetPropertyOption(this.SelectedIndex);
                    if (propertyOption != null && propertyOption.CanIncrement) {
                        bool increment = false;
                        if (this.UIInput.IsRightPressed) {
                            // pressing the right button to increment
                            increment = true;
                        } else if (propertyOption.CanHoldChange) {
                            // holding the right button to increment
                            if (timestamp > _rightPressedTime + this.HoldChangeWaitDuration &&
                                timestamp > _incrementTime + this.HoldChangePeriod) {
                                increment = true;
                            }
                        }

                        if (increment) {
                            propertyOption.Increment();
                            _incrementTime = timestamp;
                            this.OnPropertyValueChanged(this.SelectedIndex, true);
                        }
                    }
                }
            }

            this.OnUpdate();
        }

        /// <summary>
        /// Called by Unity when this object is destroyed.
        /// </summary>
        protected void OnDestroy() {
            this.OnDerivedDestroy();
        }

        #endregion

        #region Private

        private List<IOption> _options = new List<IOption>();

        private float _leftPressedTime = -9999;
        private float _rightPressedTime = -9999;
        private float _decrementTime = -9999;
        private float _incrementTime = -9999;

        /// <summary>
        /// The modal window attached to this object or parent.  Can be null.
        /// </summary>
        private ModalWindow _parentModalWindow = null;

        #endregion
    }
}