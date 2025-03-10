using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour, ISavable
{
    public static MapManager instance;

    [Header("Grid Settings")]
    public Vector2Int gridSize;
    public GameObject hexPrefab;

    private List<GameObject> tileObjects = new();
    
    [Header("Tiles")]
    public HexTile[,] tiles;
    public HexTile hoveredTile;
    public List<Material> materials;

    [Header("Rails")]
    public GameObject railPrefab;

    [Header("Other")]
    public List<GameObject> decorationsForest;
    public List<GameObject> decorationsMountain;
    public List<GameObject> decorationsCity;
    public List<GameObject> decorationsStation;

    private void Awake() {
        if(instance != null){
            Debug.LogError("More than one Map Manager found!");
        }
        
        instance = this;
        
        // Map loader
        if (MapLoader.instance != null && MapLoader.instance.loadingEditor){
            gridSize = MapLoader.instance.mapSize;
        }


        tiles = new HexTile[gridSize.x, gridSize.y];
    }

    private void Start() {
        //LayoutGrid();
    }

    public void LayoutGrid(){
        foreach (GameObject tile in tileObjects)
        {
            Destroy(tile);
        }
        
        tileObjects.Clear();
        tiles = new HexTile[gridSize.x, gridSize.y];
        
        for(int x = 0; x < gridSize.x; x++){
            for(int y = 0; y < gridSize.y; y++){
                GameObject hex = Instantiate(hexPrefab, transform.position, Quaternion.identity);
                hex.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, -y));
                hex.transform.rotation = Quaternion.Euler(-90, -90, 0);
                tileObjects.Add(hex);
                
                tiles[x, y] = hex.GetComponent<HexTile>();
                tiles[x, y].Initialize(new Vector2Int(x, y));
                

                hex.transform.SetParent(transform, true);
            }
        }
        
        foreach (HexTile tile in tiles)
        {
            tile.GetNeighbors();
        }
    }

    public void GenerateMap(HexTile[,] hexTiles){
        foreach (GameObject tile in tileObjects)
        {
            Destroy(tile);
        }
        
        tileObjects.Clear();
        tiles = new HexTile[gridSize.x, gridSize.y];
        
        for(int x = 0; x < gridSize.x; x++){
            for(int y = 0; y < gridSize.y; y++){
                GameObject hex = Instantiate(hexPrefab, transform.position, Quaternion.identity);
                hex.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, -y));
                hex.transform.rotation = Quaternion.Euler(-90, -90, 0);
                tileObjects.Add(hex);
                
                tiles[x, y] = hex.GetComponent<HexTile>();
                tiles[x, y].Initialize(hexTiles[x, y]);
                
                hex.transform.SetParent(transform, true);
            }
        }

        foreach (HexTile tile in tiles)
        {
            tile.GetNeighbors();
        }
    }

    public Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinates)
    {
        int column = coordinates.x;
        int row = coordinates.y;
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;

        
        shouldOffset = column % 2 == 0;
        width = 2f;
        height = Mathf.Sqrt(3);
        horizontalDistance = width * (3f / 4f);
        verticalDistance = height;
        offset = shouldOffset ? height / 2 : 0;
        xPosition = column * horizontalDistance;
        yPosition = row * verticalDistance - offset;
        
        
        return new Vector3(xPosition, 0, yPosition);
    }

    public int GetAngle(HexTile tile1, HexTile tile2){
        Vector2 reference = Vector2.up;
        Vector2 vector = new Vector2(tile2.transform.position.x - tile1.transform.position.x, tile2.transform.position.z - tile1.transform.position.z);
        int angle = Mathf.RoundToInt(-Vector2.SignedAngle(reference, vector));
        return FixAngle(angle);
    }

    public int GetAngle(Vector3 vector1, Vector3 vector2){
        Vector2 reference = Vector2.up;
        Vector2 vector = new Vector2(vector2.x - vector1.x, vector2.z - vector1.z);
        int angle = Mathf.RoundToInt(-Vector2.SignedAngle(reference, vector));
        return FixAngle(angle);
    }

    public static int FixAngle(int angle){
        if (angle <= -180) angle += 360;
        else if (angle > 180) angle -= 360;
        return angle;
    }

    public HexTile StationToTile(Station station){
        return tiles[station.coordinates.x, station.coordinates.y];
    }

    public HexTile CoordinatesToTile(Vector2Int coordinates){
        return tiles[coordinates.x, coordinates.y];
    }

    public void LoadData(GameData data)
    {
        if(data.map.mapSize.x == 0){
            LayoutGrid();
            return;
        }else{
            gridSize.x = data.map.mapSize.x;
            gridSize.y = data.map.mapSize.y;
        }
        
        HexTile[,] hexTiles = new HexTile[data.map.mapSize.x, data.map.mapSize.y];

        for(int x = 0; x < data.map.mapSize.x; x++){
            for(int y = 0; y < data.map.mapSize.y; y++){
                hexTiles[x, y] = new();
                hexTiles[x, y].coordinates = new Vector2Int(x, y);
                hexTiles[x, y].type = data.map.tiles[x].array[y].type;
                hexTiles[x, y].decorationIndex = data.map.tiles[x].array[y].decorationIndex;
                hexTiles[x, y].revealed = data.map.tiles[x].array[y].revealed;

                //Placing rails
                Vector2Int usedCoordinates = new(hexTiles[x, y].coordinates.x, -hexTiles[x, y].coordinates.y);
                foreach (int angle in data.map.tiles[x].array[y].angles)
                {
                    hexTiles[x, y].angles.Add(angle);
                    Instantiate(railPrefab, GetPositionForHexFromCoordinate(usedCoordinates), Quaternion.Euler(0, angle, 0));
                }
            }
        }

        GenerateMap(hexTiles);
    }

    public void SaveData(GameData data)
    {
        data.map = new(gridSize.x, gridSize.y);

        for(int x = 0; x < data.map.mapSize.x; x++){
            for(int y = 0; y < data.map.mapSize.y; y++){
                data.map.tiles[x].array[y] = new(tiles[x, y]);
            }
        }
    }

    public int GetPriority()
    {
        return 0;
    }
}
