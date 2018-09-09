using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class HeaderPrefabsAttribute : PropertyAttribute {
    public HeaderPrefabsAttribute() { }
}

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class HeaderChildrenAttribute : PropertyAttribute {
    public HeaderChildrenAttribute() { }
}

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class HeaderSceneAttribute : PropertyAttribute {
    public HeaderSceneAttribute() { }
}

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class HeaderAssetsAttribute : PropertyAttribute {
    public HeaderAssetsAttribute() { }
}
