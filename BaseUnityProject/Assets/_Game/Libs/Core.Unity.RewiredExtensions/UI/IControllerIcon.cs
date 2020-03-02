using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Rewired;

namespace Core.Unity.RewiredExtensions.UI {

    /// <summary>
    /// Interface for a graphical object displaying a controller icon.
    /// </summary>
    public interface IControllerIcon {

        /// <summary>
        /// The input method for the icon (e.g. keyboard, joystick)
        /// </summary>
        InputMethod InputMethod { get; }

        /// <summary>
        /// The <see cref="KeyCode"/> to display when <see cref="this.InputMethod"/> is <see cref="InputMethod.Keyboard"/>.
        /// </summary>
        KeyCode KeyboardKey { get; }

        /// <summary>
        /// The <see cref="JoystickStyle"/> to display when <see cref="this.InputMethod"/> is <see cref="InputMethod.Joystick"/>.
        /// </summary>
        JoystickStyleID JoystickStyle { get; }

        /// <summary>
        /// The direction of the joystick element, if it's an axis.
        /// </summary>
        Pole JoystickAxisDirection { get; }

        /// <summary>
        /// Sets this icon to a keyboard icon for the given key.
        /// </summary>
        /// <param name="key">Keyboard key.</param>
        void SetKeyboardIcon(KeyCode key);

        /// <summary>
        /// Sets this icon to a joystick icon for the given element.
        /// </summary>
        /// <param name="joystickStyle">Style of the joystick.</param>
        /// <param name="elementType">The type of the element (axis/button).</param>
        /// <param name="elementIndex">Index of the element.</param>
        /// <param name="axisDirection">Direction of the axis, if element is an axis.</param>
        void SetJoystickIcon(JoystickStyleID joystickStyle, ControllerElementType elementType, int elementIndex, Pole axisDirection);
    }
}