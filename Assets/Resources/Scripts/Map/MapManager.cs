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

    [Header("Other")]
    public GameObject railPrefab;
    public GameObject railPreview;
    public bool buildMode = false;
    public List<GameObject> previewRails;

    [Header("Other")]
    public List<GameObject> decorationsMountain;
    public List<GameObject> decorationsForest;

    private void Awake() {
        if(instance != null){
            Debug.LogError("More than one Map Manager found!");
        }
        
        instance = this;

        tiles = new HexTile[gridSize.x, gridSize.y];
    }

    private void Start() {
        //LayoutGrid();
    }

    private void Update() {
        if(Input.GetMouseButton(1)){
            buildMode = false;
            Pathfinder.instance.ResetTiles();
            UpdatePreview();
        }

        if(buildMode && Pathfinder.instance.tile1 != null){
            BuildPreviewConnection(Pathfinder.instance.tile1);
        }
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
                tiles[x, y].Initialize(new Vector2Int(x, y), hexTiles[x, y].type, hexTiles[x, y].angles);
                
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

    public HexTile CityToTile(City city){
        return tiles[city.coordinates.x, city.coordinates.y];
    }

    public void BuildRailConnection(HexTile tile1, HexTile tile2){
        List<HexTile> path = Pathfinder.instance.Pathfind(tile1, tile2);
        for(int i = 0; i < path.Count - 1; i++){
            BuildRail(path[i], path[i + 1]);
        }
        UpdatePreview();
    }

    public void BuildRail(HexTile tile1, HexTile tile2){
        int angle = GetAngle(tile1, tile2);
        int opposite = FixAngle(angle - 180);
        if(tile1.angles.Contains(opposite)){
            return;
        }
        if(tile2.angles.Contains(angle)){
            return;
        }
        Instantiate(railPrefab, tile1.transform.position, Quaternion.Euler(0, opposite, 0));
        Instantiate(railPrefab, tile2.transform.position,  Quaternion.Euler(0, angle, 0));
        tile1.angles.Add(opposite);
        tile2.angles.Add(angle);
    }

    public void BuildPreviewRail(HexTile tile1, HexTile tile2){
        int angle = GetAngle(tile1, tile2);
        int opposite = FixAngle(angle - 180);

        GameObject rail1 = Instantiate(railPreview, tile1.transform.position, Quaternion.Euler(0, opposite, 0));
        GameObject rail2 = Instantiate(railPreview, tile2.transform.position,  Quaternion.Euler(0, angle, 0));

        previewRails.Add(rail1);
        previewRails.Add(rail2);
    }

    public void BuildPreviewConnection(HexTile tile){        
        List<HexTile> path = Pathfinder.instance.Pathfind(tile, hoveredTile);
        
        if(path == null) return;

        for (int i = 0; i < path.Count - 1; i++)
        {
            BuildPreviewRail(path[i], path[i + 1]);
        }
    }

    public void UpdatePreview(){
        for (int i = previewRails.Count - 1; i >= 0; i--)
        {
            Destroy(previewRails[i]);
        }

        previewRails = new();
    }

    public void LoadData(GameData data)
    {
        if(data.map.mapSize.x == 0){
            LayoutGrid();
            return;
        }
        
        HexTile[,] hexTiles = new HexTile[data.map.mapSize.x, data.map.mapSize.y];

        for(int x = 0; x < data.map.mapSize.x; x++){
            for(int y = 0; y < data.map.mapSize.y; y++){
                hexTiles[x, y] = new();
                hexTiles[x, y].coordinates = new Vector2Int(x, y);
                hexTiles[x, y].type = data.map.tiles[x].array[y].type;
                
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
        data.map = new(gridSize.x);

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
