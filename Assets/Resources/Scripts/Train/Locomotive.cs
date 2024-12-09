using System.Collections.Generic;
using UnityEngine;

public class Locomotive : MonoBehaviour
{
    public Train train;
    public GameObject trainObject;
    public List<HexTile> currentPath;
    public Vector3 target;
    public Vector3 start;
    int currentIndex = 0;
    float moveTimer = 0;

    public bool move = false;
    public bool getRoute = false;

    private void Start() {
        train = new();
    }

    private void Update() {
        if(getRoute){
            train.SetRoute(TrainManager.instance.routes[0]);
            NextStop();
            getRoute = false;
        }

        if(!move) return;
        
        if(Vector3.Distance(trainObject.transform.position, target) < 0.01){
            if(currentIndex + 1 == currentPath.Count){
                //Debug.Log("finish"); 
                return;
            } 
            currentIndex++;   
            start = target;
            target = currentPath[currentIndex].transform.position;
            target.y = 0.5f;
            moveTimer = 0;
            trainObject.transform.rotation = Quaternion.Euler(0, MapManager.instance.GetAngle(start, target) - 180, 0);
        }

        Move();
        
    }

    public void NextStop(){
        currentPath = Pathfinder.instance.Pathfind(train.currentStop.city, train.currentRoute.NextStop(train.currentStop).city);
        currentIndex = 0;
        start = trainObject.transform.position;
        target = currentPath[0].transform.position;
    }

    public void Move(){
        //Debug.Log(Vector3.Distance(trainObject.transform.position, target));
        moveTimer += Time.deltaTime;
        trainObject.transform.position = Vector3.Lerp(start, target, moveTimer);
    }
}