using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.Scripting.APIUpdating;

public class PlayerCursor : MonoBehaviour
{
    public int gridPosX;
    public int gridPosY;

    private Vector3 nextTilePos;
    public float speed;

    private bool moving;

    private GridActor gridActorSelected;

    void Start()
    {
        gridPosX = 0;
        gridPosY = 0;
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
        if (!moving && Input.GetAxis("Horizontal") > 0 && GridManager.Inst.IsWithinGrid(gridPosX + 1, gridPosY))
        {
            nextTilePos = GridManager.Inst.GetTileCenterPos(gridPosX + 1, transform.position.y, gridPosY);
            moving = true;
            gridPosX++;
        }
        else if (!moving && Input.GetAxis("Horizontal") < 0 && GridManager.Inst.IsWithinGrid(gridPosX - 1, gridPosY))
        {
            nextTilePos = GridManager.Inst.GetTileCenterPos(gridPosX - 1, transform.position.y, gridPosY);
            moving = true;
            gridPosX--;
        }
        else if (!moving && Input.GetAxis("Vertical") > 0 && GridManager.Inst.IsWithinGrid(gridPosX, gridPosY + 1))
        {
            nextTilePos = GridManager.Inst.GetTileCenterPos(gridPosX, transform.position.y, gridPosY + 1);
            moving = true;
            gridPosY++;
        }
        else if (!moving && Input.GetAxis("Vertical") < 0 && GridManager.Inst.IsWithinGrid(gridPosX, gridPosY - 1))
        {
            nextTilePos = GridManager.Inst.GetTileCenterPos(gridPosX, transform.position.y, gridPosY - 1);
            moving = true;
            gridPosY--;
        }
        // A Button Inputs
        else if (!moving && Input.GetKeyDown(KeyCode.Z))//Input.GetAxis("A") < 0) *FIX THIS*
        {
            // [SELECT ACTOR] If nothing is selected and there is an actor on the tile
            if (!gridActorSelected && GridManager.Inst.GetTileOccupier(gridPosX, gridPosY))
            {
                gridActorSelected = GridManager.Inst.GetTileOccupier(gridPosX, gridPosY);
                GridManager.Inst.GetTileOccupier(gridPosX, gridPosY).GetComponent<GridActor>().Select();
                gridActorSelected.ShowMovementTiles();
            }
            // [MOVE ACTOR] If something is selected and there isn't an actor on the tile
            else if (gridActorSelected && !GridManager.Inst.GetTileOccupier(gridPosX, gridPosY))
            {
                gridActorSelected.PathfindingTo(new Vector2(gridPosX, gridPosY));
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
