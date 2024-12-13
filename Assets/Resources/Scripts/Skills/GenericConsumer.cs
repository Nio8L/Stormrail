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
    Dictionary<Item, float> amountOfItemsToConsume = new Dictionary<Item, float>();
    public override void OnUnlock(Industry _industry)
    {
        base.OnUnlock(_industry);
        producing = true;
    }

    public override void OnUpdate()
    {
        // Find all and calculate all items that need to be consumed
        amountOfItemsToConsume.Clear();
        bool canProduce = true;
        for (int i = 0; i < itemsToConsume.Count; i++){
            float consumption = industry.city.CalculateConsumption(consumptionPerWorker[i], industry.city.workersPerIndustry[industry]) * Time.deltaTime;
            // If there aren't enough items shut off production and stop calculating
            if (!industry.city.CheckInventoryFor(itemsToConsume[i], consumption)){
                canProduce = false;
                break; 
            }

            amountOfItemsToConsume.Add(itemsToConsume[i], consumption);
        }

        if (canProduce){
            // Continue production
            if (!producing){
                // Restore production
                producing = true;
                for (int i = 0; i < items.Count; i++){
                    if (!setTo){
                        industry.itemOutputPerWorker[items[i]] += perWorker[i];
                    }else{
                        industry.itemOutputPerWorker[items[i]] = perWorker[i];
                    }
                }
            }
            
            // Consume items
            for (int i = 0; i < amountOfItemsToConsume.Count; i++){
                KeyValuePair<Item, float> item = amountOfItemsToConsume.ElementAt(i);
                industry.city.consumingThisFrame[item.Key] += item.Value;
            }
        }else if (producing){
            // Stop production
            ReverseEffect();
        }
    }

    public override void ReverseEffect()
    {
        if (!producing) return;
        producing = false;

        base.ReverseEffect();
    }
}
