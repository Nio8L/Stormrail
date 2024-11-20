using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoutePlateUI : MonoBehaviour
{
    public TextMeshProUGUI routeName;

    public void Initialize(string newName){
        routeName.text = newName;
    }

    public void SelectRoute(){
        TrainMenu.instance.DeleteStops();
        TrainMenu.instance.routeName.text = routeName.text;
        TrainMenu.instance.selectedRoute = TrainManager.instance.GetRoute(routeName.text);
        TrainMenu.instance.AddStops();
    }
}
