using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.EventSystems;

public class PlayerCursor : MonoBehaviour
{
    public int gridPosX;
    public int gridPosY;

    private Vector3 nextTilePos;
    public float speed;

    public GameObject UIMenu;
    public GameObject UIArrow;

    private bool movementLocked;

    public enum UnitState { None, Movement, Attack };
    private UnitState selectedUnitState;

    private bool moving;

    private GridActor gridActorSelected;

    void Start()
    {
        selectedUnitState = UnitState.None;
        gridPosX = 0;
        gridPosY = 0;
        moving = false;
        movementLocked = false;

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

        if (!movementLocked)
        {
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
                }
                // [MOVE ACTOR] If something is selected and there is no other actor on the tile
                else if (gridActorSelected && (!GridManager.Inst.GetTileOccupier(gridPosX, gridPosY) || GridManager.Inst.GetTileOccupier(gridPosX, gridPosY) == gridActorSelected))
                {
                    gridActorSelected.MoveTo(gridPosX, gridPosY);
                    selectedUnitState = UnitState.Movement;
                    movementLocked = true;
                    UIMenu.SetActive(true);
                    // Get the first button
                    EventSystem.current.SetSelectedGameObject(UIMenu.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);
                    //gridActorSelected = null;
                }
            }
            // B Button Inputs
            else if (Input.GetKeyDown(KeyCode.X))
            {
                // [DESELECT ACTOR] If something is selected and it hasn't moved
                if (gridActorSelected && selectedUnitState == UnitState.None)
                {
                    gridActorSelected.Deselect();
                    gridActorSelected = null;
                }
                // [RESET ACTOR] If something is selected and it has moved
                //else if (gridActorSelected && selectedUnitState == UnitState.Movement)
                //{
                //    gridActorSelected.ReturnToStartPosition();
                //    selectedUnitState = UnitState.None;
                //}
            }
        }
        // Menu Inputs
        else
        {
            UIArrow.transform.position = new Vector3(UIArrow.transform.position.x, EventSystem.current.currentSelectedGameObject.transform.position.y, UIArrow.transform.position.z);

            // B Button Inputs
            if (Input.GetKeyDown(KeyCode.X))
            {
                // [RESET ACTOR] If something is selected and it has moved
                movementLocked = false;
                EventSystem.current.SetSelectedGameObject(null);
                UIMenu.SetActive(false);
                gridActorSelected.ReturnToStartPosition();
                selectedUnitState = UnitState.None;
            }
        }
    }

    public void UIEndTurn()
    {
        gridActorSelected.EndTurn();
        gridActorSelected = null;

        // [RESET ACTOR] If something is selected and it has moved
        movementLocked = false;
        EventSystem.current.SetSelectedGameObject(null);
        UIMenu.SetActive(false);
    }
}
