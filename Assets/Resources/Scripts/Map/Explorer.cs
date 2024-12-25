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
    int currentIndex = 0;
    bool move = false;
    
    [Header("Path preview")]
    public List<GameObject> previewPath;
    public GameObject pathPreview;

    private void Start() {
        transform.position = MapManager.instance.GetPositionForHexFromCoordinate(new Vector2Int(coordinates.x, -coordinates.y));
    }

    private void Update() {
        if(Input.GetMouseButton(1) && MapManager.instance.selectedExplorer == this){
            MapManager.instance.selectedExplorer = null;
            UpdatePreview();
        }
        
        if(MapManager.instance.selectedExplorer == this && MapManager.instance.mode == MapManager.Mode.Explore){
            BuildPreviewConnection(MapManager.instance.tiles[coordinates.x, coordinates.y]);
        }

        if(Input.GetMouseButtonUp(0)){
            if(MapManager.instance.selectedExplorer == this && MapManager.instance.mode == MapManager.Mode.Explore){
                if(MapManager.instance.hoveredTile != MapManager.instance.CoordinatesToTile(coordinates)){
                    currentPath = Pathfinder.instance.PathfindAll(MapManager.instance.CoordinatesToTile(coordinates), MapManager.instance.hoveredTile);
                    FirstMove();
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

    public void BuildPreviewRail(HexTile tile1, HexTile tile2){
        int angle = MapManager.instance.GetAngle(tile1, tile2);
        int opposite = MapManager.FixAngle(angle - 180);

        GameObject rail1 = Instantiate(pathPreview, tile1.transform.position, Quaternion.Euler(0, opposite, 0));
        GameObject rail2 = Instantiate(pathPreview, tile2.transform.position,  Quaternion.Euler(0, angle, 0));

        previewPath.Add(rail1);
        previewPath.Add(rail2);
    }

    public void BuildPreviewConnection(HexTile tile){        
        List<HexTile> path = Pathfinder.instance.PathfindAll(tile, MapManager.instance.hoveredTile);
        
        if(path == null) return;

        for (int i = 0; i < path.Count - 1; i++)
        {
            BuildPreviewRail(path[i], path[i + 1]);
        }
    }

    public void UpdatePreview(){
        for (int i = previewPath.Count - 1; i >= 0; i--)
        {
            Destroy(previewPath[i]);
        }

        previewPath = new();
    }

    private void OnMouseDown() {
        MapManager.instance.mode = MapManager.Mode.Explore;
        MapManager.instance.selectedExplorer = this;
    }
}
