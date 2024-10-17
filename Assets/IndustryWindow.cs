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

    public void UpdateIndustry(Industry newIndustry){
        industry = newIndustry;
        nameTextBox.text = industry.industryName;
        workerTextBox.text = CityMenu.instance.currentCity.workersPerIndustry[industry].ToString();
        workerBar.Initialize(this);
    }
}
