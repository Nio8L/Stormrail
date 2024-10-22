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
    }

    public void SetTypeDecoration(Type newType){
        type = newType;

        MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();

        meshRenderer.material = MapManager.instance.materials[(int)type];

        if(newType == Type.City){
            decorations = Instantiate(CityManager.instance.cityPrefab, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
            CityManager.instance.cities.Add(decorations.GetComponent<City>());
            CityManager.instance.cities[^1].Initialize(coordinates, coordinates.x + ", "  + coordinates.y, coordinates.x + coordinates.y);
        }else if(decorations != null && decorations.GetComponent<City>() != null){
            decorations.GetComponent<City>().DestroyCity();
            decorations = null;
        }
    }

    private void OnMouseDown() {
        Debug.Log(coordinates);
        //Reveal();

        if(type == Type.City){
            SetTypeDecoration(Type.Empty);
        }else{
            SetTypeDecoration(type + 1);
        }
    }
}
