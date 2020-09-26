﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridPathfinding : MonoBehaviour
{
    public List<GridTile> path = new List<GridTile>();
    public List<GridTile> FindPath (int _startPosX, int _startPosY, int _targetPosX, int _targetPosY)
    {
        GridTile startTile = GridManager.Inst.GetTile(_startPosX, _startPosY);
        GridTile targetTile = GridManager.Inst.GetTile(_targetPosX, _targetPosY);

        List<GridTile> openSet = new List<GridTile>();
        HashSet<GridTile> closedSet = new HashSet<GridTile>();
        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            GridTile currentTile = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost() < currentTile.fCost() || openSet[i].fCost() == currentTile.fCost() && openSet[i].hCost < currentTile.hCost)
                    currentTile = openSet[i];
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);


            // If path was found
            if (currentTile == targetTile)
            {
                GridTile tempTile = targetTile;

                // Fill out the correct path and reorient it
                while (tempTile != startTile)
                {
                    path.Add(tempTile);
                    tempTile = tempTile.pathParent;
                }
                path.Reverse();

                openSet.Clear();
            }
                

            // Iterate through all tile neighbors to find path
            foreach (GridTile neighbor in GridManager.Inst.GetTileNeighbors(currentTile.GetGridPosX(), currentTile.GetGridPosY()))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int movementToNeighbor = currentTile.gCost + GetDistance(currentTile, neighbor);
                if (!openSet.Contains(neighbor) || movementToNeighbor < neighbor.gCost)
                {
                    neighbor.gCost = movementToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetTile);
                    neighbor.pathParent = currentTile;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return path;
    }

    // Get distance in tiles
    public int GetDistance(GridTile _startTile, GridTile _targetTile)
    {
        return (Mathf.Abs(_startTile.GetGridPosX() - _targetTile.GetGridPosX()) + Mathf.Abs(_startTile.GetGridPosY() - _targetTile.GetGridPosY()));
    }
}
