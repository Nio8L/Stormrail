using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class ConditionPlateUI : MonoBehaviour
{
    public StopPlateUI stop;

    public Item item;
    public TextMeshProUGUI action;
    public TMP_InputField amount;
    public TMP_Dropdown dropdown;

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
        amount.text = condition.amount + "";
        dropdown.value = DataBase.instance.allItems.IndexOf(condition.item);
       
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

    public void ChangeAmount(){
        if (int.TryParse(amount.text, out int a))
        {
            stop.ChangeAmount(this, a);
        }
    }

    public void ChangeItem(){
        stop.ChangeItem(this, DataBase.instance.allItems[dropdown.value]);
    }
}
