using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// Base class for an options menu
    /// </summary>
    public abstract class OptionsMenuBase : MonoBehaviour {

        #region Inspector Fields

        [Header("OptionsMenuBase Children")]

        [SerializeField]
        [Tooltip("Transform to put the option controls in.")]
        private VerticalLayoutGroup _controlGroup = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the index of the control that is currently selected.  -1 means no control is selected.
        /// </summary>
        public int SelectedIndex { get; private set; } = -1;

        /// <summary>
        /// Gets the control that is currently selected, or null if no control is selected.
        /// </summary>
        public OptionControlBase SelectedControl {
            get {
                return this.GetOptionControl(this.SelectedIndex);
            }
        }

        /// <summary>
        /// Gets the number of options.
        /// </summary>
        public int OptionsCount {
            get { return _optionControls.Count; }
        }

        /// <summary>
        /// Gets the timestamp (in <see cref="Time.unscaledTime"/>) when the selected option changed.
        /// </summary>
        public float SelectedOptionTime { get; private set; } = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the option control at the given index.  Returns null if the index is invalid.
        /// </summary>
        /// <param name="optionIndex">Option index.</param>
        /// <returns>Option control.</returns>
        public OptionControlBase GetOptionControl(int optionIndex) {
            if (optionIndex < 0 || optionIndex >= this.OptionsCount)
                return null;

            return _optionControls[optionIndex];
        }

        /// <summary>
        /// Selects the option control at the given index.  Does nothing if the index is the same as the current index.
        /// </summary>
        /// <param name="optionIndex">Index of the option to select.  Input -1 to select no option.</param>
        public void SelectOption(int optionIndex) {
            if (optionIndex < 0 || optionIndex >= this.OptionsCount) {
                optionIndex = -1;
            }

            if (optionIndex == this.SelectedIndex)
                return;

            int prevIndex = this.SelectedIndex;
            OptionControlBase prevControl = this.SelectedControl;
            if (prevControl != null) {
                prevControl.Unselect();
            }
            this.SelectedOptionTime = Time.unscaledTime;
            this.SelectedIndex = optionIndex;
            OptionControlBase selectedControl = this.SelectedControl;
            if (selectedControl != null) {
                selectedControl.Select();
            }
            this.OnSelectionChanged(prevIndex, this.SelectedIndex);
        }

        #endregion

        #region Protected Properties to be Overridden

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

        /// <summary>
        /// When true, if there is a <see cref="ModalWindow"/> attached to this gameObject or a parent and it's disabled,
        /// then the Update() method for this menu (i.e. the user input) will also be disabled.
        /// </summary>
        protected virtual bool DisableUpdateIfParentModalWindowIsDisabled { get; } = true;

        #endregion

        #region Protected Methods to be Overridden

        /// <summary>
        /// Initializes the list of option models for controls to be made from.
        /// </summary>
        /// <returns>Option models.</returns>
        protected abstract IList<IOption> InitOptions();

        /// <summary>
        /// Creates a new <see cref="OptionControlBase"/> game object with the given option.  Used to specify custom option control implementations for each option.
        /// </summary>
        /// <param name="option">Option to create a control from.</param>
        /// <returns>Option control.</returns>
        protected abstract OptionControlBase CreateControl(IOption option);

        /// <summary>
        /// Destroys the given control.  By default this uses Unity's Destroy() method, can be overridden to recycle instead.
        /// </summary>
        /// <param name="control">Control to destroy.</param>
        protected virtual void DestoryControl(OptionControlBase control) {
            Destroy(control.gameObject);
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="prevIndex">The previous <see cref="SelectedIndex"/>.</param>
        /// <param name="currentIndex">The current <see cref="SelectedIndex"/>.</param>
        protected virtual void OnSelectionChanged(int prevIndex, int currentIndex) { }

        /// <summary>
        /// Called when the value of a property option is decremented or incremented.
        /// </summary>
        /// <param name="propertyOptionControl">The control of the option changed.</param>
        /// <param name="incremented">True if the value was incremented, false if the value was decremented.</param>
        protected virtual void OnPropertyValueChanged(PropertyOptionControlBase propertyOptionControl, bool incremented) { }

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

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates (or recreates) the option controls for this menu.
        /// </summary>
        protected void CreateOptionControls() {
            // create option models
            this.ClearOptions();
            _optionModels.AddRange(this.InitOptions());
            
            // create controls
            foreach (IOption optionModel in _optionModels) {
                OptionControlBase optionControl = this.CreateControl(optionModel);
                if (optionControl == null) {
                    Debug.LogError("Error when creating option control");
                    continue;
                }
                RectTransform rectTransform = optionControl.EnsureComponent<RectTransform>();
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.SetParent(_controlGroup.transform);
                _optionControls.Add(optionControl);
            }
        }

        /// <summary>
        /// Destroys all options and option controls.
        /// </summary>
        protected void ClearOptions() {
            foreach (OptionControlBase optionControl in _optionControls) {
                this.DestoryControl(optionControl);
            }
            _optionControls.Clear();
            _optionModels.Clear();
        }

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        protected void Awake() {
            _parentModalWindow = this.GetComponentInParent<ModalWindow>();

            this.OnAwake();

            this.CreateOptionControls();
            this.SelectOption(0);
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected void Update() {
            if (this.DisableUpdateIfParentModalWindowIsDisabled) {
                if (_parentModalWindow != null && !_parentModalWindow.enabled)
                    return;
            }

            float timestamp = Time.unscaledTime;
            if (this.UIInput.IsLeftPressed) {
                _leftPressedTime = timestamp;
            }
            if (this.UIInput.IsRightPressed) {
                _rightPressedTime = timestamp;
            }

            OptionControlBase selectedOptionControl = this.SelectedControl;
            if (this.OptionsCount > 0) {
                if (this.UIInput.IsSubmitPressed) {
                    // execute the selected option
                    ButtonOptionControlBase buttonOptionControl = selectedOptionControl as ButtonOptionControlBase;
                    if (buttonOptionControl != null && buttonOptionControl.ButtonOption.CanExecute) {
                        buttonOptionControl.Execute();
                    }
                } else if (this.UIInput.IsCancelPressed) {
                    // call derived cancel method
                    this.OnCancelPressed();
                } else if (this.UIInput.IsDownPressed) {
                    // increment selected index
                    if (this.SelectedIndex < 0 || this.SelectedIndex >= this.OptionsCount - 1) {
                        this.SelectOption(0);
                    } else {
                        this.SelectOption(this.SelectedIndex + 1);
                    }
                } else if (this.UIInput.IsUpPressed) {
                    // decrement selected index
                    if (this.SelectedIndex < 0 || this.SelectedIndex >= this.OptionsCount) {
                        this.SelectOption(0);
                    } else if (this.SelectedIndex == 0) {
                        this.SelectOption(this.OptionsCount - 1);
                    } else {
                        this.SelectOption(this.SelectedIndex - 1);
                    }
                } else if (this.UIInput.IsLeftHeld && selectedOptionControl != null) {
                    PropertyOptionControlBase propertyOptionControl = selectedOptionControl as PropertyOptionControlBase;
                    if (propertyOptionControl != null && propertyOptionControl.PropertyOption.CanDecrement) {
                        // decrement property option if conditions are met
                        bool decrement = false;
                        if (this.UIInput.IsLeftPressed) {
                            // pressing the left button to decrement
                            decrement = true;
                        } else if (propertyOptionControl.PropertyOption.CanHoldChange) {
                            // holding the left button to decrement
                            if (timestamp > _leftPressedTime + this.HoldChangeWaitDuration &&
                                timestamp > _decrementTime + this.HoldChangePeriod) {
                                decrement = true;
                            }
                        }

                        if (decrement) {
                            propertyOptionControl.Decrement();
                            _decrementTime = timestamp;
                            this.OnPropertyValueChanged(propertyOptionControl, false);
                        }
                    }
                } else if (this.UIInput.IsRightHeld && selectedOptionControl != null) {
                    PropertyOptionControlBase propertyOptionControl = selectedOptionControl as PropertyOptionControlBase;
                    if (propertyOptionControl != null && propertyOptionControl.PropertyOption.CanIncrement) {
                        // increment property option if conditions are met
                        bool increment = false;
                        if (this.UIInput.IsRightPressed) {
                            // pressing the right button to increment
                            increment = true;
                        } else if (propertyOptionControl.PropertyOption.CanHoldChange) {
                            // holding the right button to increment
                            if (timestamp > _rightPressedTime + this.HoldChangeWaitDuration &&
                                timestamp > _incrementTime + this.HoldChangePeriod) {
                                increment = true;
                            }
                        }

                        if (increment) {
                            propertyOptionControl.Increment();
                            _incrementTime = timestamp;
                            this.OnPropertyValueChanged(propertyOptionControl, true);
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
            this.ClearOptions();
        }

        #endregion

        #region Private Fields

        private List<IOption> _optionModels = new List<IOption>();
        private List<OptionControlBase> _optionControls = new List<OptionControlBase>();

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