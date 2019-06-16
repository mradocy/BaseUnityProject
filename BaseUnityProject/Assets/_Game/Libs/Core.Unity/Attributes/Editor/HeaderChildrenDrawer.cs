using UnityEditor;
using UnityEngine;

namespace Core.Unity.Attributes {

    [CustomPropertyDrawer(typeof(HeaderChildrenAttribute))]
    public class HeaderChildrenDrawer : HeaderCustomDrawer {

        public override string Label {
            get { return "Children"; }
        }

        public override Color HeaderColor {
            get { return new Color(.23f, .23f, 0); }
        }

    }
}