using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;
    
    public Texture2D neutralCursor;
    public Texture2D buildCursor;
    public Texture2D exploreCursor;

    public enum Mode{
        Neutral,
        Build,
        Explore
    }

    public Mode mode;

    private void Awake() {
        if (instance != null) Destroy(instance);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if(Input.GetMouseButton(1)){
            SetMode(Mode.Neutral);
            Pathfinder.instance.ResetTiles();
        }
    }

    public void SetMode(Mode mode){
        this.mode = mode;
        
        if(CheckMode(Mode.Neutral)){
            Cursor.SetCursor(neutralCursor, Vector2.zero, CursorMode.Auto);
        }else if(CheckMode(Mode.Build)){
            Cursor.SetCursor(buildCursor, Vector2.zero, CursorMode.Auto);
        }else if(CheckMode(Mode.Explore)){
            Cursor.SetCursor(exploreCursor, new Vector2(exploreCursor.width / 2, exploreCursor.height / 2), CursorMode.Auto);
        }
    }

    public bool CheckMode(Mode mode){
        return this.mode == mode;
    }
}
