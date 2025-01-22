using TMPro;
using UnityEngine;

public class UnitWindow : MonoBehaviour{
    public GameObject foodBarPrefab;
    public GameObject foodBarHolder;
    
    public TextMeshProUGUI nameplate;

    public Explorer explorer;

    private void Start() {
        nameplate.text = explorer.unitName;

        //Fill the food meter according to the explorer's supply
        for(int i = 0; i < explorer.foodSupply; i++){
            Instantiate(foodBarPrefab, foodBarHolder.transform);
        }
    }

    public void AddFood(){
        if(explorer.foodSupply < 5){
            Instantiate(foodBarPrefab, foodBarHolder.transform);
            explorer.foodSupply++;
        }
    }

    public void RemoveFood(){
        if(explorer.foodSupply > 0){
            Destroy(foodBarHolder.transform.GetChild(foodBarHolder.transform.childCount - 1).gameObject);
            explorer.foodSupply--;
        }
    }

}