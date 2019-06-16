using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

namespace Core.Unity.RewiredExtensions.JoystickStyles {

    /// <summary>
    /// Implementation for an Generic controller joystick style.
    /// This is the default style if a style for the connected controller can't be found.
    /// </summary>
    public class GenericJoystickStyle : IJoystickStyle {

        /// <inheritdoc />
        public JoystickStyleID ID => JoystickStyleID.Generic;

        /// <inheritdoc />
        public System.Guid RewiredHardwareGuid => _rewiredHardwareGuid;

        /// <inheritdoc />
        public string Name => "Generic Controller";

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
                if (0 <= elementIndex && elementIndex < 14) {
                    return 11 + elementIndex;
                }
                return 0;
            }
        }

        #region Private

        private static System.Guid _rewiredHardwareGuid = new System.Guid("00000000-0000-0000-0000-000000000000");

        #endregion
    }

}