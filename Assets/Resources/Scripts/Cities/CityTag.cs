using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityTag : MonoBehaviour
{
    public Station stationToFollow;
    GameObject objectToLookAt;
    public TextMeshPro cityNameBox;

    void Start(){
        cityNameBox.text = stationToFollow.cityName;
        objectToLookAt = Camera.main.gameObject;
    }
    void Update(){
        if (!MapManager.instance.StationToTile(stationToFollow).revealed && !MapLoader.instance.loadingEditor){
            gameObject.SetActive(false);
            return;
        }

        Vector3 newPosition = new Vector3(stationToFollow.transform.position.x, stationToFollow.transform.position.y + 1f, stationToFollow.transform.position.z);
        transform.position = newPosition;

        transform.LookAt(objectToLookAt.transform);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 90f, transform.eulerAngles.y, transform.eulerAngles.z+180f);
    }
}
