using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace Core.Unity.RewiredExtensions {

    /// <summary>
    /// Interface for a joystick style.
    /// </summary>
    public interface IJoystickStyle {

        /// <summary>
        /// Enumerated ID for this style.
        /// </summary>
        JoystickStyleID ID { get; }

        /// <summary>
        /// The Guid of the controller hardware given by Rewired.
        /// </summary>
        System.Guid RewiredHardwareGuid { get; }

        /// <summary>
        /// Name of this joystick style (e.g. "XBox 360 Controller")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the index of the icon corresponding to the given element in this joystick style's texture.
        /// </summary>
        /// <param name="elementType">Type of the controller element (axis or button)</param>
        /// <param name="elementIndex">Index of the controller element in its type.</param>
        /// <param name="axisDirection">Direction of the axis, if the element is an axis.</param>
        /// <returns>Image index.</returns>
        int GetImageIndex(ControllerElementType elementType, int elementIndex, Pole axisDirection);

    }
}