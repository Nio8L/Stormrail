using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Environment Stat Augmentor")]
public class EnvironmentStatAugmentor : GenericStatAugmentor
{
    public List<HexTile.Type> boostedByTiles;
    public List<float> boostAmount;
    public int lookRadius;
    float lastBoost;
    HexTile tile;
    public override void OnUnlock(Industry _industry)
    {
        base.OnUnlock(_industry);
        tile = MapManager.instance.CityToTile(_industry.city);
        float boost = GetEnvironmentBoost(tile);
        for (int i = 0; i < items.Count; i++){
            if (!setTo){
                industry.itemOutputPerWorker[items[i]] += perWorker[i] * boost;
            }else{
                industry.itemOutputPerWorker[items[i]] = perWorker[i] * boost;
            }
        }   
        _industry.city.hungerDrainModifier *= hungerDrainModifier;

        lastBoost = boost;
    }

    public override void OnUpdate()
    {
        float boost = GetEnvironmentBoost(tile);
        if (boost != lastBoost){
            ReverseEffect();
            OnUnlock(industry);
        }
    }

    public override void ReverseEffect()
    {
        for (int i = 0; i < items.Count; i++){
            if (!setTo){
                industry.itemOutputPerWorker[items[i]] -= perWorker[i] * lastBoost;
            }else{
                industry.itemOutputPerWorker[items[i]] = 0;
            }
        }
    }

    float GetEnvironmentBoost(HexTile tile){
        float totalBoost = 0;
        List<HexTile.Type> types = tile.GetNeighborsType(lookRadius);
        foreach(HexTile.Type type in types){
            if (boostedByTiles.Contains(type)){
                totalBoost += boostAmount[boostedByTiles.IndexOf(type)];
            }
        }
        Debug.Log("Boost: " + totalBoost + " city: " + industry.city);
        return totalBoost;
    }
}
