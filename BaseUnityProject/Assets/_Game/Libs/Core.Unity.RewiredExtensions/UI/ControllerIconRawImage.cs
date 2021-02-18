using Core.Unity.Attributes;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unity.RewiredExtensions.UI {

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
        [Tooltip("If the icon is pressed.")]
        private bool _isPressed = false;


        [Header("Children")]

        [SerializeField]
        [Tooltip("Child RawImage component to display frames from the keyboard background textures or the joystick textures.")]
        private RawImage _backgroundRawImage = null;

        [SerializeField]
        [Tooltip("Child RawImage component to display frames from the keyboard text textures.")]
        private RawImage _textRawImage = null;


        [Header("Keyboard")]

        [SerializeField, LongLabel]
        [Tooltip("The atlas image containing backgrounds for default keys to display in the background RawImage component.")]
        private Texture _keyboardBackgroundsDefaultTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The atlas image containing backgrounds for pressed keys to display in the background RawImage component.")]
        private Texture _keyboardBackgroundsPressedTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The atlas image containing text for default keys to display in the text RawImage component.")]
        private Texture _keyboardTextDefaultTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The atlas image containing text for pressed keys to display in the text RawImage component.")]
        private Texture _keyboardTextPressedTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("Number of columns the keyboard background atlas images have.")]
        private int _keyboardBackgroundNumColumns = 4;

        [SerializeField, LongLabel]
        [Tooltip("Number of rows the keyboard background atlas images have.")]
        private int _keyboardBackgroundNumRows = 1;

        [SerializeField, LongLabel]
        [Tooltip("Number of columns the keyboard text atlas images have.")]
        private int _keyboardTextNumColumns = 10;

        [SerializeField, LongLabel]
        [Tooltip("Number of rows the keyboard text atlas images have.")]
        private int _keyboardTextNumRows = 14;


        [Header("Joystick")]

        [SerializeField, LongLabel]
        [Tooltip("The Joystick - Generic image to display for default buttons in the background RawImage component.")]
        private Texture _joystickGenericDefaultTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The Joystick - Generic image to display for pressed buttons in the background RawImage component.")]
        private Texture _joystickGenericPressedTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The Joystick - XBox 360 image to display for default buttons in the background RawImage component.")]
        private Texture _joystickXBox360DefaultTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The Joystick - XBox 360 image to display for pressed buttons in the background RawImage component.")]
        private Texture _joystickXBox360PressedTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The Joystick - DS4 image to display for default buttons in the RawImage component.")]
        private Texture _joystickDS4DefaultTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The Joystick - DS4 image to display for pressed buttons in the RawImage component.")]
        private Texture _joystickDS4PressedTexture = null;

        [SerializeField, LongLabel]
        [Tooltip("The number of columns all the joystick atlas images have.")]
        private int _joystickNumColumns = 10;

        [SerializeField, LongLabel]
        [Tooltip("The number of rows all the joystick atlas images have.")]
        private int _joystickNumRows = 3;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets if the icon is visible.
        /// </summary>
        public bool IsVisible {
            get {
                if (_backgroundRawImage == null)
                    return false;
                return _backgroundRawImage.enabled;
            }
            set {
                if (_backgroundRawImage != null) {
                    _backgroundRawImage.enabled = value;
                }
                if (_textRawImage != null) {
                    _textRawImage.enabled = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets if the icon is pressed.
        /// </summary>
        public bool IsPressed {
            get { return _isPressed; }
            set {
                if (_isPressed == value) return;
                _isPressed = value;
                this.UpdateRawImage();
            }
        }

        public InputMethod InputMethod => _inputMethod;

        public KeyCode KeyboardKey => _keyboardKey;

        public JoystickStyleID JoystickStyle => _joystickStyleID;

        public Pole JoystickAxisDirection => _joystickAxisDirection;

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
        /// Updates the source and UV rect of the raw image, based on the current input method, joystick style, key, button, pressed
        /// </summary>
        private void UpdateRawImage() {

            if (_inputMethod == InputMethod.Keyboard) {
                // update keyboard map
                if (_keyboardMap == null) {
                    _keyboardMap = RewiredUtils.GetKeyboardMap();
                }

                // get image indices
                int textImageIndex = _keyboardMap.GetTextImageIndex(_keyboardKey);
                int backgroundImageIndex = _keyboardMap.GetBackgroundImageIndex(_keyboardKey);

                // update text raw image
                if (_textRawImage != null) {
                    _textRawImage.gameObject.SetActive(true);
                    if (_isPressed) {
                        _textRawImage.texture = _keyboardTextPressedTexture;
                    } else {
                        _textRawImage.texture = _keyboardTextDefaultTexture;
                    }
                    SetRawImageUVRect(_textRawImage, _keyboardTextNumColumns, _keyboardTextNumRows, textImageIndex);
                }

                // update background raw image
                if (_backgroundRawImage != null) {
                    if (_isPressed) {
                        _backgroundRawImage.texture = _keyboardBackgroundsPressedTexture;
                    } else {
                        _backgroundRawImage.texture = _keyboardBackgroundsDefaultTexture;
                    }
                    SetRawImageUVRect(_backgroundRawImage, _keyboardBackgroundNumColumns, _keyboardBackgroundNumRows, backgroundImageIndex);
                }
            } else {
                // update joystick style
                if (_joystickStyle == null || _joystickStyle.ID != _joystickStyleID) {
                    _joystickStyle = RewiredUtils.GetJoystickStyle(_joystickStyleID);
                }

                if (_backgroundRawImage != null) {
                    // set texture
                    if (_isPressed) {
                        switch (_joystickStyleID) {
                        case JoystickStyleID.Generic:
                            _backgroundRawImage.texture = _joystickGenericPressedTexture;
                            break;
                        case JoystickStyleID.XBox360:
                            _backgroundRawImage.texture = _joystickXBox360PressedTexture;
                            break;
                        case JoystickStyleID.DS4:
                            _backgroundRawImage.texture = _joystickDS4PressedTexture;
                            break;
                        }
                    } else {
                        switch (_joystickStyleID) {
                        case JoystickStyleID.Generic:
                            _backgroundRawImage.texture = _joystickGenericDefaultTexture;
                            break;
                        case JoystickStyleID.XBox360:
                            _backgroundRawImage.texture = _joystickXBox360DefaultTexture;
                            break;
                        case JoystickStyleID.DS4:
                            _backgroundRawImage.texture = _joystickDS4DefaultTexture;
                            break;
                        }
                    }

                    // set image index
                    int backgroundImageIndex = _joystickStyle.GetImageIndex(_joystickElementType, _joystickElementIndex, _joystickAxisDirection);
                    SetRawImageUVRect(_backgroundRawImage, _joystickNumColumns, _joystickNumRows, backgroundImageIndex);
                }

                if (_textRawImage != null) {
                    _textRawImage.gameObject.SetActive(false);
                }
            }
        }

        private static void SetRawImageUVRect(RawImage rawImage, int numColumns, int numRows, int index) {
            if (rawImage == null)
                return;

            if (numColumns < 1 || numRows < 1 ||
                index < 0 || index >= numColumns * numRows) {
                rawImage.uvRect = new Rect(0, 0, 1, 1);
                return;
            }

            int col = index % numColumns;
            int row = numRows - 1 - index / numColumns;

            rawImage.uvRect = new Rect(
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
        private void Awake() { }

        /// <summary>
        /// Called by Unity every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() {

            if (Application.isEditor) {

                // failsafe
                _keyboardTextNumColumns = Mathf.Max(1, _keyboardTextNumColumns);
                _keyboardTextNumRows = Mathf.Max(1, _keyboardTextNumRows);
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