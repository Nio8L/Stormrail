using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder instance;
    public HexTile tile1;
    public HexTile tile2;

    [Header("Preview")]
    public GameObject previewObject;
    public List<GameObject> previewPath;
    public HexTile start;
    public bool previewing = false;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        if(previewing){
            UpdatePreview();
        }

        if(Input.GetMouseButton(1)){
            previewing = false;
            start = null;
            DeletePreview();
        }
    }

    public void TryToConnect(HexTile tile){
        if(tile1 == null){
            tile1 = tile;
        }else{
            tile2 = tile;
            
           // BuilderManager.instance.CreateRailProject(Pathfind(tile1, tile2));
            
            ResetTiles();
        }
    }

    public void ResetTiles(){
        tile1 = null;
        tile2 = null;
    }

    public List<HexTile> Pathfind(HexTile start, HexTile end){
        Queue<HexTile> frontier = new();
        frontier.Enqueue(start);

        Dictionary<HexTile, HexTile> cameFrom = new();
        cameFrom.Add(start, null);
        while (frontier.Any())
        {
            HexTile current = frontier.Dequeue();

            if(current == end){
                List<HexTile> path = new();
                while (current != start)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (HexTile neighbor in current.Neighbors)
            {
                if(!cameFrom.ContainsKey(neighbor) && neighbor.walkable){
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }
        return null;
    }

    public List<HexTile> PathfindOnRails(HexTile start, HexTile end){
        Queue<HexTile> frontier = new();
        frontier.Enqueue(start);

        Dictionary<HexTile, HexTile> cameFrom = new();
        cameFrom.Add(start, null);
        while (frontier.Any())
        {
            HexTile current = frontier.Dequeue();

            if(current == end){
                List<HexTile> path = new();
                while (current != start)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (HexTile neighbor in current.Neighbors)
            {
                if(!cameFrom.ContainsKey(neighbor)){
                    int angle = MapManager.instance.GetAngle(current, neighbor);
                    if((current.angles.Contains(MapManager.FixAngle(angle - 180)) && neighbor.angles.Contains(angle))){
                        frontier.Enqueue(neighbor);
                        cameFrom[neighbor] = current;
                    }
                }
            }
        }
        return null;
    }

    public List<HexTile> PathfindOnRails(Station city1, Station city2){
        HexTile start = MapManager.instance.StationToTile(city1);
        HexTile end = MapManager.instance.StationToTile(city2);
        return PathfindOnRails(start, end);
    }

    public List<HexTile> PathfindAll(HexTile start, HexTile end){
        Queue<HexTile> frontier = new();
        frontier.Enqueue(start);

        Dictionary<HexTile, HexTile> cameFrom = new();
        cameFrom.Add(start, null);
        while (frontier.Any())
        {
            HexTile current = frontier.Dequeue();

            if(current == end){
                List<HexTile> path = new();
                while (current != start)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (HexTile neighbor in current.Neighbors)
            {
                if(!cameFrom.ContainsKey(neighbor)){
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }
        return null;
    }

     public List<HexTile> PathfindAll(HexTile start, HexTile end, int limit){
        Queue<HexTile> frontier = new();
        frontier.Enqueue(start);

        Dictionary<HexTile, HexTile> cameFrom = new();
        cameFrom.Add(start, null);
        while (frontier.Any())
        {
            HexTile current = frontier.Dequeue();

            if(current == end){
                List<HexTile> path = new();
                while (current != start)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(start);
                path.Reverse();
                return path.Take(limit + 1).ToList();
            }

            foreach (HexTile neighbor in current.Neighbors)
            {
                if(!cameFrom.ContainsKey(neighbor)){
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }
        return null;
    }


    public void BuildPreviewRail(HexTile tile1, HexTile tile2){
        int angle = MapManager.instance.GetAngle(tile1, tile2);
        int opposite = MapManager.FixAngle(angle - 180);

        GameObject rail1 = Instantiate(previewObject, tile1.transform.position, Quaternion.Euler(0, opposite, 0));
        GameObject rail2 = Instantiate(previewObject, tile2.transform.position,  Quaternion.Euler(0, angle, 0));

        previewPath.Add(rail1);
        previewPath.Add(rail2);
    }

    public void BuildPreviewConnection(HexTile tile){        
        List<HexTile> path = PathfindAll(tile, MapManager.instance.hoveredTile);
        
        if(path == null) return;

        for (int i = 0; i < path.Count - 1; i++)
        {
            BuildPreviewRail(path[i], path[i + 1]);
        }
    }

    public void UpdatePreview(){
        DeletePreview();

        BuildPreviewConnection(start);
        
    }

    public void DeletePreview(){
        for (int i = previewPath.Count - 1; i >= 0; i--)
        {
            Destroy(previewPath[i]);
        }

        previewPath = new();
    }

    public void StopPreview(){
        DeletePreview();
        previewing = false;
    }
}
