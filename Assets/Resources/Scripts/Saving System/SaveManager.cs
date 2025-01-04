using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    public static SaveManager instance { get; private set;}

    private GameData gameData;
    private List<ISavable> savableObjects;
    private FileDataHandler dataHandler;

    private void Awake() {
        if(instance != null){
            Debug.LogError("More than one Save Manager found!");
        }
        
        instance = this;

        // Map loader
        if (MapLoader.instance != null){
            fileName = MapLoader.instance.mapName + ".json";
        }
    }

    private void Start() {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        savableObjects = FindAllSavableObjects();
        LoadGame();
    }

    public void NewGame(){
        gameData = new();
    }

    public void LoadGame(){
        gameData = dataHandler.Load();

        if(gameData == null){
            NewGame();
            gameData = dataHandler.LoadStarterMap();
        }

        foreach (ISavable savable in savableObjects)
        {
            savable.LoadData(gameData);
        }
    }

    public void SaveGame(){
        foreach (ISavable savable in savableObjects)
        {
            savable.SaveData(gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit() {
        SaveGame();
    }

    private List<ISavable> FindAllSavableObjects()
    {
        IEnumerable<ISavable> savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>();
        List<ISavable> savablesList = new List<ISavable>(savableObjects);

        for (int select = 0; select < savablesList.Count-1; select++){
            int prio1 = savablesList[select].GetPriority();
            for (int look = select+1; look < savablesList.Count; look++){
                float prio2 = savablesList[look].GetPriority();
                if (prio1 > prio2){
                    (savablesList[look], savablesList[select]) = (savablesList[select], savablesList[look]);
                }
            }
        }

        return savablesList;
    }
}
