using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    public GameObject buttonHolder;
    public GameObject closeButton;

    public void OpenButtons(){
        buttonHolder.SetActive(true);
        closeButton.SetActive(true);
    }

    public void CloseButtons(){
        buttonHolder.SetActive(false);
        closeButton.SetActive(false);
    }
}
