using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EconomyDropDown : MonoBehaviour
{
    public TextMeshProUGUI produceText;
    public TextMeshProUGUI consumeText;
    Item itemToTrack;
    public void Initialize(Item _item){
        itemToTrack = _item;
    }

    void LateUpdate(){
        float productionThisSecond = CityMenu.instance.currentCity.CalculateProduction(itemToTrack);
        produceText.text = string.Format("{0:0.##}", productionThisSecond);

        float consumingThisSecond = CityMenu.instance.currentCity.consumingThisFrame[itemToTrack]/Time.deltaTime;
        consumeText.text = string.Format("{0:0.##}", consumingThisSecond);
    }
}
