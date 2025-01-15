using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkerBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Industry industry;
    public IndustryWindow industryWindow;
    bool pointerIn = false;
    public int workers = 0;
    public Image[] workerIcons;
    public TextMeshProUGUI[] workerIconsNumbers;
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIn = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left){
            ChangeWorkerCount(1);
        }else if (eventData.button == PointerEventData.InputButton.Right){
            ChangeWorkerCount(-1);
        }else{
            ChangeWorkerCount(-CityMenu.instance.currentCity.workersPerIndustry[industry]);
        }
    }

    void Update(){
        if (pointerIn){
            // Find if the player is scrolling
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0){
                // Multiply the scroll so 1 drag up or down is 1 worker
                scroll *= 10f;
                ChangeWorkerCount(Mathf.RoundToInt(scroll));
            }
        }
    }

    void ChangeWorkerCount(int amount){
        if (CityMenu.instance.currentCity.workers + amount <= CityMenu.instance.currentCity.population){
                workers += amount;

                int leftover = 0;
                if (workers < 0){leftover = Mathf.Abs(workers); workers = 0;}

                CityMenu.instance.currentCity.workersPerIndustry[industry] = workers;
                CityMenu.instance.currentCity.workers += amount + leftover;

                UpdateVisuals();
            }
    }

    int GetLimit(int limit){
        if (workers > limit) return GetLimit(limit * 5);
        return limit;
    }

    public void Initialize(IndustryWindow newIndustryWindow){
        industryWindow = newIndustryWindow;
        industry = industryWindow.industry;
        workers = CityMenu.instance.currentCity.workersPerIndustry[industry];
        UpdateVisuals();
    }

    public void UpdateVisuals(){
        int workersPerIcon = GetLimit(5)/5;
        industryWindow.workerTextBox.text = workers.ToString();

        // Make the worker indicators the correct color
        
        for (int i = 0; i < 5; i++){
            workerIcons[i].color = new Color(0, 0, 0, 0);
            workerIconsNumbers[i].color = new Color(0, 0, 0, 0);
        }

        // Find out how many of the icons should be visible
        int iconsToShow = Mathf.CeilToInt((float)workers/workersPerIcon);

        for (int i = 0; i < iconsToShow; i++){
            workerIcons[i].color = Color.white;
            workerIconsNumbers[i].color = Color.black;
        }

        // Update the worker text
        
        for (int i = 0; i < iconsToShow; i++){
            if (i != iconsToShow - 1 || workers % workersPerIcon == 0) workerIconsNumbers[i].text = workersPerIcon.ToString();
            else if (workers <= 5)    workerIconsNumbers[i].text = (workers%workersPerIcon + 1).ToString();
            else                      workerIconsNumbers[i].text = (workers%workersPerIcon).ToString();
        }
    }
}
