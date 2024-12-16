using System;
using System.Collections.Generic;

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

    public Tile(){
        coordinates = new Vector2Serialized(0, 0);
        type = HexTile.Type.Empty;
        angles = new();
    }

    public Tile(HexTile hexTile){
        coordinates = new Vector2Serialized(hexTile.coordinates.x, hexTile.coordinates.y);
        type = hexTile.type;
        angles = hexTile.angles;
    }
}

[Serializable]
public class Map{
    public Vector2Serialized mapSize;
    public ArrayWrapper[] tiles;

    public Map(int size){
        mapSize = new Vector2Serialized(size, size);
        tiles = new ArrayWrapper[size];

        for(int i = 0; i < size; i++){
            tiles[i] = new(size);
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

    public List<string> itemName;
    public List<float> outputPerWorker;

    public SkillTreeSerialized skillTree;
    public List<SkillSerilaized> unlockedSkills;
    public List<SkillSerilaized> activeSkills;
    public int skillPoints;

    public IndustrySerialized(){
        level = 0;
        itemName = new();
        outputPerWorker = new();
        skillTree = new();
        unlockedSkills = new();
        activeSkills = new();
        skillPoints = 0;
    }

    public IndustrySerialized(string industryName, int level, List<string> itemName, List<float> outputPerWorker, string skillTreePath, List<string> unlockedSkillPaths, List<string> activeSkillPaths, int skillPoints){
        this.industryName = industryName;
        this.level = level;
        this.itemName = itemName;
        this.outputPerWorker = outputPerWorker;

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

    public List<string> connections;

    public float eventTimer;
    public bool eventActive;

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
        eventActive = false;
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
        city = stop.city.cityName;
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
public class GameData
{ 
    public CameraData camera;
    public Map map;

    public List<CitySerialized> cities;
    public List<RouteSerialized> routes;
    public List<TrainSerialized> trains;

    public GameData(){
        camera = new();
        map = new(0);
        cities = new();
        routes = new();
        trains = new();
    }
}
