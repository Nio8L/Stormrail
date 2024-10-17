using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityMenuTabButton : MonoBehaviour, IPointerClickHandler
{
    public GameObject tabToTurnOn;

    public void OnPointerClick(PointerEventData eventData)
    {
        CityMenu.instance.OpenTab(tabToTurnOn);
    }
}
