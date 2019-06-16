using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.SaveData {
    public interface ISerializable {
        /// <summary>
        /// Serializes this object into a string.  Returns null if there was a problem.
        /// </summary>
        /// <param name="prettyPrint">If the result should be human readable.</param>
        /// <returns>string data</returns>
        string Serialize(bool prettyPrint);

        /// <summary>
        /// Deserializes the given string data into the object.
        /// Returns true if successful, false if there was an error.
        /// </summary>
        /// <param name="data">Data to deserialize.</param>
        /// <returns>Success.</returns>
        bool Deserialize(string data);
    }
}