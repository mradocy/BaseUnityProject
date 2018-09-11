using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShowcaseMode {

    /// <summary>
    /// Gets if ShowcaseMode is enabled.
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
    /// Key combination to be held to reset the game.  Can be customized.
    /// </summary>
    public static KeyCode[] resetGameKeyCombination = new KeyCode[] { KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

    public delegate void ResetGameFunction();

    /// <summary>
    /// Occurs when the key combination is held.
    /// </summary>
    public static event ResetGameFunction resetGame;

    #region Unity Messages

    private static void Update() {
        if (!enabled) return;

        if (resetGameKeyCombination == null || resetGameKeyCombination.Length <= 0){
            Debug.LogError("ShowcaseMode.resetGameKeyCombination must contain keys.");
            return;
        }

        bool allKeysHeld = true;
        bool allKeysNotHeld = true;
        for (int i = 0; i < resetGameKeyCombination.Length; i++){
            allKeysHeld &= Input.GetKey(resetGameKeyCombination[i]);
            allKeysNotHeld &= !Input.GetKey(resetGameKeyCombination[i]);
        }
        if (allKeysNotHeld) {
            keysCurrentlyHeld = false;
        }
        if (allKeysHeld && !keysCurrentlyHeld) {
            keysCurrentlyHeld = true;

            // reset game
            if (resetGame == null){
                Debug.LogWarning("Reset game combination pressed, but resetGame event has no functions.");
            } else {
                resetGame();
            }
        }

    }
    private static bool keysCurrentlyHeld = false;

    #endregion


    #region Static Constructor, Setup to Receive Unity messages

    static ShowcaseMode() {
        GameObject mbGO = new GameObject();
        mb = mbGO.AddComponent<MB>();
    }
    private static MB mb = null;
    private class MB : MonoBehaviour {
        void Awake() {
            DontDestroyOnLoad(gameObject);
        }
        void Update() {
            ShowcaseMode.Update();
        }
        void OnDestroy() {
            if (mb == this)
                mb = null;
        }
    }

    #endregion

}
