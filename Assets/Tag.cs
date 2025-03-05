using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag : MonoBehaviour
{
    GameObject objectToLookAt;
    void Start(){
        objectToLookAt = Camera.main.gameObject;
    }
    void Update(){
        Vector3 newPosition = new Vector3(transform.parent.position.x, transform.parent.position.y + 1.25f, transform.parent.position.z);
        transform.position = newPosition;

        transform.LookAt(objectToLookAt.transform);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 90f, transform.eulerAngles.y, transform.eulerAngles.z+180f);
    }
}
