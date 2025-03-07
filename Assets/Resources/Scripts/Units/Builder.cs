using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public string unitName;
    public Vector2Int coordinates;
    public float speed;

    public Project currentProject;

    [Header("Pathfinding")]
    public List<HexTile> currentPath;
    public Vector3 target;
    
    [Header("Movement")]
    float moveTimer = 0;
    public int currentIndex = 0;
    public bool move = false;

    [Header("Supplies")]
    public int foodSupply = 0;
    public int steelSupply = 0;
    public City supplierCity;

    private void Start() {
        transform.position = MapManager.instance.GetPositionForHexFromCoordinate(new Vector2Int(coordinates.x, -coordinates.y));
        /*if(CityManager.instance.GetCity(coordinates)){
            supplierCity = CityManager.instance.GetCity(coordinates);
        }*/
    }

    private void Update() {
        if(Input.GetMouseButton(1) && BuilderManager.instance.selectedBuilder == this){
            BuilderManager.instance.selectedBuilder = null;
            ProjectMenu.instance.CloseMenu();
        }

        if(!move) return;

        if(Vector3.Distance(transform.position, target) < 0.01){
            coordinates = currentPath[currentIndex].coordinates;

            if(currentIndex == currentPath.Count - 1){
                move = false;

                //int blueprintCount = currentProject.blueprints.Count;
                if(currentProject.blueprints.Count > 0){
                    HexTile tile1 = currentProject.occupiedTiles[0];
                    HexTile tile2 = tile1;
                    if(currentProject.blueprints.Count >= 2){
                        tile2 = currentProject.occupiedTiles[1];
                    }

                    currentProject.buildMethod(tile1, tile2);
                    
                    Destroy(currentProject.blueprints[0]);
                    currentProject.blueprints.RemoveAt(0);
                    
                    if(currentProject.blueprints.Any()){
                        Destroy(currentProject.blueprints[0]);
                        currentProject.blueprints.RemoveAt(0);
                    }
                    
                    currentProject.occupiedTiles.RemoveAt(0);

                    NewPath(tile2);

                    if(currentProject.blueprints.Count == 0){
                        BuilderManager.instance.projects.Remove(currentProject);
                        ProjectMenu.instance.LoadProjects();
                    }
                }

               /* if(CityManager.instance.GetCity(currentPath[currentIndex]) != null){
                    //supplierCity = CityManager.instance.GetCity(currentPath[currentIndex]);
                    //returning = false;
                }else{
                    //returning = true;
                    currentPath.Reverse();
                    FirstMove();
                }*/

                //UpdateUnitTab();
            }else{
                currentIndex++;
                target = currentPath[currentIndex].transform.position;
                target.y = 0.5f;
                moveTimer = 0;
                
               /* if(!returning){
                    foodSupply--;
                }*/
            }
        }
        
        Move();
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

    public void StartProject(Project project){
        currentProject = project;

        NewPath(MapManager.instance.CoordinatesToTile(currentProject.startingCoordinates));
    }

    //Handles updating the unit tab, in case the builder is entering a city 
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
        BuilderManager.instance.selectedBuilder = this;
        ProjectMenu.instance.OpenMenu();
    }

    private void OnMouseEnter() {
        MapManager.instance.hoveredTile = MapManager.instance.CoordinatesToTile(coordinates);
    }
}
