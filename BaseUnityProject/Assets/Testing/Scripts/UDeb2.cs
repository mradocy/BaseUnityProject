using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used for debugging.  Can display messages and input commands.
/// None of this works if debug == false.
/// Press ` to toggle showing window.
/// </summary>
public static class UDeb2 {

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
    /// Post a value to the screen.  This is only for showing the value, and can't be changed in-game.
    /// </summary>
    public static void post(string propName, string value) {
        postValue(propName, Type.STRING, value, 0);
    }

    public static void post(string propName, float value) {
        postValue(propName, Type.FLOAT, "", value);
    }

    public static void post(string propName, int value) {
        postValue(propName, Type.INT, "", value);
    }

    public static void post(string propName, bool value) {
        postValue(propName, Type.BOOL, "", value ? 1 : 0);
    }

    /// <summary>
    /// Exposes a value to the screen.  This can be changed in-game.
    /// Returns the new value of the property.
    /// </summary>
    public static string expose(string propName, string value) {
        return exposeValue(propName, Type.STRING, value);
    }

    public static float expose(string propName, float value, float min, float max) {
        return exposeValue(propName, Type.FLOAT, value, min, max);
    }

    public static int expose(string propName, int value, int min, int max) {
        return Mathf.FloorToInt(exposeValue(propName, Type.INT, value, min, max));
    }

    public static bool expose(string propName, bool value) {
        return exposeValue(propName, Type.BOOL, value ? 1 : 0, 0, 1) != 0;
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
    private static Dictionary<string, Value> values = new Dictionary<string, Value>();
    
    #region Static Constructor, Setup to Receive Unity messages

    static UDeb2() {
        GameObject mbGO = new GameObject();
        mb = mbGO.AddComponent<MB>();
    }
    private static MB mb = null;
    private class MB : MonoBehaviour {
        void Awake() {
            DontDestroyOnLoad(gameObject);
        }
        void Update() {
            UDeb2.Update();
        }
        void OnGUI() {
            UDeb2.OnGUI();
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

        windowRect = GUILayout.Window(13579, windowRect, windowFunction, "UDeb2 (press ` to show / hide)", GUILayout.MaxWidth(WINDOW_WIDTH));
    }

    #endregion

    /// <summary>
    /// A window that displays the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    private static void windowFunction(int windowID) {

        // printing properties
        GUILayout.BeginVertical();


        //GUILayout.BeginHorizontal();
        //GUILayout.Label("test prop bool", GUILayout.Width(LABEL_WIDTH));
        //testPropBool = GUILayout.Toggle(testPropBool, "");
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("test label", GUILayout.Width(LABEL_WIDTH));
        //GUILayout.Label("test label val");
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("test prop str", GUILayout.Width(LABEL_WIDTH));
        //testPropStr = GUILayout.TextField(testPropStr);
        //GUILayout.EndHorizontal();

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
        GUI.DragWindow(titleBarRect);
    }

    public static bool windowVisible { get; private set; }

    private const int WINDOW_MARGIN = 20;
    private const int WINDOW_WIDTH = 250;
    private const int WINDOW_HEIGHT = 500;
    private const int LABEL_WIDTH = 100;
    private const int EXPOSED_NUM_WIDTH = 45;
    private static Rect windowRect = new Rect(WINDOW_MARGIN, WINDOW_MARGIN, WINDOW_WIDTH, WINDOW_HEIGHT);
    private static Rect titleBarRect = new Rect(0, 0, 10000, 20);
    

}
