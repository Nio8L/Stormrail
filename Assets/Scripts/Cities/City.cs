using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class City : MonoBehaviour
{
    public string cityName;
    public int population;
    public int lastOpenTab = 0;
    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    public List<Item> allItems = new List<Item>();
    public Dictionary<Industry, int> workersPerIndustry = new Dictionary<Industry, int>();
    public List<Industry> allIndustries = new List<Industry>();
    
    void Start(){
        for (int i = 0; i < allItems.Count; i++){
            inventory.Add(allItems[i], 0);
        }
        for (int i = 0; i < allIndustries.Count; i++){
            allIndustries[i] = Instantiate(allIndustries[i]);
            workersPerIndustry.Add(allIndustries[i], 0);
        }
    }

    void Update(){
        inventory[allItems[7]]++;
    }
}
