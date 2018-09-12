using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
    
    public int width = 10;
    public int height = 5;

    public int x {
        get {
            return Mathf.RoundToInt(transform.localPosition.x);
        }
    }
    public int y {
        get {
            return Mathf.RoundToInt(transform.localPosition.y);
        }
    }
    public int left {
        get {
            return x;
        }
    }
    public int right {
        get {
            return x + width;
        }
    }
    public int bottom {
        get {
            return y;
        }
    }
    public int top {
        get {
            return y + height;
        }
    }

    public List<Level> adjacentLeftLevels = new List<Level>();
    public List<Level> adjacentTopLevels = new List<Level>();
    public List<Level> adjacentRightLevels = new List<Level>();
    public List<Level> adjacentBottomLevels = new List<Level>();

    /// <summary>
    /// Updates adjacent levels.
    /// Only considers levels that have been added to the LevelMaster's levels (which should be all the levels in the scene).
    /// </summary>
    public static void updateAdjacentLevels() {
        if (LevelMaster.instance == null) {
            Debug.LogWarning("LevelMaster has not been created.");
            return;
        }
        Level[] allLevels = LevelMaster.instance.levels;
        foreach (Level level in allLevels) {
            level.adjacentLeftLevels.Clear();
            level.adjacentBottomLevels.Clear();
            level.adjacentRightLevels.Clear();
            level.adjacentTopLevels.Clear();
        }
        for (int i=0; i < allLevels.Length; i++) {
            Level level = allLevels[i];
            if (level == null) continue;
            for (int j=i + 1; j < allLevels.Length; j++) {
                Level l2 = allLevels[j];
                if (l2 == null) continue;
                
                if (level.right == l2.left &&
                    level.top > l2.bottom && level.bottom < l2.top) {
                    level.adjacentRightLevels.Add(l2);
                    l2.adjacentLeftLevels.Add(level);
                }
                
                if (level.left == l2.right &&
                    level.top > l2.bottom && level.bottom < l2.top) {
                    level.adjacentLeftLevels.Add(l2);
                    l2.adjacentRightLevels.Add(level);
                }

                if (level.top == l2.bottom &&
                    level.right > l2.left && level.left < l2.right) {
                    level.adjacentTopLevels.Add(l2);
                    l2.adjacentBottomLevels.Add(level);
                }

                if (level.bottom == l2.top &&
                    level.right > l2.left && level.left < l2.right) {
                    level.adjacentBottomLevels.Add(l2);
                    l2.adjacentTopLevels.Add(level);
                }

            }
        }
    }


    void Awake() {

	}
	
	void Update() {

	}

    void OnDestroy() {
        
    }
    

}



