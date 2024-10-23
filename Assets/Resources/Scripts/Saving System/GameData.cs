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

    public Tile(){
        coordinates = new Vector2Serialized(0, 0);
        type = HexTile.Type.Empty;
    }

    public Tile(HexTile hexTile){
        coordinates = new Vector2Serialized(hexTile.coordinates.x, hexTile.coordinates.y);
        type = hexTile.type;
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
    public List<SkillSerilaized> skills;

    public int skillPoints;

    public IndustrySerialized(){
        level = 0;
        itemName = new();
        outputPerWorker = new();
        skillTree = new();
        skills = new();
        skillPoints = 0;
    }

    public IndustrySerialized(string industryName, int level, List<string> itemName, List<float> outputPerWorker, string skillTreePath, List<string> skillPaths, int skillPoints){
        this.industryName = industryName;
        this.level = level;
        this.itemName = itemName;
        this.outputPerWorker = outputPerWorker;

        skillTree = new(skillTreePath);
        skills = new();
        foreach (string skillPath in skillPaths)
        {
            SkillSerilaized newSkill = new(skillPath);
            skills.Add(newSkill);
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

    public List<IndustrySerialized> industries;
    public List<int> workerAmount;

    public CitySerialized(){
        cityName = "New Sofia Default";
        population = 0;
        workers = 0;

        coordinates = new(0, 0);

        itemName = new();
        itemAmount = new();

        industries = new();
        workerAmount = new();
    }
}

[Serializable]
public class GameData
{ 
    public CameraData camera;
    public Map map;

    public List<CitySerialized> cities;

    public GameData(){
        camera = new();
        map = new(0);
        cities = new();
    }
}
