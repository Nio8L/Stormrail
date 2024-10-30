using System;
using System.Collections;
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

    public void FindPath(HexTile startTile, HexTile endTile) {
        List<HexTile> toSearch =  new() {startTile};
        List<HexTile> proccesed = new();

        while(toSearch.Any()){
            HexTile currentTile = toSearch[0];

            foreach (HexTile tile in toSearch)
            {   
                if(tile.fCost < currentTile.fCost || tile.fCost == currentTile.fCost && tile.hCost < currentTile.hCost){
                    currentTile = tile;
                }
            }

            proccesed.Add(currentTile);
            toSearch.Remove(currentTile);

            //Start tracing back
            if(currentTile == endTile){
                HexTile currentPathTile = endTile;
                List<HexTile> path = new();
                
                while(currentPathTile != startTile){
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.Connection;
                }

                foreach (HexTile tile in path) tile.SetType(HexTile.Type.City);
                
                path.Reverse();
                //return path;
                Debug.Log("yes path");
                return;
            }

            foreach (HexTile neighbor in currentTile.Neighbors.Where(tile => tile.walkable && !proccesed.Contains(tile)))
            {
                bool inSearch = proccesed.Contains(neighbor);

                float costToNeighbor = currentTile.gCost + currentTile.GetDistance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.gCost) {
                    neighbor.gCost = costToNeighbor;
                    neighbor.Connection = currentTile;

                    if (!inSearch) {
                        neighbor.hCost = neighbor.GetDistance(endTile);
                        toSearch.Add(neighbor);
                    }
                }
            }
        }
        //return null;
        Debug.Log("no path");
        return;
    }
}
