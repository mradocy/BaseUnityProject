using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.CoScripts {

    public interface ICoScript {

        /// <summary>
        /// Gets if this <see cref="ICoScript"/> can be skipped by the player.
        /// </summary>
        bool CoScriptCanSkip { get; }

        /// <summary>
        /// The body of the <see cref="ICoScript"/>, intended to be overridden.  This contains the first instructions to be executed.
        /// </summary>
        IEnumerator CoScriptBody();

        /// <summary>
        /// If the player skips this <see cref="ICoScript"/>, then execution in the <see cref="CoScriptBody"/> will terminate immediately and instructions defined in this portion will play.
        /// </summary>
        IEnumerator CoScriptSkip();

        /// <summary>
        /// Instructions contained in this portion are played when the <see cref="CoScriptBody"/> or <see cref="CoScriptSkip"/> portions finish.
        /// </summary>
        /// <returns></returns>
        IEnumerator CoScriptEnd();
    }
}