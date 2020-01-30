using Core.Unity.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unity.UI {

    [ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class BindText : BindTextBase {

        /// <summary>
        /// Updates the text in the text component.
        /// </summary>
        protected override void UpdateText() {
            Text textComponent = this.GetComponent<Text>();
            if (textComponent == null)
                return;

            textComponent.text = this.GetComponentText();
        }
    }
}