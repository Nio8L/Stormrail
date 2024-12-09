using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decision")]
public class Decision : ScriptableObject
{
    public string eventName;
    [TextArea]
    public string eventDescription;
    public List<Option> options = new List<Option>();

}
[Serializable]
public class Option{
    public string optionName;
    public string optionDescription;
}
