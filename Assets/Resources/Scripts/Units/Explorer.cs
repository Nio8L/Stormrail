using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explorer : MonoBehaviour
{
    public Vector2Int coordinates;
    public int speed;
    public int revealRadius;

    [Header("Pathfinding")]
    public List<HexTile> currentPath;
    public Vector3 target;
    
    [Header("Movement")]
    float moveTimer = 0;
    public int currentIndex = 0;
    bool move = false;

    private void Start() {
        //ExplorerManager.instance.explorers.Add(this);
        transform.position = MapManager.instance.GetPositionForHexFromCoordinate(new Vector2Int(coordinates.x, -coordinates.y));
    }

    private void Update() {
        if(Input.GetMouseButton(1) && ExplorerManager.instance.selectedExplorer == this){
            ExplorerManager.instance.selectedExplorer = null;
            ExplorerManager.instance.UpdatePreview();
        }
        
        if(ExplorerManager.instance.selectedExplorer == this && MapManager.instance.mode == MapManager.Mode.Explore){
            ExplorerManager.instance.BuildPreviewConnection(MapManager.instance.tiles[coordinates.x, coordinates.y]);
        }

        if(Input.GetMouseButtonUp(0)){
            if(ExplorerManager.instance.selectedExplorer == this && MapManager.instance.mode == MapManager.Mode.Explore){
                if(MapManager.instance.hoveredTile != MapManager.instance.CoordinatesToTile(coordinates)){
                    NewPath(MapManager.instance.hoveredTile);
                    MapManager.instance.mode = MapManager.Mode.None;
                }
            }
        }

        if(!move) return;

        if(Vector3.Distance(transform.position, target) < 0.01){
            coordinates = currentPath[currentIndex].coordinates;

            Reveal();

            if(currentIndex == currentPath.Count - 1){
                move = false;
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

    private void OnMouseDown() {
        MapManager.instance.mode = MapManager.Mode.Explore;
        ExplorerManager.instance.selectedExplorer = this;
    }
}
