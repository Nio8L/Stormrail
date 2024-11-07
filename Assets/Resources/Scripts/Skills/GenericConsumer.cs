using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Generic Consumer")]
public class GenericConsumer : GenericStatAugmentor
{
    public List<Item> itemsToConsume;
    public List<float> consumptionPerWorker;
    bool producing;
    public override void OnUnlock(Industry _industry)
    {
        base.OnUnlock(_industry);
        producing = true;
    }

    public override void OnUpdate()
    {
        // Find all and calculate all items that need to be consumed
        Dictionary<Item, float> amountOfItemsToConsume = new Dictionary<Item, float>();
        bool canProduce = true;
        for (int i = 0; i < itemsToConsume.Count; i++){
            float consumption = industry.city.CalculateConsumption(consumptionPerWorker[i], industry.city.workersPerIndustry[industry]) * Time.deltaTime;
            // If there aren't enough items shut off production and stop calculating
            if (industry.city.inventory[itemsToConsume[i]] < consumption){
                canProduce = false;
                break; 
            }

            amountOfItemsToConsume.Add(itemsToConsume[i], consumption);
        }

        if (canProduce){
            // Continue production
            if (!producing){
                producing = true;
                for (int i = 0; i < items.Count; i++){
                    industry.itemOutputPerWorker[items[i]] += perWorker[i];
                }
            }

            // Consume items
            for (int i = 0; i < amountOfItemsToConsume.Count; i++){
                KeyValuePair<Item, float> item = amountOfItemsToConsume.ElementAt(i);
                industry.city.consumingThisFrame[item.Key] += item.Value;
            }
        }else if (producing){
            // Stop production
            producing = false;
            for (int i = 0; i < items.Count; i++){
                industry.itemOutputPerWorker[items[i]] -= perWorker[i];
            }
        }
    }
}
