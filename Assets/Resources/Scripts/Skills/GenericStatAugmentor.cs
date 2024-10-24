using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Generic Stat Augmentor")]
public class GenericStatAugmentor : Skill
{
    public List<Item> items;
    public List<float> perWorker;
    public float hungerDrainModifier;
    public bool setTo; // Used to set stats to a value instead of adding them
    public override void OnUnlock(Industry _industry)
    {
        base.OnUnlock(_industry);
        for (int i = 0; i < items.Count; i++){
            if (!setTo){
                industry.itemOutputPerWorker[items[i]] += perWorker[i];
            }else{
                industry.itemOutputPerWorker[items[i]] = perWorker[i];
            }
        }   
        _industry.city.hungerDrainModifier *= hungerDrainModifier;
    }

    public override void OnUpdate()
    {
        
    }
}
