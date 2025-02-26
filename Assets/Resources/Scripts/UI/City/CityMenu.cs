using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CityMenu : MonoBehaviour
{
    public static CityMenu instance;
    public City currentCity;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    int currentTabIndex;
    public List<GameObject> tabs;
    public TextMeshProUGUI populationText;
    public TMP_InputField nameBox;

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
        nameBox.text = cityToOpen.cityName;
        populationText.text = cityToOpen.population.ToString();
        
        instance.gameObject.SetActive(true);
        
        // Open the overview tab by default
        OpenTab(tabs[cityToOpen.lastOpenTab]);
        // Update overview information
        tabs[0].GetComponent<OverviewTab>().UpdateInformation();

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
        if (tabs[0].activeSelf) tabs[0].GetComponent<OverviewTab>().UpdateInformation();
    }

    public void PreviousCity(){
        int index = CityManager.instance.cities.IndexOf(currentCity);

        if(index == 0){
            index = CityManager.instance.cities.Count - 1;
        }else{
            index--;
        }

        OpenMenu(CityManager.instance.cities[index]);
        CityManager.instance.cities[index].transform.GetComponent<Followable>().LockOn();
    }

    public void NextCity(){
        int index = CityManager.instance.cities.IndexOf(currentCity);

        if(index == CityManager.instance.cities.Count - 1){
            index = 0;
        }else{
            index++;
        }

        OpenMenu(CityManager.instance.cities[index]);
        CityManager.instance.cities[index].transform.GetComponent<Followable>().LockOn();
    }

    void Update(){
        if (!ignoreClose && Input.GetMouseButtonDown(0) && !RaycastChecker.Check()){
             CloseMenu();
        }else{
            ignoreClose = false;
        }
    }

    public void RenameCity(){
        if (nameBox.text == ""){
            nameBox.text = currentCity.cityName;
        }else{
            currentCity.cityName = nameBox.text;
            currentCity.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = nameBox.text;
        }
    }
}
