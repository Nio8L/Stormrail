using System;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Vector2Int coordinates;

    public bool revealed = false;

    public GameObject hexStructure;

    public GameObject decorations;
    public int decorationIndex;

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
    }

    public void Initialize(Vector2Int coordinates, Type type, List<int> angles, int decorationIndex){
        this.coordinates = coordinates;
        this.angles = angles;
        this.decorationIndex = decorationIndex;
        SetType(type);
        
        if(type == Type.Mountain){
            GameObject prefabMountain = MapManager.instance.decorationsMountain[decorationIndex];
            decorations = Instantiate(prefabMountain, transform.position + new Vector3(0, 0.24f, 0), Quaternion.Euler(-90, 0, 0));
        }else if(type == Type.Forest){
            GameObject prefabForest = MapManager.instance.decorationsForest[decorationIndex];
            decorations = Instantiate(prefabForest, transform.position + new Vector3(0, 0.24f, 0.3f), Quaternion.Euler(0, 0, 0));
        }
    }

    public void Reveal(){

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
            if (decorations.GetComponent<City>() != null){
                decorations.GetComponent<City>().DestroyCity();
                decorations = null;
            }else{
                Destroy(decorations);
            }
        }

        if(newType == Type.City){
            decorations = Instantiate(CityManager.instance.cityPrefab, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
            CityManager.instance.cities.Add(decorations.GetComponent<City>());
            CityManager.instance.cities[^1].Initialize(coordinates, coordinates.x + ", "  + coordinates.y, coordinates.x + coordinates.y);
            decorations.GetComponent<City>().OnFirstCreate();
        }else if (newType == Type.Mountain){
            decorationIndex = UnityEngine.Random.Range(0, MapManager.instance.decorationsMountain.Count);
            GameObject prefabMountain = MapManager.instance.decorationsMountain[decorationIndex];
            decorations = Instantiate(prefabMountain, transform.position + new Vector3(0, 0.24f, 0), Quaternion.Euler(-90, 0, 0));
        }else if (newType == Type.Forest){
            decorationIndex = UnityEngine.Random.Range(0, MapManager.instance.decorationsForest.Count);
            GameObject prefabForest = MapManager.instance.decorationsForest[decorationIndex];
            decorations = Instantiate(prefabForest, transform.position + new Vector3(0, 0.24f, 0.3f), Quaternion.Euler(0, 0, 0));
        }
        
    }

    private void OnMouseDown() {
        //Debug.Log(coordinates);
        //Reveal();
        if (RaycastChecker.Check()) return;

        if(!MapManager.instance.buildMode){ 
            if(type == Type.City){
                SetTypeDecoration(Type.Empty);
            }else{
                SetTypeDecoration(type + 1);
            }
        }else{
            Pathfinder.instance.TryToConnect(this);
        }

        //Debug.Log(walkable);

        //GetNeighbors();
    }

    public void GetNeighbors(){
        foreach (HexTile tile in MapManager.instance.tiles)
        {
            if(GetDistance(tile) == 1){
                Neighbors.Add(tile);
            }
        }

       /* foreach (HexTile tile in Neighbors)
        {
            tile.SetType(Type.Mountain);
        }*/
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
    }
}
