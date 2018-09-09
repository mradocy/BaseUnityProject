using UnityEngine;
using System.Collections;

/// <summary>
/// Spawns stuff on mouse click
/// </summary>
public class MouseSpawnTester : MonoBehaviour {

    public GameObject lmbPrefab;
    public GameObject rmbPrefab;
    public GameObject mmbPrefab;

    void Awake() {
        
	}
    
    void Update() {

        if (!UDeb.debug)
            return;

        if (!enabled) return;

        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject prefab = null;
        if (Input.GetMouseButtonDown(0))
            prefab = lmbPrefab;
        if (Input.GetMouseButtonDown(1))
            prefab = rmbPrefab;
        if (Input.GetMouseButtonDown(2))
            prefab = mmbPrefab;

        if (prefab != null) {
            Instantiate(prefab, position, Quaternion.identity);
        }
        

    }

}
