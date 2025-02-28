using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherStation : MonoBehaviour
{
    public int neededSteel;
    public int neededBricks;

    public Item steel;
    public Item bricks;

    Station station;

    public void Awake()
    {
        station = GetComponent<Station>();
    }

    void Update()
    {
        if (station.inventory[steel] >= neededSteel && station.inventory[bricks] >= neededBricks){
            Debug.Log("Win");
        }
    }
}
