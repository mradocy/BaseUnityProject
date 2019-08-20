using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.Unity {

    /// <summary>
    /// Used for debugging.  Can display messages and input commands.
    /// None of this works if IsEnabled == false.
    /// Press ` to toggle showing window.
    /// </summary>
    public static class UDeb {

        /// <summary>
        /// When false, all calls to UDeb do nothing.
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
        /// If the windows are currently visible (press `)
        /// </summary>
        public static bool IsWindowVisible { get; private set; }

        #region Posting and Exposing Values

        /// <summary>
        /// Post a string value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, string value) {
            PostValue(propName, Type.String, value, 0);
        }
        /// <summary>
        /// Post a float value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, float value) {
            PostValue(propName, Type.Float, "", value);
        }
        /// <summary>
        /// Post an int value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, int value) {
            PostValue(propName, Type.Int, "", value);
        }
        /// <summary>
        /// Post a bool value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, bool value) {
            PostValue(propName, Type.Bool, "", value ? 1 : 0);
        }
        /// <summary>
        /// Post an object value to the UDeb screen (value.ToString() is called).  This is only for showing the value, and can't be changed in-game.
        /// </summary>
        public static void Post(string propName, object value) {
            PostValue(propName, Type.String, value == null ? "null" : value.ToString(), 0);
        }

        /// <summary>
        /// Exposes a string value to the screen.  This can be changed in-game.
        /// Returns the new value of the property.
        /// </summary>
        public static string Expose(string propName, string value) {
            return ExposeValue(propName, Type.String, value);
        }
        /// <summary>
        /// Exposes a float value to the screen.  This can be changed in-game with a slider.
        /// Returns the new value of the property.
        /// </summary>
        /// <param name="min">The min value of the slider.</param>
        /// <param name="max">The max value of the slider.</param>
        public static float Expose(string propName, float value, float min, float max) {
            return ExposeValue(propName, Type.Float, value, min, max);
        }
        /// <summary>
        /// Exposes an int value to the screen.  This can be changed in-game with a slider.
        /// Returns the new value of the property.
        /// </summary>
        /// <param name="min">The min value of the slider.</param>
        /// <param name="max">The max value of the slider.</param>
        public static int Expose(string propName, int value, int min, int max) {
            return Mathf.FloorToInt(ExposeValue(propName, Type.Int, value, min, max));
        }
        /// <summary>
        /// Exposes a bool value to the screen.  This can be changed in-game.
        /// Returns the new value of the property.
        /// </summary>
        public static bool Expose(string propName, bool value) {
            return ExposeValue(propName, Type.Bool, value ? 1 : 0, 0, 1) != 0;
        }

        #endregion

        #region Register Functions

        public delegate void Function0Args();
        public delegate void Function1Arg(string arg0);
        public delegate void Function2Args(string arg0, string arg1);

        /// <summary>
        /// Registers a function with no arguments.  Functions should only be registered once.
        /// </summary>
        /// <param name="function">The function to be called when the corresponding button is pressed.</param>
        public static void RegisterFunction(string name, Function0Args function) {
            if (function == null) {
                Debug.LogError($"Cannot register null function (name: \"{name}\")");
                return;
            }
            if (_functions.ContainsKey(name)) {
                Debug.LogError($"Function with name \"{name}\" already exists.  Function not registered.");
                return;
            }
            FunctionContainer container = new FunctionContainer();
            container.Function0 = function;
            _functions[name] = container;
        }
        /// <summary>
        /// Registers a function with 1 string argument.  Functions should only be registered once.
        /// </summary>
        /// <param name="function">The function to be called when the corresponding button is pressed.</param>
        public static void RegisterFunction(string name, Function1Arg function, string defaultArg = "") {
            if (function == null) {
                Debug.LogError("Cannot register null function (name: " + name + ")");
                return;
            }
            if (_functions.ContainsKey(name)) {
                Debug.LogError("Function with name " + name + " already exists.  Function not registered.");
                return;
            }
            FunctionContainer container = new FunctionContainer();
            container.Function1 = function;
            container.Args.Add(defaultArg);
            _functions[name] = container;
        }
        /// <summary>
        /// Registers a function with 2 string arguments.  Functions should only be registered once.
        /// </summary>
        /// <param name="function">The function to be called when the corresponding button is pressed.</param>
        public static void RegisterFunction(string name, Function2Args function, string defaultArg0 = "", string defaultArg1 = "") {
            if (function == null) {
                Debug.LogError("Cannot register null function (name: " + name + ")");
                return;
            }
            if (_functions.ContainsKey(name)) {
                Debug.LogError("Function with name " + name + " already exists.  Function not registered.");
                return;
            }
            FunctionContainer container = new FunctionContainer();
            container.Function2 = function;
            container.Args.Add(defaultArg0);
            container.Args.Add(defaultArg1);
            _functions[name] = container;
        }

        /// <summary>
        /// Gets if the function with the given name is already registered.
        /// </summary>
        public static bool IsFunctionRegistered(string name) {
            return _functions.ContainsKey(name);
        }

        /// <summary>
        /// Unregisters the function with the given name.
        /// </summary>
        public static void UnregisterFunction(string name) {
            if (!_functions.Remove(name)) {
                Debug.LogError("Function " + name + " not removed because it wasn't registered");
            }
        }

        #endregion

        #region Keyboard Input

        /// <summary>
        /// Gets if the given key was pressed this frame.  Returns false if debug == false.
        /// </summary>
        public static bool IsKeyPressed(KeyCode keyCode) {
            if (!IsEnabled)
                return false;
            return Input.GetKeyDown(keyCode);
        }
        /// <summary>
        /// Gets if the given key is being held this frame.  Returns false if debug == false.
        /// </summary>
        public static bool IsKeyHeld(KeyCode keyCode) {
            if (!IsEnabled)
                return false;
            return Input.GetKey(keyCode);
        }

        /// <summary>
        /// Gets if the 1 key was pressed this frame.  Returns false if debug == false.
        /// </summary>
        public static bool Num1Pressed {
            get {
                return IsKeyPressed(KeyCode.Alpha1);
            }
        }
        /// <summary>
        /// Gets if the 1 key is being held this frame.  Returns false if debug == false.
        /// </summary>
        public static bool Num1Held {
            get {
                return IsKeyHeld(KeyCode.Alpha1);
            }
        }
        /// <summary>
        /// Gets if the 2 key was pressed this frame.  Returns false if debug == false.
        /// </summary>
        public static bool Num2Pressed {
            get {
                return IsKeyPressed(KeyCode.Alpha2);
            }
        }
        /// <summary>
        /// Gets if the 2 key is being held this frame.  Returns false if debug == false.
        /// </summary>
        public static bool Num2Held {
            get {
                return IsKeyHeld(KeyCode.Alpha2);
            }
        }
        /// <summary>
        /// Gets if the 3 key was pressed this frame.  Returns false if debug == false.
        /// </summary>
        public static bool Num3Pressed {
            get {
                return IsKeyPressed(KeyCode.Alpha3);
            }
        }
        /// <summary>
        /// Gets if the 3 key is being held this frame.  Returns false if debug == false.
        /// </summary>
        public static bool Num3Held {
            get {
                return IsKeyHeld(KeyCode.Alpha3);
            }
        }
        /// <summary>
        /// Gets if the 4 key was pressed this frame.  Returns false if debug == false.
        /// </summary>
        public static bool Num4Pressed {
            get {
                return IsKeyPressed(KeyCode.Alpha4);
            }
        }
        /// <summary>
        /// Gets if the 4 key is being held this frame.  Returns false if debug == false.
        /// </summary>
        public static bool Num4Held {
            get {
                return IsKeyHeld(KeyCode.Alpha4);
            }
        }

        #endregion

        #region Setup to Receive Unity messages

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod() {
            // Remove dependency from Initialization, and thus all Core.Unity files
            //Initialization.CallOnInitialize(Initialize);

            Initialize();
        }

        private static void Initialize() {
            if (!IsEnabled)
                return;

            GameObject mbGO = new GameObject("UDeb");
            _mb = mbGO.AddComponent<MB>();

            // default functions here?
        }

        private static MB _mb = null;
        private class MB : MonoBehaviour {
            private void Awake() {
                DontDestroyOnLoad(gameObject);
            }
            private void Update() {
                UDeb.Update();
            }
            private void OnGUI() {
                UDeb.OnGUI();
            }
            private void OnDestroy() {
                if (_mb == this)
                    _mb = null;
            }
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

            _propertyWindowRect = GUILayout.Window(123400, _propertyWindowRect, PropertyWindowFunction, "Properties", GUILayout.MaxWidth(WindowWidth));
            _functionWindowRect = GUILayout.Window(123401, _functionWindowRect, FunctionWindowFunction, "Functions", GUILayout.MaxWidth(WindowWidth));
        }

        #endregion

        #region Private Posting and Exposing Values

        private const int WindowMargin = 20;
        private const int WindowWidth = 300;
        private const int WindowHeight = 100;
        private const int LabelWidth = 100;
        private const int ExposedNumWidth = 45;
        private static Rect _propertyWindowRect = new Rect(WindowMargin, WindowMargin, WindowWidth, WindowHeight);
        private static Rect _propertyTitleBarRect = new Rect(0, 0, 10000, 20);

        private enum Type {
            None,
            String,
            Float,
            Int,
            Bool
        }

        /// <summary>
        /// Value can be changed, so shouldn't be a struct
        /// </summary>
        private class Value {
            public Type Type;
            public bool Editable;
            public bool Changed;
            public string StrValue;
            public float NumValue;
            public float NumMin;
            public float NumMax;
        }

        private static void PostValue(string propName, Type type, string strValue, float numValue) {
            if (!IsEnabled) return;
            Value v;
            // create value if not created yet
            if (!_values.TryGetValue(propName, out v)) {
                v = new Value();
                _values[propName] = v;
            }
            // update value
            v.Type = type;
            v.StrValue = strValue;
            v.NumValue = numValue;
            v.Editable = false;
            v.Changed = false;
        }

        private static string ExposeValue(string propName, Type type, string strValue) {
            if (!IsEnabled) return strValue;
            Value v;
            // create value if not created yet
            if (!_values.TryGetValue(propName, out v)) {
                v = new Value();
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

        private static float ExposeValue(string propName, Type type, float numValue, float numMin, float numMax) {
            if (!IsEnabled) return numValue;

            // bounds checking
            if (numMin > numMax) {
                Debug.LogError("Cannot expose " + propName + ", min: " + numMin + " max: " + numMax);
                return numValue;
            }
            if (numValue < numMin || numValue > numMax) {
                Debug.LogError("Cannot expose " + propName + ", value: " + numValue + " min: " + numMin + " max: " + numMax);
                return numValue;
            }

            Value v;
            // create value if not created yet
            if (!_values.TryGetValue(propName, out v)) {
                v = new Value();
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
            if (type == Type.Int) {
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
        /// A window that displays the posted and exposed properties.
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        private static void PropertyWindowFunction(int windowID) {

            // printing properties
            GUILayout.BeginVertical();

            foreach (string propName in _values.Keys) {

                Value value = _values[propName];

                GUILayout.BeginHorizontal();
                GUILayout.Label(propName + ":", GUILayout.Width(LabelWidth));

                switch (value.Type) {
                case Type.String:
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
                case Type.Float:
                    if (value.Editable) {
                        GUILayout.Label(value.NumValue.ToString("0.00"), GUILayout.Width(ExposedNumWidth));
                        float outVal = GUILayout.HorizontalSlider(value.NumValue, value.NumMin, value.NumMax);
                        if (value.NumValue != outVal) {
                            value.NumValue = outVal;
                            value.Changed = true;
                        }
                    } else {
                        GUILayout.Label(value.NumValue.ToString("0.00"), GUILayout.Width(ExposedNumWidth));
                    }
                    break;
                case Type.Int:
                    if (value.Editable) {
                        GUILayout.Label("" + Mathf.FloorToInt(value.NumValue), GUILayout.Width(ExposedNumWidth));
                        float outVal = GUILayout.HorizontalSlider(value.NumValue, value.NumMin, value.NumMax);
                        if (Mathf.Floor(value.NumValue) != Mathf.Floor(outVal)) {
                            value.Changed = true;
                        }
                        value.NumValue = outVal;
                    } else {
                        GUILayout.Label("" + Mathf.FloorToInt(value.NumValue), GUILayout.Width(ExposedNumWidth));
                    }
                    break;
                case Type.Bool:
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

        private static Dictionary<string, Value> _values = new Dictionary<string, Value>();

        #endregion

        #region Private Registering Functions

        private const int FUNCTION_WINDOW_MARGIN = 20;
        private const int FUNCTION_WINDOW_WIDTH = 300;
        private const int FUNCTION_WINDOW_HEIGHT = 100;
        private static Rect _functionWindowRect = new Rect(WindowMargin + WindowWidth + FUNCTION_WINDOW_MARGIN, FUNCTION_WINDOW_MARGIN, FUNCTION_WINDOW_WIDTH, FUNCTION_WINDOW_HEIGHT);
        private static Rect _functionTitleBarRect = new Rect(0, 0, 10000, 20);

        /// <summary>
        /// A window that displays buttons that calls functions
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        private static void FunctionWindowFunction(int windowID) {

            // printing functions

            GUILayout.BeginVertical();

            foreach (string name in _functions.Keys) {

                FunctionContainer container = _functions[name];

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(name)) {
                    // call function
                    switch (container.Args.Count) {
                    case 0:
                        container.Function0();
                        break;
                    case 1:
                        container.Function1(container.Args[0]);
                        break;
                    case 2:
                        container.Function2(container.Args[0], container.Args[1]);
                        break;
                    }
                }
                // argument input
                for (int i = 0; i < container.Args.Count; i++) {
                    container.Args[i] = GUILayout.TextField(container.Args[i]);
                }

                GUILayout.EndHorizontal();

            }

            GUILayout.EndVertical();


            // Allow the window to be dragged by its title bar.
            GUI.DragWindow(_functionTitleBarRect);
        }

        private class FunctionContainer {
            public List<string> Args = new List<string>();
            public Function0Args Function0;
            public Function1Arg Function1;
            public Function2Args Function2;
        }
        private static Dictionary<string, FunctionContainer> _functions = new Dictionary<string, FunctionContainer>();

        #endregion

    }
}