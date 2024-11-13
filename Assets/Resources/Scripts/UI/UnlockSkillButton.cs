using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnlockSkillButton : MonoBehaviour, IPointerClickHandler
{
    public SkillTreeMenu skillTreeMenu;
    public void Start(){
        skillTreeMenu = SkillTreeMenu.instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        skillTreeMenu.UnlockSkill();
    }
}
