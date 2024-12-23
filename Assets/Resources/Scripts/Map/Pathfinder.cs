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
        int loops = 0;
        while (frontier.Any())
        {
            loops++;
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
        int loops = 0;
        while (frontier.Any())
        {
            loops++;
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

    public List<HexTile> PathfindOnRails(City city1, City city2){
        HexTile start = MapManager.instance.CityToTile(city1);
        HexTile end = MapManager.instance.CityToTile(city2);
        return PathfindOnRails(start, end);
    }

    public List<HexTile> PathfindOnRails(Vector2Int coord1, Vector2Int coord2){
        HexTile start = MapManager.instance.tiles[coord1.x, coord1.y];
        HexTile end = MapManager.instance.tiles[coord2.x, coord2.y];
        return PathfindOnRails(start, end);
    }

    
}
