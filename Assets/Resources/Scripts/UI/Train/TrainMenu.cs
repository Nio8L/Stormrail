using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMenu : MonoBehaviour
{
    public GameObject trainMenuObject;
    public GameObject routeObject;
    public GameObject routeHolder;

    public void OpenMenu(){
        trainMenuObject.SetActive(true);
    }

    public void CloseMenu(){
        trainMenuObject.SetActive(false);
    }

    public void AddRoute(){
        GameObject newRoute = Instantiate(routeObject);
        newRoute.transform.SetParent(routeHolder.transform);
        TrainManager.instance.CreateRoute();
    }

    public void SelectRoute(){
        
    }
}
