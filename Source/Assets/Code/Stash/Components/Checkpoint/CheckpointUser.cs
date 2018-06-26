using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointUser : MonoBehaviour {

    /* When this component is enabled, the following messages sent to sibling components:
    
    /// <summary>
    /// Message received when checkpoint saving.  Only sent if component is enabled.
    /// </summary>
    void OnCheckpointSave() { }

    /// <summary>
    /// Message received when loading from a checkpoint.  Only sent if component is enabled.
    /// </summary>
    void OnCheckpointLoad() { }
    
    */

    #region Static Constants

    public const float loadFadeInDuration = .5f;
    
    #endregion

    #region Inspector Properties

    [Tooltip("autoLocalPosition - Will automatically save and load the gameObject's localPosition on OnCheckpointSave and OnCheckpointLoad.")]
    public bool autoLocalPosition = true;
    [Tooltip("autoRb2dVelocity = Will automatically save and load the gameObject's Rigidbody2D's velocity on OnCheckpointSave and OnCheckpointLoad")]
    public bool autoRb2dVelocity = true;

    #endregion
    
    #region Saving and loading checkpoint (static)

    /// <summary>
    /// Struct holding properties to be used when loading a checkpoint.
    /// </summary>
    public struct CheckpointProperties {
        public Vector2 spawnPos;
        public bool flippedHoriz;
    }

    /// <summary>
    /// During LateUpdate(), will send the OnCheckpointSave message to siblings of all enabled CheckpointUsers.
    /// </summary>
    public static void saveCheckpoint() {
        callSaveCheckpoint = true;
        checkpointPropertiesSet = false;
    }
    /// <summary>
    /// During LateUpdate(), will send the OnCheckpointSave message to siblings of all enabled CheckpointUsers.
    /// </summary>
    public static void saveCheckpoint(CheckpointProperties properties) {
        saveCheckpoint();
        checkpointPropertiesSet = true;
        checkpointProperties = properties;
    }
    
    /// <summary>
    /// If properties were set when saveCheckpoint() was last called.
    /// </summary>
    public static bool checkpointPropertiesSet { get; private set; }

    /// <summary>
    /// The properties optionally set when saving a checkpoint.
    /// </summary>
    public static CheckpointProperties checkpointProperties { get; private set; }
    
    /// <summary>
    /// During LateUpdate(), will send the OnCheckpointLoad message to siblings of all enabled CheckpointUsers.
    /// </summary>
    public static void loadCheckpoint() {
        if (callLoadCheckpoint)
            return;
        callLoadCheckpoint = true;

        // black screen fade in
        if (BlackScreen.instance != null) {
            BlackScreen.instance.fadeIn(loadFadeInDuration);
        }

    }

    /// <summary>
    /// Gets the amount of time that passed (according to Time.time) since loadCheckpoint() was last called.
    /// </summary>
    public static float timeSinceLastLoaded {
        get {
            return Time.time - timeWhenLastLoadedCheckpoint;
        }
    }

    #endregion

    #region Private saving and loading checkpoint (static)

    /// <summary>
    /// Flag for calling saveCheckpointLU in LateUpdate()
    /// </summary>
    private static bool callSaveCheckpoint = false;

    /// <summary>
    /// Function used to save the checkpoint.  Called in LateUpdate after saveCheckpoint() is called.
    /// </summary>
    private static void saveCheckpointLU() {

        Debug.Log("save checkpoint");

        // send message to all checkpoint users
        foreach (CheckpointUser cu in checkpointUsers) {
            if (!cu.enabled) continue;
            cu._saveCheckpoint();            
        }

    }

    /// <summary>
    /// Flag for calling loadCheckpointLU in LateUpdate()
    /// </summary>
    private static bool callLoadCheckpoint = false;

    /// <summary>
    /// Function used to load a checkpoint.  Called in LateUpdate after loadCheckpoint() is called.
    /// </summary>
    private static void loadCheckpointLU() {

        timeWhenLastLoadedCheckpoint = Time.time;

        // send message to all checkpoint users
        foreach (CheckpointUser cu in checkpointUsers) {
            if (!cu.enabled) continue;
            cu._loadCheckpoint();
        }

        // destroy all gameObjects with component DestroyOnCheckpointLoad.
        DestroyOnCheckpointLoad[] destroyList = new DestroyOnCheckpointLoad[DestroyOnCheckpointLoad.list.Count];
        DestroyOnCheckpointLoad.list.CopyTo(destroyList);
        for (int i=0; i<destroyList.Length; i++) {
            if (destroyList[i] == null) continue;
            if (!destroyList[i].enabled) continue;
            Destroy(destroyList[i].gameObject);
        }

    }

    private static float timeWhenLastLoadedCheckpoint = 0;

    private static List<CheckpointUser> checkpointUsers = new List<CheckpointUser>();

    #endregion

    #region Private member functions

    void _saveCheckpoint() {
        if (!enabled) return;

        if (autoLocalPosition) {
            cpLocalPosition = transform.localPosition;
        }
        if (autoRb2dVelocity) {
            Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
            if (rb2d != null) {
                cpRb2dVelocity = rb2d.velocity;
            }
        }

        SendMessage("OnCheckpointSave", SendMessageOptions.DontRequireReceiver);
    }

    void _loadCheckpoint() {
        if (!enabled) return;

        if (autoLocalPosition) {
            transform.localPosition = new Vector3(cpLocalPosition.x, cpLocalPosition.y, transform.localPosition.z);
#if PCOL
            PCol.Actor actor = GetComponent<PCol.Actor>();
            if (actor != null) {
                actor.prePhysicsPosition = transform.position;
            }
#endif
        }
        if (autoRb2dVelocity) {
            Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
            if (rb2d != null) {
                rb2d.velocity = cpRb2dVelocity;
            }
        }

        SendMessage("OnCheckpointLoad", SendMessageOptions.DontRequireReceiver);
    }

    Vector2 cpLocalPosition = new Vector2();
    Vector2 cpRb2dVelocity = Vector2.zero;

    #endregion
    
    #region Unity Events

    void Awake() {
        checkpointUsers.Add(this);
    }

    void Start() {
        if (autoLocalPosition) {
            cpLocalPosition = transform.localPosition;
        }
    }

    void LateUpdate() {
        if (callSaveCheckpoint) {
            saveCheckpointLU();
            callSaveCheckpoint = false;
        }
        if (callLoadCheckpoint) {
            loadCheckpointLU();
            callLoadCheckpoint = false;
        }
    }

    void OnDestroy() {
        checkpointUsers.Remove(this);
    }

    #endregion

    
}
