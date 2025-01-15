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
    void Awake(){
        instance = this;
    }
    public void LoadGame(string path){
        MapLoader.LoadGame(path);
        SceneManager.LoadScene("Map Generation");
    }
    public void LoadMap(string path){
        MapLoader.LoadMap(path);
        SceneManager.LoadScene("Map Generation");
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

        GameObject saveBox = Instantiate(prefabSaveBox, mapHolder.transform);
        SaveBox box = saveBox.GetComponent<SaveBox>();
        box.path = "starter.json";
        box.loadStarter = true;
        saveBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Starter";


        foreach (string file in Directory.EnumerateFiles(Application.persistentDataPath, "*.map"))
        {
            string fileName = file.Replace(Application.persistentDataPath, "");
            fileName = fileName.Substring(1);
            fileName = fileName.Replace(".map", "");

            saveBox = Instantiate(prefabSaveBox, mapHolder.transform);
            box = saveBox.GetComponent<SaveBox>();
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
        SceneManager.LoadScene("Map Editor");
    }
}
