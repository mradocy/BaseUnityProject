using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Used for debugging.  Can display messages and input commands.
/// None of this works if debug == false.
/// Press ` to toggle showing window.
/// </summary>
public class UDeb {

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
    
    #region Posting Messages

    /// <summary>
    /// Posts a message to a line of the window.
    /// </summary>
    /// <param name="line">Number number.</param>
    /// <param name="message">Message to print to the line.</param>
    /// <param name="traceStack">If the function name of the caller should also be printed.</param>
    public static void post(int line, object message, bool traceStack = true) {
        string str = "";
        if (traceStack) {
            // ideally filename and line number would be better, but this info can't be extracted in Unity
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame callerFrame = stackTrace.GetFrame(1);
            if (callerFrame != null) {
                string className = callerFrame.GetMethod().DeclaringType.Name;
                string methodName = callerFrame.GetMethod().Name;
                str = className + "." + methodName + "() - " + (message == null ? "null" : message.ToString());
            }
        }

        postFunc(line, str);
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

    #region Commands (inputted in the window)

    /// <summary>
    /// Event sent whenever user inputs a line into the text field.  Function in the format 'void funcName(string[] args)'
    /// </summary>
    public static event CommandInterpretFunction commandEvent;

    /// <summary>
    /// Function signature for interpreting commands inputted in the text field.
    /// </summary>
    /// <param name="args">Array of command-line-like arguments inputted by the user.</param>
    public delegate void CommandInterpretFunction(string[] args);

    #endregion

    #region Window Properties

    public static bool windowVisible { get; private set; }

    private const int WINDOW_MARGIN = 20;
    private const int WINDOW_WIDTH = 500;
    private const int WINDOW_HEIGHT = 300;
    private static Rect windowRect = new Rect(WINDOW_MARGIN, WINDOW_MARGIN, WINDOW_WIDTH, WINDOW_HEIGHT);
    private static Rect titleBarRect = new Rect(0, 0, 10000, 20);

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

        // handling command input
        while (true) {
            int index = commandStr.IndexOf("\n");
            if (index == -1)
                break;

            // send line to be interpreted
            string line = commandStr.Substring(0, index).Trim();
            if (line != "") {
                if (commandEvent != null) {
                    commandEvent(splitCommandLine(line));
                }
            }

            // trim line out
            commandStr = commandStr.Substring(index + 1);
        }

    }

    private static void OnGUI() {
        if (!enabled)
            return;
        if (!windowVisible)
            return;

        windowRect = GUILayout.Window(123456, windowRect, windowFunction, "UDeb (press ` to show/hide)");
    }

    #endregion

    #region Private

    private static void postFunc(int line, string str) {
        if (!enabled)
            return;
        if (line < 0)
            return;

        while (line >= lines.Count) {
            lines.Add("");
        }
        lines[line] = str;
    }
    private static List<string> lines = new List<string>();
    
    
    /// <summary>
    /// A window that displays the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    private static void windowFunction(int windowID) {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < lines.Count; i++) {
            GUI.contentColor = Color.white;
            GUILayout.Label("[" + i + "] " + lines[i]);
        }

        GUILayout.EndScrollView();

        // adds text input
        commandStr = GUILayout.TextArea(commandStr);

        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }
    private static Vector2 scrollPosition;
    
    /// <summary>
    /// The string currently in the command text field in the window.
    /// </summary>
    private static string commandStr = "";

    /// <summary>
    /// Splits a string into command-line-like arguments.
    /// </summary>
    private static string[] splitCommandLine(string commandLine) {
        bool inQuotes = false;

        List<string> args = new List<string>();

        StringBuilder arg = new StringBuilder();

        for (int i = 0; i < commandLine.Length; i++) {
            char c = commandLine[i];

            if (inQuotes) {
                // add anything that isn't another quote
                if (c == '\"') {
                    inQuotes = false;
                    // arg complete, add and clear
                    if (arg.Length > 0) {
                        args.Add(arg.ToString());
                        arg.Remove(0, arg.Length);
                    }
                } else {
                    arg.Append(c);
                }
            } else {
                // add anything that isn't white space or quotes
                if (char.IsWhiteSpace(c)) {
                    // arg complete, add and clear
                    if (arg.Length > 0) {
                        args.Add(arg.ToString());
                        arg.Remove(0, arg.Length);
                    }
                } else if (c == '\"') {
                    // arg complete, add and clear
                    if (arg.Length > 0) {
                        args.Add(arg.ToString());
                        arg.Remove(0, arg.Length);
                    }
                    // begin being in quotes
                    inQuotes = true;
                } else {
                    arg.Append(c);
                }
            }

        }
        // add last arg
        if (arg.Length > 0) {
            args.Add(arg.ToString());
            arg.Remove(0, arg.Length);
        }

        return args.ToArray();
    }

    #endregion

}
