using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerClickHandler
{
    public Skill thisSkill;
    public List<SkillButton> prerequisites;
    public bool requireAllPrerequisites;
    public List<SkillButton> branches;
    public bool unlocked = false;
    public GameObject lineRenderer;
    Image thisImage;
    void Start(){
        thisImage = GetComponent<Image>();
        thisImage.sprite = thisSkill.skillIcon;

        if (SkillTreeMenu.instance.currentIndustry.unlockedSkills.Contains(thisSkill)){
            unlocked = true;
        }

        SkillTreeMenu.instance.allSkillButtons.Add(this);

        SpawnLines();
        CheckUnlockableState();
    }

    public void CheckUnlockableState(){
        if (unlocked){
            thisImage.color = Color.white;
        }else{
            if (!SkillTreeMenu.instance.SkillUnlockable(this) || SkillTreeMenu.instance.currentIndustry.skillPoints == 0){
                thisImage.color = new Color(0.6f, 0.1f, 0.1f, 1f);
            }else{
                thisImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SkillTreeMenu.instance.SelectSkill(this);
    }

    void SpawnLines(){
        foreach(SkillButton skillPrerequisite in prerequisites){
            UILineRenderer newLine = Instantiate(lineRenderer, transform.parent).GetComponent<UILineRenderer>();
            newLine.transform.SetAsFirstSibling();
            Vector2[] linePositions = new Vector2[2];
            linePositions[0] = transform.localPosition;
            linePositions[1] = skillPrerequisite.transform.localPosition;
            newLine.points = linePositions;
        }
    }
}
