using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action<City> OpenCity;
    public static Action<City> CloseCity;
}
