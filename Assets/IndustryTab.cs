using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndustryTab : MonoBehaviour
{
    public List<IndustryWindow> industryWindows;

    private void OnEnable() {
        EventManager.OpenCity += SwitchingCity;

        SwitchingCity(CityMenu.instance.currentCity);
    }

    private void OnDisable() {
        EventManager.OpenCity -= SwitchingCity;
    }

    public void SwitchingCity(City currentCity){
        for (int i = 0; i < industryWindows.Count; i++){
            IndustryWindow window = industryWindows[i];
            window.UpdateIndustry(currentCity.allIndustries[i]);
        }
    }
}
