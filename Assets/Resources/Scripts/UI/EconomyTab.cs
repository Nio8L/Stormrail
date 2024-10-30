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
    public City currentCity;
    void Awake(){
        instance = this;
    }

    private void OnEnable() {
        EventManager.OpenCity += SwitchingCity;

        SwitchingCity(CityMenu.instance.currentCity);
    }

    private void OnDisable() {
        EventManager.OpenCity -= SwitchingCity;
    }

    public void SwitchingCity(City newCity){
        currentCity = newCity;

        for (int i = 0; i < currentCity.inventory.Count; i++){
            KeyValuePair<Item, float> pair = currentCity.inventory.ElementAt(i);
            itemDisplays[i].Initialize(pair.Key);
        }
    }

    void Update(){
        if (currentCity == null) return;
        for (int i = 0; i < currentCity.inventory.Count; i++){
            KeyValuePair<Item, float> pair = currentCity.inventory.ElementAt(i);
            // Set the amount of the display
            itemDisplays[i].amountTextBox.text = Mathf.RoundToInt(pair.Value) + "kg";
        }
    }
}
