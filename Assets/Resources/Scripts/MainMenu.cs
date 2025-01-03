using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject mapEditorWindow;
    public GameObject mainMenuWindow;
    public GameObject playWindow;
    public TMP_InputField mapEditorNameField;
    public TMP_InputField mapEditorXSizeField;
    public TMP_InputField mapEditorYSizeField;
    public TMP_InputField playMenuNameField;
    public void ButtonPlayGame(){
        string mapName = playMenuNameField.text;

        MapLoader.LoadGame(mapName);
        SceneManager.LoadScene("Map Generation");
    }
    public void ButtonOpenMapEditor(){ 
        mainMenuWindow.SetActive(false);
        playWindow.SetActive(false);
        mapEditorWindow.SetActive(true);
    }
    public void ButtonQuit(){
        Application.Quit();
    }

    public void ButtonOpenMainMenu(){
        mainMenuWindow.SetActive(true);
        playWindow.SetActive(false);
        mapEditorWindow.SetActive(false);
    }
    public void ButtonOpenPlayMenu(){
        mainMenuWindow.SetActive(false);
        playWindow.SetActive(true);
        mapEditorWindow.SetActive(false);
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
