using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route{
    public string name;
    public List<City> stops;

    public Route(){
        name = "New Route";
        stops = new();
    }
}

public class Stop{
    public City city;
    public List<Condition> conditions;

    public Stop(){
        city = CityManager.instance.cities[0];
        conditions = new();
    }
}

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

    public List<Route> routes;

    private void Awake() {
        instance = this;
    }

    public void CreateRoute(){
        routes.Add(new Route());
    }
}
