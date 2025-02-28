using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectDisplay : MonoBehaviour
{
    public TextMeshProUGUI nameplate;
    public Image icon;

    public Project project;

    public void Initialize(Project project){
        nameplate.text = project.name;
        this.project = project;
    }

    public void Build(){
        BuilderManager.instance.selectedBuilder.StartProject(project);
    }
}
