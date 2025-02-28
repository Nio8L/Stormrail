using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationMenu : MonoBehaviour
{
    public static StationMenu instance;
    public Station currentStation;
     void Awake(){
        instance = this;
        gameObject.SetActive(false);
    }
    public void OpenMenu(Station stationToOpen){
        if (currentStation != null){
            // If the player tries to open the same city close the menu instead
            if (stationToOpen == currentStation) {CloseMenu(); return;}
            // If the player opens a new city then close the old one
        }

        if (CityMenu.instance.currentCity != null) CityMenu.instance.CloseMenu();

        currentStation = stationToOpen;
        
        instance.gameObject.SetActive(true);
    }

    public void CloseMenu(){
        currentStation = null;
        instance.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !RaycastChecker.Check()){
             CloseMenu();
        }
    }
}
