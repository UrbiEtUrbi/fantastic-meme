using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ControllerLoadingScene : MonoBehaviour
{

    [SerializeField, SceneDetails]
    SerializedScene Scene;

    public static ControllerLoadingScene Instance;


    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

        Load();
        ControllerGameFlow.Instance.LoadNewScene(Scene.BuildIndex);
    }



    
    public SaveData SaveData;
    public bool HasSave => SaveData != null;

    const string SaveStr = "save_str";

    public void Save(SaveData saveData)
    {
        PlayerPrefs.SetString(SaveStr, JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
    }

    public void Load()
    {
       var str =  PlayerPrefs.GetString(SaveStr,string.Empty);
        if (!string.IsNullOrEmpty(str))
        {
            SaveData = JsonUtility.FromJson<SaveData>(str);
        }
        else
        {
            SaveData = null;
        }
    }





}

[System.Serializable]
public class SaveData
{
    public bool HasStick;
    public bool HasIceMelee;
    public bool HasSpike;
    public int Room;
    public int Entrance;
    public int MaxHp;
    public int CurrentHP;
    public int MaxStamina;
    public int CurrentStamina;
    public List<string> Dialogues;
    public List<string> Pickups;
}
