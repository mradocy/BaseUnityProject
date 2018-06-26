using UnityEngine;
using System.Collections;

public class ScreenStatus : MonoBehaviour {

    [Tooltip("Extends the screen border by this amount when checking if gameObject is on screen.")]
    public float screenBorder = 0;
    [Tooltip("Offsets transform's position by this amount first when checking if gameObject is on screen.")]
    public Vector2 positionOffset = new Vector2();
    [Tooltip("After not being on screen for this duration, ScreenStatus will consider the gameObject inactive.")]
    public float notOnScreenDuration = 5;
    
    /// <summary>
    /// If was shown on screen at least once.
    /// </summary>
    public bool shown { get; private set; }

    /// <summary>
    /// If is currently on screen.
    /// </summary>
    public bool onScreen {
        get {
            return CameraControl.instance.pointInBounds(new Vector2(transform.position.x, transform.position.y) + positionOffset, screenBorder);
        }
    }

    /// <summary>
    /// Time object has been on screen.  Resets whenever object goes off-screen.
    /// </summary>
    public float timeOnScreen { get; private set; }

    /// <summary>
    /// Time passed since the last time this object was on screen.
    /// </summary>
    public float timeSinceOnScreen { get; private set; }

    /// <summary>
    /// If gameObject should be "active".  This is when the object is on screen, or it's been less than notOnScreenDuration seconds since it went off screen.
    /// </summary>
    public bool active {
        get {
            if (!shown)
                return false;
            if (timeSinceOnScreen > notOnScreenDuration)
                return false;
            return true;
        }
    }

    void Awake() {
        timeSinceOnScreen = 99999;
	}
    
    void Update() {
        
        if (onScreen) {
            shown = true;        
            timeOnScreen += Time.deltaTime;
            timeSinceOnScreen = 0;
        } else {
            timeOnScreen = 0;
            timeSinceOnScreen += Time.deltaTime;
        }

    }

    #region Checkpoint save/load

    bool cpShown = false;
    float cpTimeOnScreen = 0;
    float cpTimeSinceOnScreen = 99999;

    void OnCheckpointSave() {
        cpShown = shown;
        cpTimeOnScreen = timeOnScreen;
        cpTimeSinceOnScreen = timeSinceOnScreen;
    }

    void OnCheckpointLoad() {
        shown = cpShown;
        timeOnScreen = cpTimeOnScreen;
        timeSinceOnScreen = cpTimeSinceOnScreen;
    }

    #endregion

}
