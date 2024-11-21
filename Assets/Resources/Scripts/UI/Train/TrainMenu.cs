using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrainMenu : MonoBehaviour
{
    public static TrainMenu instance;

    [Header("Objects and Holders")]
    public GameObject trainMenuObject;
    public GameObject routeHolder;
    public GameObject stopHolder;

    [Header("Route Window")]
    public Route selectedRoute;
    public TextMeshProUGUI routeName;

    [Header("Instantiables")]
    public GameObject routeObject;
    public GameObject stopObject;

    private void Awake() {
        instance = this;
    }

    public void OpenMenu(){
        trainMenuObject.SetActive(true);
    }

    public void CloseMenu(){
        trainMenuObject.SetActive(false);
    }

    public void AddRoute(){
        GameObject newRoute = Instantiate(routeObject);
        newRoute.GetComponent<RoutePlateUI>().Initialize(TrainManager.instance.routes.Count + 1 + "");
        newRoute.transform.SetParent(routeHolder.transform);
        TrainManager.instance.CreateRoute(TrainManager.instance.routes.Count + 1 + "");
    }

    public void AddStop(){
        if(selectedRoute.name == "") return;
        GameObject newStop = Instantiate(stopObject, stopHolder.transform);
        newStop.GetComponent<StopPlateUI>().Initialize(selectedRoute.stops.Count + 1 + "");
        selectedRoute.stops.Add(new(selectedRoute.stops.Count + 1 + ""));
    }

    public void DeleteStops(){
        for(int i = 0; i < stopHolder.transform.childCount; i++){
            Destroy(stopHolder.transform.GetChild(i).gameObject);
        }
    }

    public void DeleteStop(StopPlateUI stopToDelete){
        for(int i = 0; i < stopHolder.transform.childCount; i++){
            if(stopHolder.transform.GetChild(i).GetComponent<StopPlateUI>() == stopToDelete){
                selectedRoute.stops.Remove(TrainManager.instance.GetStop(selectedRoute, stopHolder.transform.GetChild(i).GetComponent<StopPlateUI>().stopName.text));
                Destroy(stopHolder.transform.GetChild(i).gameObject);
            }
        }
    }

    public void AddStops(){
        for(int i = 0; i < selectedRoute.stops.Count; i++){
            GameObject newStop = Instantiate(stopObject, stopHolder.transform);
            StopPlateUI stopScript = newStop.GetComponent<StopPlateUI>();
            stopScript.Initialize(selectedRoute.stops[i].name);
            
            for(int j = 0; j < selectedRoute.stops[i].conditions.Count; j++){
                stopScript.CreateConditionObject(selectedRoute.stops[i].conditions[j]);
            }
        }
    }

    public void BuildMode(){
        TrainManager.instance.buildMode = true;
    }
}
