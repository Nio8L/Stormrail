using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IndustryWindow : MonoBehaviour
{
    public Industry industry;

    public TextMeshProUGUI nameTextBox;
    public TextMeshProUGUI workerTextBox;
    public WorkerBar workerBar;
    public UpgradeButton upgradeButton;
    public TextMeshProUGUI levelText;
    public RectTransform levelBar;

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
        float barWidth = 40f * industry.level;
        levelBar.sizeDelta = new Vector2(barWidth, levelBar.sizeDelta.y);

    }
}
