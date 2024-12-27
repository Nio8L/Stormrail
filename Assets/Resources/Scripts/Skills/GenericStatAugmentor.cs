using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Generic Stat Augmentor")]
public class GenericStatAugmentor : Skill
{
    public List<Item> items;
    public List<float> perWorker;
    public float hungerDrainModifier;
    public override void OnUnlock(Industry _industry)
    {
        base.OnUnlock(_industry);
        for (int i = 0; i < items.Count; i++){
            productionPerSecond[items[i]] = perWorker[i];
        }   
        _industry.city.hungerDrainModifier *= hungerDrainModifier;
    }

    public override void OnUpdate()
    {
        
    }

    public override void ReverseEffect()
    {
        base.ReverseEffect();
        for (int i = 0; i < items.Count; i++){
            productionPerSecond[items[i]] = 0;
        }
    }

    public override void OnLoad(Industry _industry){
        base.OnLoad(_industry);
        OnUnlock(_industry);
    }
}
