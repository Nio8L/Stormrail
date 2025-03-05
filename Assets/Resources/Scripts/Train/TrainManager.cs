using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Train{
    public string name;
    public Route currentRoute;
    public Stop currentStop;
    public float speed;
    public int currentIndex;

    public Vector2Int cameFrom;
    public Vector2Int goingTo;
    public Dictionary<Item, float> inventory = new();

    public Train(){
        name = "";
        currentRoute = new();
        speed = 1;
        currentStop = new Stop();
        
        currentIndex = 0;  
    }

    public Train(string _name){
        name = _name;
        currentRoute = new();
        speed = 1;
        currentStop = new Stop();
        
        currentIndex = 0;   
    }

    public void SetRoute(Route newRoute){
        currentRoute = newRoute;
        currentStop = currentRoute.stops[0];
        
        currentIndex = 0;
    }

    public void CompleteCondition(Station station, Condition condition){ 
        Debug.Log("complete "  + condition.amount);
        if(condition.load){
            if(station.inventory[condition.item] < condition.amount){         
                inventory[condition.item] += station.inventory[condition.item];
                station.ConsumeResource(condition.item, station.inventory[condition.item]);
            }else{
                inventory[condition.item] += condition.amount;
                station.inventory[condition.item] -= condition.amount;
                station.ConsumeResource(condition.item, condition.amount);
            }
        }else{
            if(inventory[condition.item] < condition.amount){
                station.GainResource(condition.item, inventory[condition.item]);
                inventory[condition.item] -= inventory[condition.item];
            }else{
                station.GainResource(condition.item, condition.amount);
                inventory[condition.item] -= condition.amount;
            }
        }
    }

    public void CompleteAllConditions(Station station, Stop stop){
       Debug.Log("complete all");
        foreach (Condition condition in stop.conditions)
        {
            CompleteCondition(station, condition);
        }
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
        return stops[0];
    }
}

[System.Serializable]
public class Stop{
    public Station station;
    public string name;
    public List<Condition> conditions;

    public Stop(){
        station = null;
        name = "";
        conditions = new();
    }

    public Stop(string stopName){
        station = null;
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

public class TrainManager : MonoBehaviour, ISavable
{
    public static TrainManager instance;

    public GameObject trainPrefab;

    public List<Route> routes = new();
    public List<Train> trains = new();
    public List<Locomotive> locomotives = new();
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

    public Train GetTrain(string trainName){
        foreach (Train train in trains)
        {
            if(train.name == trainName){
                return train;
            }
        }
        return null;
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


    public void InstantiateTrain(TrainSerialized train){
        GameObject newTrain = Instantiate(trainPrefab);
        newTrain.transform.position = new Vector3(newTrain.transform.position.x, 0.3f, newTrain.transform.position.z);
        Locomotive locomotive = newTrain.GetComponent<Locomotive>();
        locomotive.trainObject = newTrain;
        locomotive.LoadTrain(train);
        locomotives.Add(locomotive);
    }

    public void InstantiateTrain(TrainSerialized train, Station city){  
        GameObject newTrain = Instantiate(trainPrefab, city.transform.position, Quaternion.identity);
        newTrain.transform.position = new Vector3(newTrain.transform.position.x, 0.3f, newTrain.transform.position.z);
        Locomotive locomotive = newTrain.GetComponent<Locomotive>();
        locomotive.trainObject = newTrain;
        locomotive.LoadTrain(train);
        locomotives.Add(locomotive);
    }

    public void LoadData(GameData data)
    {
        for(int i = 0; i < data.routes.Count; i++){
            RouteSerialized routeSerialized = data.routes[i];
            Route routeToLoad = new();
            routeToLoad.name = routeSerialized.name;
            
            List<Stop> stopsToLoad = new();

            for(int j = 0; j < routeSerialized.stops.Count; j++){
                StopSerialized stopSerialized = routeSerialized.stops[j];
                Stop stopToLoad = new();
                stopToLoad.station = CityManager.instance.GetCity(stopSerialized.city);
                stopToLoad.name = stopSerialized.name;
                
                List<Condition> conditionsToLoad = new();

                for(int k = 0; k < stopSerialized.conditions.Count; k++){
                    ConditionSerialized conditionSerialized = stopSerialized.conditions[k];
                    Condition conditionToLoad = new();
                    
                    conditionToLoad.load = conditionSerialized.load;
                    conditionToLoad.amount = conditionSerialized.amount;
                    conditionToLoad.item = DataBase.instance.GetItem(conditionSerialized.item);

                    conditionsToLoad.Add(conditionToLoad);
                }
                stopToLoad.conditions = conditionsToLoad;

                stopsToLoad.Add(stopToLoad);
            }
            routeToLoad.stops = stopsToLoad;

            routes.Add(routeToLoad);
        }
        
        for(int i = 0; i < data.trains.Count; i++){
            InstantiateTrain(data.trains[i]);
        }
    }

    public void SaveData(GameData data)
    {
        
        data.routes = new();

        for(int i = 0; i < routes.Count; i++){
            RouteSerialized routeToSave = new(routes[i]);
            data.routes.Add(routeToSave);
        }

        data.trains = new();

        for(int i = 0; i < trains.Count; i++){
            TrainSerialized trainToSave = new(trains[i]);
            data.trains.Add(trainToSave);
        }
    }

    public int GetPriority()
    {
        return 2;
    }
}
