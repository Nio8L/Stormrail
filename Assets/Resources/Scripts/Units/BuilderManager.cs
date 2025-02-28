using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Project{
    public string name;
    public List<GameObject> blueprints;
    public List<HexTile> occupiedTiles;
    public Vector2Int startingCoordinates;

    public BuilderManager.BuildDelegate buildMethod;

    public Project(){
        name = "";
        blueprints = new();
        occupiedTiles = new();
        startingCoordinates = new(0, 0);
    }

    public Project(string name, List<GameObject> blueprints, List<HexTile> occupiedTiles){
        this.name = name;
        this.blueprints = blueprints;
        this.occupiedTiles = occupiedTiles;
        startingCoordinates = occupiedTiles[0].coordinates;
    }

    public Project(List<GameObject> blueprints, List<HexTile> occupiedTiles, BuilderManager.BuildDelegate buildDelegate){
        name = "New project " + (BuilderManager.instance.projects.Count + 1);
        this.blueprints = blueprints;
        this.occupiedTiles = occupiedTiles;
        startingCoordinates = occupiedTiles[0].coordinates;
        //startingCoordinates = MapManager.instance.GetCoordinatesFromPosition(blueprints[0].transform.position);
        buildMethod = buildDelegate;
    }
}

public class BuilderManager : MonoBehaviour, ISavable
{
    public static BuilderManager instance;

    public List<Builder> builders; 
    
    [Header("Builder")]
    public Builder selectedBuilder; 
    public GameObject builderPrefab;

    [Header("Preview")]
    public GameObject cityPreviewPrefab;
    public GameObject cityPreview;

    [Header("Debug")]
    public bool spawnBuilder = false;

    [Header("Projects")]
    public List<Project> projects;
    public Construction construction;
    public delegate void BuildDelegate(HexTile tile1, HexTile tile2);

    public enum Construction{
        Rail,
        City
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        spawnBuilder = true;
    }

    private void Update() {
        if(spawnBuilder){
            SpawnBuilder(MapManager.instance.StationToTile(CityManager.instance.cities[0]));
            spawnBuilder = false;
        }

        if(CursorManager.instance.CheckMode(CursorManager.Mode.Build) && construction == Construction.City){
            if(cityPreview != null){
                cityPreview.transform.position = MapManager.instance.GetPositionForHexFromCoordinate(new Vector2Int(MapManager.instance.hoveredTile.coordinates.x, -MapManager.instance.hoveredTile.coordinates.y));
            }else{
                cityPreview = Instantiate(cityPreviewPrefab);
            }
        }else{
            if(cityPreview != null){
                Destroy(cityPreview);
            }
        }
    }

    public void BuildMode(){
        CursorManager.instance.SetMode(CursorManager.Mode.Build);
    }

    public void SetConstruct(int construction){
        this.construction = (Construction)construction;
    }

    public void CreateCityProject(HexTile tile){
        List<GameObject> blueprints = new();
        List<HexTile> occupiedTiles = new()
        {
            tile
        };

        blueprints.Add(Instantiate(cityPreviewPrefab, tile.transform.position, Quaternion.identity));

        projects.Add(new(blueprints, occupiedTiles, BuildCity));
    }

    public void BuildCity(HexTile tile1, HexTile tile2){
        GameObject newCityObject = Instantiate(CityManager.instance.cityPrefab, tile1.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        City newCity = newCityObject.GetComponent<City>();
        newCity.Initialize(tile1.coordinates, "New Settlement", 10);
        newCity.OnFirstCreate();

        tile1.decorationIndex = UnityEngine.Random.Range(0, MapManager.instance.decorationsCity.Count);
        GameObject prefabCity = MapManager.instance.decorationsCity[tile1.decorationIndex];
        tile1.decorations = Instantiate(prefabCity, tile1.transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));

        CityManager.instance.cities.Add(newCity);
    }

    public void CreateRailProject(List<HexTile> path){
        if(path == null) return;

        List<GameObject> blueprints = Pathfinder.instance.previewPath;
        List<GameObject> newBlueprints = new();
        foreach (GameObject blueprint in blueprints)
        {
            newBlueprints.Add(Instantiate(blueprint));
        }
        projects.Add(new(newBlueprints, path, BuildRail));
    }

    public void BuildRail(HexTile tile1, HexTile tile2){
        if(tile1 == tile2) return;
        
        int angle = MapManager.instance.GetAngle(tile1, tile2);
        int opposite = MapManager.FixAngle(angle - 180);
        if(tile1.angles.Contains(opposite)){
            return;
        }
        if(tile2.angles.Contains(angle)){
            return;
        }
        Instantiate(MapManager.instance.railPrefab, tile1.transform.position, Quaternion.Euler(0, opposite, 0));
        Instantiate(MapManager.instance.railPrefab, tile2.transform.position,  Quaternion.Euler(0, angle, 0));
        tile1.angles.Add(opposite);
        tile2.angles.Add(angle);
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
        builderScript.coordinates = spawnTile.coordinates;
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

   

    public int GetPriority()
    {
        return 3;
    }

    public void LoadData(GameData data)
    {
        foreach (BuilderSerialized builderSerialized in data.builders)
        {
            Vector2Int startCoordinates = new(builderSerialized.cameFrom.x, builderSerialized.cameFrom.y);            
            Vector2Int targetCoordinates = new(builderSerialized.goingTo.x, builderSerialized.goingTo.y);

            HexTile startTile = MapManager.instance.CoordinatesToTile(startCoordinates);
            HexTile targetTile = MapManager.instance.CoordinatesToTile(targetCoordinates);

            SpawnBuilder(startTile);
            builders[^1].unitName = builderSerialized.name;
            builders[^1].coordinates = startTile.coordinates;
            builders[^1].speed = builderSerialized.speed;
            builders[^1].foodSupply = builderSerialized.foodSupply;
            builders[^1].steelSupply = builderSerialized.steelSupply;
            builders[^1].NewPath(targetTile);

            if(builders[^1].unitName == null){
                builders[^1].unitName = "Explorer group " + builders.Count;
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.builders = new();

        foreach (Builder builder in builders)
        {
            BuilderSerialized builderToSave = new(builder);
            data.builders.Add(builderToSave);            
        }
    }
}
