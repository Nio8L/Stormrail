using TMPro;
using UnityEngine;

public class UnitWindow : MonoBehaviour{
    public GameObject barObject;
    public GameObject foodBarHolder;
    
    public TextMeshProUGUI nameplate;

    public Explorer explorer;

    private void Start() {
        nameplate.text = explorer.unitName;

        //Fill the food meter according to the explorer's supply
        for(int i = 0; i < explorer.foodSupply; i++){
            Instantiate(barObject, foodBarHolder.transform);
        }
    }

    public void AddFood(){
        if(explorer.supplierCity.inventory[DataBase.instance.GetItem("Food")] <= 0){
            return;
        }

        if(explorer.foodSupply == 5){
            return;
        }
        
        Instantiate(barObject, foodBarHolder.transform);
        explorer.foodSupply++;
        explorer.supplierCity.ConsumeResource(DataBase.instance.GetItem("Food"), 1);
        
    }

    public void RemoveFood(){
        if(explorer.foodSupply <= 0){
            return;
        }
        
        Destroy(foodBarHolder.transform.GetChild(foodBarHolder.transform.childCount - 1).gameObject);
        explorer.foodSupply--;
        explorer.supplierCity.GainResource(DataBase.instance.GetItem("Food"), 1);
        
    }

}