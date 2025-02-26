using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject gameMenu;

    private void Update() {
        if(Input.GetKeyUp(KeyCode.Escape)){
            gameMenu.SetActive(!gameMenu.activeSelf);
            TimeControl.instance.PauseTime();
        }
    }

    public void Resume(){
        gameMenu.SetActive(false);
    }

    public void MainMenu(){
        SceneManager.LoadScene("Main Menu");

        Time.timeScale = 1;
    }
}
