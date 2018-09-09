using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneID))]
public class SceneIDEditor : Editor {

    public override void OnInspectorGUI() {
        
        DrawDefaultInspector();
        
        SceneID targetSceneID = target as SceneID;

        // return if this is just a prefab in the heirarchy, not an object in the game
        if (targetSceneID.gameObject.scene.name == null)
            return;
        
        // set scene name
        targetSceneID.z_sceneName = targetSceneID.gameObject.scene.name;

        // check at start
        if (!prechecked) {
            targetSceneID.z_prevID = -1;
            prechecked = true;
        }

        if (targetSceneID.id != targetSceneID.z_prevID) {
            // id changed
            if (targetSceneID.id < 0 || !idIsUnique()) {
                targetSceneID.id = getUniqueID();
            }
            targetSceneID.z_prevID = targetSceneID.id;
        }

        targetSceneID.z_setInEditor = true;

    }
    bool prechecked = false;
    
    bool idIsUnique() {
        SceneID targetSceneID = target as SceneID;
        SceneID[] sceneIDs = FindObjectsOfType<SceneID>();
        foreach (SceneID sceneID in sceneIDs) {
            if (sceneID == targetSceneID) continue;
            if (sceneID.z_sceneName != targetSceneID.z_sceneName) continue;
            if (targetSceneID.id == sceneID.id)
                return false;
        }
        return true;
    }

    int getUniqueID() {
        SceneID targetSceneID = target as SceneID;
        int id = 0;
        SceneID[] sceneIDs = FindObjectsOfType<SceneID>();
        foreach (SceneID sceneID in sceneIDs) {
            if (sceneID.z_sceneName != targetSceneID.z_sceneName) continue;
            id = Mathf.Max(id, sceneID.id);
        }
        id++;
        return id;
    }
    

}