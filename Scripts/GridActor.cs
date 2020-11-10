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

    private List<GridTile> possibleMovementTiles;

    // Pathfinding Stuff
    public List<GridTile> currentPath;
    public Vector3 startTilePos;
    private Vector3 nextTilePos;
    private Vector3 targetTilePos;

    void Start()
    {
        isSelected = false;
        currentMovementPoints = maxMovementPoints;
        possibleMovementTiles = new List<GridTile>();

        // Make aware of grid
        SetGridPos((int)transform.position.x, (int)transform.position.z);

        startTilePos = transform.position;

        // When instatiated let the grid know it occupies the space
        if (!GridManager.Inst.GetTileOccupier(gridPosX, gridPosY))
            GridManager.Inst.SetTileOccupier(this, gridPosX, gridPosY);
    }

    public void MoveTo(int _targetX, int _targetY)
    {
        targetTilePos = GridManager.Inst.GetTileCenterPos(_targetX, 0, _targetY);

        if (transform.position != targetTilePos)
        {
            currentPath.Clear();
            currentPath = GetComponent<GridPathfinding>().FindPath(gridPosX, gridPosY, _targetX, _targetY);

            StopCoroutine("MoveToTarget");
            StartCoroutine("MoveToTarget");
        }
        else
        {
            HideMovementTiles();
            Deselect();
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

    public void FindMovementTiles()
    {
        possibleMovementTiles.Clear();

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
                    possibleMovementTiles.Add(GridManager.Inst.GetTile(x, y));
                }
            }
        }
    }

    public void EndTurn()
    {
        currentMovementPoints = maxMovementPoints;
        startTilePos = transform.position;

        Deselect();
    }

    public void ShowMovementTiles()
    {
        foreach (GridTile currTile in possibleMovementTiles)
        {
            currTile.SetState(GridTile.TileState.Movement);
        }
    }

    public void HideMovementTiles()
    {
        foreach (GridTile currTile in possibleMovementTiles)
        {
            currTile.SetState(GridTile.TileState.None);
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
        FindMovementTiles();
        ShowMovementTiles();
    }
    public void Deselect() 
    {
        isSelected = false;
        HideMovementTiles();
    }
}
