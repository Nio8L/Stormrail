using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDecisionMenuButton : MonoBehaviour
{
    public static ShowDecisionMenuButton instance;

    void Awake(){
        instance = this;
        gameObject.SetActive(false);
    }
    public void OnClick(){
        DecisionMenu.instance.gameObject.SetActive(!DecisionMenu.instance.gameObject.activeSelf);
    }
}
