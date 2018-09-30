using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TestStart : MonoBehaviour {
    
    public float shortName = 2;
    [LongLabel, Tooltip("Works with Tooltip")]
    public int superUltraMegaLongName = 0;

    [HeaderScene]
    public Camera referencingCamFromScene;


    [System.Serializable]
    public class IntDictionary : SerializableDictionary<string, int> { }

    [System.Serializable]
    public class SaveData : BaseSaveData {
        
        public int integer = 0;
        public IntDictionary intDic = new IntDictionary();

        public override void loadFromString(string str) {
            JsonUtility.FromJsonOverwrite(str, this);
        }
        public override string saveToString() {
            return JsonUtility.ToJson(this);
        }
        public override void clearDefault() {
            integer = 0;
        }
    }

    void Awake() {

        SaveManager.initialize(new SaveData());
        
        
        
        ShowcaseMode.resetGame += showcaseResetFunc;


        // testing UDeb
        UDeb.registerFunction("Test Function 0", udebTestFunction0);
        UDeb.registerFunction("Test Function 1", udebTestFunction1, "def");
        UDeb.registerFunction("Test Function 2", udebTestFunction2);

    }

    void showcaseResetFunc() {
        Debug.Log("Showcase mode reset code pressed");
    }

    void Update() {
        
        // testing UDeb
        UDeb.post("stringProp", "string val");
        exposedProp = UDeb.expose("exposed prop", exposedProp);
        exposedNum = UDeb.expose("exposed num", exposedNum, -999, 999);
        exposedInt = UDeb.expose("exposed int", exposedInt, -4, 12);
        UDeb.post("int post", -555);
        UDeb.post("float post", 8.11234f);
        UDeb.post("bool post", true);
        exposedBool = UDeb.expose("exposed bool", exposedBool);

    }
    
    string exposedProp = "val 1";
    float exposedNum = 50;
    int exposedInt = 6;
    bool exposedBool = false;

    void udebTestFunction0() {
        Debug.Log("udeb test function 0 called");
    }
    void udebTestFunction1(string arg) {
        Debug.Log("udeb test function 1 called with arg " + arg);
    }
    void udebTestFunction2(string arg0, string arg1) {
        Debug.Log("2 args!  0: " + arg0 + " 1: " + arg1);
    }

}
