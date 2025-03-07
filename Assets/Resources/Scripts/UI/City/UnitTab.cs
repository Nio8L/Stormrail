using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTab : MonoBehaviour
{
    public GameObject explorerWindowPrefab;
    public GameObject builderWindowPrefab;
    public GameObject unitWindowHolder;
    public List<GameObject> unitWindows;

    public void SwitchingCity(City currentCity){
        Refresh();
    }

    public void Refresh(){
        //DeleteAllWindows();
        //AddBuilderWindows();
    }

    public void AddBuilderWindows(){
        List<Builder> buildersInCity = BuilderManager.instance.GetBuildersInCity(CityMenu.instance.currentCity);

        foreach (Builder builder in buildersInCity)
        {
            GameObject newUnitWindow = Instantiate(builderWindowPrefab, unitWindowHolder.transform);
            BuilderWindow unitWindow = newUnitWindow.GetComponent<BuilderWindow>();
            
            unitWindow.builder = builder;
            unitWindows.Add(newUnitWindow);
        }
    }

    public void DeleteAllWindows(){
        for(int i = unitWindows.Count - 1; i >= 0; i--){
            Destroy(unitWindows[i]);
        }
        unitWindows.Clear();
    }

    private void OnEnable() {
        EventManager.OpenCity += SwitchingCity;

        SwitchingCity(CityMenu.instance.currentCity);
    }

    private void OnDisable() {
        EventManager.OpenCity -= SwitchingCity;
    }
}
