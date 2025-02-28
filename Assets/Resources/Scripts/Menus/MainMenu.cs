using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    public GameObject mapEditorWindow;
    public GameObject mainMenuWindow;
    public GameObject playWindow;
    public GameObject loadWindow;
    public GameObject saveHolder;
    public GameObject mapHolder;
    public GameObject prefabSaveBox;
    public TMP_InputField mapEditorNameField;
    public TMP_InputField mapEditorXSizeField;
    public TMP_InputField mapEditorYSizeField;
    string sceneToLoad;
    float loadTime = 0.25f;
    float timeLeft;
    void Awake(){
        instance = this;
    }

    void Update()
    {
        if (timeLeft > 0){
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0){
                LoadScene();
            }
        }
    }
    public void LoadGame(string path){
        MapLoader.LoadGame(path);
        sceneToLoad = "Map Generation";
        timeLeft = loadTime;
    }
    public void LoadMap(string path){
        MapLoader.LoadMap(path);
        sceneToLoad = "Map Generation";
        timeLeft = loadTime;
    }
    public void ButtonOpenMapEditor(){ 
        mainMenuWindow.SetActive(false);
        playWindow.SetActive(false);
        mapEditorWindow.SetActive(true);
        loadWindow.SetActive(false);
    }
    public void ButtonQuit(){
        Application.Quit();
    }

    public void ButtonOpenMainMenu(){
        mainMenuWindow.SetActive(true);
        playWindow.SetActive(false);
        mapEditorWindow.SetActive(false);
        loadWindow.SetActive(false);
    }
    public void ButtonOpenPlayMenu(){
        mainMenuWindow.SetActive(false);
        playWindow.SetActive(true);
        mapEditorWindow.SetActive(false);
        loadWindow.SetActive(false);

        for (int i = 0; i < mapHolder.transform.childCount; i++){
            Destroy(mapHolder.transform.GetChild(i).gameObject);
        }

        foreach (string file in Directory.EnumerateFiles(Application.persistentDataPath, "*.map"))
        {
            string fileName = file.Replace(Application.persistentDataPath, "");
            fileName = fileName.Substring(1);
            fileName = fileName.Replace(".map", "");

            GameObject saveBox = Instantiate(prefabSaveBox, mapHolder.transform);
            SaveBox box = saveBox.GetComponent<SaveBox>();
            box.path = file;
            box.map = true;
            saveBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = fileName;
        }
    }

    public void ButtonOpenLoadMenu(){
        mainMenuWindow.SetActive(false);
        playWindow.SetActive(false);
        mapEditorWindow.SetActive(false);
        loadWindow.SetActive(true);

        for (int i = 0; i < saveHolder.transform.childCount; i++){
            Destroy(saveHolder.transform.GetChild(i).gameObject);
        }

        foreach (string file in Directory.EnumerateFiles(Application.persistentDataPath, "*.json"))
        {
            string fileName = file.Replace(Application.persistentDataPath, "");
            fileName = fileName.Substring(1);
            fileName = fileName.Replace(".json", "");

            GameObject saveBox = Instantiate(prefabSaveBox, saveHolder.transform);
            saveBox.GetComponent<SaveBox>().path = file;
            saveBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = fileName;
        }
    }

    public void ButtonCreateNewMap(){
        // Get map size
        Vector2Int sizeVector;
        if (mapEditorXSizeField.text == "" && mapEditorYSizeField.text == ""){
            sizeVector = new Vector2Int(10, 10);
        }else{
            sizeVector = new Vector2Int(int.Parse(mapEditorXSizeField.text),int.Parse(mapEditorYSizeField.text));
        }
        // Get map name
        string mapName = mapEditorNameField.text;

        // Load editor
        MapLoader.LoadEditor(mapName, sizeVector);
        sceneToLoad = "Map Editor";
        timeLeft = loadTime;
    }

    public void ButtonPlayScenario(){
        MapLoader.instance.loadStarter = true;
        instance.LoadMap("Scenario.map");
    }

    void LoadScene(){
        SceneManager.LoadScene(sceneToLoad);
    }
}
