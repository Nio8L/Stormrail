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
        newCondition.GetComponent<ConditionPlateUI>().Initialize(this);
    }

    public void CreateConditionObject(Condition condition){
        GameObject newCondition = Instantiate(conditionObject, conditionHolder.transform);
        newCondition.GetComponent<ConditionPlateUI>().Initialize(this, condition);
    }

    public void AddCondition(){
        CreateConditionObject();
        Stop stop = TrainManager.instance.GetStop(TrainMenu.instance.selectedRoute, stopName.text);
        stop.conditions.Add(new());
    }

    public void DeleteStop(){
        TrainMenu.instance.DeleteStop(this);
    }

    public void DeleteCondition(ConditionPlateUI conditionToDelete){
        for(int i = 0; i < conditionHolder.transform.childCount; i++){
            if(conditionHolder.transform.GetChild(i).GetComponent<ConditionPlateUI>() == conditionToDelete){
                Destroy(conditionHolder.transform.GetChild(i).gameObject);
                TrainManager.instance.GetStop(TrainMenu.instance.selectedRoute, stopName.text).conditions.RemoveAt(i);
            }
        }
    }

    public void ChangeAction(ConditionPlateUI condition, bool newAction){
        for(int i = 0; i < conditionHolder.transform.childCount; i++){
            if(conditionHolder.transform.GetChild(i).GetComponent<ConditionPlateUI>() == condition){
                TrainManager.instance.GetStop(TrainMenu.instance.selectedRoute, stopName.text).conditions[i].load = newAction;
            }
        }
    }

    public void ChangeAmount(ConditionPlateUI condition, int newAmount){
        for(int i = 0; i < conditionHolder.transform.childCount; i++){
            if(conditionHolder.transform.GetChild(i).GetComponent<ConditionPlateUI>() == condition){
                TrainManager.instance.GetStop(TrainMenu.instance.selectedRoute, stopName.text).conditions[i].amount = newAmount;
            }
        }
    }

    public void ChangeItem(ConditionPlateUI condition, Item newItem){
        for(int i = 0; i < conditionHolder.transform.childCount; i++){
            if(conditionHolder.transform.GetChild(i).GetComponent<ConditionPlateUI>() == condition){
                TrainManager.instance.GetStop(TrainMenu.instance.selectedRoute, stopName.text).conditions[i].item = newItem;
            }
        }
    }
}
