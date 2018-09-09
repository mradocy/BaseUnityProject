using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;

/// <summary>
/// Records user actions and writes them to a file.
/// File is written to a folder called ActionLogs, in the root folder of the app.
/// </summary>
public static class ActionLog {

    /// <summary>
    /// Path to place the logs.
    /// </summary>
    public static string FILE_PATH {
        get {
            return Path.Combine(Application.dataPath, Path.Combine("..", "ActionLogs"));
        }
    }

    public static string HEADER {
        get {
            return "** " + UnityEditor.PlayerSettings.productName + " - " + logStartDate.ToString() + " **";
        }
    }

    /// <summary>
    /// The filename for the action log created at the given time.
    /// </summary>
    public static string fileName(DateTime dateTime) {
        StringBuilder sb = new StringBuilder();
        //sb.Append("log_");
        sb.Append(dateTime.Year.ToString("0000") + "-");
        sb.Append(dateTime.Month.ToString("00") + "-");
        sb.Append(dateTime.Day.ToString("00"));
        sb.Append("_");
        sb.Append(dateTime.Hour.ToString("00") + "-");
        sb.Append(dateTime.Minute.ToString("00") + "-");
        sb.Append(dateTime.Second.ToString("00"));
        sb.Append(".log");
        return sb.ToString();
    }

    /// <summary>
    /// When false, all calls to ActionLog do nothing.
    /// </summary>
    public static bool enabled {
        get {
            return UDeb.debug;
        }
    }

    /// <summary>
    /// If currently has an action log open to writing.
    /// </summary>
    public static bool isWritingLogOpen {
        get {
            return currentStreamWriter != null;
        }
    }

    /// <summary>
    /// DateTime of when the current log started.
    /// </summary>
    public static DateTime logStartDate {
        get; private set;
    }

    /// <summary>
    /// Realtime since startup (in seconds) when the current log started.
    /// </summary>
    public static float logStartTime {
        get; private set;
    }

    /// <summary>
    /// Starts a new action log.
    /// Ends the current log if one is already running.
    /// </summary>
    public static void startLog() {

        if (!enabled) return;

        endLog();

        if (!Directory.Exists(FILE_PATH)) {
            Directory.CreateDirectory(FILE_PATH);
        }

        logStartDate = DateTime.Now;
        logStartTime = Time.realtimeSinceStartup;
        currentFileStream = new FileStream(Path.Combine(FILE_PATH, fileName(logStartDate)), FileMode.CreateNew);
        currentStreamWriter = new StreamWriter(currentFileStream);

        currentStreamWriter.WriteLine(HEADER);
        currentStreamWriter.WriteLine();
        currentStreamWriter.Flush();

        Debug.Log("Action log started - " + logStartDate.ToString());

    }

    /// <summary>
    /// Adds the given entry to the current log, prefaced by the time passed since the log started.
    /// Does nothing if not currently writing to a log.
    /// </summary>
    /// <param name="entry"></param>
    public static void logEntry(string entry) {

        if (!enabled) return;
        if (!isWritingLogOpen) return;

        float timeDiff = Time.realtimeSinceStartup - logStartTime;
        int mins = Mathf.FloorToInt(timeDiff / 60);
        int secs = Mathf.FloorToInt(timeDiff) - mins * 60;

        StringBuilder sb = new StringBuilder();
        sb.Append(mins.ToString("00") + ":" + secs.ToString("00"));
        sb.Append(" - ");
        sb.Append(entry);

        currentStreamWriter.WriteLine(sb.ToString());
        currentStreamWriter.Flush();

        Debug.Log("Action Log entry logged: " + sb.ToString());

    }

    /// <summary>
    /// Ends writing to the current log, if currently writing to a log.
    /// </summary>
    public static void endLog() {
        
        if (currentStreamWriter != null) {
            currentStreamWriter.Close();
            currentStreamWriter.Dispose();
            currentStreamWriter = null;
            currentFileStream = null;

            Debug.Log("Action Log ended.");
        }

    }

    
    private static FileStream currentFileStream = null;
    private static StreamWriter currentStreamWriter = null;

    

}
