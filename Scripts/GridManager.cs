using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine.SceneManagement;
using UnityEditorInternal;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    private static GridManager _inst;
    public static GridManager Inst { get { return _inst; } }

    private int gridWidth;
    private int gridHeight;
    private float tileSize;

    private GameObject[,] gridArray;
    public GameObject gridTilePrefab;

    private void Awake()
    {
        if (_inst != null && _inst != this)
            Destroy(this.gameObject);
        else
            _inst = this;
    }

    void Update()
    {

    }
    
    // Set grid size and populate the grid array
    public void SetSize(int _width, int _height, float _cellSize)
    {
        this.gridWidth = _width;
        this.gridHeight = _height;
        this.tileSize = _cellSize;

        gridArray = new GameObject[_width, _height];

        // Fill gridArray with tile objects
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = Instantiate(gridTilePrefab, GetTileCenterPos(x, 0, y), Quaternion.identity);
                gridArray[x, y].GetComponent<GridTile>().SetGridPos(x, y);
            }
        }
    }


    //
    // Utility Functions
    //


    // Check if coordinates are within the grid bounds
    public bool IsWithinGrid(int x, int y)
    {
        if (x >= 0 && x <= gridWidth - 1 && y >= 0 && y <= gridHeight - 1)
            return true;
        else
            return false;
    }

    //private Vector3 GetWorldPosition(float x, float y, float z) { return new Vector3(x, y, z) * tileSize; }
    public Vector3 GetTileCenterPos(float x, float y, float z) { return new Vector3(x, y, z) + new Vector3(tileSize, 0, tileSize) * 0.5f; }

    public GridTile GetTile(int x, int y)
    {
        return gridArray[x, y].GetComponent<GridTile>();
    }
    public List<GridTile> GetTileNeighbors(int x, int y)
    {
        List<GridTile> neighbors = new List<GridTile>();

        if (IsWithinGrid(x + 1, y))
            neighbors.Add(GetTile(x + 1, y));
        if (IsWithinGrid(x - 1, y))
            neighbors.Add(GetTile(x - 1, y));
        if (IsWithinGrid(x, y + 1))
            neighbors.Add(GetTile(x, y + 1));
        if (IsWithinGrid(x, y - 1))
            neighbors.Add(GetTile(x, y - 1));

        return neighbors;
    }
    public void SetTileOccupier(GridActor a, int x, int y) 
    {
        gridArray[x, y].GetComponent<GridTile>().SetOccupier(a);
    }
    public GridActor GetTileOccupier(int x, int y)
    {
        return gridArray[x, y].GetComponent<GridTile>().GetOccupier();
    }

    public void SetTileState(GridTile.TileState s, int x, int y) { gridArray[x, y].GetComponent<GridTile>().SetState(s); }
}
