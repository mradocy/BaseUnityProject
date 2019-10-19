using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;

namespace Core.Unity.Attributes {

    /// <summary>
    /// Restricts the type (e.g. an interface) of a component that can be set to a field in the inspector.
    /// </summary>
    public class TypeRestrictionAttribute : PropertyAttribute {

        public TypeRestrictionAttribute(System.Type type, bool allowSceneObjects = true) {
            if (type == null)
                throw new System.ArgumentNullException(nameof(type));

            this.Type = type;
            this.AllowSceneObjects = allowSceneObjects;
        }

        public System.Type Type { get; }

        public bool AllowSceneObjects { get; }

    }
}