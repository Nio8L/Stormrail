using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HappinessSource
{
    public string sourceName;
    public float happinessModifier;
    public float daysLeft;
    public bool infiniteDuration;

    public HappinessSource(string _sourceName, float _happinessModifier, float _timeLeft, bool _infiniteDuration){
        sourceName = _sourceName;
        happinessModifier = _happinessModifier;
        daysLeft = _timeLeft;
        infiniteDuration = _infiniteDuration;
    }

    public HappinessSource Copy(){
        return new HappinessSource(sourceName, happinessModifier, daysLeft, infiniteDuration);
    }
}
