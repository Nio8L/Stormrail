using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public GameObject extraInfoMenuPrefab;
    GameObject extraInfoMenu;
    public TextMeshProUGUI nameTextBox;
    public TextMeshProUGUI amountTextBox;
    public Image iconObject;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (extraInfoMenu == null){
            extraInfoMenu = Instantiate(extraInfoMenuPrefab, EconomyTab.instance.content.transform);
            extraInfoMenu.transform.SetSiblingIndex(transform.GetSiblingIndex()+1);
        }else{
            Destroy(extraInfoMenu);
        }
    }
}
