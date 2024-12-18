using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndustryWindow : MonoBehaviour
{
    public Industry industry;

    public TextMeshProUGUI nameTextBox;
    public TextMeshProUGUI workerTextBox;
    public WorkerBar workerBar;
    public UpgradeButton upgradeButton;
    public TextMeshProUGUI levelText;
    public RectTransform levelBar;
    public RectTransform levelBarBox;
    public Transform costBoxHolder;
    public GameObject prefabCostBox;

    public void UpdateIndustry(Industry newIndustry){
        industry = newIndustry;
        nameTextBox.text = industry.industryName;
        workerTextBox.text = CityMenu.instance.currentCity.workersPerIndustry[industry].ToString();
        workerBar.Initialize(this);
        upgradeButton.Initialize(this);
        LoadIndustryLevel();
    }

    public void LoadIndustryLevel(){
        levelText.text = industry.level + "/5";
        float barWidth = levelBarBox.rect.width/5 * industry.level;
        levelBar.sizeDelta = new Vector2(barWidth, levelBar.sizeDelta.y);

        IndustryLevelArray levelArray = DataBase.instance.industryLevelCosts[industry.level];
        for (int i = costBoxHolder.childCount; i > 0; i--){
            DestroyImmediate(costBoxHolder.GetChild(i-1).gameObject);
        }

        for (int i = 0; i < levelArray.industryLevels.Count; i++){
            IndustryLevelCost industryLevelCost = levelArray.industryLevels[i];
            GameObject newItemLabel = Instantiate(prefabCostBox, costBoxHolder);
            newItemLabel.transform.GetChild(0).GetComponent<Image>().sprite = industryLevelCost.item.itemIcon;
            newItemLabel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = industryLevelCost.cost.ToString();
        }
    }
}
