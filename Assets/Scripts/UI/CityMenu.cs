using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CityMenu : MonoBehaviour
{
    public static CityMenu instance;
    public City currentCity;

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    int currentTabIndex;
    public List<GameObject> tabs;

    bool ignoreClose = false;

    void Awake(){
        instance = this;
        gameObject.SetActive(false);
    }
    public void OpenMenu(City cityToOpen){
        if (currentCity != null){
            // If the player tries to open the same city close the menu instead
            if (cityToOpen == currentCity) {CloseMenu(); return;}
            // If the player opens a new city then close the old one
            else CloseCurrentCity();
        }

        currentCity = cityToOpen;
        EventManager.OpenCity?.Invoke(currentCity);

        // Setup city menu
        instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cityToOpen.cityName;
        instance.gameObject.SetActive(true);
        
        // Open the overview tab by default
        OpenTab(tabs[cityToOpen.lastOpenTab]);

        // Makes it so the menu can't be closed on the frame its opened 
        //(This is here to prevent the menu from closing when clicking to open another city while the menu is open)
        ignoreClose = true;

        
    }

    public void CloseMenu(){
        // Close the menu
        CloseCurrentCity();

        instance.gameObject.SetActive(false);
    }
    void CloseCurrentCity(){
        EventManager.CloseCity?.Invoke(currentCity);

        // Save the last open tab
        currentCity.lastOpenTab = currentTabIndex;
        // Remove currentCity
        currentCity = null;
    }

    public void OpenTab(GameObject tabToOpen){
        // Set tabToOpen to be active
        tabToOpen.SetActive(true);

        // Set all other tabs to be inactive
        foreach (GameObject tab in tabs){
            // Save the current open tab
            if (tab == tabToOpen) {
                currentTabIndex = tabs.IndexOf(tab);
                continue;
            }

            tab.SetActive(false);
        }

        
    }

    void Update(){
        if (!ignoreClose && Input.GetMouseButtonDown(0) && Input.mousePosition.x > 320f){
             CloseMenu();
        }else{
            ignoreClose = false;
        }
    }


    /*
        UI RAYCAST
        
        if (Input.GetMouseButtonUp(0)){
            // Raycast on left click
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.position = Input.mousePosition;
            eventData.position = new Vector3(eventData.position.x, eventData.position.y, 10);

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            foreach (RaycastResult hit in results){
                // If a tabButton is hit use it
                CityMenuTabButton buttonScript = hit.gameObject.GetComponent<CityMenuTabButton>();
                if (buttonScript != null){
                    
                }
            }
        }
        
    */
}
