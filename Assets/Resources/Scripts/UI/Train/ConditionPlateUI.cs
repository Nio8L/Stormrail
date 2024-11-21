using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConditionPlateUI : MonoBehaviour
{
    public StopPlateUI stop;

    public Item item;
    public TextMeshProUGUI action;

    public void Initialize(StopPlateUI newStop){
        stop = newStop;
    }

    public void Initialize(StopPlateUI newStop, Condition condition){
        stop = newStop;
        if(condition.load){
            action.text = "TAKE";
        }else{
            action.text = "GIVE";
        }
    }

    public void DeleteCondition(){
        stop.DeleteCondition(this);
    }

    public void ChangeAction(){
        if(action.text == "TAKE"){
            action.text = "GIVE";
            stop.ChangeAction(this, false);
        }else{
            action.text = "TAKE";
            stop.ChangeAction(this, true);
        }
    }
}
