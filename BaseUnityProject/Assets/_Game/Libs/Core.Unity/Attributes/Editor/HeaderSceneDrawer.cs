using UnityEngine;
using UnityEditor;

namespace Core.Unity.Attributes {

    [CustomPropertyDrawer(typeof(HeaderSceneAttribute))]
    public class HeaderSceneDrawer : HeaderCustomDrawer {

        public override string Label {
            get { return "Scene"; }
        }

        public override Color HeaderColor {
            get { return new Color(.23f, 0, .23f); }
        }

    }
}