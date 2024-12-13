using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Event Pool")]
public class EventPool : ScriptableObject
{
    public List<Decision> events;
}
