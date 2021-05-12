using Core.Unity.Assets;
using Core.Unity.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: delete this

namespace Core.Unity.TextMeshPro {

    [ExecuteAlways]
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class BindTMProText : BindTextBase {

        /// <summary>
        /// Updates the text in the text component.
        /// </summary>
        protected override void UpdateText() {
            TMPro.TMP_Text textComponent = this.GetComponent<TMPro.TMP_Text>();
            if (textComponent == null)
                return;

            textComponent.text = this.GetComponentText();
        }
    }
}