using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

namespace Core.Unity.RewiredExtensions.JoystickStyles {

    /// <summary>
    /// Implementation for an XBox 360 controller joystick style.
    /// </summary>
    public class DS4JoystickStyle : IJoystickStyle {

        /// <inheritdoc />
        public JoystickStyleID ID => JoystickStyleID.DS4;

        /// <inheritdoc />
        public System.Guid RewiredHardwareGuid => _rewiredHardwareGuid;

        /// <inheritdoc />
        public string Name => "DualShock 4 Controller";

        /// <inheritdoc />
        public int GetImageIndex(ControllerElementType elementType, int elementIndex, Pole axisDirection) {
            if (elementType == ControllerElementType.Axis) {
                if (elementIndex < 0) {
                    return 0;
                } else if (elementIndex < 4) {
                    return 1 + elementIndex * 2 + (axisDirection == Pole.Negative ? 0 : 1);
                } else if (elementIndex < 6) {
                    return 5 + elementIndex;
                } else {
                    return 0;
                }
            } else {
                if (elementIndex < 0) {
                    return 0;
                } else if (elementIndex < 9) {
                    return 11 + elementIndex;
                } else if (elementIndex == 9) {
                    return 17;
                } else if (elementIndex < 16) {
                    return 9 + elementIndex;
                }
                return 0;
            }
        }

        #region Private

        private static System.Guid _rewiredHardwareGuid = new System.Guid("cd9718bf-a87a-44bc-8716-60a0def28a9f");

        #endregion
    }

}