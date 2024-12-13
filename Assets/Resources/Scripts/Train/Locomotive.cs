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
        Invoke("FirstMove", 0.01f);
    }

    public void FirstMove(){
        currentPath = Pathfinder.instance.PathfindOnRails(train.currentStop.city, train.currentRoute.NextStop(train.currentStop).city);
    
        if(currentPath != null && currentPath.Count > 1){
            trainObject.transform.position = currentPath[train.currentIndex].transform.position;
            move = true;
        }
        
        TrainManager.instance.trains.Add(train);
    }

    public void LoadTrain(TrainSerialized trainSerialized){
        //train = new(TrainManager.instance.GetRoute(trainSerialized.route));
        train.currentRoute = TrainManager.instance.GetRoute(trainSerialized.route);
        train.currentStop = TrainManager.instance.GetStop(train.currentRoute, trainSerialized.stop);
        train.speed = trainSerialized.speed;

        train.currentIndex = trainSerialized.currentIndex;
        
        startCoordinates = new(trainSerialized.cameFrom.x, -trainSerialized.cameFrom.y);
        targetCoordinates = new(trainSerialized.goingTo.x, -trainSerialized.goingTo.y);
    
        start = MapManager.instance.GetPositionForHexFromCoordinate(startCoordinates);
        target = MapManager.instance.GetPositionForHexFromCoordinate(targetCoordinates);

        for(int i = 0; i < trainSerialized.items.Count; i++){
            Item item = DataBase.instance.GetItem(trainSerialized.items[i]);
            train.inventory[item] = trainSerialized.amounts[i];
        }
    }

    private void Update() {
        //Debug.Log(currentIndex + 1 >= currentPath.Count);
        if(getRoute){
            train.SetRoute(TrainManager.instance.routes[routeIndex]);
            getRoute = false;
            //NextStop();
        }

        if(!move) return;
        Debug.Log(train.inventory[DataBase.instance.allItems[0]]);
        if(Vector3.Distance(trainObject.transform.position, target) < 0.01){
            if(currentPath == null || currentPath.Count == 0){
                NextStop();
            }else if(train.currentIndex + 1 >= currentPath.Count){
                train.currentStop = train.currentRoute.NextStop(train.currentStop);
                train.CompleteAllConditions(train.currentStop.city, train.currentStop);
                NextStop();
            }else{
                train.cameFrom = currentPath[train.currentIndex];
                train.goingTo = currentPath[train.currentIndex + 1];

                train.currentIndex++;
                
                start = target;
                target = currentPath[train.currentIndex].transform.position;
                target.y = 0.5f;

                moveTimer = 0;
                trainObject.transform.rotation = Quaternion.Euler(0, MapManager.instance.GetAngle(start, target) - 180, 0);
            }
        }

        Move();
        
    }

    public void NextStop(){
        currentPath = Pathfinder.instance.PathfindOnRails(train.currentStop.city, train.currentRoute.NextStop(train.currentStop).city);

        train.currentIndex = 0;
        start = trainObject.transform.position;
        target = currentPath[train.currentIndex].transform.position;

        train.cameFrom = currentPath[0];
        train.goingTo = currentPath[1];
    }

    public void Move(){
        moveTimer += Time.deltaTime;
        trainObject.transform.position = Vector3.Lerp(start, target, moveTimer);
    }
}