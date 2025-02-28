using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EconomyTab : MonoBehaviour
{
    public static EconomyTab instance;
    public GameObject content;
    public List<ItemDisplay> itemDisplays;
    public Station currentStation;
    void Awake(){
        instance = this;
    }

    private void OnEnable() {
        EventManager.OpenCity += SwitchingCity;

        if (CityMenu.instance != null && CityMenu.instance.currentCity != null)
            SwitchingCity(CityMenu.instance.currentCity);
        else if (StationMenu.instance != null && StationMenu.instance.currentStation != null)
            SwitchingCity(StationMenu.instance.currentStation);
    }

    private void OnDisable() {
        EventManager.OpenCity -= SwitchingCity;
    }

    public void SwitchingCity(Station newStation){
        currentStation = newStation;

        for (int i = 0; i < currentStation.inventory.Count; i++){
            KeyValuePair<Item, float> pair = currentStation.inventory.ElementAt(i);
            itemDisplays[i].Initialize(pair.Key);
        }
    }

    void Update(){
        if (currentStation == null) return;
        for (int i = 0; i < currentStation.inventory.Count; i++){
            KeyValuePair<Item, float> pair = currentStation.inventory.ElementAt(i);
            // Set the amount of the display
            itemDisplays[i].amountTextBox.text = Mathf.RoundToInt(pair.Value) + "kg";
        }
    }
}
