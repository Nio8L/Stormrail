using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexTile : MonoBehaviour
{
    public Vector2Int coordinates;

    public bool fogged = false;

    public GameObject hexStructure;
    public ParticleSystem cloudParticles;

    public GameObject decorations;

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
        
        hexStructure.SetActive(false);
        
        if(fogged){
            cloudParticles.Play();
        }else{
            Reveal();
        }
    }

    public void Initialize(Vector2Int coordinates, Type type){
        this.coordinates = coordinates;
        SetType(type);

        hexStructure.SetActive(false);
        
        if(fogged){
            cloudParticles.Play();
        }else{
            Reveal();
        }
    }

    public void Reveal(){
        fogged = false;
        cloudParticles.Stop();
        cloudParticles.gameObject.SetActive(false);
        hexStructure.SetActive(true);
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

        if(newType == Type.City){
            decorations = Instantiate(CityManager.instance.cityPrefab, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
            CityManager.instance.cities.Add(decorations.GetComponent<City>());
            CityManager.instance.cities[^1].Initialize(coordinates, coordinates.x + ", "  + coordinates.y, coordinates.x + coordinates.y);
            decorations.GetComponent<City>().OnFirstCreate();
        }else if(decorations != null && decorations.GetComponent<City>() != null){
            decorations.GetComponent<City>().DestroyCity();
            decorations = null;
        }
    }

    private void OnMouseDown() {
        //Debug.Log(coordinates);
        //Reveal();
        if (RaycastChecker.Check()) return;

        if(type == Type.City){
            SetTypeDecoration(Type.Empty);
        }else{
            SetTypeDecoration(type + 1);
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

}
