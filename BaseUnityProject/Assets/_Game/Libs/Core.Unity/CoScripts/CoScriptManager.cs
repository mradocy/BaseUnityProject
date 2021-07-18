using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity.CoScripts {

    public static class CoScriptManager {

        #region Initialization

        /// <summary>
        /// Called by Unity before any scene is loaded.
        /// https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute-ctor.html
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            PersistantGameObject.UpdateEvent += OnUpdate;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event invoked when a script is started.
        /// </summary>
        public static event UnityAction<ICoScript> ScriptStarted;

        /// <summary>
        /// Event invoked when a script is skipped.
        /// </summary>
        public static event UnityAction<ICoScript> ScriptSkipped;

        /// <summary>
        /// Event invoked when a script is stopped.
        /// </summary>
        public static event UnityAction<ICoScript> ScriptStopped;

        #endregion

        #region Methods

        /// <summary>
        /// Starts the given <see cref="ICoScript"/> on the <see cref="CoScriptBase.CoScriptBody"/> method.
        /// The first commands of the script aren't called until the next Update() step.
        /// </summary>
        public static void StartScript(ICoScript coScript) {
            CoScriptWrapper coScriptWrapper = CoScriptWrapper.Create(coScript);
            _coScriptWrappers.Add(coScriptWrapper);

            coScriptWrapper.StartMethod(Method.Body);
            ScriptStarted?.Invoke(coScript);
        }

        /// <summary>
        /// Gets if any scripts are currently running.
        /// </summary>
        public static bool IsAnyScriptRunning {
            get => _coScriptWrappers.Count > 0;
        }

        /// <summary>
        /// Gets if the given script is currently running.
        /// </summary>
        public static bool IsScriptRunning(ICoScript coScript) {
            foreach (CoScriptWrapper wrapper in _coScriptWrappers) {
                if (wrapper.CoScript == coScript)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets if there are any scripts running, and that all the running scripts can be skipped.
        /// </summary>
        public static bool CanSkipScripts() {
            if (!IsAnyScriptRunning)
                return false;
            foreach (CoScriptWrapper wrapper in _coScriptWrappers) {
                if (!wrapper.CanSkip) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Sets a flag that will skip every running skippable script at the end of Update().
        /// This ensures that each skippable script would have updated at least once.
        /// </summary>
        public static void SkipScripts() {
            _skipScriptsAtUpdateEnd = true;
        }

        /// <summary>
        /// Stops the given <see cref="ICoScript"/> if it's running.
        /// </summary>
        public static void StopScript(ICoScript coScript) {
            // get index of script
            CoScriptWrapper coScriptWrapper = null;
            int index;
            for (index = 0; index < _coScriptWrappers.Count; index++) {
                if (_coScriptWrappers[index].CoScript == coScript) {
                    coScriptWrapper = _coScriptWrappers[index];
                    break;
                }
            }
            if (coScriptWrapper == null)
                return;

            // reached end of script, remove wrapper from list and recycle
            _coScriptWrappers.RemoveAt(index);
            if (index <= _scriptsIt) {
                _scriptsIt--;
            }

            ScriptStopped?.Invoke(coScript);

            coScriptWrapper.Recycle();
        }

        /// <summary>
        /// Stops all the running scripts.
        /// </summary>
        public static void StopAllScripts() {
            while (_coScriptWrappers.Count > 0) {
                StopScript(_coScriptWrappers[0].CoScript);
            }
        }

        #endregion

        #region Private

        private static void OnUpdate() {
            float time = Time.time;
            int frameCount = Time.frameCount;

            for (_scriptsIt = 0; _scriptsIt < _coScriptWrappers.Count; _scriptsIt++) {
                CoScriptWrapper coScriptWrapper = _coScriptWrappers[_scriptsIt];
                bool endOfScript = false;

                while (true) {

                    // get yielded object
                    object current = coScriptWrapper.Enumerator.Current;

                    // block execution if waiting for something
                    bool blockExecution = false;
                    if (current != null) {
                        Wait wait = (Wait)current;
                        Wait.WaitId id = wait.Id;
                        switch (id) {
                        case Wait.WaitId.Duration:
                            if (time - coScriptWrapper.TimeAtMoveNext < wait.ParamFloat) {
                                // not enough time has passed, keep waiting
                                blockExecution = true;
                            }
                            break;
                        case Wait.WaitId.Frames:
                            if (frameCount - coScriptWrapper.FrameCountAtMoveNext < Mathf.RoundToInt(wait.ParamFloat)) {
                                // not enough frames have passed, keep waiting
                                blockExecution = true;
                            }
                            break;
                        case Wait.WaitId.Subroutine:
                            // start subroutine if not started yet
                            if (!coScriptWrapper.SubroutineStarted) {
                                coScriptWrapper.SubroutineStarted = true;
                                wait.ParamSubroutine.Start();
                            }
                            // invoke update
                            wait.ParamSubroutine.Update();
                            // block execution until subroutine is finished
                            blockExecution = !wait.ParamSubroutine.IsComplete;
                            break;
                        default:
                            Debug.LogError($"Cannot yet handle id of {id}");
                            break;
                        }
                    }
                    if (blockExecution)
                        break;

                    // move to next yield
                    bool moveNext = coScriptWrapper.Enumerator.MoveNext();
                    coScriptWrapper.TimeAtMoveNext = time;
                    coScriptWrapper.FrameCountAtMoveNext = frameCount;
                    coScriptWrapper.SubroutineStarted = false;

                    if (moveNext) {

                        // 'yield return null' to have the same effect as waiting for 1 frame
                        if (coScriptWrapper.Enumerator.Current == null) {
                            break;
                        }

                    } else {
                        // no more yield statements, reached end of method
                        if (coScriptWrapper.Method == Method.Body || coScriptWrapper.Method == Method.Skip) {
                            // start end method
                            coScriptWrapper.StartMethod(Method.End);
                        } else {
                            // reached end of script
                            endOfScript = true;
                            break;
                        }
                    }
                } // while

                // skip if flag set
                if (_skipScriptsAtUpdateEnd) {
                    if (coScriptWrapper.CanSkip) {
                        coScriptWrapper.StartMethod(Method.Skip);
                        ScriptSkipped?.Invoke(coScriptWrapper.CoScript);
                    }
                }

                if (endOfScript) {
                    // reached end of script, stop and remove
                    StopScript(coScriptWrapper.CoScript);
                    continue;
                }

            } // for

            _scriptsIt = -1;

            // clear skip scripts flag
            if (_skipScriptsAtUpdateEnd) {
                _skipScriptsAtUpdateEnd = false;
            }
        }

        public enum Method {
            None,
            Body,
            Skip,
            End,
        }

        private class CoScriptWrapper {

            public static CoScriptWrapper Create(ICoScript coScript) {
                if (coScript == null) {
                    Debug.LogError($"Param {nameof(coScript)} must be defined");
                    return null;
                }

                CoScriptWrapper coScriptWrapper;
                if (_recycledCoScriptWrappers.Count > 0) {
                    coScriptWrapper = _recycledCoScriptWrappers[_recycledCoScriptWrappers.Count - 1];
                    _recycledCoScriptWrappers.RemoveAt(_recycledCoScriptWrappers.Count - 1);

                    coScriptWrapper._isRecycled = false;
                } else {
                    coScriptWrapper = new CoScriptWrapper();
                }

                // reset properties
                coScriptWrapper.Enumerator = null;
                coScriptWrapper.Method = Method.None;
                coScriptWrapper.TimeAtMoveNext = 0;
                coScriptWrapper.FrameCountAtMoveNext = 0;
                coScriptWrapper.SubroutineStarted = false;

                coScriptWrapper.CoScript = coScript;
                return coScriptWrapper;
            }

            private CoScriptWrapper() { }

            public ICoScript CoScript { get; private set; }
            public IEnumerator Enumerator { get; private set; }
            public Method Method { get; private set; }

            public float TimeAtMoveNext { get; set; }
            public int FrameCountAtMoveNext { get; set; }
            public bool SubroutineStarted { get; set; }

            public bool CanSkip {
                get => this.CoScript.CoScriptCanSkip &&
                    this.Method == Method.Body;
            }

            public void StartMethod(Method method) {
                if (method == Method.None) {
                    Debug.LogError($"Cannot start method {method}");
                    return;
                }

                this.StopCurrentCommand();

                this.Method = method;

                switch (this.Method) {
                case Method.Body:
                    this.Enumerator = this.CoScript.CoScriptBody();
                    break;
                case Method.Skip:
                    this.Enumerator = this.CoScript.CoScriptSkip();
                    break;
                case Method.End:
                    this.Enumerator = this.CoScript.CoScriptEnd();
                    break;
                }
            }

            public void Recycle() {
                if (_isRecycled) {
                    Debug.LogError($"Cannot recycle {nameof(CoScriptWrapper)} because it's already recycled.");
                    return;
                }

                this.StopCurrentCommand();

                this.CoScript = null;
                _isRecycled = true;
                _recycledCoScriptWrappers.Add(this);
            }

            private void StopCurrentCommand() {
                object current = this.Enumerator?.Current;
                if (current != null) {
                    Wait wait = (Wait)current;
                    if (wait.Id == Wait.WaitId.Subroutine) {
                        if (this.SubroutineStarted) {
                            wait.ParamSubroutine.Stop();
                        }
                    }
                }
            }

            private bool _isRecycled = false;

            private static List<CoScriptWrapper> _recycledCoScriptWrappers = new List<CoScriptWrapper>();
        }

        /// <summary>
        /// Iterator for <see cref="_coScriptWrappers"/>.  Is -1 when not looping over the scripts in Update().
        /// </summary>
        private static int _scriptsIt = -1;
        private static List<CoScriptWrapper> _coScriptWrappers = new List<CoScriptWrapper>();
        private static bool _skipScriptsAtUpdateEnd = false;

        #endregion
    }
}