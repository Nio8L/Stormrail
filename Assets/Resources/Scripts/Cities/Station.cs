using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public string cityName;
    public Vector2Int coordinates;
    [Header("Inventory and industry")]
    public Dictionary<Item, float> inventory = new Dictionary<Item, float>();
    protected virtual void Start()
    {
        //MapManager.instance.tiles[coordinates.x, coordinates.y].SetType(HexTile.Type.Station);
    }

    public void Initialize(Vector2Int coordinates, string cityName){
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            inventory.Add(DataBase.instance.allItems[i], 0);
        }

        this.coordinates = coordinates;
        this.cityName = cityName;
    }

    public virtual City GetCity(){
        return null;
    }

    internal void DestroyStation()
    {
        CityManager.instance.stations.Remove(this);
        Destroy(gameObject);
    }

    public void ConsumeResource(Item itemToConsume, float amount){
        // Consume a given resource
        inventory[itemToConsume] -= amount;
        if (inventory[itemToConsume] < 0) inventory[itemToConsume] = 0; 
    }

    public void GainResource(Item itemToGain, float amount){
        // Gain a single resource
        inventory[itemToGain] += amount;
    }

    protected virtual void Update(){
        if (MapManager.instance.StationToTile(this).revealed) transform.GetChild(0).gameObject.SetActive(true);
    }
}
