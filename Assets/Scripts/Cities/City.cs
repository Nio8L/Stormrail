using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class City : MonoBehaviour
{
    public string cityName;
    public int population;
    public int workers;
    public int lastOpenTab = 0;
    public Dictionary<Item, float> inventory = new Dictionary<Item, float>();
    public Dictionary<Industry, int> workersPerIndustry = new Dictionary<Industry, int>();
    void Start(){
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            //inventory.Add(DataBase.instance.allItems[i], 0);
        }
        for (int i = 0; i < DataBase.instance.allIndustries.Count; i++){
            // Industry newIndustry = Instantiate(DataBase.instance.allIndustries[i])
            // newIndustry.Initialize.Initialize(this)
            // workersPerIndustry.Add(newIndustry, 0);
        }

        //CityManager.instance.cities.Add(this);
    }

    void Update(){
        GainResources();
    }

    void GainResources(){
        for (int i = 0; i < workersPerIndustry.Count; i++){
            // Get every industry in this city
            KeyValuePair<Industry, int> industryPair = workersPerIndustry.ElementAt(i);

            if (industryPair.Key.level == 0) continue;

            for (int product = 0; product < industryPair.Key.itemOutputPerWorker.Count; product++){
                // Get every product of that industry
                KeyValuePair<Item, float> itemPair = industryPair.Key.itemOutputPerWorker.ElementAt(product);
                // Get the level multiplayer
                float multiplier = industryPair.Key.levelMultiplier[industryPair.Key.level];

                // Add the product to the inventory of the city
                float amountToGain = itemPair.Value * multiplier * industryPair.Value * Time.deltaTime;
                inventory[itemPair.Key] += amountToGain;
            }
        }
    }
}
