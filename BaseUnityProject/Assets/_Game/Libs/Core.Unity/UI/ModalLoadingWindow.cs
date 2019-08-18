using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.UI {

    /// <summary>
    /// Modal window representing something loading.
    /// </summary>
    public abstract class ModalLoadingWindow : ModalWindow {

        #region Properties to Override

        /// <inheritdoc />
        public override bool ZeroTimeScaleOnLaunch { get { return true; } }

        /// <summary>
        /// Gets if this window should automatically close when the load is done.
        /// </summary>
        public abstract bool AutoCloseOnLoadDone { get; }

        #endregion

        #region Methods to Override

        /// <summary>
        /// Gets if the loading is complete.
        /// </summary>
        public abstract bool GetIsLoadDone();

        #endregion

        #region Protected

        /// <summary>
        /// Checks if the load is done each frame.
        /// </summary>
        protected override void OnUpdate() {
            this.CheckLoadDone();
        }

        /// <summary>
        /// Checks if the load is done, calling events and closing window when appropriate.
        /// </summary>
        protected void CheckLoadDone() {

            if (this._cachedLoadDone)
                return;

            if (this.GetIsLoadDone()) {
                // load just completed now
                this._cachedLoadDone = true;

                // TODO: Call event?

                if (this.AutoCloseOnLoadDone) {
                    this.Close(ModalWindowResult.OK);
                }
            }
        }

        #endregion

        #region Private

        private bool _cachedLoadDone = false;

        #endregion
    }
}