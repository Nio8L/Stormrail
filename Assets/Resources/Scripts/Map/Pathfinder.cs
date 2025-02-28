using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder instance;
    public HexTile tile1;
    public HexTile tile2;

    private void Awake() {
        instance = this;
    }

    public void TryToConnect(HexTile tile){
        if(tile1 == null){
            tile1 = tile;
        }else{
            tile2 = tile;
            
            MapManager.instance.BuildRailConnection(tile1, tile2);
            
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

}
