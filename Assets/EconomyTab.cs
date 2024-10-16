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
    City currentCity;
    void Awake(){
        instance = this;
    }
    void OnEnable(){
        currentCity = CityMenu.instance.currentCity;

        for (int i = 0; i < currentCity.inventory.Count; i++){
            KeyValuePair<Item, int> pair = currentCity.inventory.ElementAt(i);
            // Set the sprite of the display
            itemDisplays[i].iconObject.sprite = pair.Key.itemIcon;
            // Set the name of the display
            itemDisplays[i].nameTextBox.text = pair.Key.itemName;
            // Set the amount of the display
            itemDisplays[i].amountTextBox.text = pair.Value.ToString() + "kg";
        }
    }
    void Update(){
         for (int i = 0; i < currentCity.inventory.Count; i++){
            KeyValuePair<Item, int> pair = currentCity.inventory.ElementAt(i);
            // Set the amount of the display
            itemDisplays[i].amountTextBox.text = pair.Value.ToString() + "kg";
        }
    }
}
