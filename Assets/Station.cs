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
    protected void Start()
    {
        MapManager.instance.tiles[coordinates.x, coordinates.y].SetType(HexTile.Type.Station);
    }

    public void Initialize(Vector2Int coordinates, string cityName){
        for (int i = 0; i < DataBase.instance.allItems.Count; i++){
            inventory.Add(DataBase.instance.allItems[i], 0);
        }

        this.coordinates = coordinates;
        this.cityName = cityName;
    }
}
