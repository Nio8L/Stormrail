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

    float tempTimer = 1.25f;

    void Awake(){
        instance = this;
    }

    void Start()
    {
        ScreenCover.instance.InstantSetToBlack();
    }

    void Update()
    {
        if (tempTimer > 0){
            tempTimer-= Time.unscaledDeltaTime;
            if (tempTimer <= 0){
                ScreenCover.instance.TurnAnimationOn();
            }
        }   
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
