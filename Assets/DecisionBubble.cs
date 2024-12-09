using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionBubble : MonoBehaviour
{
    GameObject objectToLookAt;
    public SpriteRenderer eventIcon;
    public Decision decision;

    void Start(){
        objectToLookAt = Camera.main.gameObject;
    }
    void Update(){
        transform.LookAt(objectToLookAt.transform);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z+180f);
    }
}
