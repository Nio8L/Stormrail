using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder instance;

    private void Awake() {
        instance = this;
    }
    
    public List<HexTile> Pathfind(City city1, City city2){
        HexTile start = MapManager.instance.tiles[city1.coordinates.x, city1.coordinates.y];
        HexTile end = MapManager.instance.tiles[city2.coordinates.x, city2.coordinates.y];
        return Pathfind(start, end);
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
        Debug.Log(loops);
        return null;
    }
}
