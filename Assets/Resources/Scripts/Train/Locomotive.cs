using System.Collections.Generic;
using UnityEngine;

public class Locomotive : MonoBehaviour
{
    public Train train;
    public GameObject trainObject;
    public List<HexTile> currentPath = new();
    public Vector3 target;
    public Vector3 start;
    public Vector2Int startCoordinates;
    public Vector2Int targetCoordinates;
    float moveTimer = 0;

    public bool move = false;
    public bool getRoute = false;
    public int routeIndex = 0;

    private void Start() {
        FirstMove();
        if (train.inventory.Count != 0) return;
        
        foreach (Item item in DataBase.instance.allItems)
        {
            train.inventory.Add(item, 0);
        } 
    }

    public void FirstMove(){
        if (train.currentRoute.name == "") return;
        train.currentStop = train.currentRoute.stops[0];
        currentPath = Pathfinder.instance.PathfindOnRails(train.currentStop.station, train.currentRoute.NextStop(train.currentStop).station);
    
        if(currentPath != null && currentPath.Count > 1){
            trainObject.transform.position = currentPath[train.currentIndex].transform.position;
            move = true;
        }
    }
    public void LoadTrain(TrainSerialized trainSerialized){
        Debug.Log(trainSerialized.cameFrom.x + " "  + trainSerialized.cameFrom.y);
        TrainManager.instance.trains.Add(train);
        train.name = trainSerialized.name;
        for(int i = 0; i < trainSerialized.items.Count; i++){
            Item item = DataBase.instance.GetItem(trainSerialized.items[i]);
            train.inventory[item] = trainSerialized.amounts[i];
        }

        startCoordinates = new(trainSerialized.cameFrom.x, -trainSerialized.cameFrom.y);
        targetCoordinates = new(trainSerialized.cameFrom.x, -trainSerialized.cameFrom.y);
        start = MapManager.instance.GetPositionForHexFromCoordinate(startCoordinates);
        target = MapManager.instance.GetPositionForHexFromCoordinate(targetCoordinates);
        if(TrainManager.instance.GetRoute(trainSerialized.route) == null) return;
        
        //train = new(TrainManager.instance.GetRoute(trainSerialized.route));
        train.currentRoute = TrainManager.instance.GetRoute(trainSerialized.route);
        train.currentStop = TrainManager.instance.GetStop(train.currentRoute, trainSerialized.stop);
        train.speed = trainSerialized.speed;

        train.currentIndex = trainSerialized.currentIndex;
        
        targetCoordinates = new(trainSerialized.goingTo.x, -trainSerialized.goingTo.y);
    
        target = MapManager.instance.GetPositionForHexFromCoordinate(targetCoordinates);
    }

    private void Update() {
        if(getRoute){
            train.SetRoute(TrainManager.instance.routes[routeIndex]);
            getRoute = false;
            //NextStop();
        }

        if(!move) return;

        if(Vector3.Distance(trainObject.transform.position, target) < 0.01){
            if(currentPath == null || currentPath.Count == 0){
                NextStop();
            }else if(train.currentIndex + 1 >= currentPath.Count){
                train.currentStop = train.currentRoute.NextStop(train.currentStop);
                train.CompleteAllConditions(train.currentStop.station, train.currentStop);
                NextStop();
            }else{
                train.cameFrom.x = currentPath[train.currentIndex].coordinates.x;
                train.cameFrom.y = currentPath[train.currentIndex].coordinates.y;
                train.goingTo.x = currentPath[train.currentIndex + 1].coordinates.x;
                train.goingTo.y = currentPath[train.currentIndex + 1].coordinates.y;

                train.currentIndex++;
                
                start = target;
                target = currentPath[train.currentIndex].transform.position;
                target.y = 0.3f;

                moveTimer = 0;
                trainObject.transform.rotation = Quaternion.Euler(0, MapManager.instance.GetAngle(start, target) - 180, 0);
            }
        }

        Move();
        
    }

    public void NextStop(){
        currentPath = Pathfinder.instance.PathfindOnRails(train.currentStop.station, train.currentRoute.NextStop(train.currentStop).station);

        train.currentIndex = 0;
        if (train.currentStop.station == train.currentRoute.NextStop(train.currentStop).station){
            train.currentStop = train.currentRoute.NextStop(train.currentStop);
            train.CompleteAllConditions(train.currentStop.station, train.currentStop);
            NextStop();
            return;
        }

        start = trainObject.transform.position;
        target = currentPath[train.currentIndex].transform.position;
        target.y = 0.3f;

        train.cameFrom.x = currentPath[0].coordinates.x;
        train.cameFrom.y = currentPath[0].coordinates.y;
        train.goingTo.x = currentPath[1].coordinates.x;
        train.goingTo.y = currentPath[1].coordinates.y;
    }

    public void Move(){
        moveTimer += Time.deltaTime;
        trainObject.transform.position = Vector3.Lerp(start, target, moveTimer);
    }
}