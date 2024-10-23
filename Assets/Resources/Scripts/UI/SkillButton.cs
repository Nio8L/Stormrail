using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerClickHandler
{
    public Skill thisSkill;
    public SkillButton prerequisite;
    public bool unlocked = false;
    void Start(){
        GetComponent<Image>().sprite = thisSkill.skillIcon;

        if (SkillTreeMenu.instance.currentIndustry.unlockedSkills.Contains(thisSkill)){
            unlocked = true;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SkillTreeMenu.instance.SelectSkill(this);
    }
}
