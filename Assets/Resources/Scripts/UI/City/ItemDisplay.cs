using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public GameObject extraInfoMenuPrefab;
    GameObject extraInfoMenu;
    public TextMeshProUGUI nameTextBox;
    public TextMeshProUGUI amountTextBox;
    public Image iconObject;
    public Item itemToTrack;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (extraInfoMenu == null){
            extraInfoMenu = Instantiate(extraInfoMenuPrefab, EconomyTab.instance.content.transform);
            extraInfoMenu.transform.SetSiblingIndex(transform.GetSiblingIndex()+1);
            extraInfoMenu.GetComponent<EconomyDropDown>().Initialize(itemToTrack);
        }else{
            Destroy(extraInfoMenu);
        }
    }

    public void Initialize(Item _item){
        itemToTrack = _item;
        // Set the sprite of the display
        iconObject.sprite = itemToTrack.itemIcon;
        // Set the name of the display
        nameTextBox.text = itemToTrack.itemName;
        // Set the amount of the display
        if (CityMenu.instance.currentCity != null){
            amountTextBox.text = Mathf.RoundToInt(CityMenu.instance.currentCity.inventory[itemToTrack]) + "kg";
        }else if (StationMenu.instance.currentStation != null){
            amountTextBox.text = Mathf.RoundToInt(StationMenu.instance.currentStation.inventory[itemToTrack]) + "kg";
        }
    }

    private void OnEnable() {
        EventManager.CloseCity += SwitchingCity;
    }

    private void OnDisable() {
        EventManager.CloseCity -= SwitchingCity;
    }

    public void SwitchingCity(City currentCity){
        if(extraInfoMenu == null) return;
        Destroy(extraInfoMenu);
    }
}
