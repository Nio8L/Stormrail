using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Train{
    public Route currentRoute;
    public Stop currentStop;
    public float speed;

    public Train(){
        currentRoute = new();
        speed = 1;
        currentStop = new Stop();
    }

    public Train(Route newRoute){
        currentRoute = newRoute;
        speed = 1;
        currentStop = currentRoute.stops[0];
    }

    public void SetRoute(Route newRoute){
        currentRoute = newRoute;
        currentStop = currentRoute.stops[0];
    }
}

[System.Serializable]
public class Route{
    public string name;
    public List<Stop> stops;

    public Route(){
        name = "";
        stops = new();
    }

    public Route(string routeName){
        name = routeName;
        stops = new();
    }

    public Stop NextStop(Stop currentStop){
        int index = stops.IndexOf(currentStop);
        if(index + 1 < stops.Count){
            return stops[index + 1];
        }
        return null;
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
        amount = 0;
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
        string checker = routeName;
        int counter = 0;
        while(GetRoute(checker) != null){
            counter++;
            checker = routeName + "_" + counter;
        }
        
        routes.Add(new Route(checker));
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
