using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill: ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public Sprite skillIcon;
    protected Industry industry;
    public Skill upgradesFrom;
    public Dictionary<Item, float> productionPerSecond = new();
    public virtual void OnUnlock(Industry _industry){
        // If this skill is an uprade find and remove the old upgrade
        if (upgradesFrom != null){
            for (int i = 0; i < industry.activeSkills.Count; i++){
                Skill skill = industry.activeSkills[i];
                if (skill.skillName == upgradesFrom.skillName){
                    skill.ReverseEffect();
                    industry.activeSkills.Remove(skill);
                    break;
                }
            }
        }
    }
    public virtual void OnUpdate(){

    }

    public virtual void OnLoad(Industry _industry){
        industry = _industry;
        productionPerSecond.Clear();
        foreach (Item item in DataBase.instance.allItems){
            productionPerSecond.Add(item, 0);
        }
    }

    public virtual void ReverseEffect(){
        
    }
}
