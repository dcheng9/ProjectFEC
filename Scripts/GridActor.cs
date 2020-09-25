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

    public void MoveTo(Vector2 _target)
    {
        currentPath.Clear();
        currentPath = GetComponent<GridPathfinding>().FindPath(new Vector2(gridPosX, gridPosY), _target);

        targetTilePos = new Vector3(_target.x, 0, _target.y);

        StartCoroutine(MoveToTarget());
        SetGridPos((int)_target.x, (int)_target.y);
    }

    // Move to tile
    IEnumerator MoveToTarget()
    {
        int count = 0;
        nextTilePos = currentPath[count].GetPos();
        currentMovementPoints--;

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
    }

    public void ReturnToStartPosition()
    {
        StopCoroutine(MoveToTarget());

        transform.position = startTilePos;

        SetGridPos((int)startTilePos.x, (int)startTilePos.z);
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

    public void SetGridPos (int x, int y) 
    {
        GridManager.Inst.SetTileOccupier(null, gridPosX, gridPosY);

        gridPosX = x; 
        gridPosY = y;

        GridManager.Inst.SetTileOccupier(this, x, y);
    }

    public void Select() { isSelected = true; }
    public void Deselect() 
    {
        HideMovementTiles();
        isSelected = false;
    }
}
