using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    private Vector2 gridPosXY;

    public GridActor occupier;

    public enum TileState { None, Movement, Attack };
    public TileState currState;

    public Material matMovement;
    public Material matAttack;


    void Start()
    {
        SetState(TileState.None);
        occupier = null;
    }

    void Update()
    {
        
    }

    // Set tile color and state for movement
    public void SetState(TileState _state)
    {
        switch (_state)
        {
            case TileState.None:
                GetComponent<Renderer>().enabled = false;
                break;
            case TileState.Movement:
                GetComponent<Renderer>().enabled = true;
                GetComponent<Renderer>().material = matMovement;
                break;
            case TileState.Attack:
                GetComponent<Renderer>().enabled = true;
                GetComponent<Renderer>().material = matAttack;
                break;
        }
        currState = _state;
    }

    public void SetGridXY(int x, int y) { gridPosXY.x = x; gridPosXY.y = y;  }
    public Vector2 GetXY() { return gridPosXY; }

    public void SetOccupier(GridActor a) { occupier = a; }
    public GridActor GetOccupier() { return occupier; }
}
