using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityTag : MonoBehaviour
{
    public City cityToFollow;
    GameObject objectToLookAt;
    public TextMeshPro cityNameBox;

    public bool updateRotation;

    void Start(){
        cityNameBox.text = cityToFollow.cityName;
        objectToLookAt = Camera.main.gameObject;
    }
    void Update(){
        if (!updateRotation) return;

        Vector3 newPosition = new Vector3(cityToFollow.transform.position.x, cityToFollow.transform.position.y + 1f, cityToFollow.transform.position.z);
        transform.position = newPosition;

        transform.LookAt(objectToLookAt.transform);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 90f, transform.eulerAngles.y, transform.eulerAngles.z+180f);
    }
}
