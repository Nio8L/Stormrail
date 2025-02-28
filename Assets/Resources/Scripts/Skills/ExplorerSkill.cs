using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Create Explorer")]
public class ExplorerSkill : Skill
{
    public override void OnUnlock(Industry _industry)
    {
        ExplorerManager.instance.SpawnExplorer(MapManager.instance.StationToTile(_industry.city));
    }
}
