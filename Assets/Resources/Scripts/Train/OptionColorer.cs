using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionColorer : MonoBehaviour
{
    string cityName;
    int index;

    public void Initialize(int newIndex){
        /*index = newIndex;

        cityName = transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;

        if(index != 0){
            City myCity = CityManager.instance.GetCityByName(cityName);
            City previousCity = CityManager.instance.GetCityByName(TrainMenu.instance.selectedRoute.stops[index - 1].name);
            if(myCity.connections.Contains(previousCity)){
                transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.red;
            }
        }*/
    }

    private void Start() {
        
    }
}
