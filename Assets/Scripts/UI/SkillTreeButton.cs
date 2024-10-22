using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTreeButton : MonoBehaviour, IPointerClickHandler
{
    public IndustryWindow industryWindow;
    public void OnPointerClick(PointerEventData eventData)
    {
        SkillTreeMenu.instance.gameObject.SetActive(true);
        SkillTreeMenu.instance.SelectIndustry(industryWindow.industry);
    }
}
