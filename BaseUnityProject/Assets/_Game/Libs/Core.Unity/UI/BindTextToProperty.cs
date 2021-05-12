using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using UnityEngine.UI;

namespace Core.Unity.UI {

    [ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class BindTextToProperty : BindTextToPropertyBase {

        protected override void UpdateText() {
            Text textComponent = this.GetComponent<Text>();
            if (textComponent == null)
                return;

            textComponent.text = this.GetComponentText();
        }
    }
}