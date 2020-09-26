using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridActor : MonoBehaviour
{
    // Start is called before the first frame update

    public int gridPosX;
    public int gridPosY;

    public int maxMovementPoints;
    public int currentMovementPoints;
    public int attackRange;
    public float speed;
    public bool isSelected;

    public List<GridTile> currentPath;
    public Vector3 startTilePos;
    private Vector3 nextTilePos;
    private Vector3 targetTilePos;

    void Start()
    {
        isSelected = false;
        currentMovementPoints = maxMovementPoints;

        // Make aware of grid
        SetGridPos((int)transform.position.x, (int)transform.position.z);

        startTilePos = transform.position;

        // When instatiated let the grid know it occupies the space
        if (!GridManager.Inst.GetTileOccupier(gridPosX, gridPosY))
            GridManager.Inst.SetTileOccupier(this, gridPosX, gridPosY);
    }

    public void MoveTo(int targetX, int targetY)
    {
        targetTilePos = GridManager.Inst.GetTileCenterPos(targetX, 0, targetY);

        if (transform.position != targetTilePos)
        {
            currentPath.Clear();
            currentPath = GetComponent<GridPathfinding>().FindPath(gridPosX, gridPosY, targetX, targetY);

            StopCoroutine("MoveToTarget");
            StartCoroutine("MoveToTarget");
        }
        else
        {
            HideMovementTiles();
            ShowAttackTiles();
        }
    }

    // Move to tile
    IEnumerator MoveToTarget()
    {
        int count = 0;
        nextTilePos = currentPath[count].GetPos();

        // Loop through the path until object gets to target
        while (transform.position != targetTilePos)
        {
            // Go to next waypoint
            if (transform.position == nextTilePos)
            {
                count++;
                if (count >= currentPath.Count)
                    yield break;
                nextTilePos = currentPath[count].GetPos();
                currentMovementPoints--;
            }
            transform.position = Vector3.MoveTowards(transform.position, nextTilePos, speed * Time.deltaTime);

            yield return null;
        }

        // Arrived at destination
        currentMovementPoints--;
        SetGridPos((int)transform.position.x, (int)transform.position.z);
        HideMovementTiles();
        ShowAttackTiles();
    }

    public void ReturnToStartPosition()
    {
        // Stop any movement
        StopCoroutine("MoveToTarget");

        // Reset position movement
        transform.position = startTilePos;
        SetGridPos((int)startTilePos.x, (int)startTilePos.z);
        currentMovementPoints = maxMovementPoints;
        ShowMovementTiles();
    }

    public void ShowMovementTiles()
    {
        // Check movement range
        for (int x = gridPosX - maxMovementPoints; x <= gridPosX + maxMovementPoints; x++)
        {
            for (int y = gridPosY - maxMovementPoints; y <= gridPosY + maxMovementPoints; y++)
            {
                // Skip tiles outside movement range
                if ((Mathf.Abs(x - gridPosX) + Mathf.Abs(y - gridPosY)) > maxMovementPoints)
                    continue;

                if (GridManager.Inst.IsWithinGrid(x, y))
                {
                    // Set tile materials to blue (movement)
                    GridManager.Inst.SetTileState(GridTile.TileState.Movement, x, y);
                }
            }
        }
    }

    public void HideMovementTiles()
    {
        for (int x = gridPosX - maxMovementPoints; x <= gridPosX + maxMovementPoints; x++)
        {
            for (int y = gridPosY - maxMovementPoints; y <= gridPosY + maxMovementPoints; y++)
            {
                if (GridManager.Inst.IsWithinGrid(x, y))
                    GridManager.Inst.SetTileState(GridTile.TileState.None, x, y);
            }
        }
    }

    public void ShowAttackTiles()
    {
        // Check movement range
        for (int x = gridPosX - attackRange; x <= gridPosX + attackRange; x++)
        {
            for (int y = gridPosY - attackRange; y <= gridPosY + attackRange; y++)
            {
                // Skip tiles outside movement range
                if ((Mathf.Abs(x - gridPosX) + Mathf.Abs(y - gridPosY)) > attackRange)
                    continue;
                else if (gridPosX == x && gridPosY == y)
                    continue;

                if (GridManager.Inst.IsWithinGrid(x, y))
                {
                    // Set tile materials to blue (movement)
                    GridManager.Inst.SetTileState(GridTile.TileState.Attack, x, y);
                }
            }
        }
    }

    public void HideAttackTiles()
    {
        for (int x = gridPosX - attackRange; x <= gridPosX + attackRange; x++)
        {
            for (int y = gridPosY - attackRange; y <= gridPosY + attackRange; y++)
            {
                if (GridManager.Inst.IsWithinGrid(x, y))
                    GridManager.Inst.SetTileState(GridTile.TileState.None, x, y);
            }
        }
    }

    public void SetGridPos (int x, int y) 
    {
        GridManager.Inst.SetTileOccupier(null, gridPosX, gridPosY);

        gridPosX = x; 
        gridPosY = y;

        GridManager.Inst.SetTileOccupier(this, x, y);
    }

    public void Select() 
    { 
        isSelected = true;
        ShowMovementTiles();
    }
    public void Deselect() 
    {
        HideMovementTiles();
        isSelected = false;
    }
}
