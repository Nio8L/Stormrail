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
