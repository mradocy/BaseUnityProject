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

        //SaveManager.debugForceSaveStatus = SaveManager.SaveStatus.PROBLEM_WRITING_TO_FILE;
        //SaveManager.debugForceDeleteStatus = SaveManager.DeleteStatus.PROBLEM_DELETING_FILE;
        //SaveManager.debugForceLoadStatus = SaveManager.LoadStatus.PROBLEM_READING_FROM_FILE;
        

        UDeb.commandEvent += udebCommand;



        List<int?> list = new List<int?>(new int?[] {0, 1, 2});
        list.Resize(6);

        Debug.Log(list.ToStringElements());


        ShowcaseMode.resetGame += showcaseResetFunc;


    }

    void showcaseResetFunc() {
        Debug.Log("Showcase mode reset code pressed");
    }

    void Update() {
        
        if (UDeb.num1Pressed) {
            SaveManager.save(0, saveCallback);
        }

        if (UDeb.num2Pressed) {
            SaveManager.data<SaveData>().integer++;
            SaveManager.data<SaveData>().intDic["integer"] = SaveManager.data<SaveData>().integer;
            Debug.Log("integer = " + SaveManager.data<SaveData>().intDic["integer"]);
        }

        if (UDeb.num3Pressed) {
            SaveManager.deleteAll(deleteCallback);
        }

        if (UDeb.num4Pressed) {
            SaveManager.load(0, loadCallback);
        }



        // testing UDeb2
        UDeb2.post("stringProp", "string val");
        exposedProp = UDeb2.expose("exposed prop", exposedProp);
        exposedNum = UDeb2.expose("exposed num", exposedNum, -999, 999);
        exposedInt = UDeb2.expose("exposed int", exposedInt, -4, 12);
        UDeb2.post("int post", -555);
        UDeb2.post("float post", 8.11234f);
        UDeb2.post("bool post", true);
        exposedBool = UDeb2.expose("exposed bool", exposedBool);

    }

    void saveCallback(SaveManager.SaveStatus status) {
        string logStr = "save complete.  Status: " + status + ".  fileIndex = " + SaveManager.fileIndex + ".  Currently " + SaveManager.getAvailableSaveFiles().Length + " save files available.";
        Debug.Log(logStr);
        UDeb.post(0, logStr);
    }

    void deleteCallback(SaveManager.DeleteStatus status) {
        Debug.Log("delete complete.  Status: " + status);
    }

    void loadCallback(SaveManager.LoadStatus status) {
        Debug.Log("load complete.  Status: " + status + ".  fileIndex = " + SaveManager.fileIndex + ".  integer = " + SaveManager.data<SaveData>().integer);
    }

    void udebCommand(string[] args) {
        Debug.Log("received command: " + args[0]);
    }

    string exposedProp = "val 1";
    float exposedNum = 50;
    int exposedInt = 6;
    bool exposedBool = false;

}
