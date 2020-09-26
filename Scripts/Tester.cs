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
            if (!GridManager.Inst.GetTileOccupier(3, 7))
                Instantiate(testPlayerPrefab, GridManager.Inst.GetTileCenterPos(3, 0, 7), Quaternion.identity);
        }
    }
}
