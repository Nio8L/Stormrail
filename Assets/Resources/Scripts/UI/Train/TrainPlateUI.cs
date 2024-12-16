using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrainPlateUI : MonoBehaviour
{
    public TextMeshProUGUI trainName;
    public void Initialize(string newName){
        trainName.text = newName;
    }

    public void SelectTrain(){
        TrainMenu.instance.trainNotSelectedWindow.SetActive(false);
        TrainMenu.instance.trainOverview.gameObject.SetActive(true);
        TrainMenu.instance.trainOverview.Initialize();
        TrainMenu.instance.trainOverview.SelectTrain(TrainManager.instance.GetTrain(trainName.text));
    }
}
