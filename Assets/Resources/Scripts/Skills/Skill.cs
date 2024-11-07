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
    protected Industry industry;
    public virtual void OnUnlock(Industry _industry){
        OnLoad(_industry);
    }
    public virtual void OnUpdate(){

    }

    public void OnLoad(Industry _industry){
        industry = _industry;
    }
}
