using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Create Builder")]
public class BuilderSkill : Skill
{
    public override void OnUnlock(Industry _industry)
    {
        BuilderManager.instance.SpawnBuilder(MapManager.instance.StationToTile(_industry.city));
    }
}
