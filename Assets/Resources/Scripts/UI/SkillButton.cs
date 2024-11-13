using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerClickHandler
{
    public Skill thisSkill;
    public List<SkillButton> prerequisites;
    public bool requireAllPrerequisites;
    public List<SkillButton> branches;
    public bool unlocked = false;
    void Start(){
        GetComponent<Image>().sprite = thisSkill.skillIcon;

        if (SkillTreeMenu.instance.currentIndustry.unlockedSkills.Contains(thisSkill)){
            unlocked = true;
        }
    }

    void Update(){
         if (!prerequisites.Any() && !prerequisite.unlocked){
            GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
         }else{
             GetComponent<Image>().color = Color.white;
         }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SkillTreeMenu.instance.SelectSkill(this);
    }
}
