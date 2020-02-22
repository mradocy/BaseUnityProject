using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unity.RewiredExtensions {

    [RequireComponent(typeof(RawImage))]
    [ExecuteInEditMode]
    public class ControllerIconRawImage : MonoBehaviour, IControllerIcon {

        #region Inspector Fields

        [Header("Editor Preview")]

        [SerializeField]
        [Tooltip("The input method for the icon to use.")]
        private InputMethod _inputMethod = InputMethod.Keyboard;

        [SerializeField]
        [Tooltip("The keyboard key to display.")]
        private KeyCode _keyboardKey = KeyCode.None;

        [SerializeField]
        [Tooltip("The joystick style of the icon.")]
        private JoystickStyleID _joystickStyleID = JoystickStyleID.Generic;

        [SerializeField]
        [Tooltip("The type of the joystick element (axis/button)")]
        private ControllerElementType _joystickElementType = ControllerElementType.Button;

        [SerializeField]
        [Tooltip("The index of the joystick element to display.")]
        private int _joystickElementIndex = 0;

        [SerializeField]
        [Tooltip("The direction of the joystick element, if it's an axis.")]
        private Pole _joystickAxisDirection = Pole.Negative;

        [SerializeField]
        [Tooltip("If the icon should be transparent.  Usually denotes an action that the player can't assign.")]
        private bool _isFaded = false;


        [Header("Appearance")]

        [SerializeField]
        private float _fadedAlpha = .5f;


        [Header("Keyboard")]

        [SerializeField]
        [Tooltip("The keyboard atlas image to display in the RawImage component.")]
        private Texture _keyboardTexture = null;

        [SerializeField]
        [Tooltip("Number of columns the keyboard atlas image has.")]
        private int _keyboardNumColumns = 10;

        [SerializeField]
        [Tooltip("Number of rows the keyboard atlas image has.")]
        private int _keyboardNumRows = 14;


        [Header("Joystick")]

        [SerializeField]
        [Tooltip("The Joystick - Generic image to display in the RawImage component.")]
        private Texture _joystickGenericTexture = null;

        [SerializeField]
        [Tooltip("The Joystick - XBox 360 image to display in the RawImage component.")]
        private Texture _joystickXBox360Texture = null;

        [SerializeField]
        [Tooltip("The Joystick - DS4 image to display in the RawImage component.")]
        private Texture _joystickDS4Texture = null;

        [SerializeField]
        [Tooltip("The number of columns all the joystick atlas images have.")]
        private int _joystickNumColumns = 10;

        [SerializeField]
        [Tooltip("The number of rows all the joystick atlas images have.")]
        private int _joystickNumRows = 3;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets if the icon is visible.
        /// </summary>
        public bool IsVisible {
            get { return this.RawImage.enabled; }
            set { this.RawImage.enabled = value; }
        }

        /// <summary>
        /// Gets/sets if the icon is faded.
        /// </summary>
        public bool IsFaded {
            get { return _isFaded; }
            set {
                if (_isFaded == value) return;
                _isFaded = value;
                this.UpdateRawImage();
            }
        }

        /// <inheritdoc />
        public InputMethod InputMethod {
            get { return _inputMethod; }
        }

        /// <inheritdoc />
        public KeyCode KeyboardKey {
            get { return _keyboardKey; }
        }

        /// <inheritdoc />
        public JoystickStyleID JoystickStyle {
            get { return _joystickStyleID; }
        }

        /// <inheritdoc />
        public Pole JoystickAxisDirection {
            get { return _joystickAxisDirection; }
        }

        /// <summary>
        /// Reference to the <see cref="RawImage"/> component whose UV Rect gets set when the <see cref="KeyboardKey"/> changes.
        /// </summary>
        public RawImage RawImage { get; private set; }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void SetKeyboardIcon(KeyCode key) {
            _inputMethod = InputMethod.Keyboard;
            _keyboardKey = key;
            this.UpdateRawImage();
        }

        /// <summary>
        /// Sets this icon to a joystick icon for the given element.
        /// </summary>
        /// <param name="rewiredHardwareGuid">Rewired Guid of the controller hardware.  If the hardware couldn't be found, the generic joystick style will be used instead.</param>
        /// <param name="elementType">The type of the element (axis/button).</param>
        /// <param name="elementIndex">Index of the element.</param>
        /// <param name="axisDirection">Direction of the axis, if element is an axis.</param>
        public void SetJoystickIcon(System.Guid rewiredHardwareGuid, ControllerElementType elementType, int elementIndex, Pole axisDirection) {
            this.SetJoystickIcon(RewiredUtils.GetJoystickStyle(rewiredHardwareGuid).ID, elementType, elementIndex, axisDirection);
        }

        /// <inheritdoc />
        public void SetJoystickIcon(JoystickStyleID joystickStyle, ControllerElementType elementType, int elementIndex, Pole axisDirection) {
            _inputMethod = InputMethod.Joystick;
            _joystickStyleID = joystickStyle;
            _joystickElementType = elementType;
            _joystickElementIndex = elementIndex;
            _joystickAxisDirection = axisDirection;
            this.UpdateRawImage();
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Updates the source and UV rect of the raw image, based on the current input method, joystick style, key, button
        /// </summary>
        private void UpdateRawImage() {

            int imageIndex = 0;
            if (_inputMethod == InputMethod.Keyboard) {

                // update keyboard map
                if (_keyboardMap == null) {
                    _keyboardMap = RewiredUtils.GetKeyboardMap();
                }

                // get image index
                imageIndex = _keyboardMap.GetImageIndex(_keyboardKey);

            } else if (_inputMethod == InputMethod.Joystick) {

                // update joystick style
                if (_joystickStyle == null || _joystickStyle.ID != _joystickStyleID) {
                    _joystickStyle = RewiredUtils.GetJoystickStyle(_joystickStyleID);
                }

                // get image index
                imageIndex = _joystickStyle.GetImageIndex(_joystickElementType, _joystickElementIndex, _joystickAxisDirection);
            }

            this.SetRawImage(
                _inputMethod,
                _joystickStyleID,
                imageIndex);

            // TODO: could probably merge SetRawImage() into this.

            // set color
            Color c = this.RawImage.color;
            if (this.IsFaded) {
                c.a = _fadedAlpha;
            } else {
                c.a = 1;
            }
            this.RawImage.color = c;
        }

        /// <summary>
        /// Sets the source and UV rect of the raw image based on the given image index, input method, and number of columns, rows in the atlas image.
        /// </summary>
        /// <param name="inputMethod">The input method.  Decides which image atlas to display.  Note that all controller image atlases are formatted the same way.</param>
        /// <param name="joystickStyle">Decides which image atlas to used, if the input method is <see cref="InputMethod.Joystick"/>.</param>
        /// <param name="imageIndex">Index within the image atlas to display.</param>
        private void SetRawImage(InputMethod inputMethod, JoystickStyleID joystickStyle, int imageIndex) {
            if (Application.isEditor || this.RawImage == null) {
                this.RawImage = this.GetComponent<RawImage>();
            }

            int numColumns = 0;
            int numRows = 0;

            switch (inputMethod) {
            case InputMethod.Keyboard:
                numColumns = _keyboardNumColumns;
                numRows = _keyboardNumRows;
                this.RawImage.texture = _keyboardTexture;
                break;
            case InputMethod.Joystick:
                numColumns = _joystickNumColumns;
                numRows = _joystickNumRows;
                switch (joystickStyle) {
                case JoystickStyleID.Generic:
                    this.RawImage.texture = _joystickGenericTexture;
                    break;
                case JoystickStyleID.XBox360:
                    this.RawImage.texture = _joystickXBox360Texture;
                    break;
                case JoystickStyleID.DS4:
                    this.RawImage.texture = _joystickDS4Texture;
                    break;
                }
                break;
            }

            if (numColumns < 1 || numRows < 1 ||
                imageIndex < 0 || imageIndex >= numColumns * numRows) {
                this.RawImage.uvRect = new Rect(0, 0, 1, 1);
                return;
            }

            int col = imageIndex % numColumns;
            int row = numRows - 1 - imageIndex / numColumns;

            this.RawImage.uvRect = new Rect(
                (float)col / numColumns,
                (float)row / numRows,
                1.0f / numColumns,
                1.0f / numRows);
        }

        #endregion

        #region Unity Events

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// </summary>
        private void Awake() {
            this.RawImage = this.GetComponent<RawImage>();
        }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {

            if (Application.isEditor) {

                // failsafe
                _keyboardNumColumns = Mathf.Max(1, _keyboardNumColumns);
                _keyboardNumRows = Mathf.Max(1, _keyboardNumRows);
                _joystickNumColumns = Mathf.Max(1, _joystickNumColumns);
                _joystickNumRows = Mathf.Max(1, _joystickNumRows);

                this.UpdateRawImage();
            }
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Cached <see cref="KeyboardMap"/>.
        /// </summary>
        private KeyboardMap _keyboardMap = null;
        /// <summary>
        /// Cached <see cref="IJoystickStyle"/>.
        /// </summary>
        private IJoystickStyle _joystickStyle = null;

        #endregion

    }
}