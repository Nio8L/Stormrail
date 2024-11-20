using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StopPlateUI : MonoBehaviour
{
    public TextMeshProUGUI stopName;

    public GameObject conditionObject;
    public GameObject conditionHolder;

    public void Initialize(string newName){
        stopName.text = newName;
    }

    public void CreateConditionObject(){
        GameObject newCondition = Instantiate(conditionObject, conditionHolder.transform);
    }

    public void AddCondition(){
        CreateConditionObject();
        Stop stop = TrainManager.instance.GetStop(TrainMenu.instance.selectedRoute, stopName.text);
        stop.conditions.Add(new());
    }
}
