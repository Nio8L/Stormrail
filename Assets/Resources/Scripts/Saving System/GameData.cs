using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArrayWrapper{
    public Tile[] array;

    public ArrayWrapper(int size){
        array = new Tile[size];
    }
}

[Serializable]
public class Vector2Serialized{
    public int x;
    public int y;

    public Vector2Serialized(int x, int y){
        this.x = x;
        this.y = y;
    }

    public Vector2Serialized(HexTile hexTile){
        x = hexTile.coordinates.x;
        y = hexTile.coordinates.y;
    }

    public Vector2Serialized(Vector2Int coordinates){
        x = coordinates.x;
        y = coordinates.y;
    }
}

[Serializable]
public class Vector3Serialized{
    public float x;
    public float y;
    public float z;

    public Vector3Serialized(float x, float y, float z){
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

[Serializable]
public class TransformSerialized{
    public Vector3Serialized position;
    public Vector3Serialized rotation;

    public TransformSerialized(){
        position = new(0, 0, 0);
        rotation = new(0, 0, 0);
    }

    public TransformSerialized(Vector3Serialized position, Vector3Serialized rotation){
        this.position = position;  
        this.rotation = rotation;
    }

    public TransformSerialized(float xPosition, float yPosition, float zPosition, float xRotation, float yRotation, float zRotation){
        position = new(xPosition, yPosition, zPosition);
        rotation = new(xRotation, yRotation, zRotation);
    }
}

[Serializable]
public class Tile{
    public Vector2Serialized coordinates;
    public HexTile.Type type;
    public List<int> angles;
    public int decorationIndex;
    public bool revealed;

    public Tile(){
        coordinates = new Vector2Serialized(0, 0);
        type = HexTile.Type.Forest;
        angles = new();
        decorationIndex = 0;
        revealed = false;
    }

    public Tile(HexTile hexTile){
        coordinates = new Vector2Serialized(hexTile.coordinates.x, hexTile.coordinates.y);
        type = hexTile.type;
        angles = hexTile.angles;
        decorationIndex = hexTile.decorationIndex;
        revealed = hexTile.revealed;
    }
}

[Serializable]
public class Map{
    public Vector2Serialized mapSize;
    public ArrayWrapper[] tiles;

    public Map(int sizeX, int sizeY){
        mapSize = new Vector2Serialized(sizeX, sizeY);
        tiles = new ArrayWrapper[sizeX];

        for(int i = 0; i < sizeX; i++){
            tiles[i] = new(sizeY);
        }
    }
}

[Serializable]
public class CameraData{
    public TransformSerialized cameraTransform;
    public TransformSerialized cameraRigTransform;

    private Vector3Serialized cameraStartPositon = new(0, 50, -50);
    private Vector3Serialized cameraStartRotation = new(45, 45, 0);

    private Vector3Serialized cameraRigStartPositon = new(3, 0, -10);
    private Vector3Serialized cameraRigStartRotation = new(0, 45, 0);

    public CameraData(){
        cameraTransform = new(cameraStartPositon, cameraStartRotation);
        cameraRigTransform = new(cameraRigStartPositon, cameraRigStartRotation);
    }

    public CameraData(TransformSerialized cameraTransform, TransformSerialized cameraRigTransform){
        this.cameraTransform = cameraTransform;
        this.cameraRigTransform = cameraRigTransform;
    }
}

[Serializable]
public class SkillSerilaized{
    public string skillPath;

    public SkillSerilaized(){
        skillPath = "";
    }

    public SkillSerilaized(string path){
        skillPath = path;
    }
}

[Serializable]
public class SkillTreeSerialized{
    public string skillTreePath;

    public SkillTreeSerialized(){
        skillTreePath = "";
    }

    public SkillTreeSerialized(string path){
        skillTreePath = path;
    }
}

[Serializable]
public class IndustrySerialized{
    public string industryName;
    public int level;
    public SkillTreeSerialized skillTree;
    public List<SkillSerilaized> unlockedSkills;
    public List<SkillSerilaized> activeSkills;
    public int skillPoints;

    public IndustrySerialized(){
        level = 0;
        skillTree = new();
        unlockedSkills = new();
        activeSkills = new();
        skillPoints = 0;
    }

    public IndustrySerialized(string industryName, int level, List<string> itemName, List<float> outputPerWorker, string skillTreePath, List<string> unlockedSkillPaths, List<string> activeSkillPaths, int skillPoints){
        this.industryName = industryName;
        this.level = level;

        skillTree = new(skillTreePath);
        unlockedSkills = new();
        foreach (string skillPath in unlockedSkillPaths)
        {
            SkillSerilaized newSkill = new(skillPath);
            unlockedSkills.Add(newSkill);
        }
        activeSkills = new();
        foreach (string skillPath in activeSkillPaths)
        {
            SkillSerilaized newSkill = new(skillPath);
            activeSkills.Add(newSkill);
        }
        this.skillPoints = skillPoints;
    } 
}

[Serializable]
public class CitySerialized{
    public string cityName;
    public int population;
    public int workers;

    public Vector2Serialized coordinates;

    public List<string> itemName;
    public List<float> itemAmount;

    public List<HappinessSource> happinessSources;

    public List<IndustrySerialized> industries;
    public List<int> workerAmount;

    public float hungerDrainModifier;
    public float hungerTimer;
    public bool starving;
    public float starvationDeathResetTimer;
    public float deathPenalty;

    public List<string> connections;

    public float eventTimer;
    public string activeEvent;
    public float eventBubbleTimer;

    public CitySerialized(){
        cityName = "New Sofia Default";
        population = 0;
        workers = 0;

        coordinates = new(0, 0);

        itemName = new();
        itemAmount = new();

        industries = new();
        workerAmount = new();

        happinessSources = new();
        
        hungerDrainModifier = 1;
        hungerTimer = 0;
        starving = false;

        connections = new();

        eventTimer = 0;
        activeEvent = "";
    }
}

[Serializable]
public class StationSerialized{
    public string stationName;
    public Vector2Serialized coordinates;

    public List<string> itemName;
    public List<float> itemAmount;

    public StationSerialized(){
        stationName = "Default";

        coordinates = new(0, 0);

        itemName = new();
        itemAmount = new();
    }
}

[Serializable]
public class TrainSerialized{
    public string name;
    public string route;
    public string stop;
    public float speed;

    public int currentIndex;
    public Vector2Serialized cameFrom;
    public Vector2Serialized goingTo;

    public List<string> items = new();
    public List<float> amounts = new();

    public TrainSerialized(){
        name = "";
        route = "";
        stop = "";
        speed = 1;
        currentIndex = 0;
    }

    public TrainSerialized(Train train){
        name = train.name;
        route = train.currentRoute.name;
        stop = train.currentStop.name;
        speed = train.speed;
        
        currentIndex = train.currentIndex;
        cameFrom = new(train.cameFrom.x, train.cameFrom.y);
        goingTo = new(train.goingTo.x, train.goingTo.y);

        items = new();
        amounts = new();
        foreach (Item item in train.inventory.Keys)
        {
            items.Add(item.itemName);
            amounts.Add(train.inventory[item]);
        }
    }
}

[Serializable]
public class RouteSerialized{
    public string name;
    public List<StopSerialized> stops;

    public RouteSerialized(){
        name = "";
        stops = new();
    }
    
    public RouteSerialized(Route route){
        name = route.name;
        stops = new();
        foreach (Stop stop in route.stops)
        {
            StopSerialized stopToSerialize = new(stop);
            stops.Add(stopToSerialize);
        }
    }
}

[Serializable]
public class StopSerialized{
    public string city;
    public string name;
    public List<ConditionSerialized> conditions;

    public StopSerialized(){
        city = "";
        name = "";
        conditions = new();
    }

    public StopSerialized(Stop stop){
        city = stop.station.cityName;
        name = stop.name;
        conditions = new();
        foreach (Condition condition in stop.conditions)
        {
            ConditionSerialized conditionToSerialize = new(condition);
            conditions.Add(conditionToSerialize);
        }
    }
}

[Serializable]
public class ConditionSerialized{
    public bool load;
    public string item;
    public int amount;

    public ConditionSerialized(){
        load = false;
        item = "Food";
        amount = 0;
    }

    public ConditionSerialized(Condition condition){
        load = condition.load;
        item = condition.item.itemName;
        amount = condition.amount;
    }
}

[Serializable]
public class ExplorerSerialized{
    public string name;
    public int revealRadius;
    public float speed;

    public int foodSupply;

    public Vector2Serialized cameFrom;
    public Vector2Serialized goingTo;

    public ExplorerSerialized(){
        revealRadius = 0;
        speed = 1;
        foodSupply = 0;
    }

    public ExplorerSerialized(Explorer explorer){
        name = explorer.unitName;
        revealRadius = explorer.revealRadius;
        speed = explorer.speed;

        if(explorer.currentPath.Count == 0){
            cameFrom = new(explorer.coordinates);
            goingTo = new(explorer.coordinates);
        }else{
            cameFrom = new(explorer.currentPath[explorer.currentIndex]);
            goingTo = new(explorer.currentPath[^1]);
        }

    }
}

[Serializable]
public class BuilderSerialized{
    public string name;
    public float speed;

    public int foodSupply;
    public int steelSupply;

    public Vector2Serialized cameFrom;
    public Vector2Serialized goingTo;

    public BuilderSerialized(){
        speed = 1;
        foodSupply = 0;
        steelSupply = 0;
    }

    public BuilderSerialized(Builder builder){
        name = builder.unitName;
        speed = builder.speed;

        foodSupply = builder.foodSupply;
        steelSupply = builder.steelSupply;

        if(builder.currentPath.Count == 0){
            cameFrom = new(builder.coordinates);
            goingTo = new(builder.coordinates);
        }else{
            cameFrom = new(builder.currentPath[builder.currentIndex]);
            goingTo = new(builder.currentPath[^1]);
        }
    }
}

[Serializable]
public class TimeSerialized{
    public enum Speed{
        Slow,
        Normal,
        Fast
    }

    public float speed;
    public int day;
    public float time;

    public TimeSerialized(){
        speed = 1;
        day = 0;
        time = 0;
    }

    public TimeSerialized(TimeControl timeControl){
        speed = Time.timeScale;
        day = timeControl.day;
        time = timeControl.time;
    }
}

[Serializable]
public class GameData
{ 
    public CameraData camera;
    public Map map;
    public TimeSerialized time;

    public List<CitySerialized> cities;
    public List<StationSerialized> stations;
    public List<ExplorerSerialized> explorers;
    public List<BuilderSerialized> builders;

    public List<RouteSerialized> routes;
    public List<TrainSerialized> trains;


    public GameData(){
        camera = new();
        map = new(0, 0);
        time = new();

        cities = new();
        stations = new();
        explorers = new();
        builders = new();
        
        routes = new();
        trains = new();
    }
}
