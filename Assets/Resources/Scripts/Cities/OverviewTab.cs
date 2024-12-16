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
    public List<HappinessSource> sources;
    public void UpdateInformation(){
        if (CityMenu.instance.currentCity == null) return;

        int happiness = Mathf.RoundToInt(CityMenu.instance.currentCity.lockedHappiness * 100);
        happinessText.text = happiness.ToString() + "%";

        int sourcesToRemove = sourceHolder.childCount;
        while(sourcesToRemove != 0){
            RemoveSourceAt(sourcesToRemove-1);
            sourcesToRemove--;
        }
        sources.Clear();

        AddNewSources();
    }
    void AddNewSources(){
        // Add all sources from the city to a new list
        foreach (HappinessSource happinessSource in CityMenu.instance.currentCity.happinessSources){
            sources.Add(happinessSource);
        }
        // Sort by happiness
        for (int select = 0; select < sources.Count-1; select++){
            float sHappiness = sources[select].happinessModifier;
            for (int look = select+1; look < sources.Count; look++){
                float lHappiness = sources[look].happinessModifier;
                if (lHappiness > 0 && sHappiness < lHappiness){
                    (sources[look], sources[select]) = (sources[select], sources[look]);
                }else if (lHappiness < 0 && sHappiness < 0 && sHappiness > lHappiness){
                    (sources[look], sources[select]) = (sources[select], sources[look]);
                }
            }
        }
        // Spawn all objects
        foreach (HappinessSource happinessSource in sources){
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

    void RemoveSourceAt(int index){
        DestroyImmediate(sourceHolder.GetChild(index).gameObject);
        sources.RemoveAt(index);
    }
}
