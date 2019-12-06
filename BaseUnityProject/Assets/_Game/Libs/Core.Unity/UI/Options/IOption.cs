using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI.Options {

    /// <summary>
    /// Model of an option that would appear in an options menu.
    /// </summary>
    public interface IOption {

        /// <summary>
        /// Gets the name to be displayed in the option control.
        /// </summary>
        string DisplayName { get; }
    }
}