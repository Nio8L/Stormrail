using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastChecker : MonoBehaviour
{
    public static RaycastChecker instance;
    bool hit;
    bool checkedThisFrame;
    void Awake (){
        instance = this;
    }

    void Update(){
        checkedThisFrame = false;
        hit = false;
    }
    public static bool Check(){
        if (!instance.checkedThisFrame){
            // Raycast on left click
            PointerEventData eventData = new PointerEventData(CityMenu.instance.eventSystem);
            eventData.position = Input.mousePosition;
            eventData.position = new Vector3(eventData.position.x, eventData.position.y, 10);

            List<RaycastResult> results = new List<RaycastResult>();
            CityMenu.instance.raycaster.Raycast(eventData, results);

            if (results.Any()) instance.hit = true;

            instance.checkedThisFrame = true;
        }
        return instance.hit;
    }

}
