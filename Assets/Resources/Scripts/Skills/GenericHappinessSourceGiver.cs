using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Generic Happiness Source Giver")]
public class GenericHappinessSourceGiver : GenericStatAugmentor
{
    public HappinessSource happinessSource;
    public override void OnUnlock(Industry _industry)
    {
        base.OnUnlock(_industry);
        _industry.city.AddHappinessSource(happinessSource.Copy());
    }

    public override void OnUpdate()
    {
        
    }
}
