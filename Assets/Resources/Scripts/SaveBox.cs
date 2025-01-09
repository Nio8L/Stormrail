using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBox : MonoBehaviour
{
    public string path;
    public bool map;
    public bool loadStarter;
    public void LoadSave(){
        if (map){
            MapLoader.instance.loadStarter = false;
            MainMenu.instance.LoadMap(path);
        }else if (loadStarter){
            MapLoader.instance.loadStarter = true;
            MainMenu.instance.LoadMap(path);
        }else{
            MapLoader.instance.loadStarter = false;
            MainMenu.instance.LoadGame(path);
        }
    }
}
