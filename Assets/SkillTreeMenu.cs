using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeMenu : MonoBehaviour
{
    public static SkillTreeMenu instance;
    public GameObject descriptionPanel;
    public TextMeshProUGUI selectedSkillNameBox;
    public TextMeshProUGUI selectedSkillDescriptionBox;
    public Image selectedSkillIcon;
    Skill selectedSkill;
    void Awake(){
        instance = this;
    }
    public void SelectSkill(SkillButton skillButton){
        selectedSkill = skillButton.thisSkill;
        descriptionPanel.SetActive(true);
        UpdateVisuals();
    }

    void UpdateVisuals(){
        selectedSkillNameBox.text = selectedSkill.skillName;
        selectedSkillDescriptionBox.text = selectedSkill.skillDescription;
        selectedSkillIcon.sprite = selectedSkill.skillIcon;
    }
}
