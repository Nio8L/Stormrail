using System;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Vector2Int coordinates;

    public bool revealed = false;
    public Material hiddenMaterial;

    public GameObject hexStructure;

    public GameObject decorations;
    public int decorationIndex;

    public City city;

    public Station station;

    public enum Type{
        Empty,
        Forest,
        Mountain,
        City,
        Station
    }
    public Type type;

    [Header("Pathfinding")]
    public List<HexTile> Neighbors;
    public bool walkable = true;
    public List<int> angles = new();
    
    public void Initialize(Vector2Int coordinates){
        this.coordinates = coordinates;

        if(!revealed && (MapLoader.instance == null || !MapLoader.instance.loadingEditor)){
            MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();

            meshRenderer.material = hiddenMaterial;
        }

        SetWalkable();

        if(type != Type.Empty){
            SpawnDecoration(type);
        }
        
    }

    public void Initialize(HexTile hex){
        coordinates = hex.coordinates;
        angles = hex.angles;
        decorationIndex = hex.decorationIndex;
        revealed = hex.revealed;
        SetType(hex.type);
        
        if(type == Type.Mountain){
            GameObject prefabMountain = MapManager.instance.decorationsMountain[decorationIndex];
            decorations = Instantiate(prefabMountain, transform.position + new Vector3(0, 0.24f, 0), Quaternion.Euler(-90, 0, 0));
        }else if(type == Type.Forest){
            GameObject prefabForest = MapManager.instance.decorationsForest[decorationIndex];
            decorations = Instantiate(prefabForest, transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
        }else if(type == Type.City){
            GameObject prefabCity = MapManager.instance.decorationsCity[decorationIndex];
            decorations = Instantiate(prefabCity, transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
        }else if(type == Type.Station){
            GameObject prefabStation = MapManager.instance.decorationsStation[decorationIndex];
            decorations = Instantiate(prefabStation, transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
        }

        // Editor reveal bypass
        if (MapLoader.instance != null && MapLoader.instance.loadingEditor) return;

        if(!revealed){
            MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();

            meshRenderer.material = hiddenMaterial;
            if(type != Type.Empty){
                decorations.SetActive(false);
            }
        }
    }

    public void Reveal(){
        revealed = true;

        MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();
        meshRenderer.material = MapManager.instance.materials[(int)type];

        SetWalkable();

        if(type != Type.Empty && decorations != null){
            decorations.SetActive(true);
        }
    }

    public void SetType(Type newType){
        type = newType;

        MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();

        meshRenderer.material = MapManager.instance.materials[(int)type];

        SetWalkable();
            
    }

    public void SetTypeDecoration(Type newType){
        type = newType;

        SetWalkable();

        MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();

        meshRenderer.material = MapManager.instance.materials[(int)type];

        if (decorations != null){
            Destroy(decorations);
            decorations = null;
            if (city != null){
                city.DestroyCity();
                city = null;
            }else if (station != null){
                station.DestroyStation();
                station = null;             
            }
        }

        SpawnDecoration(newType);
    }

    public void SpawnDecoration(Type newType){
        if(newType == Type.City){
            city = Instantiate(CityManager.instance.cityPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity).GetComponent<City>();
            CityManager.instance.cities.Add(city);
            city.Initialize(coordinates, coordinates.x + ", "  + coordinates.y, 50);
            city.OnFirstCreate();

            // Decorations
            decorationIndex = UnityEngine.Random.Range(0, MapManager.instance.decorationsCity.Count);
            GameObject prefabCity = MapManager.instance.decorationsCity[decorationIndex];
            decorations = Instantiate(prefabCity, transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));

        }else if (newType == Type.Mountain){
            decorationIndex = UnityEngine.Random.Range(0, MapManager.instance.decorationsMountain.Count);
            GameObject prefabMountain = MapManager.instance.decorationsMountain[decorationIndex];
            decorations = Instantiate(prefabMountain, transform.position + new Vector3(0, 0.24f, 0), Quaternion.Euler(-90, 0, 0));
        }else if (newType == Type.Forest){
            decorationIndex = UnityEngine.Random.Range(0, MapManager.instance.decorationsForest.Count);
            GameObject prefabForest = MapManager.instance.decorationsForest[decorationIndex];
            decorations = Instantiate(prefabForest, transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
        }else if(newType == Type.Station){
            station = Instantiate(CityManager.instance.stationPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity).GetComponent<Station>();
            CityManager.instance.stations.Add(station);
            station.Initialize(coordinates, coordinates.x + ", "  + coordinates.y);

            // Decorations
            decorationIndex = UnityEngine.Random.Range(0, MapManager.instance.decorationsStation.Count);
            GameObject prefabCity = MapManager.instance.decorationsStation[decorationIndex];
            decorations = Instantiate(prefabCity, transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));

        }
        

        if(!revealed && (MapLoader.instance == null || !MapLoader.instance.loadingEditor)){
            decorations.SetActive(false);
        }
    }

    public void SetWalkable(){
        if(type != Type.Empty && type != Type.City && type != Type.Station || !revealed){
            walkable = false;
        }else{
            walkable = true;
        }
    }

    public void GetNeighbors(){
        foreach (HexTile tile in MapManager.instance.tiles)
        {
            if(GetDistance(tile) == 1){
                Neighbors.Add(tile);
            }
        }
    }

    public List<HexTile> GetNeighbors(int radius){
        List<HexTile> neighbors = new();
        
        foreach (HexTile tile in MapManager.instance.tiles)
        {
            if(GetDistance(tile) <= radius){
                neighbors.Add(tile);
            }
        }

        return neighbors;
    }

    public List<Type> GetNeighborsType(int radius){
        List<Type> types = new ();
        List<HexTile> neighbors = GetNeighbors(radius);

        foreach(HexTile tile in neighbors){
            types.Add(tile.type);
        }

        return types;
    }

    public float AxialDistance(Vector2Int tile1, Vector2Int tile2){
        return (MathF.Abs(tile1.x - tile2.x) + 
                Mathf.Abs(tile1.x + tile1.y - tile2.x - tile2.y) + 
                Mathf.Abs(tile1.y - tile2.y)) / 2;
    }

    public float GetDistance(HexTile tile){
        Vector2Int ac = EvenQtoAxial(tile);
        Vector2Int bc = EvenQtoAxial(this);
        return AxialDistance(ac, bc);
    } 

    public Vector2Int EvenQtoAxial(HexTile tile){
        int q = tile.coordinates.x;
        int r = tile.coordinates.y - (tile.coordinates.x + (tile.coordinates.x&1)) / 2;
        return new Vector2Int(q, r);
    }

    private void OnMouseUp() {
        if (RaycastChecker.Check()) return;

        if(!CursorManager.instance.CheckMode(CursorManager.Mode.Build)){ 

            if (MapLoader.instance != null && MapLoader.instance.loadingEditor)
            {
                if(type == Type.Station){
                    SetTypeDecoration(Type.Empty);
                }else{
                    SetTypeDecoration(type + 1);
                }
            }
        }else{
            if(BuilderManager.instance.construction == BuilderManager.Construction.Rail){
                if(Pathfinder.instance.previewing){
                    BuilderManager.instance.CreateRailProject(Pathfinder.instance.PathfindRevealed(Pathfinder.instance.start, this));
                    Pathfinder.instance.StopPreview();
                }else{
                    //Pathfinder.instance.TryToConnect(this);
                    Pathfinder.instance.start = this;
                    Pathfinder.instance.previewing = true;
                }
            }

            if(BuilderManager.instance.construction == BuilderManager.Construction.City && revealed){
                if(CityManager.instance.GetCity(this) == null){
                    BuilderManager.instance.CreateCityProject(this);
                }
            }
        }

    }

    private void OnMouseEnter() {
        MapManager.instance.hoveredTile = this;

        if(ExplorerManager.instance.selectedExplorer != null){
            ExplorerManager.instance.UpdatePreview();
        }

        

        if(CursorManager.instance.CheckMode(CursorManager.Mode.Build)){
            //BuilderManager.instance.UpdatePreview();
        }
    }
}
