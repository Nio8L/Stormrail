using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTab : MonoBehaviour
{
    public GameObject unitWindowPrefab;
    public GameObject unitWindowHolder;
    public List<UnitWindow> unitWindows;

    public void SwitchingCity(City currentCity){
        Refresh();
    }

    public void Refresh(){
        DeleteExplorerWindows();
        AddExplorerWindows();
    }

    public void AddExplorerWindows(){
        List<Explorer> explorersInCity = ExplorerManager.instance.GetExplorer(CityMenu.instance.currentCity);

        foreach (Explorer explorer in explorersInCity)
        {
            GameObject newUnitWindow = Instantiate(unitWindowPrefab, unitWindowHolder.transform);
            UnitWindow unitWindow = newUnitWindow.GetComponent<UnitWindow>();
            
            unitWindow.explorer = explorer;
            unitWindows.Add(unitWindow);
        }
    }

    public void DeleteExplorerWindows(){
        for(int i = unitWindows.Count - 1; i >= 0; i--){
            Destroy(unitWindows[i].gameObject);
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
