using System;

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
    private Vector3Serialized cameraStartRotation = new(45, 0, 0);

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
public class GameData
{ 
    public CameraData camera;
    public Map map;

    public GameData(){
        camera = new();
        map = new(0);
    }
}
