using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TrainOverview : MonoBehaviour
{
    public TMP_Dropdown routeDropdown;
    public TMP_InputField nameInput;
    public TextMeshProUGUI nextStopText;
    public List<TextMeshProUGUI> inventoryAmounts = new List<TextMeshProUGUI>();
    Train selectedTrain;
    public void Initialize(){
        routeDropdown.ClearOptions();
        List<string> options = new()
        {
            "No Route"
        };

        for(int i = 0; i < TrainManager.instance.routes.Count; i++){
            options.Add(TrainManager.instance.routes[i].name);
        }

        routeDropdown.AddOptions(options);
        //routeDropdown.value = cityIndex;

        selectedTrain = new();
    }

    void Update(){
        if (selectedTrain.name != ""){
            // Set next stop box
            if (selectedTrain.currentStop.name != ""){
                nextStopText.text = selectedTrain.currentStop.name;
            }else{
                nextStopText.text = "No stop selected";
            }
            // Update the train's inventory
            UpdateInventory();
        }
    }

    public void SelectTrain(Train train){
        selectedTrain = train;
        // Set name box
        nameInput.text = train.name;

        // Set next stop box
        if (train.currentStop.name != ""){
            nextStopText.text = train.currentStop.name;
        }else{
            nextStopText.text = "No stop selected";
        }
        
        // Set route dropdown to the correct value
        if (train.currentRoute.name != ""){
            int index = 0;
            for (int i = 0; i < TrainManager.instance.routes.Count; i++){
                if (TrainManager.instance.routes[i].name == train.currentRoute.name){
                    index = i + 1;
                    break;
                }
            }
            routeDropdown.value = index;
        }

        // Update the train's inventory
        UpdateInventory();
    }

    public void ChangeTrainName(){
        if(nameInput.text == ""){
            nameInput.text = selectedTrain.name;
            return;
        } 
            
        
        for(int i = 0; i < TrainMenu.instance.trainHolder.transform.childCount; i++){
            if(TrainMenu.instance.trainHolder.transform.GetChild(i).GetComponent<TrainPlateUI>().trainName.text == selectedTrain.name){
                string checker = nameInput.text;
                int counter = 0;
                while(TrainManager.instance.GetRoute(checker) != null){
                    counter++;
                    checker = nameInput.text + "_" + counter;
                }
                nameInput.text = checker;
                TrainMenu.instance.trainHolder.transform.GetChild(i).GetComponent<TrainPlateUI>().trainName.text = nameInput.text;
            }
        }
        
        selectedTrain.name = nameInput.text;
    }

    public void ChangeRoute(){
        string newRouteName = routeDropdown.options[routeDropdown.value].text;
        selectedTrain.currentRoute = TrainManager.instance.GetRoute(newRouteName);
        int index = TrainManager.instance.trains.IndexOf(selectedTrain);
        TrainManager.instance.locomotives[index].FirstMove();
    }

    void UpdateInventory(){
        for (int i = 0; i < selectedTrain.inventory.Count; i++){
            float amount = selectedTrain.inventory[DataBase.instance.allItems[i]];
            inventoryAmounts[i].text = amount + "kg";
        }
    }
}
