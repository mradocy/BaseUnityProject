using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.CoScripts {

    public abstract class CoScriptMonoBehaviourBase : MonoBehaviour, ICoScript {

        public abstract bool CoScriptCanSkip { get; }

        public virtual IEnumerator CoScriptBody() {
            yield break;
        }

        public virtual IEnumerator CoScriptSkip() {
            yield break;
        }

        public virtual IEnumerator CoScriptEnd() {
            yield break;
        }

        /// <summary>
        /// Called by Unity when this object is destroyed.
        /// </summary>
        protected void OnDestroy() {
            CoScriptManager.StopScript(this);

            this.OnDerivedDestroy();
        }

        protected virtual void OnDerivedDestroy() { }
    }
}