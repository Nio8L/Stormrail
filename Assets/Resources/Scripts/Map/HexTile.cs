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

    public enum Type{
        Empty,
        Forest,
        Mountain,
        City
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

        if(type != Type.Empty && type != Type.City){
            walkable = false;
        }else{
            walkable = true;
        }

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
        // Editor reveal bypass
        if (MapLoader.instance != null && MapLoader.instance.loadingEditor) return;

        revealed = true;

        MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();
        meshRenderer.material = MapManager.instance.materials[(int)type];

        if(type != Type.Empty){
            decorations.SetActive(true);
        }
    }

    public void SetType(Type newType){
        type = newType;

        MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();

        meshRenderer.material = MapManager.instance.materials[(int)type];

        if(type != Type.Empty && type != Type.City){
            walkable = false;
        }else{
            walkable = true;
        }
            
    }

    public void SetTypeDecoration(Type newType){
        type = newType;

        if(type != Type.Empty && type != Type.City){
            walkable = false;
        }else{
            walkable = true;
        }

        MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();

        meshRenderer.material = MapManager.instance.materials[(int)type];

        if (decorations != null){
            Destroy(decorations);
            if (city != null){
                city.DestroyCity();
            }
        }

        SpawnDecoration(newType);
        
    }

    public void SpawnDecoration(Type newType){
        if(newType == Type.City){
            city = Instantiate(CityManager.instance.cityPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity).GetComponent<City>();
            CityManager.instance.cities.Add(city);
            city.Initialize(coordinates, coordinates.x + ", "  + coordinates.y, (coordinates.x + coordinates.y) * 3);
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
        }else if(newType == Type.City){
            city = Instantiate(CityManager.instance.cityPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity).GetComponent<City>();
            CityManager.instance.cities.Add(city);
            city.Initialize(coordinates, coordinates.x + ", "  + coordinates.y, coordinates.x + coordinates.y);
            city.OnFirstCreate();

            // Decorations
            decorationIndex = UnityEngine.Random.Range(0, MapManager.instance.decorationsCity.Count);
            GameObject prefabCity = MapManager.instance.decorationsCity[decorationIndex];
            decorations = Instantiate(prefabCity, transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));

        }

        if(!revealed && (MapLoader.instance == null || !MapLoader.instance.loadingEditor)){
            decorations.SetActive(false);
        }
    }

    private void OnMouseDown() {

        if (RaycastChecker.Check()) return;

        if(MapManager.instance.mode != MapManager.Mode.Build){ 

            if (MapLoader.instance != null && MapLoader.instance.loadingEditor)
            {
                if(type == Type.City){
                    SetTypeDecoration(Type.Empty);
                }else{
                    SetTypeDecoration(type + 1);
                }
            }else{
                Reveal();
            }
        }else{
            Pathfinder.instance.TryToConnect(this);
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

    public List<HexTile.Type> GetNeighborsType(int radius){
        List<HexTile.Type> types = new ();
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

    private void OnMouseEnter() {
        MapManager.instance.hoveredTile = this;
        MapManager.instance.UpdatePreview();

        if(ExplorerManager.instance.selectedExplorer != null){
            ExplorerManager.instance.UpdatePreview();
        }
    }
}
