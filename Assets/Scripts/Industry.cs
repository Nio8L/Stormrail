using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Industry")]
public class Industry : ScriptableObject
{
    public string industryName;
    public int level;

    public float[] levelMultiplier;
    public Dictionary<Item, float> itemOutputPerWorker = new Dictionary<Item, float>();
    public List<Item> items;
    public List<float> perWorker;

    public void Initialize(City city){
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            itemOutputPerWorker.Add(DataBase.instance.allItems[i], 0);
        }
        /*for (int i = 0; i < items.Count; i++){
            itemOutputPerWorker[items[i]] = perWorker[i];
        }*/
    }
    public void LevelUp(){
        level++;
    }
}
