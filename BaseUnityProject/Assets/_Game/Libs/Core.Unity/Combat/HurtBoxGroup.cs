using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Combat {

    /// <summary>
    /// A group of <see cref="HurtBox"/>s.  Each hurt box in this group is considered to have the same <see cref="UniqueId"/>.
    /// </summary>
    public class HurtBoxGroup : MonoBehaviour {

        /// <summary>
        /// Gets an id that uniquely defines this hurt box group instance.  e.g. the result of this.GetInstanceID().
        /// </summary>
        public int UniqueId {
            get { return this.GetInstanceID(); }
        }
    }
}