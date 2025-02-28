using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorerManager : MonoBehaviour, ISavable
{
    public static ExplorerManager instance;

    public List<Explorer> explorers;
    
    [Header("Explorer")]
    public Explorer selectedExplorer;
    public GameObject explorerPrefab;

    [Header("Path preview")]
    public GameObject pathPreview;
    public List<GameObject> previewPath;

    [Header("Debug")]
    public bool spawnExplorer = false;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        if(spawnExplorer){
            SpawnExplorer();
            spawnExplorer = false;
        }
    }

    public void BuildPreviewRail(HexTile tile1, HexTile tile2){
        int angle = MapManager.instance.GetAngle(tile1, tile2);
        int opposite = MapManager.FixAngle(angle - 180);

        GameObject rail1 = Instantiate(pathPreview, tile1.transform.position, Quaternion.Euler(0, opposite, 0));
        GameObject rail2 = Instantiate(pathPreview, tile2.transform.position,  Quaternion.Euler(0, angle, 0));

        previewPath.Add(rail1);
        previewPath.Add(rail2);
    }

    public void BuildPreviewConnection(HexTile tile){        
        List<HexTile> path = Pathfinder.instance.PathfindAll(tile, MapManager.instance.hoveredTile, selectedExplorer.foodSupply);
        
        if(path == null) return;

        for (int i = 0; i < path.Count - 1; i++)
        {
            BuildPreviewRail(path[i], path[i + 1]);
        }
    }

    public void UpdatePreview(){
       DeletePreview();

        if(selectedExplorer != null && CursorManager.instance.CheckMode(CursorManager.Mode.Explore)){
            BuildPreviewConnection(MapManager.instance.CoordinatesToTile(selectedExplorer.coordinates));
        }
    }

    public void DeletePreview(){
        for (int i = previewPath.Count - 1; i >= 0; i--)
        {
            Destroy(previewPath[i]);
        }

        previewPath = new();
    }

    public void SpawnExplorer(){
        Vector3 spawnPosition = MapManager.instance.GetPositionForHexFromCoordinate(new (0, 0));
        GameObject newExplorer = Instantiate(explorerPrefab, spawnPosition, Quaternion.identity);

        Explorer explorerScript = newExplorer.GetComponent<Explorer>();
        explorers.Add(explorerScript);
    }
    public void SpawnExplorer(HexTile spawnTile){
        Vector3 spawnPosition = new(spawnTile.transform.position.x, 1, spawnTile.transform.position.z);
        GameObject newExplorer = Instantiate(explorerPrefab, spawnPosition, Quaternion.identity);

        Explorer explorerScript = newExplorer.GetComponent<Explorer>();
        explorerScript.coordinates = spawnTile.coordinates;
        explorers.Add(explorerScript);
    }

    public List<Explorer> GetExplorersInCity(City city){
        List<Explorer> found = new();
        
        foreach (Explorer explorer in explorers)
        {
            if(explorer.coordinates == city.coordinates && !explorer.move){
                found.Add(explorer);
            }
        }

        return found;
    }

    public int GetPriority()
    {
        return 3;
    }

    public void LoadData(GameData data)
    {
        foreach (ExplorerSerialized explorerSerialized in data.explorers)
        {
            Vector2Int startCoordinates = new(explorerSerialized.cameFrom.x, explorerSerialized.cameFrom.y);            
            Vector2Int targetCoordinates = new(explorerSerialized.goingTo.x, explorerSerialized.goingTo.y);

            HexTile startTile = MapManager.instance.CoordinatesToTile(startCoordinates);
            HexTile targetTile = MapManager.instance.CoordinatesToTile(targetCoordinates);

            SpawnExplorer(startTile);
            explorers[^1].unitName = explorerSerialized.name;
            explorers[^1].coordinates = startTile.coordinates;
            explorers[^1].speed = explorerSerialized.speed;
            explorers[^1].revealRadius = explorerSerialized.revealRadius;
            explorers[^1].foodSupply = explorerSerialized.foodSupply;
            explorers[^1].NewPath(targetTile);

            if(explorers[^1].unitName == null){
                explorers[^1].unitName = "Explorer group " + explorers.Count;
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.explorers = new();
        foreach (Explorer explorer in explorers)
        {
            ExplorerSerialized explorerToSave = new(explorer);
            data.explorers.Add(explorerToSave);
        }
    }
}
