using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity.CoScripts {

    public struct Wait {
        /// <summary>
        /// Wait for one frame before proceeding.
        /// </summary>
        public static Wait OneFrame {
            get => new Wait(WaitId.Frames, 1, null);
        }
        /// <summary>
        /// Wait for the given duration before proceeding.
        /// </summary>
        /// <param name="duration">Duration (in seconds)</param>
        public static Wait Duration(float duration) {
            return new Wait(WaitId.Duration, duration, null);
        }
        /// <summary>
        /// Wait for the given number of frames before proceeding.
        /// </summary>
        public static Wait Frames(int frameCount) {
            return new Wait(WaitId.Frames, frameCount, null);
        }
        /// <summary>
        /// Waits until the given <see cref="ISubroutine"/> is complete.
        /// <see cref="ISubroutine.Start"/> is called at the beginning of the next <see cref="CoScriptManager.OnUpdate"/> step,
        /// and progression is blocked until <see cref="ISubroutine.IsComplete"/> is true.
        /// </summary>
        public static Wait Subroutine(ISubroutine subroutine) {
            if (subroutine == null) {
                Debug.LogError($"Given {nameof(subroutine)} must be defined");
            }
            return new Wait(WaitId.Subroutine, 0, subroutine);
        }

        public WaitId Id { get; }
        public float ParamFloat { get; }
        public ISubroutine ParamSubroutine { get; }

        private Wait(WaitId id, float param0, ISubroutine subroutine) {
            this.Id = id;
            this.ParamFloat = param0;
            this.ParamSubroutine = subroutine;
        }

        public enum WaitId : byte {
            None,
            Duration,
            Frames,
            Subroutine
        }
    }
}