using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Interface for objects with their own implementation of <see cref="Time.deltaTime"/>, <see cref="Time.fixedDeltaTime"/>.
    /// Useful for cases of individual slowdown or hit stop.
    /// </summary>
    public interface ICustomTime {

        /// <summary>
        /// Represents a custom implementation of <see cref="Time.deltaTime"/>.
        /// </summary>
        float DeltaTime { get; }

        /// <summary>
        /// Represents a custom implementation of <see cref="Time.fixedDeltaTime"/>.
        /// </summary>
        float FixedDeltaTime { get; }

        /// <summary>
        /// Represents <see cref="Time.time"/>, an acculmulation of <see cref="DeltaTime"/> every Update().
        /// </summary>
        float Time { get; }

        /// <summary>
        /// Represents <see cref="Time.fixedTime"/>, an accumulation of <see cref="FixedDeltaTime"/> every FixedUpdate().
        /// </summary>
        float FixedTime { get; }

    }
}