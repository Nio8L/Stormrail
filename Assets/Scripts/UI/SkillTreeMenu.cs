using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeMenu : MonoBehaviour
{
    public static SkillTreeMenu instance;
    public TextMeshProUGUI industryNameBox;
    public TextMeshProUGUI skillPointsAmountBox;
    public GameObject descriptionPanel;
    public TextMeshProUGUI selectedSkillNameBox;
    public TextMeshProUGUI selectedSkillDescriptionBox;
    public Image selectedSkillIcon;
    public GameObject unlockSkillButton;
    public GameObject unlockedTextBox;
    public Skill selectedSkill;
    public SkillButton currentSkillButton;
    public Industry currentIndustry;


    public Skill[] skills;
    void Awake(){
        instance = this;
        gameObject.SetActive(false);
        descriptionPanel.SetActive(false);
    }
    public void SelectIndustry(Industry industry){
        currentIndustry = industry;

        Instantiate(industry.skillTree, transform);

        industryNameBox.text = currentIndustry.industryName;
        skillPointsAmountBox.text = currentIndustry.skillPoints.ToString();
    }
    public void SelectSkill(SkillButton skillButton){
        currentSkillButton = skillButton;
        selectedSkill = skillButton.thisSkill;
        descriptionPanel.SetActive(true);
        UpdateVisuals();
    }

    public void UnlockSkill(){
        currentIndustry.skillPoints--;

        currentIndustry.unlockedSkills.Add(selectedSkill);
        currentSkillButton.unlocked = true;

        selectedSkill.OnUnlock(currentIndustry);

        skillPointsAmountBox.text = currentIndustry.skillPoints.ToString();
        
        UpdateVisuals();
    }

    void UpdateVisuals(){
        selectedSkillNameBox.text = selectedSkill.skillName;
        selectedSkillDescriptionBox.text = selectedSkill.skillDescription;
        selectedSkillIcon.sprite = selectedSkill.skillIcon;

        if (currentSkillButton.unlocked){
            unlockedTextBox.SetActive(true);
            unlockSkillButton.SetActive(false);
        }else{
            unlockedTextBox.SetActive(false);
            unlockSkillButton.SetActive(true);
        }
    }

    public void CloseMenu(){
        Destroy(transform.GetChild(2).gameObject);
        gameObject.SetActive(false);
    }
}