using UnityEngine;
using UnityEditor;

namespace Core.Unity.Attributes {

    [CustomPropertyDrawer(typeof(HeaderAssetsAttribute))]
    public class HeaderAssetsDrawer : HeaderCustomDrawer {

        public override string Label {
            get { return "Assets"; }
        }

        public override Color HeaderColor {
            get { return new Color(0, 0, .23f); }
        }

    }
}