using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Create Train")]
public class TrainSkill : Skill
{
    public string nameSuffix;
    public override void OnUnlock(Industry _industry)
    {
        Train newTrain = new Train(_industry.city.cityName + " train" + nameSuffix);
        TrainSerialized train = new(newTrain);
        train.cameFrom = new(_industry.city.coordinates);
        TrainManager.instance.InstantiateTrain(train, _industry.city);
    }
}
