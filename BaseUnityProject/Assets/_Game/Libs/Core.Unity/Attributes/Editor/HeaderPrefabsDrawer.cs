using UnityEditor;
using UnityEngine;

namespace Core.Unity.Attributes {

    [CustomPropertyDrawer(typeof(HeaderPrefabsAttribute))]
    public class HeaderPrefabsDrawer : HeaderCustomDrawer {

        public override string Label {
            get { return "Prefabs"; }
        }

        public override Color HeaderColor {
            get { return new Color(0, .23f, .23f); }
        }

    }
}