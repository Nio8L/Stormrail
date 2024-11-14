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
    
    void Awake(){
        instance = this;
        gameObject.SetActive(false);
        descriptionPanel.SetActive(false);
    }
    void OnEnable(){
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
        if (currentIndustry.skillPoints == 0 || !SkillUnlockable()) return;
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

    public bool SkillUnlockable(){
        // Default case
        return SkillUnlockable(currentSkillButton);
    }
    public bool SkillUnlockable(SkillButton skillToCheck){
        // If the skill is unlocked return false
        if (skillToCheck.unlocked) return false;

        // Find how many prerequisites are unlcoked
        if (skillToCheck.prerequisites.Count != 0){
            int unlockedPrerequisites = 0;
            foreach(SkillButton button in skillToCheck.prerequisites){
                if (button.unlocked) unlockedPrerequisites++;
            }
            // Check prerequisite cases
            if (skillToCheck.requireAllPrerequisites){ 
                if (unlockedPrerequisites != skillToCheck.prerequisites.Count) return false;
            }
            else if(unlockedPrerequisites == 0) return false;
        }

        // Find how many branches are unlocked
        if (skillToCheck.branches.Count != 0){
            int unlockedBranches = 0;
            foreach(SkillButton button in skillToCheck.branches){
                if (button.unlocked) unlockedBranches++;
            }
            // Check branch cases
            if (unlockedBranches != 0) return false;
        }

        // All checks passed
        return true;
    }
}
