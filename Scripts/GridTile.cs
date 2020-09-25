using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    private int gridPosX;
    private int gridPosY;

    private GridActor occupier;

    public enum TileState { None, Movement, Attack };
    private TileState currState;

    public Material matMovement;
    public Material matAttack;

    public int gCost;
    public int hCost;
    public GridTile pathParent;

    void Start()
    {
        SetState(TileState.None);
        occupier = null;
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
                StartCoroutine(FadeIn());
                break;
            case TileState.Attack:
                GetComponent<Renderer>().enabled = true;
                GetComponent<Renderer>().material = matAttack;
                break;
        }
        currState = _state;
    }

    // Fade material
    IEnumerator FadeIn()
    {
        Color tempColor = this.GetComponent<Renderer>().material.color;
        tempColor.a = 0;
        this.GetComponent<Renderer>().material.color = tempColor;

        while (this.GetComponent<Renderer>().material.color.a < 0.5f)
        {
            tempColor = new Color(tempColor.r, tempColor.g, tempColor.b, tempColor.a + (4 * Time.deltaTime));

            this.GetComponent<Renderer>().material.color = tempColor;
            yield return null;
        }
    }

    //
    // Utility Functions
    //

    public void SetGridPos(int x, int y) { gridPosX = x; gridPosY = y;  }
    public int GetGridPosX() { return gridPosX; }
    public int GetGridPosY() { return gridPosY; }
    public Vector3 GetPos() { return transform.position; }

    public void SetOccupier(GridActor a) { occupier = a; }
    public GridActor GetOccupier() { return occupier; }
    public int fCost() { return gCost + hCost; }
}
