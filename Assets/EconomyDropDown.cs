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
        produceText.text = productionThisSecond.ToString();

        float consumingThisSecond = CityMenu.instance.currentCity.consumingThisFrame[itemToTrack]/Time.deltaTime;
        consumeText.text = consumingThisSecond.ToString();
    }
}
