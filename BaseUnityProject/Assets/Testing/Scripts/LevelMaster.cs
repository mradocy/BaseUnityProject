using UnityEngine;
using System.Collections;

/// <summary>
/// Contains all the Levels in this scene.
/// Must be added manually.
/// </summary>
public class LevelMaster : MonoBehaviour {

    [HeaderScene]
    public Level[] levels;

    public static LevelMaster instance { get; private set; }

    void Awake() {
        if (instance == null) {
            Debug.LogError("There can only be one instance of LevelMaster");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Level.updateAdjacentLevels();
    }
    
    void Update() {

    }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }

    

}
