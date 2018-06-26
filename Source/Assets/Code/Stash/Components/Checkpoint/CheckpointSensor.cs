using UnityEngine;
using System.Collections;

public class CheckpointSensor : MonoBehaviour {

    #region Inspector Properties

    [Tooltip("Sets the spawnPos property in CheckpointUser.checkpointProperties.  Can be null.")]
    public Transform spawnPos;
    [Tooltip("If player should be facing left on respawn.")]
    public bool flippedHoriz;

    #endregion

    /// <summary>
    /// Set to true when triggered.  Once checkpoint is saved, it won't be saved again.
    /// </summary>
    public bool checkpointSaved { get; private set; }

    void Awake() {
        
	}
    
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D c2d) {
        
        if (checkpointSaved)
            return;
        
        CheckpointUser.CheckpointProperties properties = new CheckpointUser.CheckpointProperties();

        // get spawn position
        if (spawnPos == null) {
            properties.spawnPos = transform.position;
        } else {
            properties.spawnPos = spawnPos.position;
        }

        properties.flippedHoriz = flippedHoriz;

        // save checkpoint with properties
        CheckpointUser.saveCheckpoint(properties);

        checkpointSaved = true;

    }


}
