using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerClickHandler
{
    IndustryWindow industryWindow;
    Industry industry;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (industry.level < 5){
            industry.LevelUp();
            industryWindow.LoadIndustryLevel();
        }
    }

    public void Initialize(IndustryWindow newIndustryWindow){
        industryWindow = newIndustryWindow;
        industry = industryWindow.industry;
    }
}
