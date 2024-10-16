using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationInteractor : MonoBehaviour
{
    public CityMenu cityMenu;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            // Return of the mouse is click on top of the city menu ui
            if (cityMenu.currentCity != null && Input.mousePosition.x < 256f) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits){
                if (hit.collider.CompareTag("CityTag")){
                    CityMenu.instance.OpenMenu(hit.collider.GetComponent<CityTag>().cityToFollow);
                }
            }
        }
    }
}
