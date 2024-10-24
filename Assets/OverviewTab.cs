using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class OverviewTab : MonoBehaviour
{
    public TextMeshProUGUI happinessText;
    public GameObject sourceTextPrefab;
    public Transform sourceHolder;
    public List<string> sourceNames;

    void Update(){
        int happiness = Mathf.RoundToInt(CityMenu.instance.currentCity.overallHappiness * 100);
        happinessText.text = happiness.ToString() + "%";

        if (sourceHolder.transform.childCount < CityMenu.instance.currentCity.happinessSources.Count){
            // Add new sources
            AddNewSources();
        }else if (sourceHolder.transform.childCount > CityMenu.instance.currentCity.happinessSources.Count){
            RemoveUnusedSources();
        }
    }

    void AddNewSources(){
        foreach (HappinessSource happinessSource in CityMenu.instance.currentCity.happinessSources){
            if (sourceNames.Contains(happinessSource.sourceName)) continue;
            else{
                sourceNames.Add(happinessSource.sourceName);
                GameObject newTextObject = Instantiate(sourceTextPrefab, sourceHolder);
                TextMeshProUGUI nameTextBox = newTextObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI amountTextBox = newTextObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                int happiness = Mathf.RoundToInt(happinessSource.happinessModifier * 100);
                nameTextBox.text = happinessSource.sourceName;
                amountTextBox.text = happiness.ToString() + "%";

                if (happiness > 0){
                    nameTextBox.color = Color.green;
                    amountTextBox.color = Color.green;
                }else{
                    nameTextBox.color = Color.red;
                    amountTextBox.color = Color.red;
                }
                
            }
        }
    }

    void RemoveUnusedSources(){
        // Finds which objects need to be removed
        List<string> namesToRemove = new List<string>();
        foreach (string sourceName in sourceNames){
            bool found = false;
            // Loop through all city happiness sources
            for (int i = 0; i < CityMenu.instance.currentCity.happinessSources.Count; i++){
                HappinessSource happinessSource =  CityMenu.instance.currentCity.happinessSources[i];
                if (happinessSource.sourceName == sourceName){
                    // This object shouldn't be removed
                    found = true;
                    break;
                } 
            }

            // If the source isn't found remove the text object
            if (!found){
                for (int i = 0; i < sourceHolder.childCount; i++){
                    Transform textObject = sourceHolder.GetChild(i);
                    if (textObject.GetComponent<TextMeshProUGUI>().text == sourceName){
                        Destroy(textObject.gameObject);
                        namesToRemove.Add(sourceName);
                        break;
                    }
                }
            }
        }
        // Remove the names of the deleted objects from the list
        int amountOfRemovals = namesToRemove.Count;
        for (int i = 0; i < amountOfRemovals; i++){
            sourceNames.Remove(namesToRemove[0]);
            namesToRemove.RemoveAt(0);
        }
        
    }
}
