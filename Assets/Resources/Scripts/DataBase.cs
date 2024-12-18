using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    public static DataBase instance;
    public List<Item> allItems = new List<Item>();
    public List<Industry> allIndustries = new List<Industry>();
    public List<IndustryLevelArray> industryLevelCosts;
    public float dayLenghtInSeconds;
    public float baseFoodConsumedPerDayPerPerson;

    void Awake(){
        instance = this;
    }

    public Item GetItem(string itemName){
        foreach (Item item in allItems)
        {
            if(item.itemName == itemName){
                return item;
            }
        }
        return null;
    }
}
