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
        if(builder.foodSupply < 5){
            Instantiate(barObject, foodBarHolder.transform);
            builder.foodSupply++;
        }
    }
    
    public void RemoveFood(){
        if(builder.foodSupply > 0){
            Destroy(foodBarHolder.transform.GetChild(foodBarHolder.transform.childCount - 1).gameObject);
            builder.foodSupply--;
        }
    }

    public void AddSteel(){
        if(builder.steelSupply < 5){
            Instantiate(barObject, steelBarHolder.transform);
            builder.steelSupply++;
        }
    }

    public void RemoveSteel(){
        if(builder.steelSupply > 0){
            Destroy(steelBarHolder.transform.GetChild(steelBarHolder.transform.childCount - 1).gameObject);
            builder.steelSupply--;
        }
    }
}
