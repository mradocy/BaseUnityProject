using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages why gameplay is paused (timeScale == 0).
/// There are many reasons why a game would be paused.  This handles the game being paused for several reasons at once
/// </summary>
public static class PauseManager {
    
    public enum Reason {
        OTHER,

        MENU,
        HIT_PAUSE,
        CUTSCENE,
        DEBUG_MODE,
        SCREENSHOT_MODE,

    }

    /// <summary>
    /// Returns if the game is paused (timeScale == 0) for any reason.
    /// </summary>
    public static bool isPaused {
        get {
            return pauseReasons.Count > 0;
        }
    }

    /// <summary>
    /// Gets if the game is paused (at least in part) for the given reason. 
    /// </summary>
    public static bool isPausedForReason(Reason reason) {
        return pauseReasons.Contains(reason);
    }

    /// <summary>
    /// Pauses the game, setting timeScale to 0.  A reason must be given.
    /// If the game just now becomes paused, the current Time.timeScale value is saved then is set to 0.  
    /// </summary>
    /// <param name="reason"></param>
    public static void pause(Reason reason) {
        if (isPausedForReason(reason)) {
            Debug.LogWarning("Game is already paused for reason " + reason);
            return;
        }

        if (!isPaused) {
            // become paused
            storedTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        pauseReasons.Add(reason);
    }

    /// <summary>
    /// Resumes the pause for the given reason.
    /// If no more reasons to pause the game exist, then Time.timeScale goes back to the value it was at before the game was paused.
    /// </summary>
    /// <param name="reason"></param>
    public static void resume(Reason reason) {
        if (!isPausedForReason(reason)) {
            Debug.LogWarning("Game is not paused for reason " + reason);
            return;
        }

        pauseReasons.Remove(reason);

        if (pauseReasons.Count <= 0) {
            // completely unpausing game
            Time.timeScale = storedTimeScale;
        }

    }

    private static List<Reason> pauseReasons = new List<Reason>();
    private static float storedTimeScale = 1;

}
