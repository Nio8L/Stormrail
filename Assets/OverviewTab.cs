using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverviewTab : MonoBehaviour
{
    public TextMeshProUGUI happinessText;

    void Update(){
        int happiness = Mathf.RoundToInt(CityMenu.instance.currentCity.overallHappiness * 100);
        happinessText.text = happiness.ToString() + "%";
    }
}
