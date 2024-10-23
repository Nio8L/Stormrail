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
        if (skillTreeMenu.currentIndustry.skillPoints > 0 && SkillUnlockable()){
            skillTreeMenu.UnlockSkill();
        }
    }

    bool SkillUnlockable(){
        // If there is no skill selected, or if the skill is unlocked or if the prerequisite of the skill is not unlocked return false
        if (skillTreeMenu.selectedSkill == null || skillTreeMenu.currentSkillButton.unlocked 
        || (skillTreeMenu.currentSkillButton.prerequisite != null && !skillTreeMenu.currentSkillButton.prerequisite.unlocked)) return false;

        return true;
    }
}
