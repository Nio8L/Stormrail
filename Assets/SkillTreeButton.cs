using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTreeButton : MonoBehaviour, IPointerClickHandler
{
    GameObject skillTreeMenu;

    void Start(){
        skillTreeMenu = GameObject.Find("SkillTreeMenu");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        skillTreeMenu.SetActive(true);
    }
}
