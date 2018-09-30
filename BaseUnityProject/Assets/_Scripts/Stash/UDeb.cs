using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used for debugging.  Can display messages and input commands.
/// None of this works if debug == false.
/// Press ` to toggle showing window.
/// </summary>
public static class UDeb {

    /// <summary>
    /// When false, all calls to UDeb do nothing.
    /// </summary>
    public static bool enabled {
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
    public static bool windowVisible { get; private set; }

    #region Posting and Exposing Values

    /// <summary>
    /// Post a string value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
    /// </summary>
    public static void post(string propName, string value) {
        postValue(propName, Type.STRING, value, 0);
    }
    /// <summary>
    /// Post a float value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
    /// </summary>
    public static void post(string propName, float value) {
        postValue(propName, Type.FLOAT, "", value);
    }
    /// <summary>
    /// Post an int value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
    /// </summary>
    public static void post(string propName, int value) {
        postValue(propName, Type.INT, "", value);
    }
    /// <summary>
    /// Post a bool value to the UDeb screen.  This is only for showing the value, and can't be changed in-game.
    /// </summary>
    public static void post(string propName, bool value) {
        postValue(propName, Type.BOOL, "", value ? 1 : 0);
    }
    /// <summary>
    /// Post an object value to the UDeb screen (value.ToString() is called).  This is only for showing the value, and can't be changed in-game.
    /// </summary>
    public static void post(string propName, object value) {
        postValue(propName, Type.STRING, value == null ? "null" : value.ToString(), 0);
    }

    /// <summary>
    /// Exposes a string value to the screen.  This can be changed in-game.
    /// Returns the new value of the property.
    /// </summary>
    public static string expose(string propName, string value) {
        return exposeValue(propName, Type.STRING, value);
    }
    /// <summary>
    /// Exposes a float value to the screen.  This can be changed in-game with a slider.
    /// Returns the new value of the property.
    /// </summary>
    /// <param name="min">The min value of the slider.</param>
    /// <param name="max">The max value of the slider.</param>
    public static float expose(string propName, float value, float min, float max) {
        return exposeValue(propName, Type.FLOAT, value, min, max);
    }
    /// <summary>
    /// Exposes an int value to the screen.  This can be changed in-game with a slider.
    /// Returns the new value of the property.
    /// </summary>
    /// <param name="min">The min value of the slider.</param>
    /// <param name="max">The max value of the slider.</param>
    public static int expose(string propName, int value, int min, int max) {
        return Mathf.FloorToInt(exposeValue(propName, Type.INT, value, min, max));
    }
    /// <summary>
    /// Exposes a bool value to the screen.  This can be changed in-game.
    /// Returns the new value of the property.
    /// </summary>
    public static bool expose(string propName, bool value) {
        return exposeValue(propName, Type.BOOL, value ? 1 : 0, 0, 1) != 0;
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
    public static void registerFunction(string name, Function0Args function) {
        if (function == null) {
            Debug.LogWarning("Cannot register null function (name: " + name + ")");
            return;
        }
        if (functions.ContainsKey(name)) {
            Debug.LogWarning("Function with name " + name + " already exists.  Function not registered.");
            return;
        }
        FunctionContainer container = new FunctionContainer();
        container.function0 = function;
        functions[name] = container;
    }
    /// <summary>
    /// Registers a function with 1 string argument.  Functions should only be registered once.
    /// </summary>
    /// <param name="function">The function to be called when the corresponding button is pressed.</param>
    public static void registerFunction(string name, Function1Arg function, string defaultArg = "") {
        if (function == null) {
            Debug.LogWarning("Cannot register null function (name: " + name + ")");
            return;
        }
        if (functions.ContainsKey(name)) {
            Debug.LogWarning("Function with name " + name + " already exists.  Function not registered.");
            return;
        }
        FunctionContainer container = new FunctionContainer();
        container.function1 = function;
        container.args.Add(defaultArg);
        functions[name] = container;
    }
    /// <summary>
    /// Registers a function with 2 string arguments.  Functions should only be registered once.
    /// </summary>
    /// <param name="function">The function to be called when the corresponding button is pressed.</param>
    public static void registerFunction(string name, Function2Args function, string defaultArg0 = "", string defaultArg1 = "") {
        if (function == null) {
            Debug.LogWarning("Cannot register null function (name: " + name + ")");
            return;
        }
        if (functions.ContainsKey(name)) {
            Debug.LogWarning("Function with name " + name + " already exists.  Function not registered.");
            return;
        }
        FunctionContainer container = new FunctionContainer();
        container.function2 = function;
        container.args.Add(defaultArg0);
        container.args.Add(defaultArg1);
        functions[name] = container;
    }

    /// <summary>
    /// Gets if the function with the given name is already registered.
    /// </summary>
    public static bool isFunctionRegistered(string name) {
        return functions.ContainsKey(name);
    }

    /// <summary>
    /// Unregisters the function with the given name.
    /// </summary>
    public static void unregisterFunction(string name) {
        if (!functions.Remove(name)) {
            Debug.LogWarning("Function " + name + " not removed because it wasn't registered");
        }
    }

    #endregion
    
    #region Keyboard Input

    /// <summary>
    /// Gets if the given key was pressed this frame.  Returns false if debug == false.
    /// </summary>
    public static bool keyPressed(KeyCode keyCode) {
        if (!enabled)
            return false;
        return Input.GetKeyDown(keyCode);
    }
    /// <summary>
    /// Gets if the given key is being held this frame.  Returns false if debug == false.
    /// </summary>
    public static bool keyHeld(KeyCode keyCode) {
        if (!enabled)
            return false;
        return Input.GetKey(keyCode);
    }

    /// <summary>
    /// Gets if the 1 key was pressed this frame.  Returns false if debug == false.
    /// </summary>
    public static bool num1Pressed {
        get {
            return keyPressed(KeyCode.Alpha1);
        }
    }
    /// <summary>
    /// Gets if the 1 key is being held this frame.  Returns false if debug == false.
    /// </summary>
    public static bool num1Held {
        get {
            return keyHeld(KeyCode.Alpha1);
        }
    }
    /// <summary>
    /// Gets if the 2 key was pressed this frame.  Returns false if debug == false.
    /// </summary>
    public static bool num2Pressed {
        get {
            return keyPressed(KeyCode.Alpha2);
        }
    }
    /// <summary>
    /// Gets if the 2 key is being held this frame.  Returns false if debug == false.
    /// </summary>
    public static bool num2Held {
        get {
            return keyHeld(KeyCode.Alpha2);
        }
    }
    /// <summary>
    /// Gets if the 3 key was pressed this frame.  Returns false if debug == false.
    /// </summary>
    public static bool num3Pressed {
        get {
            return keyPressed(KeyCode.Alpha3);
        }
    }
    /// <summary>
    /// Gets if the 3 key is being held this frame.  Returns false if debug == false.
    /// </summary>
    public static bool num3Held {
        get {
            return keyHeld(KeyCode.Alpha3);
        }
    }
    /// <summary>
    /// Gets if the 4 key was pressed this frame.  Returns false if debug == false.
    /// </summary>
    public static bool num4Pressed {
        get {
            return keyPressed(KeyCode.Alpha4);
        }
    }
    /// <summary>
    /// Gets if the 4 key is being held this frame.  Returns false if debug == false.
    /// </summary>
    public static bool num4Held {
        get {
            return keyHeld(KeyCode.Alpha4);
        }
    }

    #endregion

    #region Static Constructor, Setup to Receive Unity messages

    static UDeb() {
        GameObject mbGO = new GameObject();
        mb = mbGO.AddComponent<MB>();
    }
    private static MB mb = null;
    private class MB : MonoBehaviour {
        void Awake() {
            DontDestroyOnLoad(gameObject);
        }
        void Update() {
            UDeb.Update();
        }
        void OnGUI() {
            UDeb.OnGUI();
        }
        void OnDestroy() {
            if (mb == this)
                mb = null;
        }
    }

    #endregion

    #region Receiving Unity messages

    private static void Update() {

        if (!enabled)
            return;

        // show screen
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            windowVisible = !windowVisible;
        }

    }

    private static void OnGUI() {
        if (!enabled)
            return;
        if (!windowVisible)
            return;

        propertyWindowRect = GUILayout.Window(123400, propertyWindowRect, propertyWindowFunction, "Properties", GUILayout.MaxWidth(WINDOW_WIDTH));
        functionWindowRect = GUILayout.Window(123401, functionWindowRect, functionWindowFunction, "Functions", GUILayout.MaxWidth(WINDOW_WIDTH));
    }

    #endregion
    
    #region Private Posting and Exposing Values

    private const int WINDOW_MARGIN = 20;
    private const int WINDOW_WIDTH = 250;
    private const int WINDOW_HEIGHT = 100;
    private const int LABEL_WIDTH = 100;
    private const int EXPOSED_NUM_WIDTH = 45;
    private static Rect propertyWindowRect = new Rect(WINDOW_MARGIN, WINDOW_MARGIN, WINDOW_WIDTH, WINDOW_HEIGHT);
    private static Rect propertyTitleBarRect = new Rect(0, 0, 10000, 20);

    private enum Type {
        NONE,
        STRING,
        FLOAT,
        INT,
        BOOL
    }

    /// <summary>
    /// Value can be changed, so shouldn't be a struct
    /// </summary>
    private class Value {
        public Type type;
        public bool editable;
        public bool changed;
        public string strValue;
        public float numValue;
        public float numMin;
        public float numMax;
    }

    private static void postValue(string propName, Type type, string strValue, float numValue) {
        if (!enabled) return;
        Value v;
        // create value if not created yet
        if (!values.TryGetValue(propName, out v)) {
            v = new Value();
            values[propName] = v;
        }
        // update value
        v.type = type;
        v.strValue = strValue;
        v.numValue = numValue;
        v.editable = false;
        v.changed = false;
    }

    private static string exposeValue(string propName, Type type, string strValue) {
        if (!enabled) return strValue;
        Value v;
        // create value if not created yet
        if (!values.TryGetValue(propName, out v)) {
            v = new Value();
            values[propName] = v;
            v.changed = false;
        }
        // if value changed in GUI, then return the changed value and don't set anything
        if (v.changed) {
            v.changed = false;
            return v.strValue;
        }
        // value wasn't changed in GUI, so set to given value
        v.type = type;
        v.strValue = strValue;
        v.editable = true;
        return strValue;
    }

    private static float exposeValue(string propName, Type type, float numValue, float numMin, float numMax) {
        if (!enabled) return numValue;

        // bounds checking
        if (numMin > numMax) {
            Debug.LogWarning("Cannot expose " + propName + ", min: " + numMin + " max: " + numMax);
            return numValue;
        }
        if (numValue < numMin || numValue > numMax) {
            Debug.LogWarning("Cannot expose " + propName + ", value: " + numValue + " min: " + numMin + " max: " + numMax);
            return numValue;
        }

        Value v;
        // create value if not created yet
        if (!values.TryGetValue(propName, out v)) {
            v = new Value();
            values[propName] = v;
            v.changed = false;
        }
        // if value changed in GUI, then return the changed value and don't set anything
        if (v.changed) {
            v.changed = false;
            return v.numValue;
        }
        // value wasn't changed in GUI, so set to given value
        v.type = type;
        if (type == Type.INT) {
            // only change if int value is different
            if (Mathf.Floor(v.numValue) != numValue) {
                v.numValue = numValue;
            }
        } else {
            v.numValue = numValue;
        }
        v.numMin = numMin;
        v.numMax = numMax;
        v.editable = true;
        return v.numValue;
    }
    
    /// <summary>
    /// A window that displays the posted and exposed properties.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    private static void propertyWindowFunction(int windowID) {

        // printing properties
        GUILayout.BeginVertical();

        foreach (string propName in values.Keys) {

            Value value = values[propName];

            GUILayout.BeginHorizontal();
            GUILayout.Label(propName + ":", GUILayout.Width(LABEL_WIDTH));

            switch (value.type) {
            case Type.STRING:
                if (value.editable) {
                    string outVal = GUILayout.TextField(value.strValue);
                    if (outVal != value.strValue) {
                        value.strValue = outVal;
                        value.changed = true;
                    }
                } else {
                    GUILayout.Label(value.strValue);
                }
                break;
            case Type.FLOAT:
                if (value.editable) {
                    GUILayout.Label(value.numValue.ToString("0.00"), GUILayout.Width(EXPOSED_NUM_WIDTH));
                    float outVal = GUILayout.HorizontalSlider(value.numValue, value.numMin, value.numMax);
                    if (value.numValue != outVal) {
                        value.numValue = outVal;
                        value.changed = true;
                    }
                } else {
                    GUILayout.Label(value.numValue.ToString("0.00"), GUILayout.Width(EXPOSED_NUM_WIDTH));
                }
                break;
            case Type.INT:
                if (value.editable) {
                    GUILayout.Label("" + Mathf.FloorToInt(value.numValue), GUILayout.Width(EXPOSED_NUM_WIDTH));
                    float outVal = GUILayout.HorizontalSlider(value.numValue, value.numMin, value.numMax);
                    if (Mathf.Floor(value.numValue) != Mathf.Floor(outVal)) {
                        value.changed = true;
                    }
                    value.numValue = outVal;
                } else {
                    GUILayout.Label("" + Mathf.FloorToInt(value.numValue), GUILayout.Width(EXPOSED_NUM_WIDTH));
                }
                break;
            case Type.BOOL:
                if (value.editable) {
                    bool outVal = GUILayout.Toggle(value.numValue != 0, "");
                    if ((value.numValue != 0) != outVal) {
                        value.changed = true;
                    }
                    value.numValue = outVal ? 1 : 0;
                } else {
                    GUILayout.Label(value.numValue != 0 ? "true" : "false");
                }
                break;
            }

            GUILayout.EndHorizontal();

        }

        GUILayout.EndVertical();
        
        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(propertyTitleBarRect);
    }
    
    private static Dictionary<string, Value> values = new Dictionary<string, Value>();

    #endregion

    #region Private Registering Functions

    private const int FUNCTION_WINDOW_MARGIN = 20;
    private const int FUNCTION_WINDOW_WIDTH = 250;
    private const int FUNCTION_WINDOW_HEIGHT = 100;
    private static Rect functionWindowRect = new Rect(WINDOW_MARGIN + WINDOW_WIDTH + FUNCTION_WINDOW_MARGIN, FUNCTION_WINDOW_MARGIN, FUNCTION_WINDOW_WIDTH, FUNCTION_WINDOW_HEIGHT);
    private static Rect functionTitleBarRect = new Rect(0, 0, 10000, 20);

    /// <summary>
    /// A window that displays buttons that calls functions
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    private static void functionWindowFunction(int windowID) {

        // printing functions

        GUILayout.BeginVertical();

        foreach (string name in functions.Keys) {

            FunctionContainer container = functions[name];

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(name)) {
                // call function
                switch (container.args.Count) {
                case 0:
                    container.function0();
                    break;
                case 1:
                    container.function1(container.args[0]);
                    break;
                case 2:
                    container.function2(container.args[0], container.args[1]);
                    break;
                }
            }
            // argument input
            for (int i = 0; i < container.args.Count; i++) {
                container.args[i] = GUILayout.TextField(container.args[i]);
            }

            GUILayout.EndHorizontal();

        }

        GUILayout.EndVertical();


        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(functionTitleBarRect);
    }

    private class FunctionContainer {
        public List<string> args = new List<string>();
        public Function0Args function0;
        public Function1Arg function1;
        public Function2Args function2;
    }
    private static Dictionary<string, FunctionContainer> functions = new Dictionary<string, FunctionContainer>();

    #endregion

}
