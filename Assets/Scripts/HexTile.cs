using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexTile : MonoBehaviour
{
    public Vector2Int coordinates;

    public bool fogged = true;

    public GameObject hexStructure;
    public ParticleSystem cloudParticles;

    public void Initialize(Vector2Int coordinates){
        this.coordinates = coordinates;
        cloudParticles.Play();
        hexStructure.SetActive(false);
    }

    public void Reveal(){
        fogged = false;
        cloudParticles.Stop();
        cloudParticles.gameObject.SetActive(false);
        hexStructure.SetActive(true);
    }

    private void OnMouseDown() {
        Debug.Log(coordinates);
        Reveal();
    }
}
