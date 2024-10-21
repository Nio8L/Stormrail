using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    public static DataBase instance;
    public List<Item> allItems = new List<Item>();
    public List<Industry> allIndustries = new List<Industry>();

    void Awake(){
        instance = this;
    }
}
