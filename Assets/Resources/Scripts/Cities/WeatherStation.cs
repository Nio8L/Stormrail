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

    bool win = false;

    public void Awake()
    {
        station = GetComponent<Station>();
    }

    void Update()
    {
        if (!win && station.inventory[steel] >= neededSteel && station.inventory[bricks] >= neededBricks){
            win = true;
            FindObjectOfType<CameraController>().LockOn(GetComponent<Followable>());
        }
    }
}
