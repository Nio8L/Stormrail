using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill")]
public class Skill: ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public Sprite skillIcon;
    public virtual void OnUnlock(Industry industry){

    }
    public virtual void OnUpdate(Industry industry){

    }
}
