using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{

    public GameObject testPlayerPrefab;
    private void Start()
    {
        GridManager.Inst.SetSize(10, 10, 1);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!GridManager.Inst.GetTileOccupier(0, 0))
                Instantiate(testPlayerPrefab, GridManager.Inst.GetTileCenterPos(0, 0.5f, 0), Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            List<GridTile> test = GetComponent<GridPathfinding>().FindPath(new Vector2(2, 1), new Vector2(1, 4));
            
            for (int i = 0; i < test.Count; i++)
            {
                test[i].SetState(GridTile.TileState.Attack);
            }
        }
    }
}
