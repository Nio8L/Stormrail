using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    public static BuilderManager instance;

    public List<Builder> builders; 
    
    [Header("Builder")]
    public Builder selectedBuilder; 
    public GameObject builderPrefab;

    [Header("Path preview")]
    public GameObject pathPreview;
    public List<GameObject> previewPath;

    [Header("Debug")]
    public bool spawnBuilder = false;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        if(spawnBuilder){
            SpawnBuilder();
            spawnBuilder = false;
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
        List<HexTile> path = Pathfinder.instance.PathfindAll(tile, MapManager.instance.hoveredTile, selectedBuilder.foodSupply);
        
        if(path == null) return;

        for (int i = 0; i < path.Count - 1; i++)
        {
            BuildPreviewRail(path[i], path[i + 1]);
        }
    }

    public void UpdatePreview(){
       DeletePreview();

        if(selectedBuilder != null && MapManager.instance.mode == MapManager.Mode.Construct){
            BuildPreviewConnection(MapManager.instance.CoordinatesToTile(selectedBuilder.coordinates));
        }
    }

    public void DeletePreview(){
        for (int i = previewPath.Count - 1; i >= 0; i--)
        {
            Destroy(previewPath[i]);
        }

        previewPath = new();
    }
    
    public void SpawnBuilder(){
        Vector3 spawnPosition = MapManager.instance.GetPositionForHexFromCoordinate(new (0, 0));
        GameObject newBuilder = Instantiate(builderPrefab, spawnPosition, Quaternion.identity);

        Builder builderScript = newBuilder.GetComponent<Builder>();
        builders.Add(builderScript);
    }

    public void SpawnBuilder(HexTile spawnTile){
        Vector3 spawnPosition = new(spawnTile.transform.position.x, 1, spawnTile.transform.position.z);
        GameObject newBuilder = Instantiate(builderPrefab, spawnPosition, Quaternion.identity);

        Builder builderScript = newBuilder.GetComponent<Builder>();
        builders.Add(builderScript);
    }

    public List<Builder> GetBuildersInCity(City city){
        List<Builder> found = new(); 
        
        foreach (Builder builder in builders)
        {
            if(builder.coordinates == city.coordinates){
                found.Add(builder);
            }
        }

        return found;
    }

}
