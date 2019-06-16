using UnityEngine;

namespace Core.Unity.Attributes {

    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class HeaderPrefabsAttribute : PropertyAttribute {
        public HeaderPrefabsAttribute() { }
    }

}