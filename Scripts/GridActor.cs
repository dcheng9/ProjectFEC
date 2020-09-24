using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridActor : MonoBehaviour
{
    // Start is called before the first frame update

    public int gridPosX;
    public int gridPosY;

    public int maxMovementPoints;
    public float speed;
    public bool isSelected;

    public List<GridTile> currentPath;
    private Vector3 nextTilePos;

    void Start()
    {
        isSelected = false;

        SetGridPos((int)transform.position.x, (int)transform.position.z);
        transform.position = GridManager.Inst.GetTileCenterPos(gridPosX, transform.position.y, gridPosY);

        // When instatiated let the grid know it occupies the space
        if (!GridManager.Inst.GetTileOccupier(gridPosX, gridPosY))
            GridManager.Inst.SetTileOccupier(this, gridPosX, gridPosY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PathfindingTo(Vector2 _target)
    {
        currentPath.Clear();
        currentPath = GetComponent<GridPathfinding>().FindPath(new Vector2(gridPosX, gridPosY), _target);

        StartCoroutine(GoTo(_target));
        Deselect();
        SetGridPos((int)_target.x, (int)_target.y);
    }

    IEnumerator GoTo(Vector2 _target)
    {
        int count = 0;
        nextTilePos = currentPath[count].GetPos();

        // Loop through the path until object gets to target
        while (transform.position != GridManager.Inst.GetTileCenterPos(_target.x, 0.5f, _target.y))
        {
            if (transform.position == nextTilePos)
            {
                count++;
                if (count >= currentPath.Count)
                    yield break;
                nextTilePos = currentPath[count].GetPos();
            }
            transform.position = Vector3.MoveTowards(transform.position, nextTilePos, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void ShowMovementTiles()
    {
        for (int x = gridPosX - maxMovementPoints; x <= gridPosX + maxMovementPoints; x++)
        {
            for (int y = gridPosY - maxMovementPoints; y <= gridPosY + maxMovementPoints; y++)
            {
                if ((Mathf.Abs(x - gridPosX) + Mathf.Abs(y - gridPosY)) > maxMovementPoints)
                    continue;

                if (GridManager.Inst.IsWithinGrid(x, y))
                    GridManager.Inst.SetTileState(GridTile.TileState.Movement, x, y);
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
        for (int x = gridPosX - maxMovementPoints; x <= gridPosX + maxMovementPoints; x++)
        {
            for (int y = gridPosY - maxMovementPoints; y <= gridPosY + maxMovementPoints; y++)
            {
                if (GridManager.Inst.IsWithinGrid(x, y))
                    GridManager.Inst.SetTileState(GridTile.TileState.None, x, y);
            }
        }
        isSelected = false; 
    }
}
