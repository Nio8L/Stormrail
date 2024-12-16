using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Industry")]
public class Industry : ScriptableObject
{
    public string industryName;
    public int level;
    public Dictionary<Item, float> itemOutputPerWorker = new Dictionary<Item, float>();
    public GameObject skillTree;
    public int skillPoints = 0;
    public City city;
    public List<Skill> unlockedSkills = new List<Skill>();
    public List<Skill> activeSkills = new List<Skill>();

    public void Initialize(City _city){
        city = _city;
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            itemOutputPerWorker.Add(DataBase.instance.allItems[i], 0);
        }
    }
    public void LevelUp(){
        level++;
        skillPoints += 2;
    }
}
