using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuilderWindow : MonoBehaviour
{
    public GameObject barObject;
    public GameObject foodBarHolder;
    public GameObject steelBarHolder;

    public TextMeshProUGUI nameplate;

    public Builder builder;

    private void Start() {
        nameplate.text = builder.unitName;

        for(int i = 0; i < builder.foodSupply; i++){
            Instantiate(barObject, foodBarHolder.transform);
        }

        for(int i = 0; i < builder.steelSupply; i++){
            Instantiate(barObject, steelBarHolder.transform);
        }
    }

    public void AddFood(){
        if(builder.supplierCity.inventory[DataBase.instance.GetItem("Food")] <= 0){
            return;
        }
        
        if(builder.foodSupply == 5){
            return;
        }

        Instantiate(barObject, foodBarHolder.transform);
        builder.foodSupply++;
        builder.supplierCity.ConsumeResource(DataBase.instance.GetItem("Food"), 1);
    }
    
    public void RemoveFood(){
        if(builder.foodSupply <= 0){
            return;
        }
        
        Destroy(foodBarHolder.transform.GetChild(foodBarHolder.transform.childCount - 1).gameObject);
        builder.foodSupply--;
        builder.supplierCity.GainResource(DataBase.instance.GetItem("Food"), 1);
    }

    public void AddSteel(){
        if(builder.supplierCity.inventory[DataBase.instance.GetItem("Steel")] <= 0){
            return;
        }
        
        if(builder.steelSupply == 5){
            return;
        }

        
        Instantiate(barObject, steelBarHolder.transform);
        builder.steelSupply++;
        builder.supplierCity.ConsumeResource(DataBase.instance.GetItem("Steel"), 1);
        
    }

    public void RemoveSteel(){
        if(builder.steelSupply <= 0){
            return;
        }
        
        
        Destroy(steelBarHolder.transform.GetChild(steelBarHolder.transform.childCount - 1).gameObject);
        builder.steelSupply--;
        builder.supplierCity.GainResource(DataBase.instance.GetItem("Steel"), 1);
    }
}
