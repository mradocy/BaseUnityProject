using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.Combat {

    /// <summary>
    /// Interface for accessing raw data detailing an attack.
    /// </summary>
    public interface IAttackData {

        /// <summary>
        /// Name of the attack.  Will be used to identify the attack in a collection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Damage of the attack.  This is the attack's base power, before any calculations are done.
        /// </summary>
        float Damage { get; }

        /// <summary>
        /// The heading of the attack in degrees.
        /// </summary>
        float Heading { get; }

        /// <summary>
        /// Gets how long the hit stop should be as a result of the attack hitting.
        /// Set to 0 for no hit stop.
        /// </summary>
        float HitStopDuration { get; }

        /// <summary>
        /// Gets if this attack contains the flag with the given id.
        /// </summary>
        /// <param name="flagId">Id of the flag.</param>
        /// <returns>Has flag.</returns>
        bool HasFlag(int flagId);

        /// <summary>
        /// Gets all the flags contained in this data by filling the given set.
        /// </summary>
        /// <param name="flags">Set of flags to fill.</param>
        void GetAllFlags(ISet<int> flags);

        /// <summary>
        /// Gets the value of the attribute with the given id.
        /// </summary>
        /// <param name="attributeId">Id of the attribute.</param>
        /// <param name="defaultValue">Value to return if the attack data does not contain the attribute.</param>
        /// <returns>Attribute value.</returns>
        float GetAttribute(int attributeId, float defaultValue = 0);

        /// <summary>
        /// Gets all the attributes contained in this data by filling the given dictionary.
        /// </summary>
        /// <param name="attributes">Dictionary of attributes to fill.</param>
        void GetAllAttributes(IDictionary<int, float> attributes);
    }
}