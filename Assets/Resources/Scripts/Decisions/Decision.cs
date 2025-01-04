using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decision")]
public class Decision : ScriptableObject
{
    public string eventName;
    [TextArea]
    public string eventDescription;
    public List<Option> options = new List<Option>();

}
[Serializable]
public class Option{
    public string optionName;
    public string optionDescription;
    public HappinessSource happinessSourceToAdd;
    public List<Item> itemReward;
    public List<float> amountToGain;
    public List<Item> itemsToConsume;
    public List<float> amountToConsume;
    public List<string> happinessSourcesToRemove = new();

    public bool amountMultipliedByPopulation;
    public bool CheckResourceRequirements(City affectedCity){
        // Check if this option is possible to take with the city's available resources
        List<float> updatedAmountToConsume = new List<float>(amountToConsume);

        // Multiply the list by the city's population if necessary
        if (amountMultipliedByPopulation){
            for (int i = 0; i < updatedAmountToConsume.Count; i++){
                updatedAmountToConsume[i] *= affectedCity.population;
            }
        }

        return affectedCity.CheckInventoryFor(itemsToConsume, updatedAmountToConsume);
    }

    public void TakeOption(City affectedCity){
        if (affectedCity == null) return;
        // Population multiplier
        float multiplier = 1;
        if (amountMultipliedByPopulation) multiplier = affectedCity.population;

        // Add happiness source
        if (happinessSourceToAdd.sourceName != ""){
            affectedCity.AddHappinessSource(happinessSourceToAdd);
        }
        // Add items
        for (int i = 0; i < itemReward.Count; i++){
            affectedCity.GainResource(itemReward[i], amountToGain[i] * multiplier);
        }
        // Remove items
        for (int i = 0; i < itemsToConsume.Count; i++){
            affectedCity.ConsumeResource(itemsToConsume[i], amountToConsume[i] * multiplier);
        }
        // Remove sources
        for (int i = 0; i < happinessSourcesToRemove.Count; i++){
            HappinessSource happinessSource = new HappinessSource(happinessSourcesToRemove[i], 0, 0, false);
            affectedCity.RemoveHappinessSource(happinessSource);
        }
    }
}

