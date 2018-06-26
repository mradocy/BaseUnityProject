using UnityEngine;

/// <summary>
/// id is guaranteed to be unique to all other SceneID components in the scene.
/// </summary>
public class SceneID : MonoBehaviour {

    [Tooltip("If this id is set to a value that already exists in this scene, it will automatically be set to a unique value.")]
    public int id = 0;
    
    /// <summary>
    /// Gets the global name for a gameObject with the SceneID component.  This is done by combining the name of the scene with the id of the sceneID.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="id"></param>
    public static string getGlobalName(string sceneName, int id) {
        return sceneName + "_" + id;
    }

    /// <summary>
    /// Gets the global name for the gameObject of this sceneID.
    /// </summary>
    public string globalName {
        get {
            return getGlobalName(z_sceneName, id);
        }
    }

    [HideInInspector]
    public string z_sceneName = "";
    [HideInInspector]
    public int z_prevID = -1;
    [HideInInspector]
    public bool z_setInEditor = false;

    void Awake() {

        if (!z_setInEditor) {
            // if instantiated during runtime
            // quickly set scene name and id
            z_sceneName = gameObject.scene.name;

            id = 0;
            SceneID[] sceneIDs = FindObjectsOfType<SceneID>();
            foreach (SceneID sceneID in sceneIDs) {
                if (sceneID.z_sceneName != z_sceneName) continue;
                id = Mathf.Max(id, sceneID.id);
            }
            id++;
        }

    }

}
