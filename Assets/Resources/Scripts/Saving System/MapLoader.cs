using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public static MapLoader instance;
    public string mapName;
    public Vector2Int mapSize;
    public bool loadingEditor;
    public bool startingNewGame;
    public bool loadStarter;

    void Awake(){
        if (instance != null) Destroy(instance);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void LoadEditor(string _mapName, Vector2Int _mapSize){
        instance.mapName = _mapName;
        instance.mapSize = _mapSize;
        instance.loadingEditor = true;

        ScreenCover.instance.TurnAnimationOn();
    }
    public static void LoadGame(string _mapName){
        instance.mapName = _mapName;
        instance.loadingEditor = false;
        instance.startingNewGame = false;

        ScreenCover.instance.TurnAnimationOn();
    }

    public static void LoadMap(string _mapName){
        instance.mapName = _mapName;
        instance.loadingEditor = false;
        instance.startingNewGame = true;

        ScreenCover.instance.TurnAnimationOn();
    }
}
