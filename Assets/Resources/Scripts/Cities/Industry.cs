using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Industry")]
public class Industry : ScriptableObject
{
    public string industryName;
    public int level;
    public GameObject skillTree;
    public int skillPoints = 0;
    public City city;
    public List<Skill> unlockedSkills = new List<Skill>();
    public List<Skill> activeSkills = new List<Skill>();

    public void Initialize(City _city){
        city = _city;
    }
    public void LevelUp(){
        List<Item> items = new();
        List<float> amounts = new();

        IndustryLevelArray levelArray = DataBase.instance.industryLevelCosts[level];
        for (int i = 0; i < levelArray.industryLevels.Count; i++){
            items.Add(levelArray.industryLevels[i].item);
            amounts.Add(levelArray.industryLevels[i].cost);
        }
        if (city.CheckInventoryFor(items, amounts)){
            for (int i = 0; i < items.Count; i++){
                city.ConsumeResource(items[i], amounts[i]);
            }
            level++;
            skillPoints += 2;
        }
    }
}

[Serializable]
public class IndustryLevelCost{
    public Item item;
    public int cost;
}

[Serializable]
public class IndustryLevelArray{
    public List<IndustryLevelCost> industryLevels;
}
