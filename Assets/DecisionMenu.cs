using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionMenu : MonoBehaviour
{
    public static DecisionMenu instance;
    public Decision currentEvent;
    public City currentCity;
    public GameObject decisionButton;
    [Header("UI")]
    public TextMeshProUGUI nameBox;
    public TextMeshProUGUI descriptionBox;
    public TextMeshProUGUI optionDescriptionBox;
    public Image imageBox;
    public Transform buttonHolder;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    public static void ChangeOptionDescription(string newDescription){
        instance.optionDescriptionBox.text = newDescription;
    }
    public static void OpenMenu(Decision eventToOpen){
        instance.currentEvent = eventToOpen;
        instance.gameObject.SetActive(true);
        instance.nameBox.text = eventToOpen.eventName;
        instance.descriptionBox.text = eventToOpen.eventDescription;

        // Clear old option
        for (int i = instance.buttonHolder.childCount-1; i >= 0; i--){
            Destroy(instance.buttonHolder.GetChild(i).gameObject);
        }

        // Create new options
        for (int i = 0; i < eventToOpen.options.Count; i++){
            Transform button = Instantiate(instance.decisionButton, instance.buttonHolder).transform;
            EventOptionButton eventOptionButton = button.GetComponent<EventOptionButton>();
            eventOptionButton.description = eventToOpen.options[i].optionDescription;
            eventOptionButton.index = i;
            button.GetChild(0).GetComponent<TextMeshProUGUI>().text = eventToOpen.options[i].optionName;
        }

        ChangeOptionDescription("");
    }

    public static void SelectOption(int index){
        if (instance.currentEvent.options[index].happinessSourceToAdd.sourceName != ""){
            instance.currentCity.AddHappinessSource(instance.currentEvent.options[index].happinessSourceToAdd);
        }
        CloseMenu();
    }

    public static void CloseMenu(){
        instance.gameObject.SetActive(false);
    }
}
