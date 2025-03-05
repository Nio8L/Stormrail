using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explorer : MonoBehaviour
{
    public string unitName;
    public Vector2Int coordinates;
    public float speed;
    public int revealRadius;

    [Header("Pathfinding")]
    public List<HexTile> currentPath;
    public Vector3 target;
    
    [Header("Movement")]
    float moveTimer = 0;
    public int currentIndex = 0;
    public bool move = false;

    private void Start() {
        transform.position = MapManager.instance.GetPositionForHexFromCoordinate(new Vector2Int(coordinates.x, -coordinates.y));
    }

    private void Update() {
        if(Input.GetMouseButton(1) && ExplorerManager.instance.selectedExplorer == this){
            ExplorerManager.instance.selectedExplorer = null;
            ExplorerManager.instance.DeletePreview();
        }

        if(Input.GetMouseButtonUp(0)){
            if(ExplorerManager.instance.selectedExplorer == this && CursorManager.instance.CheckMode(CursorManager.Mode.Explore)){
                if(MapManager.instance.hoveredTile != MapManager.instance.CoordinatesToTile(coordinates)){
                    NewPath(MapManager.instance.hoveredTile);
                    CursorManager.instance.SetMode(CursorManager.Mode.Neutral);
                    ExplorerManager.instance.DeletePreview();
                }
            }
        }

        if(!move) return;

        if(Vector3.Distance(transform.position, target) < 0.01){
            coordinates = currentPath[currentIndex].coordinates;

            Reveal();

            if(currentIndex == currentPath.Count - 1){
                move = false;

                UpdateUnitTab();
            }else{
                currentIndex++;
                target = currentPath[currentIndex].transform.position;
                target.y = 1;
                moveTimer = 0;
            }
        }
        
        Move();
    }

    public void Reveal(){
        HexTile currentTile = currentPath[currentIndex];
        List<HexTile> neighbors = currentTile.GetNeighbors(revealRadius);

        currentTile.Reveal();
        foreach (HexTile tile in neighbors)
        {
            tile.Reveal();    
        }
    }

    public void Move(){
        moveTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, target, moveTimer * speed);
    }

    public void FirstMove(){
        currentIndex = 0;
        target = currentPath[0].transform.position;
        target.y = 1;
        move = true;
    }

    public void NewPath(HexTile target){
        currentPath = Pathfinder.instance.PathfindAll(MapManager.instance.CoordinatesToTile(coordinates), target);
        FirstMove();
    }

    //Handles updating the unit tab, in case the explorer is entering a city 
    //and the city menu is looking at that city
    public void UpdateUnitTab(){
        //Check if the end of the path is a city
        if(CityManager.instance.GetCity(currentPath[currentIndex]) != null){
            //Check if the city menu is displaying that city
            if(CityMenu.instance.currentCity == CityManager.instance.GetCity(currentPath[currentIndex])){
                //Check if the unit tab is open
                if(CityMenu.instance.tabs[^1].activeSelf){
                    //Refresh the unit tab
                    CityMenu.instance.tabs[^1].GetComponent<UnitTab>().Refresh();
                }
            }
        }
    }

    private void OnMouseDown() {
        CursorManager.instance.SetMode(CursorManager.Mode.Explore);
        ExplorerManager.instance.selectedExplorer = this;
    }

    private void OnMouseEnter() {
        MapManager.instance.hoveredTile = MapManager.instance.CoordinatesToTile(coordinates);
    }
}
