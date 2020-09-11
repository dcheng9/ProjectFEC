using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridActor : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector2 gridPosXY;
    public int maxMovementPoints;
    public bool isSelected;


    void Start()
    {
        isSelected = false;

        // When instatiated let the grid know it occupies the space
        if (!GridManager.Inst.GetTileOccupier(gridPosXY.x, gridPosXY.y))
            GridManager.Inst.SetTileOccupier(this, gridPosXY.x, gridPosXY.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoTo(Vector2 target)
    {

    }

    public void ShowMovementTiles()
    {
        for (float x = gridPosXY.x - maxMovementPoints; x <= gridPosXY.x + maxMovementPoints; x++)
        {
            for (float y = gridPosXY.y - maxMovementPoints; y <= gridPosXY.y + maxMovementPoints; y++)
            {
                if (GridManager.Inst.IsWithinGrid(x, y))
                    GridManager.Inst.SetTileState(GridTile.TileState.Movement, x, y);
            }
        }
    }

    public void SetGridXYPos (float x, float y) 
    {
        GridManager.Inst.SetTileOccupier(null, gridPosXY.x, gridPosXY.y);

        gridPosXY.x = x; 
        gridPosXY.y = y;

        transform.position = GridManager.Inst.GetTileCenterPos(x, transform.position.y, y);
        GridManager.Inst.SetTileOccupier(this, x, y);
    }
    public void Select() { isSelected = true; }
    public void Deselect() 
    {
        for (float x = gridPosXY.x - maxMovementPoints; x <= gridPosXY.x + maxMovementPoints; x++)
        {
            for (float y = gridPosXY.y - maxMovementPoints; y <= gridPosXY.y + maxMovementPoints; y++)
            {
                if (GridManager.Inst.IsWithinGrid(x, y))
                    GridManager.Inst.SetTileState(GridTile.TileState.None, x, y);
            }
        }
        isSelected = false; 
    }
}
