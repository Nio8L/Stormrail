using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Route{
    public string name;
    public List<Stop> stops;

    public Route(){
        name = "New Route";
        stops = new();
    }

    public Route(string routeName){
        name = routeName;
        stops = new();
    }
}

[System.Serializable]
public class Stop{
    public City city;
    public string name;
    public List<Condition> conditions;

    public Stop(){
        city = null;
        name = "";
        conditions = new();
    }

    public Stop(string stopName){
        city = null;
        name = stopName;
        conditions = new();
    }
}

[System.Serializable]
public class Condition{
    public bool load;
    public Item item;
    public int amount;

    public Condition(){
        load = true;
        item = DataBase.instance.allItems[0];
        amount = 1;
    }
}

public class TrainManager : MonoBehaviour
{
    public static TrainManager instance;

    public bool buildMode = false;
    public City[] citiesToConnect = new City[2];

    public List<Route> routes = new();

    private void Update() {
        if(Input.GetMouseButton(1)) buildMode = false;
    }

    private void Awake() {
        instance = this;
    }

    public void CreateRoute(){
        routes.Add(new Route());
    }

    public void CreateRoute(string routeName){
        if(GetRoute(routeName) != null) return;
        routes.Add(new Route(routeName));
    }

    public Route GetRoute(string routeName){
        foreach (Route route in routes)
        {
            if(route.name == routeName){
                return route;
            }
        }
        return null;
    }

    public Stop GetStop(Route route, string stopName){
        foreach (Stop stop in route.stops)
        {
            if(stop.name == stopName){
                return stop;
            }
        }
        return null;
    }
}
