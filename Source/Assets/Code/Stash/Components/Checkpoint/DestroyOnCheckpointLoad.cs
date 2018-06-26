using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// If enabled, this gameObject will be destroyed whenever a checkpoint is loaded.
/// </summary>
public class DestroyOnCheckpointLoad : MonoBehaviour {
    
    void Awake() {
        list.Add(this);
	}

    void OnDestroy() {
        list.Remove(this);
    }
    
    public static List<DestroyOnCheckpointLoad> list = new List<DestroyOnCheckpointLoad>();

}
