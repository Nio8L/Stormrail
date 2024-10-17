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

    public void Reveal(){
        fogged = false;
        cloudParticles.Stop();
        cloudParticles.gameObject.SetActive(false);
        hexStructure.SetActive(true);
    }

    public void SetType(Type newType, Material material){
        type = newType;

        MeshRenderer meshRenderer = hexStructure.GetComponent<MeshRenderer>();

        meshRenderer.material = material;
    }

    private void OnMouseDown() {
        Debug.Log(coordinates);
        Reveal();
    }
}
