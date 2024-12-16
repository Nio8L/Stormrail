using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOptionButton : MonoBehaviour
{
    public string description;
    public int index;
    public void OnPointerEnter(){
        DecisionMenu.ChangeOptionDescription(description);
    }

    public void OnPointerExit(){
        DecisionMenu.ChangeOptionDescription("");
    }

    public void OnClick(){
        DecisionMenu.SelectOption(index);
    }
}
