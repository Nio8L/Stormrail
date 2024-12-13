using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StopPlateUI : MonoBehaviour
{
    public int index;
    public TMP_Dropdown cityList; 
    public string cityName;

    public GameObject conditionObject;
    public GameObject conditionHolder;

    public void Initialize(int newIndex){
        index = newIndex;
        cityName = TrainMenu.instance.selectedRoute.stops[index].name;
        List<string> options = new()
        {
            "No City"
        };
        int cityIndex = 0;

        for(int i = 0; i < CityManager.instance.cities.Count; i++){
            options.Add(CityManager.instance.cities[i].cityName);
            if(CityManager.instance.cities[i].cityName == cityName){
                cityIndex = i + 1;
            }
        }

        cityList.AddOptions(options);
        cityList.value = cityIndex;
        cityName = cityList.options[cityList.value].text;
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
        Stop stop = TrainMenu.instance.selectedRoute.stops[index];
        stop.conditions.Add(new());
    }

    public void DeleteStop(){
        TrainMenu.instance.DeleteStop(this);
    }

    public void DeleteCondition(ConditionPlateUI conditionToDelete){
        for(int i = 0; i < conditionHolder.transform.childCount; i++){
            if(conditionHolder.transform.GetChild(i).GetComponent<ConditionPlateUI>() == conditionToDelete){
                Destroy(conditionHolder.transform.GetChild(i).gameObject);
                TrainManager.instance.GetStop(TrainMenu.instance.selectedRoute, cityName).conditions.RemoveAt(i);
            }
        }
    }

    public void ChangeStop(){
        for(int i = 0; i < TrainMenu.instance.selectedRoute.stops.Count; i++){
            if(i == index){
                TrainMenu.instance.selectedRoute.stops[i].name = cityList.options[cityList.value].text;
                cityName = cityList.options[cityList.value].text;
                TrainMenu.instance.selectedRoute.stops[i].city = CityManager.instance.GetCityByName(cityName);
            }
        }
    }

    public void ChangeAction(ConditionPlateUI condition, bool newAction){
        for(int i = 0; i < conditionHolder.transform.childCount; i++){
            if(conditionHolder.transform.GetChild(i).GetComponent<ConditionPlateUI>() == condition){
                TrainMenu.instance.selectedRoute.stops[index].conditions[i].load = newAction;
            }
        }
    }

    public void ChangeAmount(ConditionPlateUI condition, int newAmount){
        for(int i = 0; i < conditionHolder.transform.childCount; i++){
            if(conditionHolder.transform.GetChild(i).GetComponent<ConditionPlateUI>() == condition){
                TrainMenu.instance.selectedRoute.stops[index].conditions[i].amount = newAmount;
            }
        }
    }

    public void ChangeItem(ConditionPlateUI condition, Item newItem){
        for(int i = 0; i < conditionHolder.transform.childCount; i++){
            if(conditionHolder.transform.GetChild(i).GetComponent<ConditionPlateUI>() == condition){
                TrainMenu.instance.selectedRoute.stops[index].conditions[i].item = newItem;
            }
        }
    }
}
