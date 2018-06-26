using UnityEngine;
using System.Collections;

/// <summary>
/// Manages why gameplay is paused (timeScale == 0).
/// There are many reasons why a game would be paused.  Each reason has to "reserve" a "lock".
/// </summary>
public class PauseManager {
    
    public enum State {
        NOT_PAUSED,
        PAUSE_SCREEN,
        HIT_PAUSE,
    }

    public static State state { get; private set; }

    /// <summary>
    /// Returns if the game is paused (timeScale == 0) for any reason.
    /// </summary>
    public static bool paused {
        get {
            return state != State.NOT_PAUSED;
        }
    }
    
    public static void beginState(State state, bool evenIfAlreadyPaused = false) {
        
        // don't begin if already paused
        if (PauseManager.state != State.NOT_PAUSED && !evenIfAlreadyPaused)
            return;

        // beginning NOT_PAUSED state is the same as resuming
        if (state == State.NOT_PAUSED) {
            resume();
            return;
        }

        // save previous time scale if currently not paused
        if (PauseManager.state == State.NOT_PAUSED) {
            storedTimeScale = Time.timeScale;
        }
        
        PauseManager.state = state;
        Time.timeScale = 0;
    }

    public static void resume() {
        if (state == State.NOT_PAUSED) return;

        state = State.NOT_PAUSED;
        Time.timeScale = storedTimeScale;
    }

    private static float storedTimeScale = 1;

}
