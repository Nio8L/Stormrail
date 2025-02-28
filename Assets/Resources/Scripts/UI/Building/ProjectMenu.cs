using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectMenu : MonoBehaviour
{
    public static ProjectMenu instance;

    public GameObject menu;

    public GameObject projectDisplayPrefab;

    public List<ProjectDisplay> projectDisplays;

    void Awake()
    {
        instance = this;
    }

    public void OpenMenu(){
        menu.SetActive(true);

        LoadProjects();
    }

    public void CloseMenu(){
        menu.SetActive(false);
    }

    public void LoadProjects(){
        DeleteProjects();

        foreach (Project project in BuilderManager.instance.projects)
        {
            GameObject newDisplay = Instantiate(projectDisplayPrefab, menu.transform);
            ProjectDisplay newProjectDisplay = newDisplay.GetComponent<ProjectDisplay>();
            
            projectDisplays.Add(newProjectDisplay);
            projectDisplays[^1].Initialize(project);
        }
    }

    public void DeleteProjects(){
        for(int i = projectDisplays.Count - 1; i >= 0; i--){
            Destroy(projectDisplays[i].gameObject);
        }

        projectDisplays = new();
    }
}
