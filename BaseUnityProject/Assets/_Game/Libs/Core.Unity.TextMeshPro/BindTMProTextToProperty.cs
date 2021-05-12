using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using Core.Unity.UI;

namespace Core.Unity.TextMeshPro {

    [ExecuteAlways]
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class BindTMProTextToProperty : BindTextToPropertyBase {

        protected override void UpdateText() {
            TMPro.TMP_Text textComponent = this.GetComponent<TMPro.TMP_Text>();
            if (textComponent == null)
                return;

            textComponent.text = this.GetComponentText();
        }
    }
}