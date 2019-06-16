using UnityEngine;

namespace Core.Unity.Attributes {

    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class HeaderSceneAttribute : PropertyAttribute {
        public HeaderSceneAttribute() { }
    }

}