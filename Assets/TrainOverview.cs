using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrainOverview : MonoBehaviour
{
    public TMP_Dropdown routeDropdown;
    public void Initialize(){
        routeDropdown.ClearOptions();
        List<string> options = new()
        {
            "No Route"
        };

        for(int i = 0; i < TrainManager.instance.routes.Count; i++){
            options.Add(TrainManager.instance.routes[i].name);
        }

        routeDropdown.AddOptions(options);
        //routeDropdown.value = cityIndex;
    }
}
