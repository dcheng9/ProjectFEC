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
    }
}
