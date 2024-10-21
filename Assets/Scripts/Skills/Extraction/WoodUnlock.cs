using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodUnlock : Skill
{
    public Item wood;
    public float woodPerWorker;
    public override void OnUnlock(Industry industry)
    {
        base.OnUnlock(industry);

        industry.itemOutputPerWorker[wood] = woodPerWorker;
    }
}
