using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

namespace Core.Unity.RewiredExtensions.JoystickStyles {

    /// <summary>
    /// Implementation for an XBox 360 controller joystick style.
    /// </summary>
    public class XBox360JoystickStyle : IJoystickStyle {

        /// <inheritdoc />
        public JoystickStyleID ID => JoystickStyleID.XBox360;

        /// <inheritdoc />
        public System.Guid RewiredHardwareGuid => _rewiredHardwareGuid;

        /// <inheritdoc />
        public string Name => "XBox 360 Controller";

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
                } else if (elementIndex < 8) {
                    return 11 + elementIndex;
                } else if (elementIndex < 15) {
                    return 10 + elementIndex;
                }
                return 0;
            }
        }

        #region Private

        private static System.Guid _rewiredHardwareGuid = new System.Guid("d74a350e-fe8b-4e9e-bbcd-efff16d34115");

        #endregion
    }

}