using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Unity {

    /// <summary>
    /// Used for debugging.  Can display messages and input commands.
    /// None of this works if IsEnabled == false (i.e. RELEASE is included in the scripting define symbols).
    /// Press ` to toggle showing window.
    /// </summary>
    public static class UDeb {

        /// <summary>
        /// When false, all calls to UDeb do nothing.
        /// UDeb is enabled iff RELEASE is not included in the scripting define symbols (can be set in Unity's player settings)
        /// </summary>
        public static bool IsEnabled {
            get {
#if RELEASE // note: scripting define symbols can be set in Unity's player settings.
                return false;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// Gets if the windows are currently visible (press `)
        /// </summary>
        public static bool IsWindowVisible { get; private set; }

        #region Posting and Exposing Values

        /// <summary>
        /// Post a string value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, string value) {
            PostValue(propName, PropertyType.String, value, 0);
        }
        /// <summary>
        /// Post a float value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, float value) {
            PostValue(propName, PropertyType.Float, "", value);
        }
        /// <summary>
        /// Post an int value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, int value) {
            PostValue(propName, PropertyType.Int, "", value);
        }
        /// <summary>
        /// Post a bool value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, bool value) {
            PostValue(propName, PropertyType.Bool, "", value ? 1 : 0);
        }
        /// <summary>
        /// Post an object value to the UDeb screen (value.ToString() is called).  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, object value) {
            PostValue(propName, PropertyType.String, value == null ? "null" : value.ToString(), 0);
        }

        /// <summary>
        /// Exposes a string value to the screen.  This can be changed in-game.
        /// Returns the new value of the property.
        /// </summary>
        public static string Expose(string propName, string value) {
            return ExposeValue(propName, PropertyType.String, value);
        }
        /// <summary>
        /// Exposes a float value to the screen.  This can be changed in-game with a slider.
        /// Returns the new value of the property.
        /// </summary>
        /// <param name="min">The min value of the slider.</param>
        /// <param name="max">The max value of the slider.</param>
        public static float Expose(string propName, float value, float min, float max) {
            return ExposeValue(propName, PropertyType.Float, value, min, max);
        }
        /// <summary>
        /// Exposes an int value to the screen.  This can be changed in-game with a slider.
        /// Returns the new value of the property.
        /// </summary>
        /// <param name="min">The min value of the slider.</param>
        /// <param name="max">The max value of the slider.</param>
        public static int Expose(string propName, int value, int min, int max) {
            return Mathf.FloorToInt(ExposeValue(propName, PropertyType.Int, value, min, max));
        }
        /// <summary>
        /// Exposes a long value to the screen.  This can be changed in-game with a slider.
        /// Returns the new value of the property.
        /// </summary>
        /// <param name="min">The min value of the slider.</param>
        /// <param name="max">The max value of the slider.</param>
        public static long Expose(string propName, long value, long min, long max) {
            return (long)Mathf.Floor(ExposeValue(propName, PropertyType.Long, value, min, max));
        }
        /// <summary>
        /// Exposes a bool value to the screen.  This can be changed in-game.
        /// Returns the new value of the property.
        /// </summary>
        public static bool Expose(string propName, bool value) {
            return ExposeValue(propName, PropertyType.Bool, value ? 1 : 0, 0, 1) != 0;
        }

        #endregion

        #region Registering Actions

        /// <summary>
        /// Registers an action with no arguments.
        /// </summary>
        /// <param name="action">The method to be called when the corresponding button is pressed.</param>
        public static void RegisterAction(string name, System.Action action) {
            if (!IsEnabled)
                return;
            if (action == null) {
                Debug.LogError($"Cannot register null action (name: \"{name}\")");
                return;
            }

            RegisterAction(name, new ActionContainer(action));
        }
        /// <summary>
        /// Registers an action with 1 string argument.
        /// </summary>
        /// <param name="name">Name to identify the action.</param>
        /// <param name="action">The method to be called when the corresponding button is pressed.</param>
        public static void RegisterAction(string name, System.Action<string> action) {
            if (!IsEnabled)
                return;
            if (action == null) {
                Debug.LogError($"Cannot register null action (name: \"{name}\")");
                return;
            }

            RegisterAction(name, new ActionContainerS(action, ""));
        }
        /// <summary>
        /// Registers an action with 1 bool argument.
        /// </summary>
        /// <param name="name">Name to identify the action.</param>
        /// <param name="action">The method to be called when the corresponding button is pressed.</param>
        public static void RegisterAction(string name, System.Action<bool> action) {
            RegisterAction(name, false, action);
        }
        /// <summary>
        /// Registers an action with 1 bool argument.
        /// </summary>
        /// <param name="name">Name to identify the action.</param>
        /// <param name="defaultArg">Default argument to the action.</param>
        /// <param name="action">The method to be called when the corresponding button is pressed.</param>
        public static void RegisterAction(string name, bool defaultArg, System.Action<bool> action) {
            if (!IsEnabled)
                return;
            if (action == null) {
                Debug.LogError($"Cannot register null action (name: \"{name}\")");
                return;
            }

            RegisterAction(name, new ActionContainerB(action, defaultArg));
        }
        /// <summary>
        /// Registers an action with 1 int argument.
        /// </summary>
        /// <param name="name">Name to identify the action.</param>
        /// <param name="action">The method to be called when the corresponding button is pressed.</param>
        public static void RegisterAction(string name, System.Action<int> action) {
            RegisterAction(name, 0, action);
        }
        /// <summary>
        /// Registers an action with 1 int argument.
        /// </summary>
        /// <param name="name">Name to identify the action.</param>
        /// <param name="defaultArg">Default argument to the action.</param>
        /// <param name="action">The method to be called when the corresponding button is pressed.</param>
        public static void RegisterAction(string name, int defaultArg, System.Action<int> action) {
            if (!IsEnabled)
                return;
            if (action == null) {
                Debug.LogError($"Cannot register null action (name: \"{name}\")");
                return;
            }

            RegisterAction(name, new ActionContainerI(action, defaultArg));
        }
        /// <summary>
        /// Registers an action with 1 float argument.
        /// </summary>
        /// <param name="name">Name to identify the action.</param>
        /// <param name="action">The method to be called when the corresponding button is pressed.</param>
        public static void RegisterAction(string name, System.Action<float> action) {
            RegisterAction(name, 0, action);
        }
        /// <summary>
        /// Registers an action with 1 float argument.
        /// </summary>
        /// <param name="name">Name to identify the action.</param>
        /// <param name="defaultArg">Default argument to the action.</param>
        /// <param name="action">The method to be called when the corresponding button is pressed.</param>
        public static void RegisterAction(string name, float defaultArg, System.Action<float> action) {
            if (!IsEnabled)
                return;
            if (action == null) {
                Debug.LogError($"Cannot register null action (name: \"{name}\")");
                return;
            }

            RegisterAction(name, new ActionContainerF(action, defaultArg));
        }

        /// <summary>
        /// Gets if an action with the given name is registered.
        /// </summary>
        /// <param name="name">Name of the action.</param>
        /// <returns>Is registered.</returns>
        public static bool IsActionRegistered(string name) {
            return _actions.ContainsKey(name);
        }

        /// <summary>
        /// Unregisters the action with the given name.
        /// </summary>
        public static void UnregisterAction(string name) {
            if (!IsEnabled)
                return;

            if (!_actions.Remove(name)) {
                Debug.LogWarning($"Action \"{name}\" not removed because it wasn't registered");
            }
        }

        #endregion

        #region Keyboard Input

        /// <summary>
        /// Gets if the given key was pressed this frame.  Returns false if <see cref="IsEnabled"/> is false.
        /// </summary>
        public static bool IsKeyPressed(KeyCode keyCode) {
            if (!IsEnabled)
                return false;
            return Input.GetKeyDown(keyCode);
        }
        /// <summary>
        /// Gets if the given key is being held this frame.  Returns false if <see cref="IsEnabled"/> is false.
        /// </summary>
        public static bool IsKeyHeld(KeyCode keyCode) {
            if (!IsEnabled)
                return false;
            return Input.GetKey(keyCode);
        }

        #endregion

        #region Setup to Receive Unity messages

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            PersistantGameObject.UpdateEvent += Update;
            PersistantGameObject.OnGUIEvent += OnGUI;
        }

        #endregion

        #region Receiving Unity messages

        private static void Update() {
            if (!IsEnabled)
                return;

            // show screen
            if (Input.GetKeyDown(KeyCode.BackQuote)) {
                IsWindowVisible = !IsWindowVisible;
            }
        }

        private static void OnGUI() {
            if (!IsEnabled)
                return;
            if (!IsWindowVisible)
                return;

            _propertyWindowRect = GUILayout.Window(222200, _propertyWindowRect, PropertyWindowFunction, "Properties", GUILayout.MaxWidth(_windowWidth));
            _actionWindowRect = GUILayout.Window(222201, _actionWindowRect, ActionWindowFunction, "Actions", GUILayout.MaxWidth(_windowWidth));
        }

        #endregion

        #region Private Window Dimension Consts

        private const int _windowMargin = 20;
        private const int _windowWidth = 300;
        private const int _windowHeight = 100;
        private const int _labelWidth = 100;
        private const int _exposedNumWidth = 45;
        private const int _actionWindowMargin = 20;
        private const int _actionWindowWidth = 300;
        private const int _actionWindowHeight = 100;

        #endregion

        #region Private Posting and Exposing Values

        private static Rect _propertyWindowRect = new Rect(_windowMargin, _windowMargin, _windowWidth, _windowHeight);
        private static readonly Rect _propertyTitleBarRect = new Rect(0, 0, 10000, 20);

        private enum PropertyType {
            None,
            String,
            Float,
            Int,
            Long,
            Bool
        }

        /// <summary>
        /// Value can be changed, so shouldn't be a struct
        /// </summary>
        private class PropertyValue {
            public PropertyType Type;
            public bool Editable;
            public bool Changed;
            public string StrValue;
            public float NumValue;
            public float NumMin;
            public float NumMax;
        }

        private static void PostValue(string propName, PropertyType type, string strValue, float numValue) {
            if (!IsEnabled)
                return;
            PropertyValue v;
            // create value if not created yet
            if (!_values.TryGetValue(propName, out v)) {
                v = new PropertyValue();
                _values[propName] = v;
            }
            // update value
            v.Type = type;
            v.StrValue = strValue;
            v.NumValue = numValue;
            v.Editable = false;
            v.Changed = false;
        }

        private static string ExposeValue(string propName, PropertyType type, string strValue) {
            if (!IsEnabled)
                return strValue;
            PropertyValue v;
            // create value if not created yet
            if (!_values.TryGetValue(propName, out v)) {
                v = new PropertyValue();
                _values[propName] = v;
                v.Changed = false;
            }
            // if value changed in GUI, then return the changed value and don't set anything
            if (v.Changed) {
                v.Changed = false;
                return v.StrValue;
            }
            // value wasn't changed in GUI, so set to given value
            v.Type = type;
            v.StrValue = strValue;
            v.Editable = true;
            return strValue;
        }

        private static float ExposeValue(string propName, PropertyType type, float numValue, float numMin, float numMax) {
            if (!IsEnabled)
                return numValue;

            // bounds checking
            if (numMin > numMax) {
                Debug.LogError("Cannot expose " + propName + ", min: " + numMin + " max: " + numMax);
                return numValue;
            }
            if (numValue < numMin || numValue > numMax) {
                Debug.LogError("Cannot expose " + propName + ", value: " + numValue + " min: " + numMin + " max: " + numMax);
                return numValue;
            }

            PropertyValue v;
            // create value if not created yet
            if (!_values.TryGetValue(propName, out v)) {
                v = new PropertyValue();
                _values[propName] = v;
                v.Changed = false;
            }
            // if value changed in GUI, then return the changed value and don't set anything
            if (v.Changed) {
                v.Changed = false;
                return v.NumValue;
            }
            // value wasn't changed in GUI, so set to given value
            v.Type = type;
            if (type == PropertyType.Int || type == PropertyType.Long) {
                // only change if int value is different
                if (Mathf.Floor(v.NumValue) != numValue) {
                    v.NumValue = numValue;
                }
            } else {
                v.NumValue = numValue;
            }
            v.NumMin = numMin;
            v.NumMax = numMax;
            v.Editable = true;
            return v.NumValue;
        }

        /// <summary>
        /// Draws a window that displays the posted and exposed properties.
        /// </summary>
        private static void PropertyWindowFunction(int windowId) {

            // printing properties
            GUILayout.BeginVertical();

            foreach (string propName in _values.Keys) {

                PropertyValue value = _values[propName];

                GUILayout.BeginHorizontal();
                GUILayout.Label(propName + ":", GUILayout.Width(_labelWidth));

                switch (value.Type) {
                case PropertyType.String:
                    if (value.Editable) {
                        string outVal = GUILayout.TextField(value.StrValue);
                        if (outVal != value.StrValue) {
                            value.StrValue = outVal;
                            value.Changed = true;
                        }
                    } else {
                        GUILayout.Label(value.StrValue);
                    }
                    break;
                case PropertyType.Float:
                    if (value.Editable) {
                        GUILayout.Label(value.NumValue.ToString("0.00"), GUILayout.Width(_exposedNumWidth));
                        float outVal = GUILayout.HorizontalSlider(value.NumValue, value.NumMin, value.NumMax);
                        if (value.NumValue != outVal) {
                            value.NumValue = outVal;
                            value.Changed = true;
                        }
                    } else {
                        GUILayout.Label(value.NumValue.ToString("0.00"), GUILayout.Width(_exposedNumWidth));
                    }
                    break;
                case PropertyType.Int:
                    if (value.Editable) {
                        GUILayout.Label("" + Mathf.FloorToInt(value.NumValue), GUILayout.Width(_exposedNumWidth));
                        float outVal = GUILayout.HorizontalSlider(value.NumValue, value.NumMin, value.NumMax);
                        if (Mathf.Floor(value.NumValue) != Mathf.Floor(outVal)) {
                            value.Changed = true;
                        }
                        value.NumValue = outVal;
                    } else {
                        GUILayout.Label("" + Mathf.FloorToInt(value.NumValue), GUILayout.Width(_exposedNumWidth));
                    }
                    break;
                case PropertyType.Long:
                    if (value.Editable) {
                        GUILayout.Label("" + (long)Mathf.Floor(value.NumValue), GUILayout.Width(_exposedNumWidth));
                        float outVal = GUILayout.HorizontalSlider(value.NumValue, value.NumMin, value.NumMax);
                        if (Mathf.Floor(value.NumValue) != Mathf.Floor(outVal)) {
                            value.Changed = true;
                        }
                        value.NumValue = outVal;
                    } else {
                        GUILayout.Label("" + (long)Mathf.Floor(value.NumValue), GUILayout.Width(_exposedNumWidth));
                    }
                    break;
                case PropertyType.Bool:
                    if (value.Editable) {
                        bool outVal = GUILayout.Toggle(value.NumValue != 0, "");
                        if ((value.NumValue != 0) != outVal) {
                            value.Changed = true;
                        }
                        value.NumValue = outVal ? 1 : 0;
                    } else {
                        GUILayout.Label(value.NumValue != 0 ? "true" : "false");
                    }
                    break;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            // Allow the window to be dragged by its title bar.
            GUI.DragWindow(_propertyTitleBarRect);
        }

        private static Dictionary<string, PropertyValue> _values = new Dictionary<string, PropertyValue>();

        #endregion

        #region Private Registering Actions

        private static Rect _actionWindowRect = new Rect(_windowMargin + _windowWidth + _actionWindowMargin, _actionWindowMargin, _actionWindowWidth, _actionWindowHeight);
        private static readonly Rect _actionTitleBarRect = new Rect(0, 0, 10000, 20);

        private abstract class ActionContainerBase {
            public ActionContainerBase(int argCount) {
                Args = new string[argCount];
            }
            public string[] Args = null;
            public abstract void InvokeAction();
        }

        private class ActionContainer : ActionContainerBase {
            public ActionContainer(System.Action action) : base(0) {
                _action = action;
            }
            public override void InvokeAction() {
                _action.Invoke();
            }
            private System.Action _action;
        }

        private class ActionContainerS : ActionContainerBase {
            public ActionContainerS(System.Action<string> action, string defaultArg) : base(1) {
                _action = action;
                Args[0] = defaultArg;
            }
            public override void InvokeAction() {
                _action.Invoke(Args[0]);
            }
            private System.Action<string> _action;
        }

        private class ActionContainerB : ActionContainerBase {
            public ActionContainerB(System.Action<bool> action, bool defaultArg) : base(1) {
                _action = action;
                Args[0] = defaultArg.ToString();
            }
            public override void InvokeAction() {
                if (!bool.TryParse(Args[0], out bool b)) {
                    b = false;
                }
                _action.Invoke(b);
            }
            private System.Action<bool> _action;
        }

        private class ActionContainerI : ActionContainerBase {
            public ActionContainerI(System.Action<int> action, int defaultArg) : base(1) {
                _action = action;
                Args[0] = defaultArg.ToString();
            }
            public override void InvokeAction() {
                if (!int.TryParse(Args[0], out int i)) {
                    i = 0;
                }
                _action.Invoke(i);
            }
            private System.Action<int> _action;
        }

        private class ActionContainerF : ActionContainerBase {
            public ActionContainerF(System.Action<float> action, float defaultArg) : base(1) {
                _action = action;
                Args[0] = defaultArg.ToString();
            }
            public override void InvokeAction() {
                if (!float.TryParse(Args[0], out float f)) {
                    f = 0;
                }
                _action.Invoke(f);
            }
            private System.Action<float> _action;
        }

        private static void RegisterAction(string name, ActionContainerBase actionContainer) {
            if (IsActionRegistered(name)) {
                Debug.LogWarning($"Action with name \"{name}\" has already been registered.  Previous action will be replaced with new action");
            }
            _actions[name] = actionContainer;
        }

        /// <summary>
        /// Draws the window that displays buttons that calls actions
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        private static void ActionWindowFunction(int windowID) {

            GUILayout.BeginVertical();

            foreach (KeyValuePair<string, ActionContainerBase> kvp in _actions) {
                ActionContainerBase actionContainer = kvp.Value;

                GUILayout.BeginHorizontal();

                // draw button
                if (GUILayout.Button(kvp.Key)) {
                    actionContainer.InvokeAction();
                }
                // draw arguments
                for (int i = 0; i < actionContainer.Args.Length; i++) {
                    actionContainer.Args[i] = GUILayout.TextField(actionContainer.Args[i]);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            // Allow the window to be dragged by its title bar.
            GUI.DragWindow(_actionTitleBarRect);
        }

        private static Dictionary<string, ActionContainerBase> _actions = new Dictionary<string, ActionContainerBase>();

        #endregion
    }
}