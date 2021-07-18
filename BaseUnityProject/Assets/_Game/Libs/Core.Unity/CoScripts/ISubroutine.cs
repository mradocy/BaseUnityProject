using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.CoScripts {

    public interface ISubroutine {
        /// <summary>
        /// Starts the subroutine
        /// </summary>
        void Start();
        /// <summary>
        /// Called each frame by the manager
        /// </summary>
        void Update();
        /// <summary>
        /// Forcefully stops the subroutine
        /// </summary>
        void Stop();
        /// <summary>
        /// Gets if the subroutine is currently complete
        /// </summary>
        bool IsComplete { get; }
    }
}