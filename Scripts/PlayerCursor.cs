using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.Scripting.APIUpdating;

public class PlayerCursor : MonoBehaviour
{
    public Vector2 gridPos;

    public Vector3 nextTilePos;
    public float speed;

    public bool moving;

    public GridActor gridActorSelected;

    void Start()
    {
        gridPos.x = 0;
        gridPos.y = 0;
        moving = false;

        nextTilePos = transform.position;
    }

    void Update()
    {
        PlayerInput();
    }

    void PlayerInput()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextTilePos, speed * Time.deltaTime);

        // Check if movement if finished
        if (transform.position == nextTilePos)
        {
            moving = false;
        }

        // Movement Inputs
        if (!moving && Input.GetAxis("Horizontal") > 0 && GridManager.Inst.IsWithinGrid(gridPos.x + 1, gridPos.y))
        {
            nextTilePos = GridManager.Inst.GetTileCenterPos(gridPos.x + 1, transform.position.y, gridPos.y);
            moving = true;
            gridPos.x++;
        }
        else if (!moving && Input.GetAxis("Horizontal") < 0 && GridManager.Inst.IsWithinGrid(gridPos.x - 1, gridPos.y))
        {
            nextTilePos = GridManager.Inst.GetTileCenterPos(gridPos.x - 1, transform.position.y, gridPos.y);
            moving = true;
            gridPos.x--;
        }
        else if (!moving && Input.GetAxis("Vertical") > 0 && GridManager.Inst.IsWithinGrid(gridPos.x, gridPos.y + 1))
        {
            nextTilePos = GridManager.Inst.GetTileCenterPos(gridPos.x, transform.position.y, gridPos.y + 1);
            moving = true;
            gridPos.y++;
        }
        else if (!moving && Input.GetAxis("Vertical") < 0 && GridManager.Inst.IsWithinGrid(gridPos.x, gridPos.y - 1))
        {
            nextTilePos = GridManager.Inst.GetTileCenterPos(gridPos.x, transform.position.y, gridPos.y - 1);
            moving = true;
            gridPos.y--;
        }
        // A Button Inputs
        else if (!moving && Input.GetKeyDown(KeyCode.Z))//Input.GetAxis("A") < 0) *FIX THIS*
        {
            // [SELECT ACTOR] If nothing is selected and there is an actor on the tile
            if (!gridActorSelected && GridManager.Inst.GetTileOccupier(gridPos.x, gridPos.y))
            {
                gridActorSelected = GridManager.Inst.GetTileOccupier(gridPos.x, gridPos.y);
                GridManager.Inst.GetTileOccupier(gridPos.x, gridPos.y).GetComponent<GridActor>().Select();
                //gridActorSelected.ShowMovementTiles();
            }
            // [MOVE ACTOR] If something is selected and there isn't an actor on the tile
            else if (gridActorSelected && !GridManager.Inst.GetTileOccupier(gridPos.x, gridPos.y))
            {
                //gridActorSelected.SetGridXYPos(gridPosXY.x, gridPosXY.y);



                gridActorSelected.Deselect();
                gridActorSelected = null;
            }
        }
        // B Button Inputs
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // Deselect if something is selected
            if (gridActorSelected)
            {
                gridActorSelected.Deselect();
                gridActorSelected = null;
            }
        }
    }
}
